import { useLocation, Link, useMatches } from "react-router";
import "./header.css";
import backIcon from "~/resources/Back.svg";

export default function Header() {
    const location = useLocation();
    const matches = useMatches();

    const isIndex = location.pathname === "/";

    // Get the display name from the route handle, or fall back to a default
    const currentMatch = [...matches].reverse().find((m) => m.handle && (m.handle as any).displayName);
    const displayName = currentMatch ? (currentMatch.handle as any).displayName : "LernZeit.";

    return (
        <div className="header-wrapper">
            <div className="header-bg-layer" />
            <div className="header-main-layer">
                {isIndex ? (
                    <h1 className="header-title">LernZeit.</h1>
                ) : (
                    <Link to="/" style={{ textDecoration: 'none', color: 'inherit', display: "flex", flexDirection: "row", gap: "8px"}}>
                        <img src={backIcon}  alt="Navigate Back"/>
                        <h1 className="header-title">{displayName}</h1>
                    </Link>
                )}
            </div>
        </div>
    )
}