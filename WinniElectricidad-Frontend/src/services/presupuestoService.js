import ApiError from "./ApiError";

const urlAPIPresupuesto = `${import.meta.env.VITE_API_URL}/WinniElectricidadApi/Presupuesto/`;


async function handleJsonOrText(res) {
  const contentType = res.headers.get("content-type") || "";
  if (contentType.includes("application/json")) return await res.json();
  return await res.text();
}

function getAuthHeaders() {
  const token = localStorage.getItem("token");

  const headers = {
    Accept: "application/json",
    "Content-Type": "application/json",
  };

  if (token) {
    headers.Authorization = `Bearer ${token}`;
  }

  return headers;
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

export const obtenerPresupuestoSegunReserva = async (idReserva) => {
  try {
    const res = await fetch(`${urlAPIPresupuesto}reserva/${idReserva}`, {
      method: "GET",
      headers: getAuthHeaders(),
    });

    if (res.status === 404) {
      return null; 
    }

    if (!res.ok) {
      const data = await handleJsonOrText(res);
      const message = data?.message || "Error al obtener el presupuesto de la reserva.";
      throw new ApiError(message, res.status);
    }

    return await res.json();
  } catch (err) {
    if (err instanceof ApiError) throw err;
    console.error(err);
    throw new ApiError("Error al obtener el presupuesto.");
  }
};

export const actualizarMontoPagadoPresupuesto = async (idReserva, montoPagado) => {
  try {
    const res = await fetch(
      `${urlAPIPresupuesto}reserva/${idReserva}/monto-pagado`,
      {
        method: "PATCH",
        headers: getAuthHeaders(),
        body: JSON.stringify({ montoPagado: Number(montoPagado) }),
      }
    );

    const data = await handleJsonOrText(res);

    if (!res.ok) {
      let msg = data?.message || "Error al actualizar el monto pagado.";
      if (data?.errors) msg = Object.values(data.errors).flat().join(" ");
      throw new ApiError(
        typeof msg === "string" ? msg : "Error al actualizar el monto pagado.",
        res.status
      );
    }

    return data;
  } catch (err) {
    if (err instanceof ApiError) throw err;
    console.error(err);
    throw new ApiError("Error al actualizar el monto pagado.");
  }
};