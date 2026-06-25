import {
  isRouteErrorResponse,
  Links,
  Meta,
  Outlet,
  Scripts,
  ScrollRestoration,
} from "react-router";
import { useEffect, useState } from "react";
import type { Route } from "./+types/root";
import "./app.css";
import styles from "./root.module.css";
import Header from "~/components/header/header";
import logo from "~/resources/icon.svg";
import Button from "~/components/button/button";

export const links: Route.LinksFunction = () => [
  { rel: "preconnect", href: "https://fonts.googleapis.com" },
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
    href: logo,
  },
];

export function Layout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en">
      <head>
        <meta charSet="utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1" />
        <Meta />
        <Links />
      </head>
      <body>
        {children}
        <ScrollRestoration />
        <Scripts />
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
  const BACKEND_URL =
    import.meta.env.VITE_BACKEND_URL ?? "https://localhost:7113";

  useEffect(() => {
    fetch(`${BACKEND_URL}/api/auth/me`, { credentials: "include" })
      .then((res) => res.json())
      .then((data) => {
        setUser(data);
        setLoading(false);
      })
      .catch((err) => {
        console.error("Failed to fetch auth status", err);
        setLoading(false);
      });
  }, [BACKEND_URL]);

  const login = () => {
    window.location.href = `${BACKEND_URL}/api/auth/login`;
  };

  if (loading) {
    return (
      <div className={styles.centeredScreen}>
        <p>Lade...</p>
      </div>
    );
  }

  return (
    <div className={`${styles.layoutWrapper} paper-bg paper`}>
      <Header user={user} />
      <div className={styles.pageContent}>
        {!user?.isAuthenticated ? (
          <div className={styles.centerColumn}>
            <img src={logo} alt="LernZeit Logo" className={styles.logo} />
            <h1 className={styles.title}>Willkommen bei LernZeit</h1>
            <p className={styles.introText}>
              Bitte melde dich an, um deine Lerngruppen und deinen Kalender zu
              verwalten.
            </p>
            <div className={styles.fullButtonContainer}>
              <Button onClick={login}>Mit Google anmelden</Button>
            </div>
          </div>
        ) : (
          <Outlet context={{ user }} />
        )}
      </div>
    </div>
  );
}

export function ErrorBoundary({ error }: Route.ErrorBoundaryProps) {
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
    <main className={styles.errorMain}>
      <h1>{message}</h1>
      <p>{details}</p>
      {stack && (
        <pre className={styles.errorStack}>
          <code>{stack}</code>
        </pre>
      )}
    </main>
  );
}
