import "./input.css"
import type {InputHTMLAttributes, ReactNode} from "react";
export interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
    fullWidth?: boolean;
    isLoading?: boolean;
}

export default function Input({...props} : InputProps) {
    return (
        <input className={`input-wrapper`} {...props}>
        </input>
    );
}