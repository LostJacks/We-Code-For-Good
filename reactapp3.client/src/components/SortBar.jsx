import React from "react";
import { FaArrowUp, FaArrowDown } from "react-icons/fa";
import "../styles/main.css";

export default function SortBar({ sortBy, setSortBy }) {
    const handleSortChange = (field) => {
        // If clicking the same field, toggle direction
        if (sortBy.split("-")[0] === field) {
            const isAsc = sortBy.split("-")[1] === "asc";
            setSortBy(`${field}-${isAsc ? "desc" : "asc"}`);
        } else {
            // New field, default to ascending
            setSortBy(`${field}-asc`);
        }
    };

    const getIcon = (field) => {
        const [currentField, direction] = sortBy.split("-");
        if (currentField === field) {
            return direction === "asc" ? <FaArrowUp /> : <FaArrowDown />;
        }
        return null;
    };

    return (
        <div className="sort-bar">
            <h2>Sort By:</h2>
            <button
                className={`${sortBy.startsWith("name") ? "active" : ""}`}
                onClick={() => handleSortChange("name")}
            >
                Name {getIcon("name")}
            </button>
            <button
                className={`${sortBy.startsWith("date") ? "active" : ""}`}
                onClick={() => handleSortChange("date")}
            >
                Date {getIcon("date")}
            </button>
        </div>
    );
}
