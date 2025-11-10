import React from "react";
import "../styles/main.css";
import { FaFolderOpen } from "react-icons/fa";

export default function ProjectCard({ image, title, editedAt }) {
    return (
        <div className="project-card">
            <div className="project-image">
                <img src={image} alt={title} />
            </div>
            <div className="project-info">
                <FaFolderOpen className="project-icon" />
                <div>
                    <h3>{title}</h3>
                    <p>Edited {editedAt}</p>
                </div>
            </div>
        </div>
    );
}
