import React from "react";

export default function FirstNameInput({ value, onChange }) {
    return (
        <div className="name-box small-input">
            <label htmlFor="firstName" className="name-text">First Name</label>
            <div className="name-icon">
                
            </div>
            <input
                id="firstName"
                name="firstName"
                type="text"
                className="name-input"
                
                autoComplete="given-name"
                value={value}
                onChange={onChange}
                required
            />
        </div>
    );
}