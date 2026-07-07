import express from "express";
import { createProxyMiddleware } from "http-proxy-middleware";
import "dotenv/config";

const port = process.env.PORT ?? 5174;

const accountServiceUrl =
  process.env["services__account-service__http__0"] ??
  process.env.ACCOUNT_SERVICE_HTTP ??
  "http://localhost:5165";

const lobbyServiceUrl =
  process.env["services__lobby-service__http__0"] ??
  process.env.LOBBY_SERVICE_HTTP ??
  "http://localhost:5297";

const contentServiceUrl =
  process.env["services__content-service__http__0"] ??
  process.env.CONTENT_SERVICE_HTTP ??
  "http://localhost:5029";

const app = express();

// CORS preflight + response headers.
//
// The browser cannot send custom headers on a WebSocket handshake, and SignalR's
// negotiate POST carries headers like `x-signalr-user-agent` that aren't in a
// fixed allow-list. With `Access-Control-Allow-Credentials: true` the wildcard
// `*` is not honored for `Allow-Headers`, so we reflect whatever the client
// asked for in `Access-Control-Request-Headers` (falling back to a broad list).
const DEFAULT_ALLOW_HEADERS =
  "Authorization, Content-Type, x-signalr-user-agent, x-signalr-protocol";

app.use((req, res, next) => {
  const origin = req.headers.origin;
  if (origin) {
    res.setHeader("Access-Control-Allow-Origin", origin);
    res.setHeader("Vary", "Origin");
    res.setHeader("Access-Control-Allow-Credentials", "true");
    const requested = req.headers["access-control-request-headers"];
    res.setHeader(
      "Access-Control-Allow-Headers",
      requested ? requested : DEFAULT_ALLOW_HEADERS,
    );
    res.setHeader(
      "Access-Control-Allow-Methods",
      "GET, POST, PUT, PATCH, DELETE, OPTIONS",
    );
  }
  if (req.method === "OPTIONS") {
    res.status(204).end();
    return;
  }
  next();
});

app.use(
  createProxyMiddleware({
    pathFilter: "/account",
    target: accountServiceUrl,
    changeOrigin: true,
  }),
);

app.use(
  createProxyMiddleware({
    pathFilter: "/content",
    target: contentServiceUrl,
    changeOrigin: true,
  }),
);

app.use(
  createProxyMiddleware({
    pathFilter: "/lobbies",
    target: lobbyServiceUrl,
    changeOrigin: true,
  }),
);

// SignalR hub. `ws: true` enables WebSocket proxying, but http-proxy-middleware
// v3 does not auto-attach the `upgrade` handler when using `app.listen()` — we
// wire it manually on the server below.
const lobbyHubProxy = createProxyMiddleware({
  pathFilter: "/hubs/lobby",
  target: lobbyServiceUrl,
  changeOrigin: true,
  ws: true,
});
app.use(lobbyHubProxy);

const server = app.listen(port, () => {
  console.log(`Bunker gateway listening on http://localhost:${port}`);
  console.log(`  /account        -> ${accountServiceUrl}`);
  console.log(`  /content        -> ${contentServiceUrl}`);
  console.log(`  /lobbies        -> ${lobbyServiceUrl}`);
  console.log(`  /hubs/lobby (ws)-> ${lobbyServiceUrl}`);
});

server.on("upgrade", (req, socket, head) => {
  // Only /hubs/lobby is a WebSocket endpoint; hand its upgrade to that proxy.
  lobbyHubProxy.upgrade(req, socket, head);
});