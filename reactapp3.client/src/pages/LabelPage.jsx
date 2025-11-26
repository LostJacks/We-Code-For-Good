import React, { useState } from "react";

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

    // Frame navigation
    const [currentFrame, setCurrentFrame] = useState(11);
    const totalFrames = 180;

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
                                    onChange={(e) =>
                                        setOpticalFlow(e.target.checked)
                                    }
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
                                    onChange={(e) =>
                                        setAutoAdvance(e.target.checked)
                                    }
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
                                    onChange={(e) =>
                                        setMultiSelect(e.target.checked)
                                    }
                                />
                                <span className="settings-slider"></span>
                            </label>
                        </div>
                    </div>
                )}
            </div>

            {/* MAIN CONTENT */}
            <div className="label-content">
                <LeftSidebar />
                <ImageCanvas />
                <RightSidebar />
            </div>

            {/* ✅ FRAME STRIP COMPONENT */}
            <FrameStrip
                currentFrame={currentFrame}
                totalFrames={totalFrames}
            />
        </div>
    );
}
