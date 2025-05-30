import axios from "axios";
import { store } from "../stores/store";

const agent = axios.create({
    baseURL: import.meta.env.VITE_API_URL
});

agent.interceptors.request.use(config => {
    store.uiStore.isBusy();
    return config;
})

agent.interceptors.response.use(async response => {
    try {
        return response;
    } catch (error) {
        store.uiStore.isIdle();
        return Promise.reject(error)
    } finally {
        store.uiStore.isIdle();
    }
});

export default agent;