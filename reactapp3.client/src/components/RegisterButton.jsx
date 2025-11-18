import React from "react";

export default function RegisterButton({ onClick }) {
    return (
        <div className="register-button-box">
            <button type="submit" className="register-button" onClick={onClick}>
                Register
            </button>
        </div>
    );
}