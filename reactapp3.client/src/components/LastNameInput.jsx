import React from "react";

export default function LastNameInput({ value, onChange }) {
    return (
        <div className="name-box small-input">
            <label htmlFor="lastName" className="name-text">Last Name</label>
            <div className="name-icon">
                
            </div>
            <input
                id="lastName"
                name="lastName"
                type="text"
                className="name-input"
               
                autoComplete="family-name"
                value={value}
                onChange={onChange}
                required
            />
        </div>
    );
}