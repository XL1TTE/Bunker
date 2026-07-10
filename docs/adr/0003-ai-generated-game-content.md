# AI-generated game content with a Python service, canned fallback

Game content is generated per-game by an LLM for uniqueness: a Python **AI Service**
produces a themed Bunker Card plus one Character Sheet per participant in a single
consolidated request, themed to the host-selected card packs. The
`Bunker.GameService` handoff saga drives this over **RabbitMQ**; the **Content
Service** holds canned content (all card types, including a new `BunkerCard` type)
used **only as fallback** when generation fails.

The handoff saga runs **sequentially** (not parallel): fetch lightweight pack
transfers (`Name` + a new `GenerationPrompt` field) from Content Service → one
consolidated AI request → on success build the game. On AI failure after retries,
the saga runs **canned compensation** (hydrate canned cards + a canned Bunker Card
from Content Service, assign them into sheets) so an LLM outage degrades gracefully
instead of blocking every game start. If compensation hydration also fails (a pack
ID missing), the saga fails the handoff. The LLM returns **anonymous** sheets
(keyed only by count); Game Service shuffles and assigns them to participants, so
no participant identity is sent to the LLM.

## Considered options

- **Sync HTTP/gRPC to the AI service** — rejected: holds connections over the
  multi-second LLM call, needs client-side retry/load-balancing, and a replica
  dying mid-generation double-pays the LLM. Async competing-consumer messaging
  scales by adding replicas with no ingress and decouples the slow call.
- **Parallel AI + hydration** — rejected: a failing hydration would waste already
  -spent LLM tokens. Sequential (AI first, canned only on AI failure) avoids that.
- **.NET AI service** — rejected in favor of Python for the LLM-engineering
  ecosystem, accepted as the norm for AI services going forward.
- **Canned-only / AI-only (no fallback)** — rejected: canned-only gives up the
  uniqueness payoff; AI-only lets an LLM outage hard-block all game starts.

## Consequences

- Every game start hits the LLM by default — expect ~10–30s handoff latency and a
  per-game token cost; the frontend shows a "generating game…" state, and an AI
  outage degrades *every* game to canned until recovery.
- Cross-language interop seam: Game Service configures a custom `IMessageSerializer`
  on the outgoing AI-request endpoint so Python receives plain JSON (no Wolverine
  envelope); the reply queue uses `.DefaultIncomingMessage<T>()` (no headers
  required) and the saga correlates by `StartRequestId` in the body via
  `[SagaIdentity]`. Python never touches Wolverine internals.
- Idempotency by `StartRequestId` on the Python side so a redelivery after a
  mid-generation crash doesn't double-pay the LLM.
- Content Service gains `HealthCard` and `LuggageCard` types (Character Sheet =
  Profession, Sex, Age, Health, Hobbies, Luggage, Fact) and a `BunkerCard` type;
  `CardPack` gains `GenerationPrompt` (LLM-facing, distinct from the human
  `Description` shown in the host picker).