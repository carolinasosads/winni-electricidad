import ApiError from "./ApiError";

const urlAPIServicio = `${import.meta.env.VITE_API_URL}/WinniElectricidadApi/Servicio/`;

function getAuthHeaders() {
  const token = localStorage.getItem("token");
  return {
    Accept: "application/json",
    "Content-Type": "application/json",
    Authorization: `Bearer ${token}`,
  };
}


// --- GETs ---

export async function getServiciosActivos(signal) {
  const token = localStorage.getItem("token");

  const resp = await fetch(`${urlAPIServicio}activos`, {
    method: "GET",
    headers: {
      Accept: "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
    },
    signal,
  });

  if (!resp.ok) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(
      data?.message || "No se pudieron obtener los servicios.",
      resp.status
    );
  }

  return await resp.json(); 
}

export const getServicios = async (signal) => {
  const token = localStorage.getItem("token");

  const resp = await fetch(`${urlAPIServicio}`, {
    method: "GET",
    headers: {
      Accept: "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
    },
    signal,
  });

  if (!resp.ok) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(
      data?.message || "No se pudieron obtener los servicios.",
      resp.status
    );
  }

  return await resp.json(); 
};

// -- PATCHs --

export async function desactivarServicio(idServicio) {
  const resp = await fetch(`${urlAPIServicio}desactivar/${idServicio}`, {
    method: "PATCH",
        headers: getAuthHeaders(),
      });
    
    if (!resp.ok) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(data?.message || `Error activando el servicio con id: ${idServicio}.`, resp.status);
    }
    
    return await resp.json();
}

export async function activarServicio(idServicio) {
  const resp = await fetch(`${urlAPIServicio}activar/${idServicio}`, {
    method: "PATCH",
        headers: getAuthHeaders(),
      });
    
    if (!resp.ok) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(data?.message || `Error activando el servicio con id: ${idServicio}.`, resp.status);
    }
    
    return await resp.json();
}

async function handleJsonOrText(resp) {
    const ct = resp.headers.get("content-type") || "";
    if (ct.includes("application/json")) {
        const data = await resp.json();
        
        if (data.errors) {
            const msg = Object.values(data.errors)
                .flat()
                .join(" ");
            return { message: msg };
        }
        
        if (data.message) {
            return { message: data.message };
        }
        
        return { message: JSON.stringify(data) };
    }
    const t = await resp.text();
    try {
        return { message: JSON.parse(t).message || t };
    } catch {
        return { message: t || null };
    }
}