import React from "react";
import { useEntra } from "../../auth/entraIDAuth";

const ProfileDropdown: React.FC = () => {
  const { user, signOut } = useEntra();
  const [dropdownOpen, setDropdownOpen] = React.useState(false);
  const dropdownRef = React.useRef<HTMLDivElement>(null);

  const handleSignOut = () => {
    setDropdownOpen(false);
    signOut();
  };

  // Close dropdown when clicking outside
  React.useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setDropdownOpen(false);
      }
    };

    if (dropdownOpen) {
      document.addEventListener("mousedown", handleClickOutside);
    }
    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, [dropdownOpen]);

  const account = user;
  const initials = account?.name
    ? account.name
        .split(" ")
        .filter(n => n.length > 0)
        .map((n) => n[0])
        .join("")
        .toUpperCase()
        .slice(0, 2)
    : account?.username?.slice(0, 2).toUpperCase() || "??";

  return (
    <div className="profile-dropdown" ref={dropdownRef}>
      <button
        onClick={() => setDropdownOpen(!dropdownOpen)}
        className="profile-button"
      >
        <div className="profile-avatar">
          {initials}
        </div>
        <span className="profile-name">
          {account?.name || account?.username}
        </span>
        <svg
          width="12"
          height="12"
          viewBox="0 0 12 12"
          className={`profile-dropdown-arrow ${dropdownOpen ? 'open' : ''}`}
        >
          <path d="M2 4l4 4 4-4" stroke="currentColor" strokeWidth="2" fill="none" />
        </svg>
      </button>

      {dropdownOpen && (
        <div className="profile-dropdown-menu">
          <div className="profile-dropdown-info">
            <div className="profile-dropdown-user-name">
              {account?.name || "User"}
            </div>
            <div className="profile-dropdown-user-email">
              {account?.username}
            </div>
          </div>
          <button
            onClick={handleSignOut}
            className="profile-dropdown-signout"
          >
            Sign Out
          </button>
        </div>
      )}
    </div>
  );
};

export default ProfileDropdown;
