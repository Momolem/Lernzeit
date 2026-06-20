import "./input.css";
import type {InputHTMLAttributes} from "react";
import copyIcon from "~/resources/Copy_Icon.svg";


export interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
    fullWidth?: boolean;
    isLoading?: boolean;
    showCopy?: boolean;
}

export default function Input({fullWidth, isLoading, showCopy, ...props}: InputProps) {
    const handleCopy = () => {
        if (props.value) {
            navigator.clipboard.writeText(String(props.value));
        }
    };

    return (
        <div className={`input-wrapper ${fullWidth ? 'w-full' : ''}`}>
            <input
                type="text"
                className={`input ${isLoading ? 'loading' : ''}`}
                {...props}
            />
            {showCopy && (
                <button
                    onClick={handleCopy}
                >
                    <img src={copyIcon} width="24px" alt="Copy Icon" />
                </button>
            )}
        </div>
    );
}
