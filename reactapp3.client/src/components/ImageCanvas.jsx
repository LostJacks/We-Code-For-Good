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
  mediaId = 1, // 👈 use real mediaId from parent
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
  const MIN_BOX_SIZE = 5;

  // ============================
  // LOAD IMAGE + BOXES FROM BACKEND
  // ============================
  useEffect(() => {
    const img = new Image();
    img.src = imageUrl;
    img.onload = async () => {
      imgRef.current = img;
      resizeCanvas();

      try {
        const boxesFromServer = await getBoundingBoxesByMedia(mediaId);
        console.log("✅ Loaded boxes from API:", boxesFromServer);

        setBoxes(
          boxesFromServer.map((b) => {
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
          })
        );
      } catch (err) {
        console.error("❌ Failed to load bounding boxes:", err);
      }
    };
  }, [imageUrl, mediaId, labelClasses]);

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

  // ============================
  // DRAWING LOGIC
  // ============================
  const drawAll = (tempBox = null) => {
    const canvas = canvasRef.current;
    if (!canvas || !imgRef.current) return;
    const ctx = canvas.getContext("2d");

    ctx.clearRect(0, 0, canvas.width, canvas.height);
    const { width, height } = canvas.getBoundingClientRect();
    ctx.drawImage(imgRef.current, 0, 0, width, height);

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

  // ============================
  // MOUSE HANDLERS
  // ============================
  const getMousePos = (e) => {
    const rect = e.target.getBoundingClientRect();
    const scaleX = canvasRef.current.width / rect.width;
    const scaleY = canvasRef.current.height / rect.height;
    const dpr = window.devicePixelRatio || 1;

    return {
      x: ((e.clientX - rect.left) * scaleX) / dpr,
      y: ((e.clientY - rect.top) * scaleY) / dpr,
    };
  };

  const handleMouseDown = (e) => {
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
    const { x, y } = getMousePos(e);

    if (drawing) {
      const width = Math.abs(x - start.x);
      const height = Math.abs(y - start.y);
      if (width > MIN_BOX_SIZE && height > MIN_BOX_SIZE) {
        drawAll({
          x: Math.min(start.x, x),
          y: Math.min(start.y, y),
          width,
          height,
          color: activeClass?.color || "#e53935",
          label: activeClass?.name || "Unlabeled",
        });
      } else {
        drawAll();
      }
    } else if (dragging && selectedIndex !== null) {
      const dx = x - start.x;
      const dy = y - start.y;

      setBoxes((prev) => {
        const next = prev.map((b, i) =>
          i === selectedIndex ? { ...b, x: b.x + dx, y: b.y + dy } : b
        );
        onBoxesChange && onBoxesChange(next);
        onSelectBox && onSelectBox(next[selectedIndex]);
        return next;
      });

      setStart({ x, y });
      drawAll();
    } else if (resizing && selectedIndex !== null) {
      setBoxes((prev) => {
        const next = prev.map((b, i) => {
          if (i !== selectedIndex) return b;
          const newBox = { ...b };
          resizeBox(newBox, x, y, resizeCorner);
          return newBox;
        });
        onBoxesChange && onBoxesChange(next);
        onSelectBox && onSelectBox(next[selectedIndex]);
        return next;
      });
      drawAll();
    }
  };

  const handleMouseUp = async (e) => {
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
          // use activeClass for visual label + color
          labelClassId: activeClass?.id ?? null,
          label: activeClass?.name || "Unlabeled",
          color: activeClass?.color || "#e53935",
        };

        const payload = {
          mediaId,
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

          setBoxes((prev) => {
            const next = [...prev, newBoxWithId];
            onBoxesChange && onBoxesChange(next);
            onSelectBox && onSelectBox(newBoxWithId);
            return next;
          });
        } catch (err) {
          console.error("❌ Failed to create bounding box:", err);
          // fallback so it doesn't disappear visually
          const fallback = { ...newBox, id: Date.now() };
          setBoxes((prev) => {
            const next = [...prev, fallback];
            onBoxesChange && onBoxesChange(next);
            onSelectBox && onSelectBox(fallback);
            return next;
          });
        }
      }
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
        });
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

      // Delete
      if (e.key === "Delete" || e.key.toLowerCase() === "d") {
        const box = boxes[selectedIndex];
        if (!box) return;

        try {
          await deleteBoundingBox(box.id);
        } catch (err) {
          console.error("❌ Failed to delete bounding box:", err);
        }

        setBoxes((prev) => {
          const next = prev.filter((_, i) => i !== selectedIndex);
          onBoxesChange && onBoxesChange(next);
          onSelectBox && onSelectBox(null);
          return next;
        });
        setSelectedIndex(null);
        return;
      }

      // Escape → deselect
      if (e.key === "Escape") {
        setSelectedIndex(null);
        onSelectBox && onSelectBox(null);
        return;
      }

      // Number keys → change class of SELECTED box only
      if (!isNaN(parseInt(e.key)) && labelClasses?.length) {
        const num = parseInt(e.key);
        const idx = num === 0 ? 9 : num - 1;
        const newClass = labelClasses[idx];
        if (!newClass) return;

        // 1) update local state
        let updatedBox = null;
        setBoxes((prev) => {
          const next = prev.map((b, i) => {
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
          onBoxesChange && onBoxesChange(next);
          onSelectBox && onSelectBox(updatedBox);
          return next;
        });

        // 2) sync with backend
        if (updatedBox) {
          try {
            await updateBoundingBox(updatedBox.id, {
              boundingBoxId: updatedBox.id,
              labelClassId: updatedBox.labelClassId,
              x1: updatedBox.x,
              y1: updatedBox.y,
              x2: updatedBox.x + updatedBox.width,
              y2: updatedBox.y + updatedBox.height,
            });
          } catch (err) {
            console.error("❌ Failed to update bounding box class:", err);
          }
        }
      }
    };

    window.addEventListener("keydown", handleKeyDown);
    return () => window.removeEventListener("keydown", handleKeyDown);
  }, [selectedIndex, boxes, labelClasses, onBoxesChange, onSelectBox]);

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

  // Redraw when data changes
  useEffect(() => {
    drawAll();
  }, [boxes, activeClass, selectedIndex, visibleLabel]);

  const getCursor = () => {
    if (drawing) return "crosshair";
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

  return (
    <div className="canvas-wrapper">
      <div className="image-canvas">
        <div className="image-wrapper">
          <canvas
            ref={canvasRef}
            onMouseDown={handleMouseDown}
            onMouseMove={handleMouseMove}
            onMouseUp={handleMouseUp}
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
