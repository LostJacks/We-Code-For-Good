import React from "react";
import "../styles/main.css";
import { FaFolderOpen } from "react-icons/fa";

// ✅ Automatically use your backend URL from .env
const API_BASE = import.meta.env.VITE_API_URL || "";

export default function ProjectCard({ previewImage, title, createdAt }) {
    // ✅ Build the full image URL (frontend + backend separation fix)
    const imgSrc = previewImage
        ? `${API_BASE}${previewImage.startsWith("/") ? "" : "/"}${previewImage}`
        : "/assets/default-placeholder.png";

    return (
        <div className="project-card">
            <div className="project-image">
                <img
                    src={imgSrc}
                    alt={title}
                    // ✅ fallback if image is missing or fails to load
                    onError={(e) => (e.currentTarget.src = "/assets/default-placeholder.png")}
                />
            </div>

            <div className="project-info">
                <FaFolderOpen className="project-icon" />
                <div>
                    <h3>{title}</h3>
                    <p>Created {new Date(createdAt).toLocaleDateString()}</p>
                </div>
            </div>
        </div>
    );
}
