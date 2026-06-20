import { Modal } from "~/components/modal/modal";
import { useState } from "react";
import Input from "~/components/input/input";
import ConfirmBtn from "~/components/confirmBtn/ConfirmBtn";
import styles from "./CreateGroupPopUp.module.css";

export interface CreateGroupPopUpProps {
  isOpen: boolean;
  setIsOpen: React.Dispatch<React.SetStateAction<boolean>>;
}

export default function CreateGroupPopUp({
  isOpen,
  setIsOpen,
}: CreateGroupPopUpProps) {
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
    <Modal
      title="Gruppe erstellen"
      isOpen={isOpen}
      onClose={() => {
        setIsOpen(false);
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
              setIsOpen(false);
              setGroupName("");
            }}
          ></ConfirmBtn>
          <ConfirmBtn
            yes={false}
            onClick={() => {
              setIsOpen(false);
              setGroupName("");
            }}
          ></ConfirmBtn>
        </div>
      </div>
    </Modal>
  );
}
