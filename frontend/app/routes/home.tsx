import type { Route } from "./+types/home";
import {useEffect, useState} from "react";
import plusIcon from '../resources/Plus.svg';
import calendarIcon from '../resources/Calendar.svg';
import qrCode from '../resources/QR_Code.svg';
import Button from "~/components/button/button";
import Icon from "~/components/Icon";
import {TimetableComponent} from "~/components/Timetable/Timetable";
import type {TimetableEvents} from "~/types/timetable";
import {Link, NavLink} from "react-router";


interface User {
  isAuthenticated: boolean;
  name?: string;
  email?: string;
  avatar?: string;
}

export function meta({}: Route.MetaArgs) {
  return [
    { title: "LernZeit." },
    { name: "description", content: "Finde Zeit zum Lernen" },
  ];
}

export default function Home() {
  const [user, setUser] = useState<User | null>(null);
  const BACKEND_URL = import.meta.env.REACT_APP_BACKEND_URL ?? "https://localhost:7113";

  useEffect(() => {
    // Check if user is authenticated on load
    fetch(`${BACKEND_URL}/api/auth/me`, { credentials: 'include' })
        .then(res => res.json())
        .then(data => setUser(data))
        .catch(err => console.error("Failed to fetch auth status", err));
  }, []);

  const login = () => {
    // Redirect to backend login endpoint
    window.location.href = `${BACKEND_URL}/api/auth/login`;
  };

  const logout = () => {
    fetch(`${BACKEND_URL}/api/auth/logout`, { credentials: 'include' })
        .then(() => setUser({ isAuthenticated: false }))
        .catch(err => console.error("Logout failed", err));
  };

  return (
      <>
        <div className="flex gap-4 flex-col">
          <Button variant="primary" icon={Icon(plusIcon, "plus icon")}>Gruppe erstellen</Button>
          <Button variant="ghost" icon={Icon(qrCode, "qrcode icon")}>Gruppe beitreten</Button>
          <NavLink to="/calendar" className="d"><Button variant="ghost" icon={Icon(calendarIcon, "calendar icon")}>Mein Kalender</Button></NavLink> 
        </div>

          {user?.isAuthenticated ? (
              <div>
                <p>Welcome, {user.name}!</p>
                {user.avatar && <img src={user.avatar} alt="avatar" style={{ borderRadius: '50%', width: '50px' }} />}
                <Button onClick={logout}>Logout</Button>
              </div>
          ) : (
              <div>
                <p>You are not logged in.</p>
                <Button onClick={login}>Login with Google</Button>
              </div>
          )}
      </>
  )
}
