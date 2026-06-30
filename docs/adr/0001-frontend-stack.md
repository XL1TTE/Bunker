# Frontend stack

For the MVP frontend we chose Vue 3 + Vite + TypeScript with Pinia for state
and Vue Router for routing. Styling uses CSS Modules (separate `*.module.css`
files imported as objects, classes applied via `:class="styles.foo"`, scoped
via Vite's built-in hashing). SFC `<style>` blocks are intentionally avoided
so styles remain portable across components.

The MVP is scoped to the Lobby bounded context only — authentication, lobby
creation/joining, lobby browser, lobby room with chat. Game-screen work is
deferred to a later iteration.