import Input from "~/components/input/input";
import styles from "./joinGroupPage.module.css";
import qrCodeIcon from "~/resources/QR-Code_Icon.svg";
import Button from "~/components/button/button";
import { useNavigate } from "react-router";
import { apiClient } from "~/api/client";
import { useState } from "react";

export default function JoinGroupPage() {
  const [joinSuccess, setJoinSuccess] = useState<boolean>(false);
  const [input, setInput] = useState<string>("");
  const navigate = useNavigate();

  function handleJoinGroup() {
    const groupId = input;

    apiClient
      .joinGroup(groupId)
      .then((hasJoined) => {
        setJoinSuccess(hasJoined);
        navigate(`/calendar?id=${groupId}`);
      })
      .catch((err) => {
        console.error("Group join error:", err);
        setJoinSuccess(false);
      });
  }

  return (
    <div className={styles.buttonContainer}>
      <div className={styles.qrCodeButton}>
        <div className={styles.iconFrame}>
          <img src={qrCodeIcon} className={styles.qrCodeIcon} alt="" />
        </div>
        <div className={styles.homepage}>
          <p className={styles.qrCodeButtonText}>QR Code scannen</p>
          <p className={styles.openCamText}>Kamera öffnen</p>
        </div>
      </div>
      <div className={styles.oderParent}>
        <div className={styles.oder}>oder</div>
        <img className={styles.groupChild} alt="" />
      </div>
      <Input
        value={input}
        onChange={(e) => setInput(e.target.value)}
        placeholder={"Gruppen ID"}
      />
      <Button variant="secondary" onClick={handleJoinGroup} centred={true}>
        Beitreten
      </Button>
    </div>
  );
}
