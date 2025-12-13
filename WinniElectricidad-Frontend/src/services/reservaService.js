import ApiError from "./ApiError";

const urlAPIReserva  = "https://winnielectricidadbe-dev-adgqcbd7gvbgg7fy.eastus2-01.azurewebsites.net/WinniElectricidadApi/Reserva/";

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
export const crearPresupuestoParaReserva = async (idReserva, dto) => {
  try {
    const res = await fetch(`${urlAPIPresupuesto}reserva/${idReserva}`, {
      method: "POST",
      headers: getAuthHeaders(),
      body: JSON.stringify(dto),
    });

    if (!res.ok) {
      const data = await handleJsonOrText(res);
      const message = data?.message || "Error al crear el presupuesto de la reserva.";
      throw new ApiError(message, res.status);
    }

    return await res.json(); 
  } catch (err) {
    if (err instanceof ApiError) throw err;
    console.error(err);
    throw new ApiError("Error de red al crear el presupuesto.");
  }
};

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

export async function getPresupuestoPorReserva(idReserva) {
  const resp = await fetch(`${urlAPIPresupuesto}reserva/${idReserva}`, {
    method: "GET",
    headers: getAuthHeaders(),
  });

  // 404 = la reserva no tiene presupuesto
  if (resp.status === 404) return null;

  if (!resp.ok) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(
      data?.message || "Error obteniendo presupuesto de la reserva",
      resp.status
    );
  }
  return await resp.json();
}