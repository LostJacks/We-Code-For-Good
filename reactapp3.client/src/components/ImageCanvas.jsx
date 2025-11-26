import React, { useRef, useState, useEffect } from "react";

export default function ImageCanvas({
    activeClass,
    labelClasses,
    onBoxesChange,
    onSelectBox,        // ✅ NEW: tell parent which box is selected
    visibleLabel,
    cursorColorMode,
}) {
    const canvasRef = useRef(null);
    const imgRef = useRef(null);

    const [boxes, setBoxes] = useState([]);
    const [drawing, setDrawing] = useState(false);
    const [start, setStart] = useState({ x: 0, y: 0 });
    const [selectedIndex, setSelectedIndex] = useState(null);
    const [dragging, setDragging] = useState(false);
    const [resizing, setResizing] = useState(false);
    const [resizeCorner, setResizeCorner] = useState(null);
    const imageUrl = "/assets/bags.jpg";

    // --- Load image ---
    useEffect(() => {
        const img = new Image();
        img.src = imageUrl;
        img.onload = () => {
            imgRef.current = img;
            resizeCanvas();
        };
    }, [imageUrl]);

    // --- Resize canvas ---
    const resizeCanvas = () => {
        const canvas = canvasRef.current;
        if (!canvas) return;
        const wrapper = canvas.parentElement.getBoundingClientRect();
        canvas.width = wrapper.width;
        canvas.height = wrapper.height;
        drawAll();
    };

    useEffect(() => {
        const handleResize = () => resizeCanvas();
        window.addEventListener("resize", handleResize);
        return () => window.removeEventListener("resize", handleResize);
    }, []);

    // --- Draw all boxes ---
    const drawAll = (tempBox = null) => {
        const canvas = canvasRef.current;
        if (!canvas || !imgRef.current) return;
        const ctx = canvas.getContext("2d");

        ctx.clearRect(0, 0, canvas.width, canvas.height);
        ctx.drawImage(imgRef.current, 0, 0, canvas.width, canvas.height);

        const visibleBoxes = visibleLabel
            ? boxes.filter((b) =>
                b.label.toLowerCase().includes(visibleLabel.toLowerCase())
            )
            : boxes;

        visibleBoxes.forEach((b, i) => drawBox(ctx, b, i === selectedIndex));
        if (tempBox) drawBox(ctx, tempBox, false, true);
    };

    const drawBox = (ctx, b, selected = false, temp = false) => {
        ctx.strokeStyle = b.color;
        ctx.lineWidth = selected ? 3 : temp ? 1.5 : 2;
        ctx.setLineDash(selected ? [5, 5] : []);
        ctx.globalAlpha = temp ? 0.6 : 1;
        ctx.strokeRect(b.x, b.y, b.width, b.height);
        ctx.globalAlpha = 1;
        ctx.font = "14px Arial";
        ctx.fillStyle = b.color;
        ctx.fillText(b.label, b.x + 4, b.y - 6);
        ctx.setLineDash([]);
    };

    const findBoxAt = (x, y) => {
        for (let i = boxes.length - 1; i >= 0; i--) {
            const b = boxes[i];
            if (x >= b.x && x <= b.x + b.width && y >= b.y && y <= b.y + b.height)
                return i;
        }
        return null;
    };

    // --- Mouse events ---
    const handleMouseDown = (e) => {
        const rect = e.target.getBoundingClientRect();
        const x = e.clientX - rect.left;
        const y = e.clientY - rect.top;
        const clickedIndex = findBoxAt(x, y);

        if (clickedIndex !== null && selectedIndex === clickedIndex) {
            const b = boxes[clickedIndex];
            const corner = getResizeCorner(x, y, b);
            if (corner) {
                setResizing(true);
                setResizeCorner(corner);
            } else {
                setDragging(true);
            }
        } else if (clickedIndex !== null) {
            setSelectedIndex(clickedIndex);
            onSelectBox && onSelectBox(boxes[clickedIndex]); // ✅ notify parent
            drawAll();
        } else {
            setStart({ x, y });
            setDrawing(true);
            setSelectedIndex(null);
            onSelectBox && onSelectBox(null); // ✅ deselect in sidebar
        }
    };

    const handleMouseMove = (e) => {
        const rect = e.target.getBoundingClientRect();
        const x = e.clientX - rect.left;
        const y = e.clientY - rect.top;

        if (drawing) {
            drawAll({
                x: Math.min(start.x, x),
                y: Math.min(start.y, y),
                width: Math.abs(x - start.x),
                height: Math.abs(y - start.y),
                color: activeClass?.color || "#e53935",
                label: activeClass?.name || "Unlabeled",
            });
        } else if (dragging && selectedIndex !== null) {
            const dx = x - start.x;
            const dy = y - start.y;
            const updated = [...boxes];
            updated[selectedIndex] = {
                ...updated[selectedIndex],
                x: updated[selectedIndex].x + dx,
                y: updated[selectedIndex].y + dy,
            };
            setStart({ x, y });
            setBoxes(updated);
            onBoxesChange && onBoxesChange(updated);
            onSelectBox && onSelectBox(updated[selectedIndex]); // ✅ live update
            drawAll();
        } else if (resizing && selectedIndex !== null) {
            const updated = [...boxes];
            const b = { ...updated[selectedIndex] };
            resizeBox(b, x, y, resizeCorner);
            updated[selectedIndex] = b;
            setBoxes(updated);
            onBoxesChange && onBoxesChange(updated);
            onSelectBox && onSelectBox(b); // ✅ live update during resize
            drawAll();
        }
    };

    const handleMouseUp = (e) => {
        const rect = e.target.getBoundingClientRect();
        const x = e.clientX - rect.left;
        const y = e.clientY - rect.top;

        if (drawing) {
            const width = Math.abs(x - start.x);
            const height = Math.abs(y - start.y);

            if (width > 5 && height > 5) {
                const newBox = {
                    x: Math.min(start.x, x),
                    y: Math.min(start.y, y),
                    width,
                    height,
                    color: activeClass?.color || "#e53935",
                    label: activeClass?.name || "Unlabeled",
                };
                const updated = [...boxes, newBox];
                setBoxes(updated);
                onBoxesChange && onBoxesChange(updated);
                onSelectBox && onSelectBox(newBox); // ✅ auto-select new box
            }
        }

        setDrawing(false);
        setDragging(false);
        setResizing(false);
        setResizeCorner(null);
    };

    const handleDoubleClick = (e) => {
        const rect = e.target.getBoundingClientRect();
        const x = e.clientX - rect.left;
        const y = e.clientY - rect.top;
        const clickedIndex = findBoxAt(x, y);
        setSelectedIndex(clickedIndex);
        onSelectBox && onSelectBox(clickedIndex !== null ? boxes[clickedIndex] : null); // ✅
        drawAll();
    };

    // --- Keyboard controls ---
    useEffect(() => {
        const handleKeyDown = (e) => {
            if (selectedIndex === null) return;

            // ✅ DELETE BOX (Delete key or D)
            if (e.key === "Delete" || e.key.toLowerCase() === "d") {
                const updated = boxes.filter((_, i) => i !== selectedIndex);
                setBoxes(updated);
                setSelectedIndex(null);
                onBoxesChange && onBoxesChange(updated);
                onSelectBox && onSelectBox(null); // ✅ clear sidebar
                drawAll();
                return;
            }

            // ✅ ESCAPE deselects
            if (e.key === "Escape") {
                setSelectedIndex(null);
                onSelectBox && onSelectBox(null);
                drawAll();
                return;
            }

            // ✅ NUMBER KEYS to change class
            if (!isNaN(parseInt(e.key)) && labelClasses) {
                const index = parseInt(e.key) - 1;
                if (labelClasses[index]) {
                    const updated = [...boxes];
                    updated[selectedIndex] = {
                        ...updated[selectedIndex],
                        label: labelClasses[index].name,
                        color: labelClasses[index].color,
                    };
                    setBoxes(updated);
                    onBoxesChange && onBoxesChange(updated);
                    onSelectBox && onSelectBox(updated[selectedIndex]); // ✅ live update label
                    drawAll();
                }
            }
        };

        window.addEventListener("keydown", handleKeyDown);
        return () => window.removeEventListener("keydown", handleKeyDown);
    }, [selectedIndex, boxes, labelClasses]);

    const getResizeCorner = (x, y, b) => {
        const pad = 8;
        const corners = {
            tl: { x: b.x, y: b.y },
            tr: { x: b.x + b.width, y: b.y },
            bl: { x: b.x, y: b.y + b.height },
            br: { x: b.x + b.width, y: b.y + b.height },
        };
        for (let [corner, c] of Object.entries(corners)) {
            if (Math.abs(x - c.x) <= pad && Math.abs(y - c.y) <= pad) return corner;
        }
        return null;
    };

    const resizeBox = (b, x, y, corner) => {
        switch (corner) {
            case "tl":
                b.width += b.x - x;
                b.height += b.y - y;
                b.x = x;
                b.y = y;
                break;
            case "tr":
                b.width = x - b.x;
                b.height += b.y - y;
                b.y = y;
                break;
            case "bl":
                b.width += b.x - x;
                b.x = x;
                b.height = y - b.y;
                break;
            case "br":
                b.width = x - b.x;
                b.height = y - b.y;
                break;
            default:
                break;
        }
    };

    useEffect(() => {
        drawAll();
    }, [boxes, activeClass, selectedIndex, visibleLabel]);

    // ✅ Colored crosshair cursor
    const getCursor = () => {
        if (drawing) return "crosshair";
        if (cursorColorMode && activeClass?.color) {
            const svg = `
            <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32">
                <line x1="16" y1="0" x2="16" y2="32" stroke="${activeClass.color}" stroke-width="2"/>
                <line x1="0" y1="16" x2="32" y2="16" stroke="${activeClass.color}" stroke-width="2"/>
                <circle cx="16" cy="16" r="2" fill="${activeClass.color}" />
            </svg>`;
            return `url('data:image/svg+xml;utf8,${encodeURIComponent(svg)}') 16 16, crosshair`;
        }
        return "crosshair";
    };

    return (
        <div className="canvas-wrapper">
            <div className="image-canvas">
                <div className="image-wrapper">
                    <canvas
                        ref={canvasRef}
                        onMouseDown={handleMouseDown}
                        onMouseMove={handleMouseMove}
                        onMouseUp={handleMouseUp}
                        onDoubleClick={handleDoubleClick}
                        style={{
                            width: "100%",
                            height: "100%",
                            border: "2px solid #e53935",
                            borderRadius: "12px",
                            cursor: getCursor(),
                            display: "block",
                        }}
                    />
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
