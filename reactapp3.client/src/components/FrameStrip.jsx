import React, { useRef } from "react";

export default function FrameStrip({ currentFrame = 12, totalFrames = 180 }) {

    const thumbsRef = useRef(null);

    const scrollLeft = () => {
        thumbsRef.current.scrollBy({
            left: -300,   // adjust scroll amount
            behavior: "smooth"
        });
    };

    const scrollRight = () => {
        thumbsRef.current.scrollBy({
            left: 300,    // adjust scroll amount
            behavior: "smooth"
        });
    };

    return (
        <>
            <div className="frame-counter">
                Frame: {currentFrame}/{totalFrames}
            </div>

            <div className="frame-strip">

                {/* Prev Button */}
                <button className="strip-nav-btn" onClick={scrollLeft}>
                    ◀ Prev
                </button>

                {/* Thumbnails */}
                <div className="frame-thumbnails" ref={thumbsRef}>
                    {Array.from({ length: 200 }).map((_, i) => (
                        <div
                            className={`thumb ${i === currentFrame ? "active" : ""}`}
                            key={i}
                        >
                            <img src="/placeholder-thumb.png" alt="thumb" />
                        </div>
                    ))}
                </div>

                {/* Next Button */}
                <button className="strip-nav-btn" onClick={scrollRight}>
                    Next ▶
                </button>
            </div>
        </>
    );
}
