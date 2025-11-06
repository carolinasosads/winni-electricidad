import ApiError from "./ApiError";

const urlAPI = "http://localhost:5269/WinniElectricidadApi/Usuario/"

export const login = async (email, password) => {
    const response = await fetch(`${urlAPI}login`,{
        method: "POST", 
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify({email,password}),
    });

    if(response.status === 401){
        throw new ApiError("Credenciales inválidas.", 401)
    }

    if(!response.ok){
        throw new ApiError("Error al iniciar sesión.", response.status);
    }

    const data = await response.json();

    const { token, rol } = data;

    localStorage.setItem("token", token);
    localStorage.setItem("rol", rol);

    return data;
}

export const registro = async (p) => {
  const payload = {
    nombreCompleto: p.name,             
    email: p.email,
    password: p.password,
    telefono: normalizarTelefono(p.telefono), 
    hcaptchaToken: p.hcaptchaToken,      
    direcciones: [
      normalizarDireccion(p.direccionPrincipal, true),
      ...(Array.isArray(p.direcciones) ? p.direcciones.map(d => normalizarDireccion(d, false)) : []),
    ].filter(d => d.calle || d.esquina || d.numero || d.apto),
  };

  console.log("payload registro", payload);
  const resp = await fetch(`${urlAPI}registro`, {
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
  if (data?.token) localStorage.setItem("token", data.token);
  if (data?.rol) localStorage.setItem("rol", data.rol);
  return data;
};

async function handleJsonOrText(resp) {
  const ct = resp.headers.get("content-type") || "";
  if (ct.includes("application/json")) return await resp.json();
  const t = await resp.text();
  try { return JSON.parse(t); } catch { return { message: t || null }; }
}

function normalizarTelefono(t) {
  return (t ?? "")
    .toString()
    .replace(/\D/g, "") // sólo dígitos
    .slice(0, 15); // límite 15 dígitos
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

export const logout = () => {
  localStorage.removeItem("token");
  localStorage.removeItem("rol");
};