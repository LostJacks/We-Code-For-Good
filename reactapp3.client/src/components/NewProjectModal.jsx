import React, { useState, useEffect } from "react";
import "@/styles/main.css";

export default function NewProjectModal({ isOpen, onClose }) {
    const [projectName, setProjectName] = useState("");
    const [description, setDescription] = useState("");
    const [labels, setLabels] = useState(["T-shirt", "Blazer", "Boots", "Cardigan"]);
    const [newLabel, setNewLabel] = useState("");
    const [multiUser, setMultiUser] = useState(false);

    const [file1, setFile1] = useState(null);
    const [file2, setFile2] = useState(null);
    const [file2Error, setFile2Error] = useState("");

    // ✅ Disable background scroll when modal is open
    useEffect(() => {
        if (isOpen) {
            document.body.classList.add("modal-open");
        } else {
            document.body.classList.remove("modal-open");
        }

        return () => {
            document.body.classList.remove("modal-open");
        };
    }, [isOpen]);

    const addLabel = () => {
        if (newLabel.trim()) {
            setLabels([...labels, newLabel]);
            setNewLabel("");
        }
    };

    const handleFile2Change = (e) => {
        const selectedFile = e.target.files[0];
        if (!selectedFile) return;

        const maxSize = 5 * 1024 * 1024 * 1024;
        if (selectedFile.size > maxSize) {
            setFile2(null);
            setFile2Error("⚠️ File exceeds 5 GB limit. Please choose a smaller file.");
        } else {
            setFile2(selectedFile);
            setFile2Error("");
        }
    };

    if (!isOpen) return null;

    // ✅ Close modal when clicking outside of it
    const handleOverlayClick = (e) => {
        if (e.target.classList.contains("modal-overlay")) {
            onClose();
        }
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

                <label>Project Name *</label>
                <input
                    type="text"
                    value={projectName}
                    onChange={(e) => setProjectName(e.target.value)}
                />

                <label>Project Description</label>
                <textarea
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                ></textarea>

                <div className="upload-container">
                    <div
                        className="upload-box"
                        onClick={() => document.getElementById("fileInput1").click()}
                    >
                        <p>Choose a file or drag & drop it here.</p>
                        <p className="upload-subtext">JPEG, PNG, PDF</p>
                        <button
                            type="button"
                            onClick={(e) => {
                                e.stopPropagation();
                                document.getElementById("fileInput1").click();
                            }}
                        >
                            Browse File
                        </button>
                        <input
                            type="file"
                            id="fileInput1"
                            accept=".jpg,.jpeg,.png,.pdf"
                            style={{ display: "none" }}
                            onChange={(e) => setFile1(e.target.files[0])}
                        />
                        {file1 && <p className="file-name">📄 {file1.name}</p>}
                    </div>

                    <div
                        className="upload-box"
                        onClick={() => document.getElementById("fileInput2").click()}
                    >
                        <p>Choose a file or drag & drop it here.</p>
                        <p className="upload-subtext">Maximum size 5 GB</p>
                        <button
                            type="button"
                            onClick={(e) => {
                                e.stopPropagation();
                                document.getElementById("fileInput2").click();
                            }}
                        >
                            Browse File
                        </button>
                        <input
                            type="file"
                            id="fileInput2"
                            style={{ display: "none" }}
                            onChange={handleFile2Change}
                        />
                        {file2 && <p className="file-name">📄 {file2.name}</p>}
                        {file2Error && <p className="file-error">{file2Error}</p>}
                    </div>
                </div>

                <label>Labels</label>
                <div className="label-list">
                    {labels.map((label, i) => (
                        <div key={i} className="label-item">
                            {label}
                            <span
                                className="label-remove"
                                onClick={() => setLabels(labels.filter((l) => l !== label))}
                            >
                                ×
                            </span>
                        </div>
                    ))}
                </div>

                <div className="label-add">
                    <input
                        type="text"
                        placeholder="Add labels which you would like to use"
                        value={newLabel}
                        onChange={(e) => setNewLabel(e.target.value)}
                    />
                    <button onClick={addLabel}>+</button>
                </div>

                <div className="multiuser-toggle">
                    <input
                        type="checkbox"
                        checked={multiUser}
                        onChange={() => setMultiUser(!multiUser)}
                    />
                    <span>Enable multi-user mode</span>
                </div>

                <button className="modal-done">Done</button>
            </div>
        </div>
    );
}
