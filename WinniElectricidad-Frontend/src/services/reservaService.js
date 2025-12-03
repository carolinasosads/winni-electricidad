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

export async function getReservasPorMesYAnio(mes, anio) {
  const resp = await fetch(`${urlAPIReserva}historico-mensual/${mes}?anio=${anio}`, {
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
  const resp = await fetch(`${urlAPIReserva}historico`, {
    method: "GET",
    headers: getAuthHeaders(),
  });

  if (!resp.ok) throw new ApiError("Error cargando historial de reservas");

  return await resp.json();
}

/*
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
}*/

const mockReservas = [
  {
    id: 1,
    clienteNombre: "Juan Pérez",
    servicioNombre: "Electricidad",
    direccion: "Av. Italia 1234",
    fechaReserva: "2025-12-25T09:00:00",
    estado: "Pendiente"
  },
  {
    id: 2,
    clienteNombre: "María Gómez",
    servicioNombre: "Riego",
    direccion: "Bvar. Artigas 560",
    fechaReserva: "2025-12-26T11:00:00",
    estado: "Confirmada"
  },
  {
    id: 3,
    clienteNombre: "Carlos López",
    servicioNombre: "Pintura",
    direccion: "Rivera 2030",
    fechaReserva: "2025-12-27T15:00:00",
    estado: "Cancelada"
  },
  {
    id: 4,
    clienteNombre: "Ana Suárez",
    servicioNombre: "Plomería",
    direccion: "Mercedes 1320",
    fechaReserva: "2025-12-28T10:30:00",
    estado: "Finalizada"
  }
  ,
  {
    id: 5,
    clienteNombre: "Ana Suárez",
    servicioNombre: "Plomería",
    direccion: "Mercedes 1320",
    fechaReserva: "2025-11-28T10:00:00",
    estado: "Confirmada"
  }
];

export async function getPendientes() {
  return mockReservas.filter(r => r.estado === "Pendiente");
}

export async function getConfirmadas() {
  return mockReservas.filter(r => r.estado === "Confirmada");
}

export async function getCanceladas() {
  return mockReservas.filter(r => r.estado === "Cancelada");
}

export async function getFinalizadas() {
  return mockReservas.filter(r => r.estado === "Finalizada");
}


