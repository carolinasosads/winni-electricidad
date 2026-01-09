import ApiError from "./ApiError";

const urlAPIUsuario = `${import.meta.env.VITE_API_URL}/WinniElectricidadApi/Usuario/`;
const urlAPIReserva  = `${import.meta.env.VITE_API_URL}/WinniElectricidadApi/Reserva/`;

function tryGetUserIdFromJwt(token) {
  if (!token || typeof token !== "string") return null;

  try {
    const parts = token.split(".");
    if (parts.length < 2) return null;

    const base64 = parts[1].replace(/-/g, "+").replace(/_/g, "/");
    const padded = base64.padEnd(base64.length + ((4 - (base64.length % 4)) % 4), "=");

    const json = JSON.parse(atob(padded));

    const candidates = [
      json?.id,
      json?.Id,
      json?.userId,
      json?.UserId,
      json?.sub,
      json?.nameid,
      json?.["nameid"],
      json?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"],
    ];

    const found = candidates.find((x) => x !== undefined && x !== null && `${x}`.trim() !== "");
    if (!found) return null;

    const asNumber = Number(found);
    if (!Number.isNaN(asNumber) && Number.isFinite(asNumber)) return asNumber;

    return `${found}`;
  } catch {
    return null;
  }
}

function saveAuthData({ token, rol }) {
  if (token) localStorage.setItem("token", token);
  if (rol) localStorage.setItem("rol", rol);

  const userId = tryGetUserIdFromJwt(token);
  if (userId !== null && userId !== undefined) {
    localStorage.setItem("idCliente", `${userId}`);
  }
}

export const login = async (email, password) => {
  try {
    const response = await fetch(`${urlAPIUsuario}login`,{
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({email,password}),
    });

    if (response.status === 401) {
      const data = await handleJsonOrText(response);
      throw new ApiError(data?.message || "Credenciales inválidas.", 401);
    }

    if (!response.ok) {
      const data = await handleJsonOrText(response);
      throw new ApiError(data?.message || "Error al iniciar sesión.", response.status);
    }

    const data = await response.json();

    const { token, rol } = data;

    saveAuthData({ token, rol });

    return data;

  } catch (err) {
    if (err instanceof TypeError) {
      throw new Error(
        "No se pudo conectar con el servidor. Verifica tu conexión o inténtalo más tarde."
      );
    }
    throw err;
  }
};

export const registro = async (p) => {
  try {
    const payload = {
      nombreCompleto: p.name,
      email: p.email,
      password: p.password,
      telefono: normalizarTelefono(p.telefono),
      hcaptchaToken: p.hcaptchaToken,
      direcciones: [
        normalizarDireccion(p.direccionPrincipal),
        ...(Array.isArray(p.direcciones) ? p.direcciones.map(d => normalizarDireccion(d)) : []),
      ].filter(d => d.calle || d.esquina || d.numero || d.apto),
    };

    const resp = await fetch(`${urlAPIUsuario}registro`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload),
    });

    if (!resp.ok) {
      if (resp.status === 409) {
        throw new ApiError("El email ya está en uso.", 409);
      }

      if (resp.status === 400) {
        const data = await handleJsonOrText(resp);
        throw new ApiError(data?.message || "Datos inválidos o captcha no verificado.", 400);
      }

      const data = await handleJsonOrText(resp);
      throw new ApiError(data?.message || "Error al registrarse.", resp.status);
    }

    const data = await resp.json();

    const token = data?.token;
    const rol = data?.rol;

    saveAuthData({ token, rol });

    return data;

  } catch (err) {
    if (err instanceof TypeError) {
      throw new Error(
        "No se pudo conectar con el servidor. Verifica tu conexión o inténtalo más tarde."
      );
    }
    throw err;
  }
};

export const logout = () => {
  localStorage.removeItem("token");
  localStorage.removeItem("rol");
  localStorage.removeItem("idCliente");
};

export const sendPasswordRecoveryEmail = async (email) => {
  try {
    const response = await fetch(`${urlAPIUsuario}forgot-password`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email }),
    });

    if (!response.ok) {
      const data = await handleJsonOrText(response);
      throw new ApiError(
        data?.message || "Error desconocido al enviar el correo.",
        response.status
      );
    }

    return await response.json();

  } catch (err) {
    if (err instanceof TypeError) {
      throw new Error(
        "No se pudo conectar con el servidor. Verifica tu conexión o inténtalo más tarde."
      );
    }
    throw err;
  }
};

