import ApiError from "./ApiError";

const urlAPIUsuario = `${import.meta.env.VITE_API_URL}/WinniElectricidadApi/Usuario/`;

function getAuthHeaders() {
  const token = localStorage.getItem("token");
  if (!token) throw new ApiError("Usuario no autenticado.", 401);

  return {
    Accept: "application/json",
    Authorization: `Bearer ${token}`,
  };
}

async function handleJsonOrText(resp) {
  const ct = resp.headers.get("content-type") || "";
  if (ct.includes("application/json")) return await resp.json();
  return await resp.text();
}

export async function listarClientesAdmin(signal) {
  const resp = await fetch(`${urlAPIUsuario}admin/clientes`, {
    method: "GET",
    headers: getAuthHeaders(),
    signal,
  });

  if (!resp.ok) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(data?.message || "Error obteniendo clientes.", resp.status);
  }

  return await resp.json();
}

export async function obtenerDetalleClienteAdmin(idUsuario, signal) {
  const resp = await fetch(`${urlAPIUsuario}admin/clientes/${idUsuario}`, {
    method: "GET",
    headers: getAuthHeaders(),
    signal,
  });

  if (resp.status === 404) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(data?.message || "Usuario no encontrado.", 404);
  }

  if (!resp.ok) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(data?.message || "Error obteniendo detalle del cliente.", resp.status);
  }

  return await resp.json();
}