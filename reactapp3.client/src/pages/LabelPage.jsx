import React, { useState, useEffect } from "react";
import { CLASS_COLORS } from "@/utils/classColors";
import { useParams } from "react-router-dom";
import { useNavigate } from "react-router-dom";

import LeftSidebar from "@/components/LeftSidebar";
import ImageCanvas from "@/components/ImageCanvas";
import RightSidebar from "@/components/RightSidebar";
import FrameStrip from "@/components/FrameStrip";
import { FaCog } from "react-icons/fa";

import "@/styles/labelPage.css";

const API = import.meta.env.VITE_API_URL;

export default function LabelPage() {
    const navigate = useNavigate();
    const { projectId } = useParams();

    // =========================================
    // 📁 Project & Files State
    // =========================================
    const [project, setProject] = useState(null);
    const [files, setFiles] = useState([]);
    const [loading, setLoading] = useState(true);

    const [settingsOpen, setSettingsOpen] = useState(false);
    const [opticalFlow, setOpticalFlow] = useState(false);
    const [autoAdvance, setAutoAdvance] = useState(false);
    const [multiSelect, setMultiSelect] = useState(false);

    const [selectedBox, setSelectedBox] = useState(null);
    const [cursorColorMode, setCursorColorMode] = useState(false);
    const [showCrosshair, setShowCrosshair] = useState(true);
    const [isolateMode, setIsolateMode] = useState(false);

    // Frame navigation
    const [currentFileIndex, setCurrentFileIndex] = useState(0);

    // =========================================
    // 🧩 Shared label class state
    // =========================================
    const [labelClasses, setLabelClasses] = useState([]);
    const [activeClassIndex, setActiveClassIndex] = useState(0);
    const [boxes, setBoxes] = useState([]);
    const [visibleLabel, setVisibleLabel] = useState(null);

    // Load classes from project data
    useEffect(() => {
        if (project?.labelClasses && project.labelClasses.length > 0) {
            const classesWithColors = project.labelClasses.map((name, i) => ({
                id: i + 1,
                name,
                color: CLASS_COLORS[i % CLASS_COLORS.length],
                shortcut: i === 9 ? "0" : (i + 1).toString(),
            }));
            setLabelClasses(classesWithColors);
        }
    }, [project]);

    const activeClass = labelClasses[activeClassIndex];

    // =========================================
    // ✅ LOAD PROJECT & FILES
    // =========================================
    useEffect(() => {
        const loadProject = async () => {
            try {
                const token = localStorage.getItem('accessToken');
                const res = await fetch(`${API}/api/projects/${projectId}`, {
                    headers: { 'Authorization': `Bearer ${token}` }
                });

                const data = await res.json();
                setProject(data);
                setFiles(data.files);
                console.log('📁 Files loaded:', data.files);
            } catch (err) {
                console.error('❌ Error loading project:', err);
            } finally {
                setLoading(false);
            }
        };

        if (projectId) loadProject();
    }, [projectId]);

    // =========================================
    // 🔢 Keyboard shortcuts for class switching (0–9)
    // =========================================
    useEffect(() => {
        const handleKeyDown = (e) => {
            if (!/^[0-9]$/.test(e.key)) return;
            const num = parseInt(e.key);
            const index = num === 0 ? 9 : num - 1;
            if (index >= 0 && index < labelClasses.length) {
                setActiveClassIndex(index);
            }
        };
        window.addEventListener("keydown", handleKeyDown);
        return () => window.removeEventListener("keydown", handleKeyDown);
    }, [labelClasses]);

    // =========================================
    // 📤 EXPORT HANDLER
    // =========================================
    const handleExport = async () => {
        try {
            const token = localStorage.getItem('accessToken');
            const res = await fetch(`${API}/api/projects/${projectId}/export`, {
                method: 'GET',
                headers: { 'Authorization': `Bearer ${token}` }
            });

            if (!res.ok) throw new Error('Export failed');

            const blob = await res.blob();
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = `project-export-${projectId}.zip`;
            document.body.appendChild(a);
            a.click();
            a.remove();
            window.URL.revokeObjectURL(url);

            console.log('✅ Export successful');
        } catch (err) {
            console.error('❌ Export error:', err);
            alert('Export failed: ' + err.message);
        }
    };

    // =========================================
    // ✅ LOADING STATE
    // =========================================
    if (loading) return <div>Loading project...</div>;
    if (!project) return <div>Project not found</div>;

    return (
        <div className="label-page">
            {/* HEADER */}
            <div className="label-header">
                <div className="header-left">
                    <button
                        className="back-btn"
                        onClick={() => navigate(-1)}
                        title="Back to projects"
                    >
                        ← Back
                    </button>
                    <h2>{project.title}</h2>
                    <span className="autosave-text">AutoSave</span>
                    <label className="autosave-switch">
                        <input type="checkbox" />
                        <span className="autosave-slider"></span>
                    </label>
                </div>

                <div className="header-right">
                    <button className="export-btn" onClick={handleExport}>EXPORT</button>
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
                <LeftSidebar
                    activeClassIndex={activeClassIndex}
                    onSelectClass={setActiveClassIndex}
                    labelClasses={labelClasses}
                    setLabelClasses={setLabelClasses}
                    onFilterChange={setVisibleLabel}
                    onCursorModeChange={setCursorColorMode}
                    onShowCrosshairChange={setShowCrosshair}
                    onIsolateModeChange={setIsolateMode}
                />

                <ImageCanvas
                    activeClass={activeClass}
                    labelClasses={labelClasses}
                    onBoxesChange={setBoxes}
                    onSelectBox={setSelectedBox}
                    visibleLabel={visibleLabel}
                    cursorColorMode={cursorColorMode}
                    files={files}
                    currentFileIndex={currentFileIndex}
                    onFileIndexChange={setCurrentFileIndex}
                    showCrosshair={showCrosshair}
                    isolateMode={isolateMode}
                />

                <RightSidebar activeClass={activeClass} selectedBox={selectedBox} />
            </div>

            {/* FRAME STRIP */}
            <FrameStrip
                files={files}
                currentFileIndex={currentFileIndex}
                onFileIndexChange={setCurrentFileIndex}
            />
        </div>
    );
}