import ApiError from "./ApiError";
const urlAPIPresupuesto = "https://winnielectricidadbe-dev-adgqcbd7gvbgg7fy.eastus2-01.azurewebsites.net/WinniElectricidadApi/Presupuesto/";

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
    throw new ApiError("Error de red al crear el presupuesto.");
  }
};
