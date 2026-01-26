import ApiError from "./ApiError";

const urlAPIRecordatorio = `${import.meta.env.VITE_API_URL}/WinniElectricidadApi/Notificacion/`;

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

// --- POSTs ---

export const enviarRecordatorios = async (payload) => {
  const resp = await fetch(
    `${urlAPIRecordatorio}enviar-recordatorio`,
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        ...getAuthHeaders()
      },
      body: JSON.stringify(payload)
    }
  );

  if (!resp.ok) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(data?.message || "Error enviando los recordatorios estacionales", resp.status);
  }

  return resp;
};