export const resetPassword = async (password, tokenPlain) => {
  try {
    const response = await fetch(`${urlAPIUsuario}reset-password`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ password, tokenPlain }),
    });

    if (!response.ok) {
      const data = await handleJsonOrText(response);
      throw new ApiError(
        data?.message || "Error desconocido al restablecer la contraseña.",
        response.status
      );
    }

    return await response.json();

  } catch (err) {
    if (err instanceof TypeError) {
      throw new Error("No se pudo conectar con el servidor. Inténtalo más tarde.");
    }
    throw err;
  }
};

async function handleJsonOrText(resp) {
  const ct = resp.headers.get("content-type") || "";
  if (ct.includes("application/json")) {
    const data = await resp.json();

    if (data.errors) {
      const msg = Object.values(data.errors).flat().join(" ");
      return { message: msg };
    }

    if (data.message) return { message: data.message };

    return { message: JSON.stringify(data) };
  }

  const t = await resp.text();
  try {
    return { message: JSON.parse(t).message || t };
  } catch {
    return { message: t || null };
  }
}

function normalizarTelefono(t) {
  return (t ?? "")
    .toString()
    .replace(/\D/g, "")
    .slice(0, 15);
}

function normalizarDireccion(d) {
  if (!d) return { calle: "", esquina: "", numero: null, apto: null };

  return {
    calle: d.calle?.trim() || "",
    esquina: d.esquina?.trim() || "",
    numero: d.numero?.toString().trim() || null,
    apto: d.apto?.toString().trim() || null
  };
}

export async function getDireccionesUsuario(signal) {
  const token = localStorage.getItem("token");

  if (!token) {
    throw new ApiError("Usuario no autenticado.", 401);
  }

  const resp = await fetch(`${urlAPIUsuario}direcciones`, {
    method: "GET",
    headers: {
      Accept: "application/json",
      Authorization: `Bearer ${token}`,
    },
    signal,
  });

  if (!resp.ok) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(
      data?.message || "No se pudieron obtener las direcciones.",
      resp.status
    );
  }

  const json = await resp.json();
  return json;
}

export async function getHorariosDisponibles(signal) {
  const token = localStorage.getItem("token");
  const response = await fetch(`${urlAPIReserva}disponibilidad`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    signal,
  });

  if (!response.ok) {
    const text = await response.text();
    throw new Error(
      `Error al obtener disponibilidad horaria (${response.status}): ${text}`
    );
  }

  return await response.json();
}

export async function crearReserva(reserva, signal) {
  const token = localStorage.getItem("token");

  if (!token) {
    throw new ApiError("Usuario no autenticado.", 401);
  }

  const resp = await fetch(`${urlAPIReserva}agendar`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Accept: "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(reserva),
    signal,
  });

  if (resp.status === 401) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(data?.message || "Token inválido o expirado.", 401);
  }

  if (resp.status === 409) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(
      data?.message || "El horario seleccionado ya no está disponible.",
      409
    );
  }

  if (!resp.ok) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(data?.message || "Error al crear la reserva.", resp.status);
  }

  return await resp.json();
}

export async function crearUsuarioComoAdmin(dto) {
  const resp = await fetch(`${urlAPIUsuario}admin/usuarios`, {
    method: "POST",
    headers: getAuthHeaders(),
    body: JSON.stringify(dto),
  });

  if (resp.status === 409) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(data?.message || "El email ya está en uso.", 409);
  }

  if (!resp.ok) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(data?.message || "Error al crear el usuario desde admin.", resp.status);
  }

  return await resp.json();
}

function getAuthHeaders() {
  const token = localStorage.getItem("token");

  return {
    Accept: "application/json",
    "Content-Type": "application/json",
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
  };
}

export async function buscarUsuariosAdmin(query) {
  const resp = await fetch(
    `${urlAPIUsuario}busqueda/usuarios?query=${encodeURIComponent(query)}`,
    {
      method: "GET",
      headers: getAuthHeaders(),
    }
  );

  if (!resp.ok) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(data?.message || "Error al buscar usuarios.", resp.status);
  }

  return await resp.json();
}

export async function getDireccionesUsuarioAdmin(userId, signal) {
  const token = localStorage.getItem("token");
  if (!token) throw new ApiError("Usuario no autenticado.", 401);

  const resp = await fetch(`${urlAPIUsuario}admin/usuarios/${userId}/direcciones`, {
    method: "GET",
    headers: {
      Accept: "application/json",
      Authorization: `Bearer ${token}`,
    },
    signal,
  });

  if (!resp.ok) {
    const data = await handleJsonOrText(resp);
    throw new ApiError(
      data?.message || "No se pudieron obtener las direcciones del usuario.",
      resp.status
    );
  }
  return await resp.json();
}