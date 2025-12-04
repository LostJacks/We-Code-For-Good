import "../styles/searchbar.css";
import React from "react";
import { FaSearch } from "react-icons/fa";


export default function SearchBar({ projects, onSearch }) {
    const handleSearch = (e) => {
        const query = e.target.value.toLowerCase();
        onSearch(query);
    };

    return (
        <div className="search-bar">
            <FaSearch className="search-icon" />
            <input
                type="text"
                placeholder="Search projects by name, date, or labels..."
                onChange={handleSearch} />
        </div>
    );
}
