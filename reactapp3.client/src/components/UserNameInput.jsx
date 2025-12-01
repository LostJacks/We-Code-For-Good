import React from "react";
import "../styles/main.css";

export default function UserNameInput({ value, onChange }) {
    return (
        <div className="name-box small-input">
            <label htmlFor="userName" className="name-text">Username</label>
            <div className="name-icon">

            </div>
            <input
                id="userName"
                name="userName"
                type="text"
                className="name-input"
                autoComplete="username"
                value={value}
                onChange={onChange}
                required
            />
        </div>
    );
}
