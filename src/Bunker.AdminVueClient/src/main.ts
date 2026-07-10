import { createApp } from 'vue';
import { createPinia } from 'pinia';
import App from './App.vue';
import { router } from './router';
import { registerApi } from './api/register';
import { auth } from './auth/keycloak';
import './styles/global.css';

async function bootstrap(): Promise<void> {
  await auth.init();

  const app = createApp(App);
  app.use(createPinia());
  registerApi(app, {
    getAccessToken: () => Promise.resolve(auth.getAccessToken()),
  });
  app.use(router);
  app.mount('#app');
}

bootstrap().catch((err) => {
  console.error('Bootstrap failed:', err);
});