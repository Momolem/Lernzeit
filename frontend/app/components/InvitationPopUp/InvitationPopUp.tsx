import { Modal } from "~/components/modal/modal";
import { useEffect, useState } from "react";
import styles from "./InvitationPopUp.module.css";
import copyIcon from "~/resources/Copy_Icon.svg";
import Button from "../button/button";
import Icon from "../Icon";
import crossIcon from "~/resources/No_Icon.svg";
import { QRCodeSVG } from "qrcode.react";

export interface InvitationPopUpProps {
  isOpen: boolean;
  setIsOpen: React.Dispatch<React.SetStateAction<boolean>>;
  groupId: string;
  groupName: string;
}

export default function InvitationPopUp({
  isOpen,
  setIsOpen,
  groupId,
  groupName,
}: InvitationPopUpProps) {
  const [isCopied, setIsCopied] = useState(false);
  const inviteLink = "https://iegendwas.lnages/hier/könnte/ihrlinksein";

  function copyLink() {
    navigator.clipboard.writeText(inviteLink);
    setIsCopied(true);
  }

  useEffect(() => {
    // generate QR code
  }, []);

  return (
    <Modal
      isOpen={isOpen}
      onClose={() => {
        setIsOpen(false);
      }}
    >
      <div className={styles.modalContent}>
        <div className={styles.upperFrame}>
          <div className={styles.qrCodeFrame}>
            <QRCodeSVG value={inviteLink} size={200} />
          </div>
          <p className={styles.joinGroupText}>Trete {groupName} bei</p>
          <div className={styles.linkContainer}>
            <div className={styles.linkWindow}>{inviteLink}</div>
            <button
              className={`${styles.iconcopy} ${isCopied ? styles.iconcopyCopied : ""}`}
              onClick={copyLink}
              type="button"
            >
              <img src={copyIcon} alt="Copy Icon" />
            </button>
          </div>
        </div>
        <Button
          variant="secondary"
          centred={true}
          onClick={() => {
            setIsOpen(false);
            setIsCopied(false);
          }}
          icon={Icon(crossIcon, "Cancel")}
          children={undefined}
        />
      </div>
    </Modal>
  );
}
