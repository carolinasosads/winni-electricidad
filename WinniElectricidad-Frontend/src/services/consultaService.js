import ApiError from "./ApiError";

const urlAPIConsulta = `${import.meta.env.VITE_API_URL}/WinniElectricidadApi/Consultas/`;

async function readJsonSafe(resp) {
  try {
    return await resp.json();
  } catch {
    return null;
  }
}

export async function crearConsulta(dto, signal) {
  try {
    const resp = await fetch(`${urlAPIConsulta}crear`, {
      method: "POST",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify(dto),
      signal,
    });

    if (!resp.ok) {
      const data = await readJsonSafe(resp);
      const msg =
        data?.message ||
        data?.title ||
        "No se pudo enviar la consulta. Intente nuevamente.";
      throw new ApiError(msg, resp.status);
    }

    return await resp.json();
  } catch (err) {
    if (err?.name === "AbortError") return;

    if (err instanceof ApiError) throw err;

    throw new ApiError(
      "No se pudo conectar con el servidor. Verificá tu conexión e intentá nuevamente.",
      0
    );
  }
}