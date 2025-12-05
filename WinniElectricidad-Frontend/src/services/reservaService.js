import ApiError from "./ApiError";

const urlAPIReserva  = "https://winnielectricidadbe-dev-adgqcbd7gvbgg7fy.eastus2-01.azurewebsites.net/WinniElectricidadApi/Reserva/"
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

export async function getReservasPorMesYAnio(mes, anio) {
  const resp = await fetch(`${urlAPIReserva}historico-mensual/${mes}/${anio}`, {
    method: "GET",
    headers: getAuthHeaders(),
  });

  if (!resp.ok) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(data?.message || "Error obteniendo reservas del mes", resp.status);
  }

  return await resp.json();
}

export async function getReservasPorEstado(estado) {
  const resp = await fetch(`${urlAPIReserva}por-estado?estado=${estado}`, {
    method: "GET",
    headers: getAuthHeaders(),
  });

  if (!resp.ok) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(data?.message || "Error obteniendo reservas", resp.status);
  }

  return await resp.json();
}

export async function getReservasFinalizadas() {
  const resp = await fetch(`${urlAPIReserva}finalizadas`, {
    method: "GET",
    headers: getAuthHeaders(),
  });

  if (!resp.ok) throw new ApiError("Error cargando historial de reservas");

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

  return await resp.json();
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

  return await resp.json();
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

  return await resp.json();
}

