import axios from "axios";
import { IPublicClientApplication, PopupRequest } from "@azure/msal-browser";

const baseURL = process.env.REACT_APP_API_BASE_URL;

if (!baseURL) {
  // Without this value, requests silently fall back to the UI origin (localhost:3000)
  // and hit the React dev server instead of the ASP.NET API.
  // Surfacing an explicit error makes the missing configuration obvious.
  // eslint-disable-next-line no-console
  console.error(
    "REACT_APP_API_BASE_URL is not set. Configure it to point at the PreconIQ API (e.g., https://localhost:7132 or your deployed URL)."
  );
}

const httpClient = axios.create({
  baseURL,
});

// This will be set after MSAL initializes
let msalInstanceRef: IPublicClientApplication | null = null;
let apiRequestRef: PopupRequest | null = null;

/**
 * Initialize the http client with MSAL instance
 * This should be called from index.tsx after MSAL initialization
 */
export const initializeHttpClient = (
  msalInstance: IPublicClientApplication,
  apiRequest: PopupRequest
) => {
  msalInstanceRef = msalInstance;
  apiRequestRef = apiRequest;
};

/**
 * Request interceptor to add Bearer token to all API calls
 */
httpClient.interceptors.request.use(
  async (config) => {
    if (!msalInstanceRef) {
      return config;
    }

    const account = msalInstanceRef.getActiveAccount();
    
    if (account && apiRequestRef) {
      try {
        // Try to acquire token silently (from cache)
        const response = await msalInstanceRef.acquireTokenSilent({
          scopes: apiRequestRef.scopes,
          account: account,
        });
        
        // Add Bearer token to Authorization header
        config.headers.Authorization = `Bearer ${response.accessToken}`;
      } catch (error) {
        // Token acquisition failed - let request proceed without token
        // API will return 401 if authentication is required
      }
    }
    
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

/**
 * Response interceptor to handle 401 Unauthorized errors
 */
httpClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    // Pass through all errors to be handled by calling code
    // App-level auth (AuthenticatedTemplate/UnauthenticatedTemplate) handles sign-in
    return Promise.reject(error);
  }
);

export default httpClient;