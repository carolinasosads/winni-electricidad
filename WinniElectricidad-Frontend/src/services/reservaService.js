import ApiError from "./ApiError";

const urlAPIReserva  = `${import.meta.env.VITE_API_URL}/WinniElectricidadApi/Reserva/`;

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

export async function getReservasPorMesYAnio(mes, anio, filtro = "") {
  const qs = new URLSearchParams();
  if (filtro?.trim()) qs.set("filtro", filtro.trim());

  const url = `${urlAPIReserva}historico-mensual/${mes}/${anio}${
    qs.toString() ? `?${qs.toString()}` : ""
  }`;

  const resp = await fetch(url, {
    method: "GET",
    headers: getAuthHeaders(),
  });

  if (!resp.ok) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(data?.message || "Error obteniendo reservas del mes", resp.status);
  }

  return await resp.json();
}

export async function getReservasPorEstado(estado, filtro = "") {
  const qs = new URLSearchParams();
  qs.set("estado", estado);
  if (filtro?.trim()) qs.set("filtro", filtro.trim());

  const resp = await fetch(`${urlAPIReserva}por-estado?${qs.toString()}`, {
    method: "GET",
    headers: getAuthHeaders(),
  });

  if (!resp.ok) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(data?.message || "Error obteniendo reservas", resp.status);
  }

  return await resp.json();
}

export async function getReservasFinalizadas(filtro = "") {
  const qs = new URLSearchParams();
  if (filtro?.trim()) qs.set("filtro", filtro.trim());

  const url = `${urlAPIReserva}finalizadas${
    qs.toString() ? `?${qs.toString()}` : ""
  }`;

  const resp = await fetch(url, {
    method: "GET",
    headers: getAuthHeaders(),
  });

  if (!resp.ok) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(data?.message || "Error cargando historial de reservas", resp.status);
  }

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


export async function crearReservaAdmin(idUsuario, reservaDto) {
  const resp = await fetch(`${urlAPIReserva}admin/agendar/${idUsuario}`, {
    method: "POST",
    headers: getAuthHeaders(),
    body: JSON.stringify(reservaDto),
  });

  if (!resp.ok) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(data?.message || "Error creando reserva desde admin", resp.status);
  }

  return await resp.json(); 
}

export async function getReservasPorClienteAdmin(clienteId) {
  const resp = await fetch(
    `${urlAPIReserva}Admin/Cliente/${clienteId}`,
    {
      method: "GET",
      headers: getAuthHeaders(),
    }
  );

  if (!resp.ok) {
    const text = await resp.text();
    console.error("getReservasPorClienteAdmin ERROR:", resp.status, text);
    throw new ApiError(text || "Error al obtener reservas del cliente");
  }

  return await resp.json();
}


export async function crearReservaHistoricaAdmin(idUsuario, reservaDto) {
  const resp = await fetch(`${urlAPIReserva}admin/registrar-historico/${idUsuario}`, {
    method: "POST",
    headers: getAuthHeaders(),
    body: JSON.stringify(reservaDto),
  });

  if (!resp.ok) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(data?.message || "Error creando reserva histórica desde admin", resp.status);
  }

  return await resp.json(); 
}

export async function getMisReservasCliente() {
  const resp = await fetch(`${urlAPIReserva}mis-reservas`, {
    method: "GET",
    headers: getAuthHeaders(),
  });

  if (!resp.ok) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(data?.message || "Error obteniendo tus reservas", resp.status);
  }

  return await resp.json();
}
