import type { Route } from "./+types/home";
import plusIcon from '../resources/Plus.svg';
import calendarIcon from '../resources/Calendar.svg';
import qrCode from '../resources/QR_Code.svg';
import Button from "~/components/button/button";
import Icon from "~/components/Icon";
import { NavLink, useOutletContext } from "react-router";

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
  const { user } = useOutletContext<{ user: User }>();

  return (
      <>
        <div className="flex gap-4 flex-col mb-8">
          <Button variant="primary" icon={Icon(plusIcon, "plus icon")}>Gruppe erstellen</Button>
          <Button variant="ghost" icon={Icon(qrCode, "qrcode icon")}>Gruppe beitreten</Button>
          <NavLink to="/calendar" className="d"><Button variant="ghost" icon={Icon(calendarIcon, "calendar icon")}>Mein Kalender</Button></NavLink> 
        </div>
      </>
  )
}
