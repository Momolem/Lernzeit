import type { Route } from "./+types/home";
import plusIcon from "../resources/Plus.svg";
import QRcodeIcon from "~/resources/QR-Code_Icon.svg";
import calendarIcon from "../resources/Calendar.svg";
import Button from "~/components/button/button";
import Icon from "~/components/Icon";
import { NavLink, useOutletContext } from "react-router";
import { Modal } from "~/components/modal/modal";
import { useState } from "react";
import styles from "./home.module.css";

import Input from "~/components/input/input";
import ConfirmBtn from "~/components/confirmBtn/ConfirmBtn";
import GroupCard from "~/components/GroupCard/GroupCard";
import CreateGroupPopUp from "~/components/CreateGroupPopUp/CreateGroupPopUp";
import InvitationPopUp from "~/components/InvitationPopUp/InvitationPopUp";

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
  console.log("user", user);

  const [createModalIsOpen, setCreateModalIsOpen] = useState(false);
  const [inviPopUpIsOpen, setInviPopUpIsOpen] = useState(false);
  const [groupName, setGroupName] = useState("");

  function confirmGroup() {
    //  requ to backend
    // create group comp in home
  }

  function selectGroup() {}

  function handleChange(e: any) {
    setGroupName(e.target.value);
  }

  return (
    <>
      <InvitationPopUp
        groupId="4"
        groupName="TestGruppe"
        setIsOpen={setInviPopUpIsOpen}
        isOpen={inviPopUpIsOpen}
      />
      <CreateGroupPopUp
        isOpen={createModalIsOpen}
        setIsOpen={setCreateModalIsOpen}
      />
      <div className={styles.verticalStack}>
        <div className={styles.buttonStack}>
          <Button
            onClick={() => {
              setCreateModalIsOpen(true);
            }}
            variant="primary"
            icon={Icon(plusIcon, "plus icon")}
          >
            Gruppe erstellen
          </Button>
          <NavLink to="/joinGroup" className={styles.navLink}>
            <Button variant="ghost" icon={Icon(QRcodeIcon, "qrcode icon")}>
              Gruppe beitreten
            </Button>
          </NavLink>
          <NavLink to="/calendar" className={styles.navLink}>
            <Button variant="ghost" icon={Icon(calendarIcon, "calendar icon")}>
              Mein Kalender
            </Button>
          </NavLink>
        </div>

        <p className={styles.meineGruppen}>Meine Gruppen</p>
        <div className={styles.groupWrapper}>
          <GroupCard
            groupId={1}
            groupName="Mathe"
            members={["Marie", "Peter"]}
            onClick={selectGroup}
            handleQRCodeClick={() => setInviPopUpIsOpen(true)}
          />
        </div>
      </div>
    </>
  );
}
