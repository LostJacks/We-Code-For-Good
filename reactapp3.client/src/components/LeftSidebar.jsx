import React, { useEffect, useState } from "react";
import { CLASS_COLORS } from "@/utils/classColors";

export default function LeftSidebar({
    activeClassIndex,
    onSelectClass,
    labelClasses,
    setLabelClasses,
    onFilterChange,
    onCursorModeChange,
    onShowCrosshairChange,
    onIsolateModeChange,
}) {
    const [searchTerm, setSearchTerm] = useState("");

    // --- Handle number key shortcuts globally ---
    useEffect(() => {
        const handleKeyDown = (e) => {
            const idx = parseInt(e.key) - 1;
            if (idx >= 0 && idx < labelClasses.length) {
                onSelectClass(idx);
            } else if (e.key === "0" && labelClasses.length >= 10) {
                onSelectClass(9); // 0 selects 10th class
            }
        };
        window.addEventListener("keydown", handleKeyDown);
        return () => window.removeEventListener("keydown", handleKeyDown);
    }, [labelClasses, onSelectClass]);

    // --- Send filter keyword up to LabelPage ---
    useEffect(() => {
        if (onFilterChange) {
            if (searchTerm.trim() === "") {
                onFilterChange(null); // show all
            } else {
                onFilterChange(searchTerm.toLowerCase()); // send lowercase for easier match
            }
        }
    }, [searchTerm, onFilterChange]);

    // --- Add new label class ---
    const addClass = () => {
        const newName = prompt("Enter class name:");
        if (!newName) return;

        const newColor =
            CLASS_COLORS[labelClasses.length % CLASS_COLORS.length];

        const shortcut =
            labelClasses.length < 9
                ? `${labelClasses.length + 1}` // 1–9
                : labelClasses.length === 9
                    ? "0" // 10th shortcut is 0
                    : "-"; // after 0, no shortcut

        const newId = Math.max(...labelClasses.map(c => c.id || 0), 0) + 1;

        setLabelClasses([
            ...labelClasses,
            {
                id: newId,
                name: newName,
                color: newColor,
                shortcut
            },
        ]);
    };

    return (
        <div className="left-sidebar">
            <h3 className="sidebar-title">Classes</h3>

            {/* ✅ SEARCH FIELD — filters boxes */}
            <div className="search-wrapper">
                <input
                    className="search-input"
                    placeholder="Search class..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                />
                <i className="fa-solid fa-magnifying-glass search-icon"></i>
            </div>

            {/* ✅ CLASS LIST */}
            <div className="class-list">
                {labelClasses.map((cls, index) => (
                    <div
                        key={index}
                        className={`class-item ${activeClassIndex === index ? "active-class" : ""
                            }`}
                        onClick={() => onSelectClass(index)}
                    >
                        <div className="class-left">
                            <span
                                className="class-color"
                                style={{ backgroundColor: cls.color }}
                            ></span>
                            <span>{cls.name}</span>
                        </div>
                        <span className="class-count">
                            {cls.shortcut || index + 1}
                        </span>
                    </div>
                ))}
            </div>

            {/* ADD BUTTON */}
            <button className="add-class-btn" onClick={addClass}>
                + Add class
            </button>

            {/* OPTIONS */}
            <div className="options-section">
                {/* ✅ Cursor color toggle */}
                <div className="option-row">
                    <span>Cursor = class color</span>
                    <label className="toggle-switch">
                        <input
                            type="checkbox"
                            onChange={(e) => onCursorModeChange && onCursorModeChange(e.target.checked)}
                        />
                        <span className="slider"></span>
                    </label>
                </div>

                {/* ✅ Crosshair toggle */}
                <div className="option-row">
                    <span>Crosshair</span>
                    <label className="toggle-switch">
                        <input
                            type="checkbox"
                            defaultChecked
                            onChange={(e) => onShowCrosshairChange && onShowCrosshairChange(e.target.checked)}
                        />
                        <span className="slider"></span>
                    </label>
                </div>

                {/* ✅ Isolate box toggle */}
                <div className="option-row">
                    <span>Isolate box (fade others)</span>
                    <label className="toggle-switch">
                        <input
                            type="checkbox"
                            onChange={(e) => onIsolateModeChange && onIsolateModeChange(e.target.checked)}
                        />
                        <span className="slider"></span>
                    </label>
                </div>
            </div>
        </div>
    );
}