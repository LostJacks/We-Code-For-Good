import React, { useState, useEffect } from "react";
import "@/styles/main.css";

const API = import.meta.env.VITE_API_URL || "";

export default function NewProjectModal({ isOpen, onClose, onCreated }) {
    const [projectName, setProjectName] = useState("");
    const [description, setDescription] = useState("");
    const [labels, setLabels] = useState([]);
    const [newLabel, setNewLabel] = useState("");
    const [files, setFiles] = useState([]);
    const [folderFiles, setFolderFiles] = useState([]);
    const [folderError, setFolderError] = useState("");
    const [loading, setLoading] = useState(false);

    // Toggle body scroll lock when modal is open
    useEffect(() => {
        document.body.classList.toggle("modal-open", isOpen);
        return () => document.body.classList.remove("modal-open");
    }, [isOpen]);

    if (!isOpen) return null;

    // ✅ Add label (no duplicates)
    const addLabel = () => {
        const trimmed = newLabel.trim();
        if (trimmed && !labels.includes(trimmed)) {
            setLabels([...labels, trimmed]);
        }
        setNewLabel("");
    };

    // File and folder handlers
    const handleFileChange = (e) => setFiles(Array.from(e.target.files));

    const handleFolderChange = (e) => {
        const selectedFiles = Array.from(e.target.files);
        const totalSize = selectedFiles.reduce((sum, f) => sum + f.size, 0);
        const maxSize = 5 * 1024 * 1024 * 1024; // 5 GB

        if (totalSize > maxSize) {
            setFolderFiles([]);
            setFolderError("⚠️ Folder exceeds 5 GB total limit.");
        } else {
            setFolderFiles(selectedFiles);
            setFolderError("");
        }
    };

    // ✅ Submit project
    const handleSubmit = async () => {
        if (!projectName.trim()) {
            alert("Please enter a project name!");
            return;
        }

        try {
            setLoading(true);

            // ✅ Step 1 — Create project (send correct DTO structure)
            const newProject = {
                Title: projectName,
                Description: description,
                Labels: labels, // ✅ matches backend DTO (List<string>)
            };

            const createRes = await fetch(`${API}/api/projects`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(newProject),
            });

            if (!createRes.ok) throw new Error("Failed to create project");
            const createdProject = await createRes.json();

            // ✅ Step 2 — Upload files & folders
            const allFiles = [...files, ...folderFiles];
            for (const file of allFiles) {
                const formData = new FormData();
                formData.append("file", file);
                formData.append("projectId", createdProject.id);

                await fetch(`${API}/api/projects/upload`, {
                    method: "POST",
                    body: formData,
                });
            }

            // ✅ Step 3 — Reset modal & trigger refresh
            setProjectName("");
            setDescription("");
            setLabels([]);
            setFiles([]);
            setFolderFiles([]);
            setNewLabel("");
            onClose();
            onCreated?.();
        } catch (err) {
            console.error("❌ Error:", err);
            alert("Something went wrong while creating the project.");
        } finally {
            setLoading(false);
        }
    };

    // Close modal when clicking outside content
    const handleOverlayClick = (e) => {
        if (e.target.classList.contains("modal-overlay")) onClose();
    };

    return (
        <div className="modal-overlay" onClick={handleOverlayClick}>
            <div className="modal" onClick={(e) => e.stopPropagation()}>
                <button className="modal-close" onClick={onClose}>
                    ✕
                </button>

                <h2 className="modal-title">
                    Upload a <span>New Project</span>
                </h2>
                <p className="modal-subtitle">Please provide necessary details below</p>

                {/* === Project Info === */}
                <label>Project Name *</label>
                <input
                    type="text"
                    value={projectName}
                    onChange={(e) => setProjectName(e.target.value)}
                    placeholder="Enter project title"
                />

                <label>Project Description</label>
                <textarea
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                    placeholder="Describe your project (optional)"
                ></textarea>

                {/* === Upload Boxes === */}
                <div className="upload-container">
                    {/* File Upload */}
                    <div className="upload-box">
                        <p>
                            Select <strong>images or videos</strong> for your project.
                        </p>
                        <p className="upload-subtext">Supported: JPG, PNG, MP4, MOV</p>
                        <button
                            type="button"
                            onClick={() => document.getElementById("fileInput").click()}
                        >
                            Browse Files
                        </button>
                        <input
                            type="file"
                            id="fileInput"
                            accept="image/*,video/*"
                            multiple
                            style={{ display: "none" }}
                            onChange={handleFileChange}
                        />
                        {files.length > 0 && (
                            <p className="file-name">📁 {files.length} file(s) selected</p>
                        )}
                    </div>

                    {/* Folder Upload */}
                    <div className="upload-box">
                        <p>
                            Select or drop an entire <strong>folder</strong>.
                        </p>
                        <p className="upload-subtext">Max total folder size: 5 GB</p>
                        <button
                            type="button"
                            onClick={() => document.getElementById("folderInput").click()}
                        >
                            Browse Folder
                        </button>
                        <input
                            type="file"
                            id="folderInput"
                            webkitdirectory="true"
                            directory=""
                            accept="image/*,video/*"
                            style={{ display: "none" }}
                            onChange={handleFolderChange}
                        />
                        {folderFiles.length > 0 && (
                            <p className="file-name">
                                📂 {folderFiles.length} file(s) selected
                            </p>
                        )}
                        {folderError && <p className="file-error">{folderError}</p>}
                    </div>
                </div>

                {/* === Labels === */}
                <label>Labels</label>
                <div className="label-list">
                    {labels.map((label, i) => (
                        <div key={i} className="label-item">
                            {label}
                            <span
                                className="label-remove"
                                onClick={() =>
                                    setLabels(labels.filter((l) => l !== label))
                                }
                            >
                                ×
                            </span>
                        </div>
                    ))}
                </div>

                <div className="label-add">
                    <input
                        type="text"
                        placeholder="Add labels for this project"
                        value={newLabel}
                        onChange={(e) => setNewLabel(e.target.value)}
                        onKeyDown={(e) => e.key === "Enter" && addLabel()}
                    />
                    <button onClick={addLabel}>+</button>
                </div>

                {/* === Submit Button === */}
                <div className="modal-done-container">
                    <button
                        className="modal-done"
                        onClick={handleSubmit}
                        disabled={loading}
                    >
                        {loading ? "Uploading..." : "Done"}
                    </button>
                </div>
            </div>
        </div>
    );
}
