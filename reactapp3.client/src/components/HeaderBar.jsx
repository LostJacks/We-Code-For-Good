import React from "react";
import { FaCog, FaSignOutAlt } from "react-icons/fa";
import "../styles/main.css";
import { useNavigate } from "react-router-dom";

export default function HeaderBar() {
    const navigate = useNavigate();
    return (
        <>

            <header className="header-bar">
                <div className="header-left">
                    <div className="profile-circle"></div>
                    <h3>Group One</h3>
                </div>

                <div className="header-right">
                   
                    
                <button className="logout-btn" title="Logout" onClick={() => navigate("/login")}><FaSignOutAlt /> </button>
                
                </div>
            </header>

            <div className="settings-container">
                <button className="settings-btn" title="Settings">
                    <FaCog />
                </button>
            </div>
        </>
    );
}
