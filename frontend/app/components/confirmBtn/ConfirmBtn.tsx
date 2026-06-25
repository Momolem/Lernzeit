import styles from "./ConfirmBtn.module.css";
import YesIcon from "~/resources/Yes_Icon.svg";
import NoIcon from "~/resources/No_Icon.svg";
import Icon from "../Icon";

export interface ConfirmBtnProps {
  yes: boolean;
  onClick: () => void;
}
export default function ConfirmBtn({ yes = true, onClick }: ConfirmBtnProps) {
  const className = yes ? "btnYes" : "btnNo";
  const iconSrc: string = yes ? YesIcon : NoIcon;
  return (
    <button
      className={`${styles[className]} ${styles.button}`}
      onClick={onClick}
    >
      {Icon(iconSrc, yes ? "Yes" : "Cancel")}
    </button>
  );
}
