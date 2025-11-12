import React, { useState, useEffect } from "react";
import ProjectCard from "../components/ProjectCard";
import AddProjectCard from "../components/AddProjectCard";
import SortBar from "../components/SortBar";
import NewProjectModal from "../components/NewProjectModal";
import HeaderBar from "../components/HeaderBar";
import "../styles/main.css";

export default function MainPage() {
    const [projects, setProjects] = useState([]);
    const [sortBy, setSortBy] = useState("az");
    const [isModalOpen, setIsModalOpen] = useState(false);

    const API_BASE = import.meta.env.VITE_API_URL || "https://localhost:7166";

    // ✅ Fetch all projects from backend
    const fetchProjects = async () => {
        try {
            const res = await fetch(`${API_BASE}/api/projects`);
            if (!res.ok) throw new Error("Failed to load projects");
            const data = await res.json();
            setProjects(data);
        } catch (err) {
            console.error("❌ Error fetching projects:", err);
        }
    };

    useEffect(() => {
        fetchProjects();
    }, []);

    // ✅ Sort logic
    const sortedProjects = [...projects].sort((a, b) =>
        sortBy === "az" ? a.title.localeCompare(b.title) : a.id - b.id
    );

    return (
        <div className="main-page">
            <HeaderBar />

            <div className="projects-container">
                <h1 className="projects-title">Projects</h1>
                <div className="title-line"></div>

                <SortBar sortBy={sortBy} setSortBy={setSortBy} />

                {/* ✅ Project Grid */}
                <div className="project-grid">
                    {sortedProjects.map((p) => (
                        <ProjectCard
                            key={p.id}
                            title={p.title}
                            createdAt={p.createdAt}
                            previewImage={p.previewImage}
                        />
                    ))}
                    <AddProjectCard onClick={() => setIsModalOpen(true)} />
                </div>
            </div>

            {/* ✅ New Project Modal */}
            <NewProjectModal
                isOpen={isModalOpen}
                onClose={() => setIsModalOpen(false)}
                onCreated={fetchProjects} // refresh after creating new project
            />
        </div>
    );
}
