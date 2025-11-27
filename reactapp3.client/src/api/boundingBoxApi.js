const API_BASE_URL = "https://localhost:7166/api/boundingbox";

// --- Get all boxes for a media ---
export async function getBoundingBoxesByMedia(mediaId) {
  const response = await fetch(`${API_BASE_URL}/media/${mediaId}`);
  if (!response.ok) throw new Error("Failed to fetch bounding boxes");
  return await response.json();
}

// --- Create new box ---
export async function createBoundingBox(boxData) {
  const response = await fetch(`${API_BASE_URL}`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(boxData),
  });
  if (!response.ok) throw new Error("Failed to create bounding box");
  return await response.json();
}

// --- Update box ---
export async function updateBoundingBox(id, boxData) {
  const response = await fetch(`${API_BASE_URL}/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(boxData),
  });
  if (!response.ok) throw new Error("Failed to update bounding box");
}

// --- Delete box ---
export async function deleteBoundingBox(id) {
  const response = await fetch(`${API_BASE_URL}/${id}`, {
    method: "DELETE",
  });
  if (!response.ok) throw new Error("Failed to delete bounding box");
}
