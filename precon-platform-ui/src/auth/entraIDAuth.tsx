import { Configuration, PopupRequest } from "@azure/msal-browser";
import { useMsal } from "@azure/msal-react";
import { useEffect, useState } from "react";

export const msalConfig: Configuration = {
  auth: {
    clientId: process.env.REACT_APP_MSAL_CLIENT_ID ?? "",
    authority: "https://login.microsoftonline.com/" + (process.env.REACT_APP_MSAL_TENANT_ID ?? ""),
    redirectUri: process.env.REACT_APP_MSAL_REDIRECT_URL ?? "",
    postLogoutRedirectUri: process.env.REACT_APP_MSAL_REDIRECT_URL ?? "",
  },
  cache: {
    cacheLocation: "localStorage", // "localStorage" enables SSO across tabs
    storeAuthStateInCookie: false,
  },
};

/**
 * Scopes for initial login request
 */
export const loginRequest: PopupRequest = {
  scopes: ["User.Read"],
};

/**
 * Scopes for API access - this authorizes calls to your backend
 * Format: api://{API_CLIENT_ID}/scope_name
 */
export const apiRequest = {
  scopes: [
    `api://${process.env.REACT_APP_MSAL_API_CLIENT_ID ?? ""}/api.access`
  ],
};

/**
 * Custom hook for Entra ID authentication
 * Provides a clean API: { ready, signedIn, signIn, signOut, user }
 */
export const useEntra = () => {
  const { instance, accounts, inProgress } = useMsal();
  const [ready, setReady] = useState(false);

  useEffect(() => {
    // Mark as ready once MSAL is done with any in-progress operations
    if (inProgress === "none") {
      setReady(true);
    }
  }, [inProgress]);

  const signedIn = accounts.length > 0;
  const user = accounts[0] || null;

  const signIn = async () => {
    try {
      await instance.ssoSilent(loginRequest);
    } catch (error) {
      await instance.loginRedirect(loginRequest);
    }
  };

  const signInWithPopup = async () => {
    try {
      await instance.loginPopup(loginRequest);
    } catch (error) {
      throw error;
    }
  };

  const handleSignOut = async () => {
    // Mark that user has logged out to prevent auto sign-in loop
    sessionStorage.setItem("hasLoggedOut", "true");
    await instance.logoutRedirect({
      account: user,
      postLogoutRedirectUri: process.env.REACT_APP_MSAL_REDIRECT_URL,
    });
  };

  return {
    ready,
    signedIn,
    signIn,
    signInWithPopup,
    signOut: handleSignOut,
    user,
    instance,
    inProgress,
  };
};