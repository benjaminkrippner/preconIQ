import React from "react";
import ProjectSelector from "./layout/ProjectSelector";
import ProfileDropdown from "./layout/ProfileDropdown";

type HeaderProps = {
  rightSlot?: React.ReactNode;
};

const Header: React.FC<HeaderProps> = ({ rightSlot }) => {
  return (
    <header className="topbar">
      <div className="topbar-left">
        <ProjectSelector variant="header" />
      </div>
      <div className="topbar-right">
        <ProfileDropdown />
      </div>
    </header>
  );
};

export default Header;