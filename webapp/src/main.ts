import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import PrimeVue from "primevue/config";
import "primeicons/primeicons.css";
import "./assets/styles/theme.css";
import "./assets/styles/base.css";
import "./assets/styles/toast-custom.css";
import '@coreui/coreui/dist/css/coreui.min.css'
import 'bootstrap/dist/css/bootstrap.min.css'
import ToastCmp from "primevue/toast";
import ToastService from "primevue/toastservice";

import { createPinia } from "pinia";
import { useAuthStore } from "@/stores/authenticationStore";

const app = createApp(App);
const pinia = createPinia();

// register plugins
app.use(pinia);
app.use(router);
app.use(PrimeVue);
app.use(ToastService);
app.component("ToastCmp", ToastCmp);

// restore auth session on app start
const auth = useAuthStore(pinia);
await auth.restoreSession();

app.mount("#app");
