import React from "react";
import { FaCog } from "react-icons/fa";
import "../styles/main.css";

export default function HeaderBar() {
    return (
        <>
            {/* RED HEADER */}
            <header className="header-bar">
                <div className="header-left">
                    <div className="profile-circle"></div>
                    <h3>Group One</h3>
                </div>

                {/* Gear inside red header */}
                <div className="header-right">
                    <button className="settings-btn" title="Settings">
                        <FaCog />
                    </button>
                </div>
            </header>

            {/* FILTER BUTTON BELOW HEADER */}
            <div className="filter-container">
                <button className="filter-box">FILTER</button>
            </div>
        </>
    );
}
