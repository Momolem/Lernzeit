import { Modal } from "~/components/modal/modal";
import { useEffect, useState } from "react";
import Input from "~/components/input/input";
import styles from "./InvitationPopUp.module.css";
import QRCode from "react-qr-code";

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


  const [origin, setOrigin] = useState("");

  useEffect(() => {
    if (typeof window !== "undefined") {
      setOrigin(window.location.origin);
    }
  }, []);

  return (
    <Modal
      isOpen={isOpen}
      onClose={() => {
        setIsOpen(false);
      }}
    >
      <div className={styles.modalContent}>
        <div className={styles.upperWraper}>
          <span className={styles.title}>Trete {groupName} bei</span>
          <div className={styles.qrCodeWrapper}>
            <QRCode value={`${origin}/join/${groupId}`} />
          </div>
          <p className="font-semibold m-2">oder über diesen Link beitreten</p>
          <div>
            <Input value={`${origin}/join/${groupId}`} showCopy />
          </div>
        </div>
      </div>
    </Modal>
  );
}
