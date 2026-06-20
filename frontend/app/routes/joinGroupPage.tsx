import Input from "~/components/input/input";
import styles from "./joinGroupPage.module.css";
import qrCodeIcon from "~/resources/QR-Code_Icon.svg";
import Button from "~/components/button/button";

export default function JoinGroupPage() {
  function handleJoinGroup() {}

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
      <Input placeholder={"Gruppen ID"} />
      <Button variant="secondary" onClick={handleJoinGroup} centred={true}>
        Beitreten
      </Button>
    </div>
  );
}
