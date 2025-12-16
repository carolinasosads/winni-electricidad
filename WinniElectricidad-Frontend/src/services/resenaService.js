import ApiError from "./ApiError";

const urlAPIResena  = "https://winnielectricidadbe-dev-adgqcbd7gvbgg7fy.eastus2-01.azurewebsites.net/WinniElectricidadApi/Resena";

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