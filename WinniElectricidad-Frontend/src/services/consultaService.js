import ApiError from "./ApiError";

const urlAPIConsulta = `${import.meta.env.VITE_API_URL}/WinniElectricidadApi/Consultas/`;

async function readJsonSafe(resp) {
  try {
    return await resp.json();
  } catch {
    return null;
  }
}

function buildErrorMessage(data, fallback) {
  if (data?.errors && typeof data.errors === "object") {
    const msgs = Object.entries(data.errors)
      .flatMap(([campo, arr]) =>
        Array.isArray(arr) ? arr.map((m) => `${campo}: ${m}`) : [`${campo}: ${arr}`]
      )
      .filter(Boolean);

    if (msgs.length > 0) return msgs.join(" | ");
  }

  return data?.message || data?.title || fallback;
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
      const msg = buildErrorMessage(
        data,
        "No se pudo enviar la consulta. Intente nuevamente."
      );

      throw new ApiError(msg, resp.status);
    }

    const okData = await resp.json();
    return okData;
  } catch (err) {
    if (err?.name === "AbortError") return;
    
    if (err instanceof ApiError) throw err;

    throw new ApiError(
      "No se pudo conectar con el servidor. Verificá tu conexión e intentá nuevamente.",
      0
    );
  }
}