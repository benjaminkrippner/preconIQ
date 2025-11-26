import React from "react";
import { BrowserRouter } from "react-router-dom";
import { AuthenticatedTemplate, UnauthenticatedTemplate } from "@azure/msal-react";
import "./styles/base.css";
import "./styles/layout.css";
import "./styles/components.css";
import "./styles/dashboard-theme.css";
import AppRoutes from "./routes/AppRoutes";
import { ProjectProvider } from "./state/ProjectContext";
import SignInScreen from "./components/layout/SignInScreen";

export default function App() {
  return (
    <BrowserRouter>
      <AuthenticatedTemplate>
        <ProjectProvider>
          <AppRoutes />
        </ProjectProvider>
      </AuthenticatedTemplate>
      <UnauthenticatedTemplate>
        <SignInScreen />
      </UnauthenticatedTemplate>
    </BrowserRouter>
  );
}