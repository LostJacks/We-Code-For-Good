import React from "react";

export default function RightSidebar({ activeClass, selectedBox, onDuplicate }) {
    return (
        <div className="right-sidebar">
            {/* 🔹 Current Label Section */}
            <div className="active-class-card">
                <h3 className="active-class-title">Current Label</h3>
                <div className="active-class-info">
                    <span
                        className="active-class-color"
                        style={{ backgroundColor: activeClass?.color || "#ccc" }}
                    ></span>
                    <div className="active-class-details">
                        <span className="active-class-name">
                            {activeClass?.name || "None"}
                        </span>
                        <span className="active-class-shortcut">
                            Shortcut: {activeClass?.shortcut || "-"}
                        </span>
                    </div>
                </div>
            </div>

            {/* 🔸 Properties Section */}
            <h3 className="sidebar-title">Properties</h3>

            {selectedBox ? (
                <>
                    {/* Coordinates */}
                    <div className="property-row">
                        <span className="prop-label">Coordinates:</span>

                        <div className="prop-fields">
                            <div className="coord-field">
                                <span className="coord-label">X</span>
                                <input
                                    value={Math.round(selectedBox.x)}
                                    readOnly
                                />
                            </div>
                            <div className="coord-field">
                                <span className="coord-label">Y</span>
                                <input
                                    value={Math.round(selectedBox.y)}
                                    readOnly
                                />
                            </div>
                        </div>
                    </div>

                    {/* Size */}
                    <div className="property-row">
                        <span className="prop-label">Size:</span>

                        <div className="prop-fields">
                            <div className="coord-field">
                                <span className="coord-label">W</span>
                                <input
                                    value={Math.round(selectedBox.width)}
                                    readOnly
                                />
                            </div>
                            <div className="coord-field">
                                <span className="coord-label">H</span>
                                <input
                                    value={Math.round(selectedBox.height)}
                                    readOnly
                                />
                            </div>
                        </div>
                    </div>
                </>
            ) : (
                <p style={{ color: "#888", textAlign: "center" }}>
                    No box selected
                </p>
            )}

            {/* Actions */}
            <div className="actions-title">Actions</div>

            <div className="actions-list">
                <button className="action-btn small"
                    onClick={() => onDuplicate && onDuplicate()}
                >
                    Duplicate <span className="action-icon">📄</span>
                </button>

                <button className="action-btn small">
                    Refine <span className="action-icon">✏️</span>
                </button>

                <button className="action-btn small delete">
                    Delete <span className="action-icon">🗑️</span>
                </button>
            </div>
        </div>
    );
}
