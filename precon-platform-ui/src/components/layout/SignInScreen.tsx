import React from "react";
import { useEntra } from "../../auth/entraIDAuth";

const SignInScreen: React.FC = () => {
  const { signIn, inProgress } = useEntra();
  const [autoSignInAttempted, setAutoSignInAttempted] = React.useState(false);

  React.useEffect(() => {
    // Only attempt auto sign-in once on initial load (not after logout)
    const hasLoggedOut = sessionStorage.getItem("hasLoggedOut");
    
    // Wait until no interaction is in progress
    if (!autoSignInAttempted && !hasLoggedOut && inProgress === "none") {
      setAutoSignInAttempted(true);
      signIn();
    }
  }, [signIn, autoSignInAttempted, inProgress]);

  const handleManualSignIn = () => {
    sessionStorage.removeItem("hasLoggedOut");
    signIn();
  };

  return (
    <div className="auth-container">
      <div className="auth-card">
        <h1 className="auth-title">Precon Platform</h1>
        <p className="auth-message">
          Please sign in to continue
        </p>
        <button
          onClick={handleManualSignIn}
          className="btn-primary"
        >
          Sign in with Microsoft
        </button>
      </div>
    </div>
  );
};

export default SignInScreen;
