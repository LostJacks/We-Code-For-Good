import React, { useState } from "react";
import ProjectCard from "../components/ProjectCard";
import AddProjectCard from "../components/AddProjectCard";
import SortBar from "../components/SortBar";
import NewProjectModal from "../components/NewProjectModal";
import "../styles/main.css";
import HeaderBar from "../components/HeaderBar";

import tshirt from "../assets/tshirt.jpg";
import woolshirt from "../assets/woolshirt.jpg";
import hoodie from "../assets/hoodie.jpg";
import bags from "../assets/bags.jpg";
import fabric from "../assets/fabric.jpg";

export default function MainPage() {
    const [sortBy, setSortBy] = useState("az");
    const [isModalOpen, setIsModalOpen] = useState(false);

    const projects = [
        { id: 1, title: "Project 1", image: tshirt, editedAt: "30 min ago" },
        { id: 2, title: "Project 2", image: woolshirt, editedAt: "30 min ago" },
        { id: 3, title: "Project 3", image: hoodie, editedAt: "30 min ago" },
        { id: 4, title: "Project 4", image: bags, editedAt: "30 min ago" },
        { id: 5, title: "Project 5", image: fabric, editedAt: "30 min ago" },
    ];

    const sortedProjects = [...projects].sort((a, b) =>
        sortBy === "az" ? a.title.localeCompare(b.title) : a.id - b.id
    );

    return (
        <div className="main-page">
            {/* ✅ Add the header bar at the very top */}
            <HeaderBar />

            <div className="projects-container">
                <h1 className="projects-title">Projects</h1>
                <div className="title-line"></div>

                <SortBar sortBy={sortBy} setSortBy={setSortBy} />

                <div className="project-grid">
                    {sortedProjects.map((p) => (
                        <ProjectCard key={p.id} {...p} />
                    ))}
                    <AddProjectCard onClick={() => setIsModalOpen(true)} />
                </div>
            </div>

            <NewProjectModal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} />
        </div>
    );
}
