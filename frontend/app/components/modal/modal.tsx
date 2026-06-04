import React from "react";
import styles from "./modal.module.css";

export interface ModalProps {
    isOpen: boolean;
    onClose: () => void;
    title?: string;
    children: React.ReactNode;
}

export function Modal({ isOpen, onClose, title, children }: ModalProps) {
    if (!isOpen) return null;

    return (
        <div className={styles.modalOverlay} onClick={onClose}>
            <div className={styles.modalWrapper}>
                <div className={styles.modalBgLayer}>
                    <div className={styles.modal} onClick={(e) => e.stopPropagation()}>
                        {title && <h2 className={styles.modalTitle}>{title}</h2>}
                        {children}
                    </div>
                </div>
            </div>
        </div>
    );
}
