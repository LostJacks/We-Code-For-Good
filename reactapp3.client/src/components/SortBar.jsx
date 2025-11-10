import React from "react";
import "../styles/main.css";

export default function SortBar({ sortBy, setSortBy }) {
    return (
        <div className="sort-bar">
            <h2>Sort By:</h2>
            <button
                className={sortBy === "az" ? "active" : ""}
                onClick={() => setSortBy("az")}
            >
                A-Z
            </button>
            <button
                className={sortBy === "oldest" ? "active" : ""}
                onClick={() => setSortBy("oldest")}
            >
                Oldest - Newest
            </button>
        </div>
    );
}
