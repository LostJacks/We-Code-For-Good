import React, { useRef, useState, useEffect } from "react";
import {
    getBoundingBoxesByMedia,
    createBoundingBox,
    updateBoundingBox,
    deleteBoundingBox,
} from "@/api/boundingBoxApi";

export default function ImageCanvas({
    activeClass,
    labelClasses,
    onBoxesChange,
    onSelectBox,
    visibleLabel,
    cursorColorMode,
    files = [],
    currentFileIndex = 0,
    onFileIndexChange,
    mediaId = 1,
    showCrosshair = true,
    isolateMode = false,
}) {
    const canvasRef = useRef(null);
    const imgRef = useRef(null);

    const [boxes, setBoxes] = useState([]);
    const [tempBox, setTempBox] = useState(null);
    const [drawing, setDrawing] = useState(false);
    const [start, setStart] = useState({ x: 0, y: 0 });
    const [selectedIndex, setSelectedIndex] = useState(null);
    const [dragging, setDragging] = useState(false);
    const [resizing, setResizing] = useState(false);
    const [resizeCorner, setResizeCorner] = useState(null);

    //Zoom State
    const [viewScale, setViewScale] = useState(1);
    const [viewX, setViewX] = useState(0);
    const [viewY, setViewY] = useState(0);

    const MIN_SCALE = 0.5;
    const MAX_SCALE = 6;

    const clamp = (v, min, max) => Math.max(min, Math.min(max, v));

    //Panning
    const [isPanning, setIsPanning] = useState(false);
    const [lastPanPos, setLastPanPos] = useState({ x: 0, y: 0 });
    const [spaceDown, setSpaceDown] = useState(false);

    //Crosshair image alignment
    const [crosshairPos, setCrosshairPos] = useState(null);

    useEffect(() => {
        const handleKeyDown = (e) => {
            if (e.code === "Space") {
                e.preventDefault();
                setSpaceDown(true);
            }
        };

        const handleKeyUp = (e) => {
            if (e.code === "Space") {
                e.preventDefault();
                setSpaceDown(false);
                setIsPanning(false);
            }
        };

        window.addEventListener("keydown", handleKeyDown);
        window.addEventListener("keyup", handleKeyUp);
        return () => {
            window.removeEventListener("keydown", handleKeyDown);
            window.removeEventListener("keyup", handleKeyUp);
        };
    }, []);

    // ✅ UNDO/REDO STATE
    const [history, setHistory] = useState([[]]);
    const [historyIndex, setHistoryIndex] = useState(0);

    const MIN_BOX_SIZE = 5;
    const imageUrl = files.length > 0
        ? `${import.meta.env.VITE_API_URL}${files[currentFileIndex]?.filePath}`
        : null;
    const currentFile = files.length > 0 ? files[currentFileIndex] : null;
    const currentMediaId = currentFile?.mediaId || currentFile?.id || mediaId;

    // ✅ Helper: Save state to history
    const saveToHistory = (newBoxes) => {
        const newHistory = history.slice(0, historyIndex + 1);
        newHistory.push(newBoxes);
        setHistory(newHistory);
        setHistoryIndex(newHistory.length - 1);
        setBoxes(newBoxes);
        onBoxesChange(newBoxes);
    };

    // ✅ UNDO
    const handleUndo = () => {
        if (historyIndex > 0) {
            const newIndex = historyIndex - 1;
            setHistoryIndex(newIndex);
            const previousBoxes = history[newIndex];
            setBoxes(previousBoxes);
            onBoxesChange(previousBoxes);
            setSelectedIndex(null);
            onSelectBox && onSelectBox(null);
            console.log("↶ Undo performed");
        }
    };

    // ✅ REDO
    const handleRedo = () => {
        if (historyIndex < history.length - 1) {
            const newIndex = historyIndex + 1;
            setHistoryIndex(newIndex);
            const nextBoxes = history[newIndex];
            setBoxes(nextBoxes);
            onBoxesChange(nextBoxes);
            setSelectedIndex(null);
            onSelectBox && onSelectBox(null);
            console.log("↷ Redo performed");
        }
    };

    // ✅ Keyboard shortcuts for Undo/Redo (Ctrl+Z, Ctrl+Y, Ctrl+Shift+Z)
    useEffect(() => {
        const handleKeyDown = (e) => {
            if ((e.ctrlKey || e.metaKey) && e.key === "z" && !e.shiftKey) {
                e.preventDefault();
                handleUndo();
            } else if ((e.ctrlKey || e.metaKey) && (e.key === "y" || (e.shiftKey && e.key === "z"))) {
                e.preventDefault();
                handleRedo();
            }
        };

        window.addEventListener("keydown", handleKeyDown);
        return () => window.removeEventListener("keydown", handleKeyDown);
    }, [historyIndex, history]);

    // ============================
    // LOAD IMAGE + BOXES FROM BACKEND
    // ============================
    useEffect(() => {
        console.log('Trying to load image from:', imageUrl);
        const img = new Image();
        img.src = imageUrl;
        img.onload = async () => {
            console.log('Image loaded successfully');
            console.log('Image size:', img.width, 'x', img.height);
            console.log('Image src:', img.src);
            imgRef.current = img;
            const canvas = canvasRef.current;
            if (!canvas) {
                console.error('❌ Canvas ref is null!');
                return;
            }

            resizeCanvas();

            console.log('Canvas width:', canvas.width);
            console.log('Canvas height:', canvas.height);
            console.log('Canvas style width:', canvas.style.width);
            console.log('Canvas style height:', canvas.style.height);

            try {
                const boxesFromServer = await getBoundingBoxesByMedia(currentMediaId);
                console.log("✅ Loaded boxes for mediaId", currentMediaId, ":", boxesFromServer);

                boxesFromServer.forEach((b, i) => {
                    console.log(`  Box ${i}: labelClassId=${b.labelClassId}, className=${b.className}`);
                });

                const loadedBoxes = boxesFromServer.map((b) => {
                    const meta =
                        labelClasses?.find((lc) => lc.id === b.labelClassId) ?? null;

                    return {
                        id: b.boundingBoxId,
                        mediaId: b.mediaId,
                        labelClassId: b.labelClassId,
                        label: meta?.name || b.className || "Unlabeled",
                        color: meta?.color || "#e53935",
                        x: b.x1,
                        y: b.y1,
                        width: b.x2 - b.x1,
                        height: b.y2 - b.y1,
                    };
                });

                setBoxes(loadedBoxes);
                // ✅ Reset history when loading new file
                setHistory([loadedBoxes]);
                setHistoryIndex(0);
            } catch (err) {
                console.error("❌ Failed to load bounding boxes:", err);
            }
        };
        img.onerror = (err) => {
            console.error('❌ Image failed to load!');
            console.error('Error:', err);
            console.error('URL attempted:', imageUrl);
        };

        img.src = imageUrl;
    }, [imageUrl, mediaId, labelClasses]);

    // ============================
    // NOTIFY PARENT OF BOX CHANGES
    // ============================
    useEffect(() => {
        if (onBoxesChange) {
            onBoxesChange(boxes);
        }
    }, [boxes, onBoxesChange]);

    // ============================
    // CANVAS SETUP
    // ============================
    const resizeCanvas = () => {
        const canvas = canvasRef.current;
        if (!canvas) return;

        const wrapper = canvas.parentElement.getBoundingClientRect();
        const ctx = canvas.getContext("2d");
        const dpr = window.devicePixelRatio || 1;

        canvas.width = wrapper.width * dpr;
        canvas.height = wrapper.height * dpr;
        canvas.style.width = `${wrapper.width}px`;
        canvas.style.height = `${wrapper.height}px`;

        ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
        drawAll();
    };

    useEffect(() => {
        const handleResize = () => resizeCanvas();
        window.addEventListener("resize", handleResize);
        return () => window.removeEventListener("resize", handleResize);
    }, []);

    //Reset Zoom on image change
    useEffect(() => {
        setViewScale(1);
        setViewX(0);
        setViewY(0);
    }, [imageUrl]);

    //Wheel Zoom
    useEffect(() => {
        const canvas = canvasRef.current;
        if (!canvas) return;

        const onWheel = (e) => {
            e.preventDefault();

            const rect = canvas.getBoundingClientRect();
            const sx = e.clientX - rect.left;
            const sy = e.clientY - rect.top;

            // cursor position in world coords (before zoom)
            const wx = sx / viewScale + viewX;
            const wy = sy / viewScale + viewY;

            const factor = e.deltaY < 0 ? 1.1 : 1 / 1.1;
            const nextScale = clamp(viewScale * factor, MIN_SCALE, MAX_SCALE);

            // adjust viewX/viewY so (wx,wy) stays under cursor after zoom
            const nextViewX = wx - sx / nextScale;
            const nextViewY = wy - sy / nextScale;

            setViewScale(nextScale);
            setViewX(nextViewX);
            setViewY(nextViewY);
        };

        canvas.addEventListener("wheel", onWheel, { passive: false });
        return () => canvas.removeEventListener("wheel", onWheel);
    }, [viewScale, viewX, viewY]);

    // ============================
    // DRAWING LOGIC
    // ============================
    const drawBox = (ctx, b, selected = false, temp = false) => {
        ctx.strokeStyle = b.color;
        ctx.lineWidth = selected ? 3 : temp ? 1.5 : 2;
        ctx.setLineDash(selected ? [5, 5] : []);

        if (temp) {
            ctx.globalAlpha = 0.6;
        }

        ctx.strokeRect(b.x, b.y, b.width, b.height);
        ctx.globalAlpha = 1;
        ctx.font = "14px Arial";
        ctx.fillStyle = b.color;
        ctx.fillText(b.label, b.x + 4, b.y - 6);
        ctx.setLineDash([]);
    };

    const drawAll = () => {
        const canvas = canvasRef.current;
        if (!canvas || !imgRef.current) return;
        const ctx = canvas.getContext("2d");

        const { width: cw, height: ch } = canvas.getBoundingClientRect();

        console.log("drawAll camera:", { viewX, viewY, viewScale });

        ctx.clearRect(0, 0, cw, ch);
        ctx.save();

        ctx.translate(-viewX * viewScale, -viewY * viewScale);
        ctx.scale(viewScale, viewScale);

        // 1) draw image
        ctx.drawImage(imgRef.current, 0, 0, cw, ch);

        // 2) alignment crosshair BEHIND boxes
        if (crosshairPos && showCrosshair) {
            const { x, y } = crosshairPos;

            ctx.save();
            ctx.setLineDash([]);
            ctx.globalAlpha = 0.9;
            ctx.strokeStyle = "#ffffff";
            ctx.lineWidth = 1;
            ctx.shadowColor = "rgba(0,0,0,0.6)";
            ctx.shadowBlur = 2;
            ctx.shadowOffsetX = 0;
            ctx.shadowOffsetY = 0;

            const imgWidth = cw;
            const imgHeight = ch;

            // horizontal line
            ctx.beginPath();
            ctx.moveTo(0, y);
            ctx.lineTo(imgWidth, y);
            ctx.stroke();

            // vertical line
            ctx.beginPath();
            ctx.moveTo(x, 0);
            ctx.lineTo(x, imgHeight);
            ctx.stroke();

            ctx.restore();
        }

        // 3) draw boxes
        const visibleBoxes = visibleLabel
            ? boxes.filter((b) =>
                b.label.toLowerCase().includes(visibleLabel.toLowerCase())
            )
            : boxes;

        visibleBoxes.forEach((b, i) => {
            const isSelected = i === selectedIndex;

            if (isolateMode && selectedIndex !== null && !isSelected)
                ctx.globalAlpha = 0.2;
            else
                ctx.globalAlpha = 1;

            drawBox(ctx, b, isSelected);
            ctx.globalAlpha = 1;
        });

        // 4) temp box
        if (tempBox) drawBox(ctx, tempBox, false, true);

        ctx.restore();
    };

    const findBoxAt = (x, y) => {
        for (let i = boxes.length - 1; i >= 0; i--) {
            const b = boxes[i];
            if (x >= b.x && x <= b.x + b.width && y >= b.y && y <= b.y + b.height)
                return i;
        }
        return null;
    };

    // ============================
    // MOUSE HANDLERS
    // ============================
    const getMousePos = (e) => {
        const rect = canvasRef.current.getBoundingClientRect();

        const sx = e.clientX - rect.left;
        const sy = e.clientY - rect.top;

        const x = sx / viewScale + viewX;
        const y = sy / viewScale + viewY;

        return { x, y };
    };

    const handleMouseDown = (e) => {
        const rect = canvasRef.current.getBoundingClientRect();
        const sx = e.clientX - rect.left;
        const sy = e.clientY - rect.top;

        if (spaceDown) {
            setIsPanning(true);
            setLastPanPos({ x: sx, y: sy });
            return;
        }

        const { x, y } = getMousePos(e);

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
            onSelectBox && onSelectBox(boxes[clickedIndex]);
            drawAll();
        } else {
            setStart({ x, y });
            setDrawing(true);
            setSelectedIndex(null);
            onSelectBox && onSelectBox(null);
        }
    };

    const handleMouseMove = (e) => {
        const rect = canvasRef.current.getBoundingClientRect();
        const sx = e.clientX - rect.left;
        const sy = e.clientY - rect.top;

        if (isPanning) {
            const dxScreen = sx - lastPanPos.x;
            const dyScreen = sy - lastPanPos.y;

            const dxWorld = dxScreen / viewScale;
            const dyWorld = dyScreen / viewScale;

            setViewX((prev) => {
                const next = prev - dxWorld;
                console.log("pan viewX", { prev, next, dxWorld });
                return next;
            });
            setViewY((prev) => {
                const next = prev - dyWorld;
                console.log("pan viewY", { prev, next, dyWorld });
                return next;
            });

            setLastPanPos({ x: sx, y: sy });
            return;
        }

        const { x, y } = getMousePos(e);

        if (showCrosshair) {
            setCrosshairPos({ x, y });
        }

        if (drawing) {
            const width = Math.abs(x - start.x);
            const height = Math.abs(y - start.y);

            if (width > MIN_BOX_SIZE && height > MIN_BOX_SIZE) {
                setTempBox({
                    x: Math.min(start.x, x),
                    y: Math.min(start.y, y),
                    width,
                    height,
                    color: activeClass?.color || "#e53935",
                    label: activeClass?.name || "Unlabeled",
                });
            } else {
                setTempBox(null);
            }
            return;
        }

        if (dragging && selectedIndex !== null) {
            const dx = x - start.x;
            const dy = y - start.y;

            setBoxes((prev) =>
                prev.map((b, i) =>
                    i === selectedIndex ? { ...b, x: b.x + dx, y: b.y + dy } : b
                )
            );

            onSelectBox && onSelectBox(boxes[selectedIndex]);
            setStart({ x, y });
            return;
        }

        if (resizing && selectedIndex !== null) {
            setBoxes((prev) => {
                const next = prev.map((b, i) => {
                    if (i !== selectedIndex) return b;
                    const newBox = { ...b };
                    resizeBox(newBox, x, y, resizeCorner);
                    return newBox;
                });
                return next;
            });
            return;
        }
    };

    const handleMouseUp = async (e) => {
        if (isPanning) {
            setIsPanning(false);
            return;
        }

        const { x, y } = getMousePos(e);

        if (drawing) {
            const width = Math.abs(x - start.x);
            const height = Math.abs(y - start.y);

            if (width > MIN_BOX_SIZE && height > MIN_BOX_SIZE) {
                const newBox = {
                    x: Math.min(start.x, x),
                    y: Math.min(start.y, y),
                    width,
                    height,
                    labelClassId: activeClass?.id ?? null,
                    label: activeClass?.name || "Unlabeled",
                    color: activeClass?.color || "#e53935",
                };

                console.log("📦 activeClass:", activeClass);
                console.log("📦 activeClass.id:", activeClass?.id);
                console.log("📦 activeClass keys:", Object.keys(activeClass || {}));

                const payload = {
                    mediaId: currentMediaId,
                    labelClassId: newBox.labelClassId ?? 1,
                    x1: newBox.x,
                    y1: newBox.y,
                    x2: newBox.x + newBox.width,
                    y2: newBox.y + newBox.height,
                };

                console.log("▶ POST /api/boundingbox", payload);

                try {
                    const created = await createBoundingBox(payload);

                    const meta =
                        labelClasses?.find((lc) => lc.id === payload.labelClassId) ||
                        activeClass ||
                        null;

                    const newBoxWithId = {
                        id: created.boundingBoxId,
                        mediaId: created.mediaId,
                        labelClassId: created.labelClassId,
                        label: meta?.name || newBox.label,
                        color: meta?.color || newBox.color,
                        x: created.x1,
                        y: created.y1,
                        width: created.x2 - created.x1,
                        height: created.y2 - created.y1,
                    };

                    // ✅ Save to history
                    saveToHistory([...boxes, newBoxWithId]);
                    onSelectBox && onSelectBox(newBoxWithId);
                } catch (err) {
                    console.error("❌ Failed to create bounding box:", err);
                    const fallback = { ...newBox, id: Date.now() };
                    saveToHistory([...boxes, fallback]);
                    onSelectBox && onSelectBox(fallback);
                }
            }

            setTempBox(null);
        } else if (selectedIndex !== null && (dragging || resizing)) {
            const box = boxes[selectedIndex];
            try {
                await updateBoundingBox(box.id, {
                    boundingBoxId: box.id,
                    labelClassId: box.labelClassId,
                    x1: box.x,
                    y1: box.y,
                    x2: box.x + box.width,
                    y2: box.y + box.height,
                    mediaId: currentMediaId,
                });
                saveToHistory(boxes);
            } catch (err) {
                console.error("❌ Failed to update bounding box:", err);
            }
        }

        setDrawing(false);
        setDragging(false);
        setResizing(false);
        setResizeCorner(null);
    };

    // ============================
    // KEYBOARD SHORTCUTS
    // ============================
    useEffect(() => {
        const handleKeyDown = async (e) => {
            if (selectedIndex === null) return;

            if (e.key === "Delete" || e.key.toLowerCase() === "d") {
                const box = boxes[selectedIndex];
                if (!box) return;

                try {
                    await deleteBoundingBox(box.id);
                } catch (err) {
                    console.error("❌ Failed to delete bounding box:", err);
                }

                // ✅ Save to history after deletion
                const newBoxes = boxes.filter((_, i) => i !== selectedIndex);
                saveToHistory(newBoxes);
                onSelectBox && onSelectBox(null);
                setSelectedIndex(null);
                return;
            }

            if (e.key === "Escape") {
                setSelectedIndex(null);
                onSelectBox && onSelectBox(null);
                return;
            }

            if (!isNaN(parseInt(e.key)) && labelClasses?.length) {
                const num = parseInt(e.key);
                const idx = num === 0 ? 9 : num - 1;
                const newClass = labelClasses[idx];
                if (!newClass) return;

                let updatedBox = null;
                const newBoxes = boxes.map((b, i) => {
                    if (i !== selectedIndex) return b;
                    const changed = {
                        ...b,
                        label: newClass.name,
                        labelClassId: newClass.id,
                        color: newClass.color,
                    };
                    updatedBox = changed;
                    return changed;
                });

                // ✅ Save to history after class change
                saveToHistory(newBoxes);
                onSelectBox && onSelectBox(updatedBox);

                if (updatedBox) {
                    try {
                        await updateBoundingBox(updatedBox.id, {
                            boundingBoxId: updatedBox.id,
                            labelClassId: updatedBox.labelClassId,
                            x1: updatedBox.x,
                            y1: updatedBox.y,
                            x2: updatedBox.x + updatedBox.width,
                            y2: updatedBox.y + updatedBox.height,
                            mediaId: currentMediaId,
                        });
                    } catch (err) {
                        console.error("❌ Failed to update bounding box class:", err);
                    }
                }
            }
        };

        window.addEventListener("keydown", handleKeyDown);
        return () => window.removeEventListener("keydown", handleKeyDown);
    }, [selectedIndex, boxes, labelClasses, onSelectBox, currentMediaId, history, historyIndex]);

    // ============================
    // HELPERS
    // ============================
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
    }, [boxes, tempBox, activeClass, selectedIndex, visibleLabel, isolateMode, viewX, viewY, viewScale, crosshairPos,]);

    // ✅ FEATURE: Cursor - Color Mode + Crosshair Toggle
    const getCursor = () => {
        if (isPanning || spaceDown) return "grab";

        if (drawing) return "crosshair";
        if (!showCrosshair) return "default";

        if (cursorColorMode && activeClass?.color) {
            const svg = `
      <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32">
        <line x1="16" y1="0" x2="16" y2="32" stroke="${activeClass.color}" stroke-width="2"/>
        <line x1="0" y1="16" x2="32" y2="16" stroke="${activeClass.color}" stroke-width="2"/>
        <circle cx="16" cy="16" r="2" fill="${activeClass.color}" />
      </svg>`;
            return `url('data:image/svg+xml;utf8,${encodeURIComponent(
                svg
            )}') 16 16, crosshair`;
        }

        return "crosshair";
    };

    if (!currentFile) {
        return (
            <div className="canvas-wrapper">
                <div className="image-canvas">
                    <p>No files uploaded for this project</p>
                </div>
            </div>
        );
    }

    return (
        <div className="canvas-wrapper">
            <div className="image-canvas">
                {currentFile && (
                    <div style={{
                        marginBottom: '10px',
                        display: 'flex',
                        justifyContent: 'space-between',
                        alignItems: 'center',
                        gap: '12px'
                    }}>
                        <button
                            onClick={() => onFileIndexChange && onFileIndexChange(Math.max(0, currentFileIndex - 1))}
                            disabled={currentFileIndex === 0}
                            style={{
                                padding: '8px 16px',
                                backgroundColor: '#e53935',
                                color: 'white',
                                border: 'none',
                                borderRadius: '6px',
                                cursor: currentFileIndex === 0 ? 'not-allowed' : 'pointer',
                                opacity: currentFileIndex === 0 ? 0.5 : 1,
                                fontWeight: 'bold',
                                fontSize: '14px'
                            }}
                        >
                            ← Previous
                        </button>

                        <span style={{
                            padding: '8px 16px',
                            backgroundColor: '#f5f5f5',
                            borderRadius: '6px',
                            fontWeight: 'bold',
                            color: '#333',
                            fontSize: '14px',
                            flex: 1,
                            textAlign: 'center',
                            margin: '0 16px'
                        }}>
                            File {currentFileIndex + 1} of {files.length}
                        </span>

                        <button
                            onClick={() => onFileIndexChange && onFileIndexChange(Math.min(files.length - 1, currentFileIndex + 1))}
                            disabled={currentFileIndex === files.length - 1}
                            style={{
                                padding: '8px 16px',
                                backgroundColor: '#e53935',
                                color: 'white',
                                border: 'none',
                                borderRadius: '6px',
                                cursor: currentFileIndex === files.length - 1 ? 'not-allowed' : 'pointer',
                                opacity: currentFileIndex === files.length - 1 ? 0.5 : 1,
                                fontWeight: 'bold',
                                fontSize: '14px'
                            }}
                        >
                            Next →
                        </button>
                    </div>
                )}

                <div className="image-wrapper">
                    <canvas
                        ref={canvasRef}
                        onMouseDown={handleMouseDown}
                        onMouseMove={handleMouseMove}
                        onMouseUp={handleMouseUp}
                        onMouseLeave={() => setCrosshairPos(null)}
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
        </div>
    );
}