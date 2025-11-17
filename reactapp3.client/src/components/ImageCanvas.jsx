import React from "react";

export default function ImageCanvas() {
    return (
        <div className="canvas-wrapper">

            <div className="image-canvas">
                <div className="image-wrapper">
                    <img src="/frame.png" className="label-image" />
                </div>
            </div>

            <div className="image-nav">
                <button className="nav-btn">
                    <span className="arrow-left">◀</span> Prev
                </button>

                <button className="nav-btn">
                    Next <span className="arrow-right">▶</span>
                </button>
            </div>

        </div>

    );
}
