import React from "react";
import Header from "../Header";
import SideNav from "./SideNav";
import { Outlet } from "react-router-dom";

export default function AppShell() {
  return (
    <div className="app-shell">
      <SideNav />
      <div className="shell-main">
        <Header />
        <main className="main-content">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
