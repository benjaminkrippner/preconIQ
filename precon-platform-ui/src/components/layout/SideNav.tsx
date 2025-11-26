import React from "react";
import { NavLink } from "react-router-dom";

const NAV_ITEMS = [
  { to: "/", label: "Dashboard" },
  { to: "/weather", label: "Weather Analytics" },
  { to: "/design", label: "Engineering Design" },
  { to: "/document", label: "Document Intelligence" },
  { to: "/estimating", label: "Estimating" },
];

export default function SideNav() {
  return (
    <aside className="sidebar">
      <div className="sidebar-brand">
        <div className="sidebar-mark">GW</div>
        <div>
          <div className="sidebar-title">GroundWork</div>
          <div className="sidebar-subtitle">Preconstruction Hub</div>
        </div>
      </div>
      <nav className="sidebar-nav" aria-label="Primary">
        {NAV_ITEMS.map(({ to, label }) => (
          <NavLink
            key={to}
            to={to}
            className={({ isActive }) =>
              ["sidebar-link", isActive ? "is-active" : ""].filter(Boolean).join(" ")
            }
          >
            <span className="sidebar-label">{label}</span>
          </NavLink>
        ))}
      </nav>
    </aside>
  );
}
