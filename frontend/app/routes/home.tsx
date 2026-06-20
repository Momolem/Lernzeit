import type { Route } from "./+types/home";
import plusIcon from "../resources/Plus.svg";
import QRcodeIcon from "~/resources/QR-Code_Icon.svg";
import calendarIcon from "../resources/Calendar.svg";
import Button from "~/components/button/button";
import Icon from "~/components/Icon";
import { NavLink, useOutletContext } from "react-router";
import {useEffect, useState} from "react";
import styles from "./home.module.css";

import GroupCard from "~/components/GroupCard/GroupCard";
import CreateGroupPopUp from "~/components/CreateGroupPopUp/CreateGroupPopUp";
import InvitationPopUp from "~/components/InvitationPopUp/InvitationPopUp";
import {apiClient} from "~/api/client";
import type {Group} from "~/types/groups";

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

  const [createModalIsOpen, setCreateModalIsOpen] = useState(false);
  const [inviPopUpIsOpen, setInviPopUpIsOpen] = useState(false);
  const [groupName, setGroupName] = useState("");
  const [groups, setGroups] = useState<Group[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  
  useEffect(() => {
    const fetchGroups = async (): Promise<void> => {
      try {
        setLoading(true);
        const response: Group[] = (await apiClient.getGroups()).data;
        setGroups(response);
      } catch (err: unknown) {
        console.error("Failed to fetch groups:", err);
      } finally {
        setLoading(false);
      }
    };

    fetchGroups();
  }, []);
  

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
        <Button
          onClick={() => {
            setCreateModalIsOpen(true);
          }}
          variant="primary"
          icon={Icon(plusIcon, "plus icon")}
        >
          Gruppe erstellen
        </Button>
        <NavLink to="/groups" className="d">
          <Button variant="ghost" icon={Icon(QRcodeIcon, "qrcode icon")}>
            Gruppe beitreten
          </Button>
        </NavLink>
        <NavLink to="/calendar" className="d">
          <Button variant="ghost" icon={Icon(calendarIcon, "calendar icon")}>
            Mein Kalender
          </Button>
        </NavLink>
        <p className={styles.meineGruppen}>Meine Gruppen</p>
        <div className={styles.groupWrapper}>
          {loading && <p>Lade Gruppen...</p>}
          {!loading &&
              groups.map((group: Group) => (
                  <GroupCard
                      key={group.id}
                      groupId={group.id}
                      groupName={group.name}
                      members={group.members}
                      onClick={selectGroup}
                      handleQRCodeClick={() => setInviPopUpIsOpen(true)}
                  />
              ))}
        </div>
      </div>
    </>
  );
}
