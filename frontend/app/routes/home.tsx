import type { Route } from "./+types/home";
import plusIcon from "../resources/Plus.svg";
import calendarIcon from "../resources/Calendar.svg";
import qrCode from "../resources/QR_Code.svg";
import Button from "~/components/button/button";
import Icon from "~/components/Icon";
import { NavLink, useOutletContext } from "react-router";
import { Modal } from "~/components/modal/modal";
import { useState } from "react";
import styles from "./home.module.css";

import Input from "~/components/input/input";
import ConfirmBtn from "~/components/confirmBtn/ConfirmBtn";
import GroupCard from "~/components/GroupCard/GroupCard";

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

  const [modalIsOpen, setModalIsOpen] = useState(false);
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
      <Modal
        title="Gruppe erstellen"
        isOpen={modalIsOpen}
        onClose={() => {
          setModalIsOpen(false);
        }}
      >
        <div className={styles.modalContent}>
          <Input
            placeholder="SuperGruppe"
            value={groupName}
            onChange={handleChange}
          ></Input>
          <div className={styles.btnWrapper}>
            <ConfirmBtn
              yes={true}
              onClick={() => {
                confirmGroup;
                setModalIsOpen(false);
              }}
            ></ConfirmBtn>
            <ConfirmBtn
              yes={false}
              onClick={() => {
                setModalIsOpen(false);
              }}
            ></ConfirmBtn>
          </div>
        </div>
      </Modal>
      <div className={styles.verticalStack}>
        <Button
          onClick={() => {
            setModalIsOpen(true);
          }}
          variant="primary"
          icon={Icon(plusIcon, "plus icon")}
        >
          Gruppe erstellen
        </Button>
        <NavLink to="/groups" className="d">
          <Button variant="ghost" icon={Icon(qrCode, "qrcode icon")}>
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
          <GroupCard
            groupName="Mathe"
            members={["Marie", "Peter"]}
            onClick={selectGroup}
          />
        </div>
      </div>
    </>
  );
}
