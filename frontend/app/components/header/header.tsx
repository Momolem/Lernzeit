import { useLocation, Link, useMatches } from "react-router";
import "./header.css";
import backIcon from "~/resources/Back.svg";

interface User {
    isAuthenticated: boolean;
    name?: string;
    email?: string;
    avatar?: string;
}

interface HeaderProps {
    user?: User | null;
}

export default function Header({ user }: HeaderProps) {
    const location = useLocation();
    const matches = useMatches();

    const isIndex = location.pathname === "/";

    // Get the display name from the route handle, or fall back to a default
    const currentMatch = [...matches].reverse().find((m) => m.handle && (m.handle as any).displayName);
    const displayName = currentMatch ? (currentMatch.handle as any).displayName : "LernZeit.";

    const BACKEND_URL = import.meta.env.VITE_BACKEND_URL ?? "https://localhost:7113";

    const logout = () => {
        fetch(`${BACKEND_URL}/api/auth/logout`, { credentials: 'include' })
            .then(() => {
                window.location.reload();
            })
            .catch(err => console.error("Logout failed", err));
    };

    return (
        <div className="header-wrapper">
            <div className="header-bg-layer" />
            <div className="header-main-layer">
                <div className="header-left">
                    {isIndex ? (
                        <h1 className="header-title">LernZeit.</h1>
                    ) : (
                        <Link to="/" style={{ textDecoration: 'none', color: 'inherit', display: "flex", flexDirection: "row", gap: "8px", alignItems: "center"}}>
                            <img src={backIcon}  alt="Navigate Back"/>
                            <h1 className="header-title">{displayName}</h1>
                        </Link>
                    )}
                </div>
                
                {user && user.isAuthenticated && (
                    <div className="header-right" onClick={logout} title="Logout" style={{ cursor: "pointer" }}>
                        <span className="user-name">{user.name}</span>
                        {user.avatar ? (
                            <img src={user.avatar} alt="User Avatar" className="user-avatar" />
                        ) : (
                            <div className="user-dummy-icon">
                                {user.name ? user.name.charAt(0).toUpperCase() : "U"}
                            </div>
                        )}
                    </div>
                )}
            </div>
        </div>
    )
}