import React, { useState } from "react";
import { CLASS_COLORS } from "@/utils/classColors";

export default function LeftSidebar() {
    const [classes, setClasses] = useState([
        "Fur Jacket",
        "T-Shirt",
        "Blazer",
        "Boots",
        "Cardigan"
    ]);

    const addClass = () => {
        const newName = prompt("Enter class name:");
        if (!newName) return;
        setClasses([...classes, newName]);
    };

    const getColor = (index) => CLASS_COLORS[index % CLASS_COLORS.length];

    return (
        <div className="left-sidebar">
            <h3 className="sidebar-title">Classes</h3>

            <div className="search-wrapper">
                <input className="search-input" placeholder="Search" />
                <i className="fa-solid fa-magnifying-glass search-icon"></i>
            </div>

            <div className="class-list">
                {classes.map((name, index) => (
                    <div className="class-item" key={index}>
                        <div className="class-left">
                            <span
                                className="class-color"
                                style={{ backgroundColor: getColor(index) }}
                            ></span>
                            <span>{name}</span>
                        </div>
                        <span className="class-count">{index + 1}</span>
                    </div>
                ))}
            </div>

            <button className="add-class-btn" onClick={addClass}>
                + Add class
            </button>

            <div className="options-section">
                <div className="option-row">
                    Cursor = class color
                    <input type="checkbox" />
                </div>

                <div className="option-row">
                    Crosshair
                    <label className="toggle-switch">
                        <input type="checkbox" />
                        <span className="slider"></span>
                    </label>
                </div>

                <div className="option-row">
                    Isolate box (fade others)
                    <input type="checkbox" />
                </div>
            </div>



        </div>
    );
}
