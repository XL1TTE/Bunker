import { defineConfig, loadEnv } from 'vite';
import vue from '@vitejs/plugin-vue';
import { fileURLToPath, URL } from 'node:url';

export default defineConfig(({ mode }) => {
  // Loads .env vars (including VITE_* and any unprefixed ones) for standalone dev.
  const env = loadEnv(mode, process.cwd(), '');

  // Aspire injects the Keycloak base URL into this process when the admin panel is
  // orchestrated via the AppHost (WithReference(auth)). Keycloak's proxy port is
  // random per run, so we prefer the injected URL and only fall back to .env /
  // localhost:8080 for standalone `npm run dev` without Aspire.
  const aspireKeycloakUrl =
    process.env.AUTH_HTTPS ??
    process.env.AUTH_HTTP ??
    process.env['ConnectionStrings__auth'] ??
    process.env['services__auth__https__0'] ??
    process.env['services__auth__http__0'];

  const keycloakUrl = (aspireKeycloakUrl ?? env.VITE_KEYCLOAK_URL ?? 'http://localhost:8080').replace(/\/$/, '');
  const port = Number(process.env.PORT) || 5175;

  return {
    plugins: [vue()],
    define: {
      // Exposed to the client bundle as a global; see `declare const __KEYCLOAK_URL__`
      // in env.d.ts. Avoids overriding Vite's own import.meta.env.VITE_KEYCLOAK_URL.
      __KEYCLOAK_URL__: JSON.stringify(keycloakUrl),
    },
    resolve: {
      alias: {
        '@': fileURLToPath(new URL('./src', import.meta.url)),
      },
    },
    css: {
      modules: {
        localsConvention: 'camelCaseOnly',
        generateScopedName: '[name]_[local]_[hash:base64:5]',
      },
    },
    server: {
      port,
      // Bind all interfaces so the Aspire-launched dev server is reachable.
      host: true,
    },
  };
});