import {
    isRouteErrorResponse,
    Links,
    Meta,
    Outlet,
    Scripts,
    ScrollRestoration,
} from "react-router";
import {useEffect, useState} from "react";
import type {Route} from "./+types/root";
import "./app.css";
import Header from "~/components/header/header";
import logo from "~/resources/icon.svg";
import Button from "~/components/button/button";

export const links: Route.LinksFunction = () => [
    {rel: "preconnect", href: "https://fonts.googleapis.com"},
    {
        rel: "preconnect",
        href: "https://fonts.gstatic.com",
        crossOrigin: "anonymous",
    },
    {
        rel: "stylesheet",
        href: "https://fonts.googleapis.com/css2?family=Inter:ital,opsz,wght@0,14..32,100..900;1,14..32,100..900&display=swap",
    },
    {
        rel: "stylesheet",
        href: "https://fonts.googleapis.com/css2?family=Funnel+Display:wght@300..800&display=swap",
    },
    {
        rel: "icon",
        type: "image/svg+xml",
        href: logo
    }
];

export function Layout({children}: { children: React.ReactNode }) {
    return (
        <html lang="en">
        <head>
            <meta charSet="utf-8"/>
            <meta name="viewport" content="width=device-width, initial-scale=1"/>
            <Meta/>
            <Links/>
        </head>
        <body>
        {children}
        <ScrollRestoration/>
        <Scripts/>
        </body>
        </html>
    );
}

interface User {
    isAuthenticated: boolean;
    name?: string;
    email?: string;
    avatar?: string;
}

export default function App() {
    const [user, setUser] = useState<User | null>(null);
    const [loading, setLoading] = useState(true);
    const BACKEND_URL = import.meta.env.REACT_APP_BACKEND_URL ?? "https://localhost:7113";

    useEffect(() => {
        fetch(`${BACKEND_URL}/api/auth/me`, {credentials: 'include'})
            .then(res => res.json())
            .then(data => {
                setUser(data);
                setLoading(false);
            })
            .catch(err => {
                console.error("Failed to fetch auth status", err);
                setLoading(false);
            });
    }, [BACKEND_URL]);

    const login = () => {
        window.location.href = `${BACKEND_URL}/api/auth/login`;
    };

    if (loading) {
        return (
            <div className="flex items-center justify-center min-h-screen">
                <p>Lade...</p>
            </div>
        );
    }

    return (
        <div className="max-w-5xl mx-auto min-hscreen paper-bg paper">
            <Header user={user} />
            <div className="p-4 m-0">
                {!user?.isAuthenticated ? (
                        <div className="flex flex-col items-center justify-center ">
                            <img src={logo} alt="LernZeit Logo" className="w-24 h-24 mb-8"/>
                            <h1 className="text-3xl font-bold mb-4">Willkommen bei LernZeit</h1>
                            <p className="mb-8 text-center max-w-md">Bitte melde dich an, um deine Lerngruppen und deinen
                                Kalender zu verwalten.</p>
                            <div className="w-100 h-100">
                                <Button onClick={login}>Mit Google anmelden</Button>
                            </div>
                        </div>
                    ) :
                    (
                        <Outlet context={{user}}/>
                    )}
            </div>
        </div>
    );
}

export function ErrorBoundary({error}: Route.ErrorBoundaryProps) {
    let message = "Oops!";
    let details = "An unexpected error occurred.";
    let stack: string | undefined;

    if (isRouteErrorResponse(error)) {
        message = error.status === 404 ? "404" : "Error";
        details =
            error.status === 404
                ? "The requested page could not be found."
                : error.statusText || details;
    } else if (import.meta.env.DEV && error && error instanceof Error) {
        details = error.message;
        stack = error.stack;
    }

    return (
        <main className="pt-16 p-4 container mx-auto">
            <h1>{message}</h1>
            <p>{details}</p>
            {stack && (
                <pre className="w-full p-4 overflow-x-auto">
          <code>{stack}</code>
        </pre>
            )}
        </main>
    );
}
