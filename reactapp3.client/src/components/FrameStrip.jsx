import React, { useState } from "react";

export default function FrameStrip({
    files = [],
    currentFileIndex = 0,
    onFileIndexChange = () => { },
}) {

    const [startIndex, setStartIndex] = useState(0);
    const FRAMES_VISIBLE = 10;

    const totalFrames = files.length;

    // Move by 10 frames per click
    const scrollLeft = () => {
        setStartIndex((prev) => Math.max(prev - FRAMES_VISIBLE, 0));
    };

    const scrollRight = () => {
        setStartIndex((prev) =>
            Math.min(prev + FRAMES_VISIBLE, totalFrames - FRAMES_VISIBLE)
        );
    };

    // Frames currently visible
    const visibleFrames = Array.from(
        { length: FRAMES_VISIBLE },
        (_, i) => startIndex + i
    ).filter(i => i < totalFrames); {/* ← Add this filter */ }

    // When user clicks on a frame
    const handleFrameClick = (index) => {
        onFileIndexChange(index); {/* ← Changed this */ }
    };

    if (!files || files.length === 0) return null;

    return (
        <div className="frame-strip-section">
            {/* Frame Counter */}
            <div className="frame-counter">
                Frame: {currentFileIndex + 1}/{totalFrames}
            </div>

            {/* Frame Strip */}
            <div className="frame-strip">
                <button
                    className="strip-nav-btn"
                    onClick={scrollLeft}
                    disabled={startIndex === 0}
                >
                    ◀ Prev
                </button>

                <div className="frame-thumbnails">
                    {visibleFrames.map((frameIndex) => {
                        const file = files[frameIndex];
                        return (
                            <div
                                key={frameIndex}
                                className={`thumb ${frameIndex === currentFileIndex ? "active" : ""}`}
                                onClick={() => handleFrameClick(frameIndex)}
                                style={{ cursor: "pointer" }}
                            >
                                <img
                                    src={`${import.meta.env.VITE_API_URL}${file.filePath}`}
                                    alt={`Frame ${frameIndex + 1}`}
                                    className={frameIndex === currentFileIndex ? "highlighted" : ""}
                                />
                            </div>
                        );
                    })}
                </div>

                <button
                    className="strip-nav-btn"
                    onClick={scrollRight}
                    disabled={startIndex >= totalFrames - FRAMES_VISIBLE}
                >
                    Next ▶
                </button>
            </div>
        </div>
    );
}
