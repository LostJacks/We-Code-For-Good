import React from "react";

export default function LoginButton({ onClick }) {
    return (
        <div className="login-button-box">
            <button type="submit" className="login-button" onClick={onClick}>
                Log In
            </button>
        </div>
    );
}
