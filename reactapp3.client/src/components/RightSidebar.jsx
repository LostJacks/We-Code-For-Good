import React from "react";

export default function RightSidebar() {
    return (
        <div className="right-sidebar">

            <h3 className="sidebar-title">Properties</h3>

            <div className="property-box">
                <label>Class:</label>
                <div className="prop-value">Fur Jacket</div>
            </div>

            {/* Coordinates */}
            <div className="property-row">
                <span className="prop-label">Coordinates:</span>

                <div className="prop-fields">
                    <div className="coord-field">
                        <span className="coord-label">X</span>
                        <input />
                    </div>
                    <div className="coord-field">
                        <span className="coord-label">Y</span>
                        <input />
                    </div>
                </div>
            </div>

            {/* Size */}
            <div className="property-row">
                <span className="prop-label">Size:</span>

                <div className="prop-fields">
                    <div className="coord-field">
                        <span className="coord-label">W</span>
                        <input />
                    </div>
                    <div className="coord-field">
                        <span className="coord-label">H</span>
                        <input />
                    </div>
                </div>
            </div>

            <div className="actions-title">Actions</div>

            <div className="actions-list">

                <button className="action-btn small">
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
