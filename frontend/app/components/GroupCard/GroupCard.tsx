import styles from "./GroupCard.module.css";
import QRcodeBtn from "../QRcodeBtn/QRcodeBtn";

export interface GroupCardProps {
  groupId: number;
  groupName: string;
  members: Array<string>;
  onClick: () => void;
  handleQRCodeClick: () => void;
}
export default function GroupCard({
  groupId,
  groupName,
  members,
  onClick: onClick,
  handleQRCodeClick: onQRcodeClick,
}: GroupCardProps) {
  function handleQRCode() {}

  function handleShowGroup() {}

  return (
    <div className={styles.card} onClick={onClick}>
      <div className={styles.header}>
        <div className={styles.title}>{groupName}</div>
        <QRcodeBtn onClick={onQRcodeClick} />
      </div>
      {members.map((m, index) => (
        <div key={index} className={styles.nameLabel}>
          <span>{m}</span>
        </div>
      ))}
    </div>
  );
}
