CRITICAL

<!-- C1. JoinLobby broken end-to-end — 500 + dup participants + bypassed capacity ✅
LobbyRepository.TryFindAsync (LobbyRepository.cs:15-16) no Include(Participants/Packs), no AsNoTracking. Returned Lobby.Players always empty →
- JoinLobbyHandler.cs:34 already-in guard lobby.Players.Any(...) always false → same user re-joins, dup Participants rows (new PublicId each).
- LobbyExtensions.AddPlayer capacity check sees 0 → full lobby accepts unbounded players.
- AddPlayer Result ignored (JoinLobbyHandler.cs:39-44).
- After save, ToTransfer() (Mappers.cs:32) lobby.Players.First(x => x.Role==Host) throws InvalidOperationException → middleware 500. Client retries → more dup rows.
Fix: Include(l=>l.Participants).Include(l=>l.Packs) on read; handle AddPlayer Result; harden ToTransfer (FirstOrDefault + null handling). -->

<!-- C2. FactCard validation inverted — nearly every create throws 500 ✅
Domain/Cards/FactCard.cs:14: string.IsNullOrWhiteSpace(Fact) | Fact.Length >= 8 ? throw .... Validator max 64, so any fact ≥8 chars → ArgumentException → 500. Intent was < 6.
Fix: Fact.Length < 6. -->

<!-- C3. Three card list endpoints always return empty ✅
FactCardEndpoints.cs:64, HobbiesCardEndpoints.cs:66, SexCardEndpoints.cs:69 call new GetFactCards() etc — record struct default Skip=0,Take=0 → Take(0) → Cards:[] forever (Total correct). Age/Profession endpoints correctly take Skip/Take query.
Fix: add [FromQuery] int Skip=0, [FromQuery] int Take=10, pass into command. -->

