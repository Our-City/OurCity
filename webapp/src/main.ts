import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";

import PrimeVue from "primevue/config";
import "primeicons/primeicons.css";

import "./assets/styles/theme.css";
import "./assets/styles/base.css";

const app = createApp(App);

app.use(router);
app.use(PrimeVue);

app.mount("#app");
