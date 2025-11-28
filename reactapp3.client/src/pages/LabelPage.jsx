import React, { useState, useEffect } from "react";
import { CLASS_COLORS } from "@/utils/classColors";

import LeftSidebar from "@/components/LeftSidebar";
import ImageCanvas from "@/components/ImageCanvas";
import RightSidebar from "@/components/RightSidebar";
import FrameStrip from "@/components/FrameStrip";
import { FaCog } from "react-icons/fa";

import "@/styles/labelPage.css";

export default function LabelPage() {
    const [settingsOpen, setSettingsOpen] = useState(false);
    const [opticalFlow, setOpticalFlow] = useState(false);
    const [autoAdvance, setAutoAdvance] = useState(false);
    const [multiSelect, setMultiSelect] = useState(false);

    const [selectedBox, setSelectedBox] = useState(null); // ✅ currently selected box
    const [cursorColorMode, setCursorColorMode] = useState(false); // ✅ toggle for cursor color

    // Frame navigation
    const [currentFrame, setCurrentFrame] = useState(11);
    const totalFrames = 180;

    // =========================================
    // 🧩 Shared label class state
    // =========================================
    const initialClasses = [
        "Fur Jacket",
        "T-Shirt",
        "Blazer",
        "Boots",
        "Cardigan",
    ].map((name, i) => ({
        name,
        color: CLASS_COLORS[i % CLASS_COLORS.length],
        shortcut: i === 9 ? "0" : (i + 1).toString(), // 1–9, then 0
    }));

    const [labelClasses, setLabelClasses] = useState(initialClasses);
    const [activeClassIndex, setActiveClassIndex] = useState(0);
    const [boxes, setBoxes] = useState([]);

    // ✅ Search / Filter State
    const [visibleLabel, setVisibleLabel] = useState(null);

    const activeClass = labelClasses[activeClassIndex];

    // =========================================
    // 🔢 Keyboard shortcuts for class switching (0–9)
    // =========================================
    useEffect(() => {
        const handleKeyDown = (e) => {
            if (!/^[0-9]$/.test(e.key)) return;
            const num = parseInt(e.key);
            const index = num === 0 ? 9 : num - 1; // 0 → 10th class
            if (index >= 0 && index < labelClasses.length) {
                setActiveClassIndex(index);
            }
        };
        window.addEventListener("keydown", handleKeyDown);
        return () => window.removeEventListener("keydown", handleKeyDown);
    }, [labelClasses]);

    return (
        <div className="label-page">
            {/* HEADER */}
            <div className="label-header">
                <div className="header-left">
                    <span className="autosave-text">AutoSave</span>
                    <label className="autosave-switch">
                        <input type="checkbox" />
                        <span className="autosave-slider"></span>
                    </label>
                </div>

                <div className="header-right">
                    <button className="export-btn">EXPORT</button>
                    <button
                        className="settings-btn"
                        title="Settings"
                        onClick={() => setSettingsOpen((o) => !o)}
                    >
                        <FaCog className="settings-icon" />
                    </button>
                </div>

                {settingsOpen && (
                    <div className="settings-dropdown">
                        <div className="settings-dropdown-header">
                            <span className="settings-title">Settings</span>
                            <button
                                className="settings-close"
                                onClick={() => setSettingsOpen(false)}
                            >
                                ×
                            </button>
                        </div>

                        <div className="settings-row">
                            <span>Optical Flow Prediction</span>
                            <label className="settings-switch">
                                <input
                                    type="checkbox"
                                    checked={opticalFlow}
                                    onChange={(e) => setOpticalFlow(e.target.checked)}
                                />
                                <span className="settings-slider"></span>
                            </label>
                        </div>

                        <div className="settings-row">
                            <span>Auto-Advance After Submit</span>
                            <label className="settings-switch">
                                <input
                                    type="checkbox"
                                    checked={autoAdvance}
                                    onChange={(e) => setAutoAdvance(e.target.checked)}
                                />
                                <span className="settings-slider"></span>
                            </label>
                        </div>

                        <div className="settings-row">
                            <span>Multi-Select Mode</span>
                            <label className="settings-switch">
                                <input
                                    type="checkbox"
                                    checked={multiSelect}
                                    onChange={(e) => setMultiSelect(e.target.checked)}
                                />
                                <span className="settings-slider"></span>
                            </label>
                        </div>
                    </div>
                )}
            </div>

            {/* MAIN CONTENT */}
            <div className="label-content">
                {/* ✅ Left Sidebar */}
                <LeftSidebar
                    activeClassIndex={activeClassIndex}
                    onSelectClass={setActiveClassIndex}
                    labelClasses={labelClasses}
                    setLabelClasses={setLabelClasses}
                    onFilterChange={setVisibleLabel}
                    onCursorModeChange={setCursorColorMode} // ✅ toggle cursor color
                />

                {/* ✅ Image Canvas */}
                <ImageCanvas
                    activeClass={activeClass}
                    labelClasses={labelClasses}
                    onBoxesChange={setBoxes}
                    onSelectBox={setSelectedBox} // ✅ new callback for selection
                    visibleLabel={visibleLabel}
                    cursorColorMode={cursorColorMode}
                />

                {/* ✅ Right Sidebar with selected box details */}
                <RightSidebar activeClass={activeClass} selectedBox={selectedBox} />
            </div>

            {/* FRAME STRIP */}
            <FrameStrip currentFrame={currentFrame} totalFrames={totalFrames} />
        </div>
    );
}
