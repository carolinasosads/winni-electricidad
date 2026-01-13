import ApiError from "./ApiError";

const urlAPIResena  = `${import.meta.env.VITE_API_URL}/WinniElectricidadApi/Resena/`;

function getAuthHeaders() {
  const token = localStorage.getItem("token");
  return {
    Accept: "application/json",
    "Content-Type": "application/json",
    Authorization: `Bearer ${token}`,
  };
}

async function handleJsonOrText(resp) {
  const ct = resp.headers.get("content-type") || "";
  if (ct.includes("application/json")) return await resp.json();
  return await resp.text();
}

// --- POSTs ---

export async function crearResena(formData, signal) {
  const token = localStorage.getItem("token");

  if (!token) {
    throw new ApiError("Usuario no autenticado.", 401);
  }

  const resp = await fetch(`${urlAPIResena}`, {
    method: "POST",
    headers: {
      Accept: "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: formData,
    signal,
  });

  if (resp.status === 401) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(data?.message || "Token inválido o expirado.", 401);
  }

  if (!resp.ok) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(
      data?.message || "Error al crear la reseña.",
      resp.status
    );
  }

  return await resp.json();
}

// --- GETs ---
export async function getResenasAprobadas() {
  const resp = await fetch(`${urlAPIResena}`, {
    method: "GET"
  });

  if (!resp.ok) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(data?.message || "Error obteniendo reseñas", resp.status);
  }

  return await resp.json();
}

// --- PATCHs ---

export async function desaprobarReseña(idReseña) {
  const resp = await fetch(`${urlAPIResena}${idReseña}/desaprobar`, {
    method: "PATCH",
    headers: getAuthHeaders(),
  });

  if (!resp.ok) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(data?.message || "Error desaprobando reseña", resp.status);
  }

  return await resp.json();
}

export async function getResenasDestacadas(signal) {
  const resp = await fetch(`${urlAPIResena}destacadas`, {
    method: "GET",
    signal,
  });

  if (!resp.ok) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(data?.message || "Error obteniendo reseñas destacadas", resp.status);
  }

  return await resp.json();
}