<!-- C4. AccountUpdatedHandler writes to read-only replica + swallows errors + kills retry policy ✅
Handlers/AccountUpdatedHandler.cs:41-47. AccountsDbContext → lobby-accounts-replica-db (AGENTS.md: must NOT write). catch(Exception){ Log.Information(...) } swallows → Wolverine ScheduleRetry (lines 20-23) never fires (hd, replica permanently missing. Also writes outside Wolverinedurable txn (AccountsDbContext not the UoW context). Retry-on-Exception also retries permanent failures forever.
Fix: stop writing replica (real PG replication / cross-service read). If app-level ingest intended: drop catch, route through IUnitOfWork/LobbyDbContext, Log.Error, retry only transient. -->

<!-- C5. Pack/preset change events never published; downstream never ✅
Messaging/CardPacks.cs, Messaging/PersonalityPresets.cs define CardPackUpdated/Deleted, PersonalityPresetUpdated/Deleted but no handler emits them. WolverineConfiguration.cs:47-51 registers no pack/preset exchanges. Lobby Handoff saga gets stale content, deleted packs stay referenced.
Fix: inject IMessageContext, PublishAsync after save (inside Wolverine txn), register card-pack-updates/card-pack-deleted/personality-preset-* exchanges. -->

HIGH                                                                                                                                                      
H1. All 5 Update card handlers never publish RabbitMQ updates — Features/Cards/Update*/Handler.cs no IMessageContext, no PublishAsync. Create handlers do. Downstream serves stale cards.                                                                                                                            H2. DeleteCard never publishes CardDeleted — Messaging/Cards.csexchange. Downstream holds dead card refs.
H3. Card list endpoints missing [Authorize] — Age/Fact/Hobbies/Profession/SexCardEndpoints.cs GET-list. Every other card + pack endpoint has [Authorize(Roles="content-service.admin")]. Anonymous can enumerate whole library; GetById requires admin = inconsistent = oversight. Fix: add Authorize odocument public-read.
H4. [Authorize(Roles="content-service.admin")] rejects every admin with stock Keycloak token — AuthConfiguration.cs:29-58 no RoleClaimType mapping, no IClaimsTransformation. Keycloak nests roles in realm_access.roles/resource_access.<client>.roles, not flat role → zero ClaimTypes.Role claims → all admin 403. Fix: claims transform flattening nested roles, or TokenValpe="role" + Keycloak protocol mapper. ✅ — superseded: no claims transform added; user provisions the Keycloak realm and configures a role-claim protocol mapper so tokens carry flat role claims directly. Admin [Authorize(Roles=...)] works once that mapper is in place.
H5. RequireHttpsMetadata=false hardcoded all envs — AuthConfiguration.cs:34. Prod OIDC metadata over HTTP → MITM key substitution → forged tokens. Fix: !IsDevelopment().                                                                                                                                     H6. JoinLobbyHandler SaveChangesAsync not awaited inside try/ca48. catch only catches sync pre-await exc; DB failures on thediscarded Task unobserved → handler returns Success → client 200 but nothing persisted. Fix: try { await uow.SaveChangesAsync(); } catch (DbUpdateException) { return Failure(...); }. ✅ — mooted by design: handlers no longer call SaveChangesAsync explicitly; AutoApplyTransactions + durable outbox commit the business write + envelope in one transaction.                                                                                                                             H7. JoinWithPasswordAsync throws NotImplementedException ✅ — JT /lobbies/{id} without ?inviteCode= → 500. Live routeadvertises password join. Fix: 501 or implement.
H8. Lobby password stored plaintext + never validated on invite-code path — CreateLobbyHandler.cs:25 stores raw LobbyPassword. Join via invite-code never checks password; password path is H7. Anyone with 12-char invite code joins "protected" lobby. Fix: Argon2/BCrypt hash, verify on join for non-public.
H9. CreateLobbyHandler logs plaintext password — CreateLobbyHandler.cs:35 JsonSerializer.Serialize(command) includes LobbyPassword. Fix: redact.
H10. UpdateCardPack silently wipes cards when CardIds empty/omitted — Requests.cs:39, validator only NotNull(), handler rebuilds aggregate + Update reCards collection. PUT title-only with cardIds:[] → 200 OK, emptrge-vs-existing.
H11. Lost-update race on AddCardToPack/RemoveCardFromPack/UpdateCardPack — read-modify-write on CardPackCards join, no concurrency token, Update replaces whole collection. Concurrent adds → only one survives. Fix: rowversion on CardPack → DbUpdateConcurrencyException + retry, or diff not replace.       H12. AddCardToPack/CreateCardPack/UpdateCardPack don't validate → FK violation → raw DbUpdateException → 500 not 4xx. Fix:check ICardQueries first or catch DbUpdateException → Result.NotFound.
H13. Bitwise | in domain ctors → NullReferenceException not ArgumentException — CardPack.cs:23-30, PersonalityPreset.cs:17-25: IsNullOrWhiteSpace(TitlTitle.Length < 6 doesn't short-circuit; null Title → NRE beforen likely in FactCard.cs:14 ✅ — confirmed | there.) Fix: ||.
H14. LobbyRepository.Update replaces whole collections → EF tracking hazard ✅ — LobbyRepository.cs:19-26 + ApplyUpdate re-maps Participants/Packs to brand-new entity instances. Once Includes added (C1 fix), re-creating existing participants with same PublicId alt-key → EF marks Added → INSERT violates unique → blows up. Fix: load tracked entity, mutate directly, save. No round-trip through fresh domain.
H15. AccountService outbox not transactional w/ business write — AccountService/Configuration/WolverineConfiguration.cs:17-28 has UseEntityFrameworkCoreTransactions + UseDurableOutbox but no AutoApplyTransactions() and no [Transactional]; handler calls CommitAsync() then yields  AccountUpdated. Account row + outbox envelope in separate txns ed, no event published. Fix: AutoApplyTransactions(), dropexplicit CommitAsync. ✅ — AutoApplyTransactions() added; explicit CommitAsync() dropped from CreateProfile/UpdateProfile handlers.
H16. SignalR broadcasts joins to ALL clients; hub has no group mgmt — JoinLobbyHandler.cs:59 hub.Clients.All.UserJoinedLobby(); LobbyHub.cs empty, no OnConnectedAsync. Info leak + noise. Fix: override connect/discId, send to Group(lobbyId), await it. ✅ — LobbyHub.OnConnectedAsync/OnDisconnectedAsync join/leave Group(lobbyId); JoinLobbyHandler broadcasts to Group; ILobbyHub gained GameReady/GameStartFailed; hub mapped at /hubs/lobby.
H17. Card-update exchanges declared + published but LobbyService consumes none — ContentProvision.cs declares *-card-updates + binds lobby-*-card-updates-queue; LobbyService WolverineConfiguration.cs:29-31 only listens lobby-service-account-updates. Queues grow unbounded, no lobby card
hydration from content events. Fix: add ListenToRabbitQueue con/publishes. ✅ — superseded by design: lobby no longer subscribes to card events; content is validated on-demand at game start via the Game Service saga (see CONTEXT.md "Lobby Handoff").

MEDIUM

M1. HydrationHandler yields both Failed and Hydrated for one request — Features/Hydration/HydrationHandler.cs:22-45. Missing pack → GameContentHydrationFailed then falls through to GameContentHydrated. Contradictory saga semantics. Fix: yield break; after Failed. ✅ — reworked into a single-reply publisher (RequestGameContentHydration); returns immediately after publishing Failed.
M2. ExceptionToHttpErrorMiddleware logs 500 on client cancellat-25. catch(Exception) catches OperationCanceledException →LogError + 500 (pollutes Sentry/metrics). Fix: catch (OperationCanceledException) when (ctx.RequestAborted.IsCancellationRequested) { return; } before generic.
M3. UserIdentityMiddleware silently IsAuthenticated=false when iddleware.cs:19-28. Valid JWT but no preferred_username →behaves anonymous, no log. Fix: always SetUser when authenticated, default nickname, warn on missing.
M4. CardPackIds silently dropped in CreateLobby — CreateLobbyHaby.Create, never calls AddCardPack. Validator checks them asGUIDs → client believes set. Fix: loop + AddCardPack. ✅ — CreateLobbyHandler now calls AddCardPack for each selected pack id.
M5. InitializeDatabaseAsync only in Development — Program.cs:39-44 (Lobby; pattern likely Content/Account too). EnsureCreatedAsync skipped in prod → no schema → first access throws. Fix: run outside dev guard or bootstrap schema externally.
M6. Sentry SendDefaultPii=true + 100% sampling all envs — SentryConfiguration.cs:13-15. PII to Sentry + quota risk. Fix: gate by env.
M7. Result<TSuccess,TError> broken for null success — Monads/src/Common/Result.cs:13-16,24-28. IsSuccess => _result is not null; Success(null) → neither
success nor failure, Match throws UnreachableException; Value tn (wrong type). Latent. Fix: explicit bool _isSuccessdiscriminator; InvalidOperationException.
M8. Validator inconsistencies (CreateLobby) — RequestValidators.cs:11-16: Capacity.GreaterThan(4) rejects 4 but domain allows 4 (Lobby.cs:47 rejects <4);
CardPackIds.Must(...) no NotNull() → null → NRE in predicate → o(4), add NotNull().
M9. ToTransfer() throws when no host ✅ — Mappers.cs:32. RemovePlayer only migrates host if nextPlayer != null → last-player-leave lobby hostless → next snapshot 500. Fix: FirstOrDefault + null handling, or domain host invariant.
M10. AddCardToPack/RemoveCardFromPack return 500 for legit statard throw ArgumentException (dup add / missing), handlers don'tcatch → 500. Fix: Result.Conflict/NotFound → 409/404.
M11. repository.Update return value discarded everywhere — Update returns bool (false = row vanished between find+update). All handlers ignore → 200 with
phantom aggregate. Fix: check bool → NotFound.
M12. No CancellationToken anywhere — all handlers/queries/repos. No cooperative cancellation. Fix: thread CT from endpoints.
M13. Hydration over-fetch + in-memory refilter — HydrationQueries.cs:13-25 loads TPT hierarchy (6 joins), handler OfType<T> 5× over same list. Fix: query subtype tables directly or materialize lookup once.
M14. Duplicate CardIds not rejected — CardPackValidators.cs:12,22 only NotNull(); dup Guids → AddCard throws ArgumentException → 500. Fix: Must(ids => ids.Distinct().Count()==ids.Count()).
M15. UpdateSettings capacity check ignores bots — LobbyExtensiors.Count() only; bots replaced after → capacity-4 lobby can hold 3 players + 5 bots = 8. Latent. Fix: check Players + bots <= capacity.
M16. Short nickname throws unhandled in JoinLobbyHandler — PlayerParticipant.New <4 chars throws ArgumentException outside the save try/catch → 500. Fix:
validate at endpoint/validator or wrap.
M17. LobbyRepository.Delete removes untracked entity w/ shadow PK=0 — LobbyRepository.cs:13 Remove(aggregate.ToEntity()) → fresh entity, int Id=0 → deletes nothing/wrong. Latent. Fix: load tracked then remove.

LOW

- L1. Double SaveChangesAsync — ContentService AutoApplyTransactions() on but Update/Delete/Add/Remove handlers also explicit uow.SaveChangesAsync(). Redundant round-trip. Pick one. (CreateAgeCardHandler correct.)
<!-- - L2. UnitOfWorkMiddleware dead code — defined, never wired. Delete or wire. -->
- L3. GetCardById/reads track entities — CardsDbContext.cs:9-10 + per-type repos no AsNoTracking. Add it.
- L4. Redundant [Transactional] on CreateProfessionCardHandler dy applies. Drop for consistency.
- L5. Dead HobbiesCard.CreateNew(Id,string) overload — unused.
- L6. Auth events use Console.WriteLine — AuthConfiguration.cs:45-57 bypasses Serilog, leaks exc message. Use ILogger.
- L7. Interpolated Serilog calls — AccountUpdatedHandler.cs Logstructured logging. Use templates.
- L8. IUserIdentityContext.SetUser public — any code can overwrite identity (spoof risk via IMessageBus.InvokeAsync w/o middleware). Split read/IUserIdentityContextWriter internal.
- L9. Invite-code lookup case-sensitive — ILobbyQueries.cs:23 =ase query misses. Normalize via InviteCode.Create orEF.Functions.ILike. ✅ — GetByInviteCodeAsync uses EF.Functions.ILike and Includes Participants/Packs.
- L10. Join endpoint no validator, ignores lobbyId on invite path — LobbyEndpoints.cs:60-81. Add validator (invite xor id+password), validate GUID.
- L11. UserJoinedLobby() not awaited — fire-and-forget, send failures swallowed.
- L12. LobbyRepository.Update sync FirstOrDefault ✅ — blocking DB call in async handler. FirstOrDefaultAsync.
- L13. AccountId.Create no format validation — AccountId.cs:7 accepts any string → corrupt refs. Guid.TryParse.
- L14. Persistence/Entities/Bot.cs declares PersonalityPreset — rename file.
- L15. GetAllCardPacks no pagination — CardPackQueries.cs:14-20 unbounded Include(Cards). Flag for paging.
- L16. CreateLobbyHandler unused IMessageContext messaging param — dead.
- L17. 5× copy-paste card Create/Update/Get handlers + endpoint C3 (some list endpoints got Skip/Take, others didn't) + H1(updates missed publishing in all 5). Generic CardCrudHandler<TCard,TDto> over ICardRepository<TCard> collapses ~15 files → ~3.
