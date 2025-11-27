import React from "react";

export default function ConfirmPassword({ value, onChange }) {
    return (
        <div className="password-box">
            <label htmlFor="confirmPassword" className="password-text">Confirm Password</label>
            <input
                id="confirmPassword"
                name="confirmPassword"
                type="password"
                className="password-input"
                autoComplete="new-password"
                value={value}
                onChange={onChange}
            />
        </div>
    );
}