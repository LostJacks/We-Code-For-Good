import React, { useState, useEffect } from "react";
import { FaCog } from "react-icons/fa";
import "../styles/main.css";
import { FaSignOutAlt } from "react-icons/fa";
import { useNavigate } from "react-router-dom";
import { getUserFromToken } from "../services/UserService";

export default function HeaderBar() {
    const navigate = useNavigate();
    const [user, setUser] = useState(null);

    useEffect(() => {
        const userData = getUserFromToken();
        setUser(userData);
    }, []);

    return (
        <>
            {/* RED HEADER */}
            <header className="header-bar">
                <div className="header-left">
                    <div className="profile-circle"></div>
                    <h3>{user ? `${user.firstName} ${user.lastName}` : "Group One"}</h3>
                </div>

                {/* Gear inside red header */}
                <div className="header-right">
                     <button className="logout-btn" title="Logout" onClick={() => navigate("/login")}><FaSignOutAlt /> </button>
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
