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

app.use((req, res, next) => {
  const origin = req.headers.origin;
  if (origin) {
    res.setHeader("Access-Control-Allow-Origin", origin);
    res.setHeader("Vary", "Origin");
    res.setHeader("Access-Control-Allow-Credentials", "true");
    res.setHeader("Access-Control-Allow-Headers", "Authorization, Content-Type");
    res.setHeader("Access-Control-Allow-Methods", "GET, POST, PUT, PATCH, DELETE, OPTIONS");
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

app.use(
  createProxyMiddleware({
    pathFilter: "/hubs/lobby",
    target: lobbyServiceUrl,
    changeOrigin: true,
    ws: true,
  }),
);

app.listen(port, () => {
  console.log(`Bunker gateway listening on http://localhost:${port}`);
  console.log(`  /account        -> ${accountServiceUrl}`);
  console.log(`  /content        -> ${contentServiceUrl}`);
  console.log(`  /lobbies        -> ${lobbyServiceUrl}`);
  console.log(`  /hubs/lobby (ws)-> ${lobbyServiceUrl}`);
});