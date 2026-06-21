import styles from "./QRcodeBtn.module.css";
import QRcodeIcon from "~/resources/QR-Code_Icon.svg";

export default function QRcodeBtn({ onClick }: { onClick: () => void }) {
  return (
    <button
      className={styles.button}
      onClick={(event) => {
        event.stopPropagation();
        onClick();
      }}
    >
      <img src={QRcodeIcon} alt={"QR Code Icon"} className={styles.icon} />
    </button>
  );
}
