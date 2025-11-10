import React from "react";
import "../styles/main.css";

export default function AddProjectCard({ onClick }) {
    return (
        <div className="add-card" onClick={onClick}>
            <span className="add-plus">+</span>
        </div>
    );
}
