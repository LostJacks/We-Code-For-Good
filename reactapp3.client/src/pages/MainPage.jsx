import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import ProjectCard from "../components/ProjectCard";
import AddProjectCard from "../components/AddProjectCard";
import SearchBar from "../components/SearchBar";
import SortBar from "../components/SortBar";
import NewProjectModal from "../components/NewProjectModal";
import HeaderBar from "../components/HeaderBar";
import "../styles/main.css";

export default function MainPage() {
    const navigate = useNavigate();
    const [projects, setProjects] = useState([]);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [searchQuery, setSearchQuery] = useState("");
    const [sortBy, setSortBy] = useState("name-asc");

    const API_BASE = import.meta.env.VITE_API_URL || "";

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

    useEffect(() => {
        console.log("Projects data:", projects);
        if (projects.length > 0) {
            console.log("First project labels:", projects[0].labels);
        }
    }, [projects]);

    // ✅ Filter projects by search query (including labels)
    const filteredProjects = projects.filter((p) => {
        const query = searchQuery.toLowerCase();

        if (p.title.toLowerCase().includes(query)) return true;
        if (p.description.toLowerCase().includes(query)) return true;

        const dateStr = new Date(p.createdAt).toLocaleDateString().toLowerCase();
        if (dateStr.includes(query)) return true;

        if (p.labels && Array.isArray(p.labels)) {
            if (p.labels.some(label =>
                label.labelName?.toLowerCase().includes(query)  // ✅ Changed to labelName
            )) return true;
        }

        return false;
    });

    // ✅ Sort logic
    const sortedProjects = [...filteredProjects].sort((a, b) => {
        const [sortField, direction] = sortBy.split("-");
        const isAsc = direction === "asc";

        if (sortField === "name") {
            return isAsc
                ? a.title.localeCompare(b.title)
                : b.title.localeCompare(a.title);
        } else if (sortField === "date") {
            return isAsc
                ? new Date(a.createdAt) - new Date(b.createdAt)
                : new Date(b.createdAt) - new Date(a.createdAt);
        }

        return 0;
    });

    const handleOpenProject = (projectId) => {
        navigate(`/label/${projectId}`);
    };

    return (
        <div className="main-page">
            <HeaderBar />

            <div className="projects-container">
                <h1 className="projects-title">Projects</h1>
                <div className="title-line"></div>

                <SearchBar projects={projects} onSearch={setSearchQuery} />
                <SortBar sortBy={sortBy} setSortBy={setSortBy} />

                {/* ✅ Project Grid */}
                <div className="project-grid">
                    <AddProjectCard onClick={() => setIsModalOpen(true)} />
                    {sortedProjects.map((p) => (
                        <div
                            key={p.id}
                            onClick={() => handleOpenProject(p.id)}
                            style={{ cursor: "pointer" }}
                        >
                            <ProjectCard
                                title={p.title}
                                createdAt={p.createdAt}
                                previewImage={p.previewImage}
                            />
                        </div>
                    ))}
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
