import styles from "./GroupCard.module.css";
import qrCodeIcon from "~/resources/QR-Code_Icon.svg";
import Icon from "../Icon";
// icon qrcode

export interface GroupCardProps {
  groupName: string;
  members: Array<string>;
  onClick: () => void;
}
export default function GroupCard({
  groupName,
  members,
  onClick,
}: GroupCardProps) {
  return (
    <div className={styles.card}>
      <div className={styles.header}>
        <div className={styles.title}>Mathe</div>
      </div>
      {members.map(() => (
        <div className={styles.marieWrapper}>
          <p className={styles.marie}>Marie</p>
        </div>
      ))}
    </div>
  );
}
