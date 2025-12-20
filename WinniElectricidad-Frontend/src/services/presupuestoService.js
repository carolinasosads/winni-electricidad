import ApiError from "./ApiError";

const urlAPIPresupuesto = `${import.meta.env.VITE_API_URL}/WinniElectricidadApi/Presupuesto/`;


async function handleJsonOrText(res) {
  const contentType = res.headers.get("content-type") || "";
  if (contentType.includes("application/json")) return await res.json();
  return await res.text();
}

function getAuthHeaders() {
  const token = localStorage.getItem("token");
  return {
    Accept: "application/json",
    "Content-Type": "application/json",
    Authorization: `Bearer ${token}`,
  };
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
    throw new ApiError("Error al crear el presupuesto.");
  }
};

export const obtenerPresupuestoSegunReserva = async (idReserva, token, signal) => {
  try {
    if (!idReserva || Number.isNaN(Number(idReserva))) {
      throw new ApiError("Id de reserva inválido.", 400);
    }

    const response = await fetch(`${urlAPIPresupuesto}reserva/${idReserva}`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
      }, signal,
    });

    const data = await handleJsonOrText(response);

    if (response.status === 401) {
      throw new ApiError(data?.message || "No autenticado.", 401);
    }

    if (response.status === 404) {
      throw new ApiError(data?.message || "No existe presupuesto para esa reserva.", 404);
    }

    if (!response.ok) {
      throw new ApiError(data?.message || "Error al obtener el presupuesto.", response.status);
    }

    return data; 
  } catch (err) {
    if (err?.name === "AbortError") throw err;
    if (err instanceof ApiError) throw err;

    console.error(err);
    throw new ApiError("Error al obtener el presupuesto.");
  }
};

export const actualizarMontoPagadoPresupuesto = async (idReserva, montoPagado, token) => {
  try {
    const response = await fetch(
      `${urlAPIPresupuesto}reserva/${idReserva}/monto-pagado`,
      {
        method: "PATCH",
        headers: {
          "Content-Type": "application/json",
          ...(token ? { Authorization: `Bearer ${token}` } : {}),
        },
        body: JSON.stringify({ montoPagado: Number(montoPagado) }),
      }
    );

    const data = await handleJsonOrText(response);

    if (!response.ok) {
      let msg = data?.message || "Error al actualizar el monto pagado.";
      if (data?.errors) msg = Object.values(data.errors).flat().join(" ");
      throw new ApiError(typeof msg === "string" ? msg : "Error al actualizar el monto pagado.", response.status);
    }

    return data;
  } catch (err) {
    if (err instanceof ApiError) throw err;
    console.error(err);
    throw new ApiError("Error al actualizar el monto pagado.");
  }
    };