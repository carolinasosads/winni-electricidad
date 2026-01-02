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

export const crearPresupuestoParaReserva = async (idReserva, dto, signal) => {
  try {
    const res = await fetch(`${urlAPIPresupuesto}reserva/${idReserva}`, {
      method: "POST",
      headers: getAuthHeaders(),
      body: JSON.stringify(dto),
      signal,
    });

    if (!res.ok) {
      const data = await handleJsonOrText(res);
      const message =
        data?.message || "Error al crear el presupuesto de la reserva.";
      throw new ApiError(message, res.status);
    }

    return await res.json();
  } catch (err) {
    if (err?.name === "AbortError") return; 
    if (err instanceof ApiError) throw err;
    console.error(err);
    throw new ApiError("Error al crear el presupuesto.");
  }
};

export const obtenerPresupuestoSegunReserva = async (idReserva, signal) => {
  try {
    const res = await fetch(`${urlAPIPresupuesto}reserva/${idReserva}`, {
      method: "GET",
      headers: getAuthHeaders(),
      signal,
    });

    if (res.status === 404) {
      return null;
    }

    if (!res.ok) {
      const data = await handleJsonOrText(res);
      const message =
        data?.message || "Error al obtener el presupuesto de la reserva.";
      throw new ApiError(message, res.status);
    }

    return await res.json();
  } catch (err) {
    if (err?.name === "AbortError") return; // request cancelado
    if (err instanceof ApiError) throw err;
    console.error(err);
    throw new ApiError("Error al obtener el presupuesto.");
  }
};

export const registrarPagoPresupuesto = async (idReserva, monto, signal) => {
  try {
    const res = await fetch(`${urlAPIPresupuesto}reserva/${idReserva}/pagos`, {
      method: "POST",
      headers: getAuthHeaders(),
      body: JSON.stringify({ monto: Number(monto) }),
      signal,
    });

    const data = await handleJsonOrText(res);

    if (!res.ok) {
      let msg = data?.message || "Error al registrar el pago.";
      if (data?.errors) msg = Object.values(data.errors).flat().join(" ");
      throw new ApiError(typeof msg === "string" ? msg : "Error al registrar el pago.", res.status);
    }

    return data;
  } catch (err) {
    if (err?.name === "AbortError") return;
    if (err instanceof ApiError) throw err;
    console.error(err);
    throw new ApiError("Error al registrar el pago.");
  }
};