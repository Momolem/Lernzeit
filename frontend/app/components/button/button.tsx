import "./button.css"
import type {ButtonHTMLAttributes, ReactNode} from "react";

export type ButtonVariant = "primary" | "secondary" | "ghost";
export type ButtonSize = "sm" | "md" | "lg";

export interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
    children: ReactNode;
    variant?: ButtonVariant;
    icon?: ReactNode;
    size?: ButtonSize;
    fullWidth?: boolean;
    isLoading?: boolean;
}

export default function Button({variant = "primary", icon, children, ...props} : ButtonProps) {
    return (
        <button className={`btn-wrapper btn-${variant}`} {...props}>
            {icon && (
                <span className="btn-icon-wrapper">
                    {icon}
                </span>
            )}
            <span className="btn-content">
                {children}
            </span>
        </button>
    );
}