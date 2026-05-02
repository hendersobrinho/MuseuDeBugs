import { environment } from "../../environments/environment";

declare global {
  interface Window {
    MUSEU_DEBUGS_CONFIG?: {
      apiUrl?: string;
    };
  }
}

export const API_URL = window.MUSEU_DEBUGS_CONFIG?.apiUrl ?? environment.apiUrl;
