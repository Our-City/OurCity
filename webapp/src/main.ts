import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import PrimeVue from "primevue/config";
import "primeicons/primeicons.css";
import "./assets/styles/theme.css";
import "./assets/styles/base.css";

import { createPinia } from "pinia";
import { useAuthStore } from "@/stores/authenticationStore";

const app = createApp(App);
const pinia = createPinia();

// register plugins
app.use(pinia);
app.use(router);
app.use(PrimeVue);

// restore auth session on app start
const auth = useAuthStore(pinia);
await auth.restoreSession();

app.mount("#app");
