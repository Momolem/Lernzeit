import { Modal } from "~/components/modal/modal";
import { useEffect, useState } from "react";
import Input from "~/components/input/input";
import ConfirmBtn from "~/components/confirmBtn/ConfirmBtn";
import styles from "./InvitationPopUp.module.css";
import qrCodeExample from "~/resources/qr_code_example.png";
import copyIcon from "~/resources/Copy_Icon.svg";
import Button from "../button/button";
import Icon from "../Icon";
import crossIcon from "~/resources/No_Icon.svg";

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
  // const [qrCode, setQRcode] = useState(null);

  function confirmGroup() {
    //  requ to backend
    // create group comp in home
  }

  function selectGroup() {}

  function handleChange(e: any) {
    setGroupName(e.target.value);
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
        <div className={styles.upperWraper}>
          <div className={styles.qrCodeWrapper}>
            <img src={qrCodeExample} alt="QR Code" />
          </div>
          <p>Trete {groupName} bei</p>
          <div>
            <Input />
            <button>
              <img src={copyIcon} alt="Copy Icon" />
            </button>
          </div>
        </div>
        <Button variant="primary" icon={Icon(crossIcon, "Cancel")} />
      </div>
    </Modal>
  );
}
