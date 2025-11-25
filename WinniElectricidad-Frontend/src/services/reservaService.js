import ApiError from "./ApiError";

const urlAPIReserva  = "http://localhost:5269/WinniElectricidadApi/Reserva/"
;

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

// --- GETs ---

export async function getReservasPorMes(mes) {
  const resp = await fetch(`${urlAPIReserva}historico-mensual/${mes}`, {
    method: "GET",
    headers: getAuthHeaders(),
  });

  if (!resp.ok) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(data?.message || "Error obteniendo reservas del mes", resp.status);
  }

  return await resp.json();
}

export async function getPendientes() {
  const resp = await fetch(`${urlAPIReserva}pendientes`, {
    method: "GET",
    headers: getAuthHeaders(),
  });
  if (!resp.ok) throw new ApiError("Error obteniendo pendientes", resp.status);
  return await resp.json();
}

export async function getConfirmadas() {
  const resp = await fetch(`${urlAPIReserva}confirmadas`, {
    method: "GET",
    headers: getAuthHeaders(),
  });
  if (!resp.ok) throw new ApiError("Error obteniendo confirmadas", resp.status);
  return await resp.json();
}

export async function getCanceladas() {
  const resp = await fetch(`${urlAPIReserva}canceladas`, {
    method: "GET",
    headers: getAuthHeaders(),
  });
  if (!resp.ok) throw new ApiError("Error obteniendo canceladas", resp.status);
  return await resp.json();
}

export async function getFinalizadas() {
  const resp = await fetch(`${urlAPIReserva}historico`, {
    method: "GET",
    headers: getAuthHeaders(),
  });
  if (!resp.ok) throw new ApiError("Error obteniendo finalizadas", resp.status);
  return await resp.json();
}

// --- PATCHs ---

export async function aprobarReserva(idReserva) {
  const resp = await fetch(`${urlAPIReserva}aprobar/${idReserva}`, {
    method: "PATCH",
    headers: getAuthHeaders(),
  });

  if (!resp.ok) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(data?.message || "Error aprobando reserva", resp.status);
  }
}

export async function cancelarReserva(idReserva) {
  const resp = await fetch(`${urlAPIReserva}cancelar/${idReserva}`, {
    method: "PATCH",
    headers: getAuthHeaders(),
  });

  if (!resp.ok) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(data?.message || "Error cancelando reserva", resp.status);
  }
}

export async function modificarReserva(idReserva, nuevaFecha) {
  const resp = await fetch(`${urlAPIReserva}modificar`, {
    method: "PATCH",
    headers: getAuthHeaders(),
    body: JSON.stringify({ idReserva, nuevaFecha }),
  });

  if (!resp.ok) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(data?.message || "Error modificando reserva", resp.status);
  }
}
