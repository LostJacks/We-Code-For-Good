import React, { useState } from "react";

export default function FrameStrip({
    currentFrame: initialFrame = 12,
    totalFrames = 180,
}) {
    const [startIndex, setStartIndex] = useState(0);
    const [currentFrame, setCurrentFrame] = useState(initialFrame);
    const FRAMES_VISIBLE = 10;

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
    );

    // When user clicks on a frame
    const handleFrameClick = (frameNumber) => {
        setCurrentFrame(frameNumber);
    };

    return (
        <div className="frame-strip-section">
            {/* Frame Counter */}
            <div className="frame-counter">
                Frame: {currentFrame}/{totalFrames}
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
                        const frameNumber = frameIndex + 1;
                        return (
                            <div
                                key={frameIndex}
                                className={`thumb ${frameNumber === currentFrame ? "active" : ""
                                    }`}
                                onClick={() => handleFrameClick(frameNumber)}
                                style={{ cursor: "pointer" }}
                            >
                                <img
                                    src="/assets/default-placeholder.png"
                                    alt={`Frame ${frameNumber}`}
                                    className={
                                        frameNumber === currentFrame
                                            ? "highlighted"
                                            : ""
                                    }
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
