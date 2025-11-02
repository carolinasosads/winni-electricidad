
// El helper se usa para realizar llamadas a la API que requieren que el usuario esté autenticado
//
// 1. Agrega automáticamente el token JWT en el header Authorization
// 2. Detectar si el token expiró (error 401) y redirige al login
// 3.  Evitar repetir esta lógica en cada llamada a la API

export const authorizedFetch = async (endpoint, options = {}) => {
  const token = localStorage.getItem("token");

  // Construye los headers
  // Tomamos los headers que el caller (quien usa este helper) haya pasado
  // y les agregamos automáticamente el header "Authorization" requerido
  // por el backend para validar la sesión.
  // Si no hay token, el Authorization quedará vacío ("").
  const headers = {
    ...options.headers, // preserva cualquier header que se haya agregado (ej: Content-Type)
    Authorization: token ? `Bearer ${token}` : "",
  };

  // Ejecutamos la llamada HTTP con fetch
  // fetch recibe la URL (endpoint) y un objeto de opciones:
  // - method: GET, POST, PUT, DELETE
  // - headers: los headers combinados
  // - body: (solo si es POST o PUT) el contenido a enviar
  const response = await fetch(endpoint, { ...options, headers });

  if (response.status === 401) {
    console.warn("⚠️ Token expirado o inválido. Redirigiendo al login...");

    localStorage.removeItem("token");
    localStorage.removeItem("rol");

    // Redirige al login
    window.location.href = "/login";
  }

  return response;
};

// Ejemplo 1: GET simple
// const response = await authorizedFetch("WinniElectricidadApi/Cliente/listar");
// if (!response.ok) throw new Error("Error al obtener clientes");
// const data = await response.json();
// console.log(data);

// Ejemplo 2: POST enviando datos
// const nuevoCliente = { nombre: "Juan", telefono: "099123456" };
// const response = await authorizedFetch("WinniElectricidadApi/Cliente/agregar", {
//   method: "POST",
//   headers: { "Content-Type": "application/json" },
//   body: JSON.stringify(nuevoCliente),
// });
// if (!response.ok) throw new Error("Error al crear cliente");
// const data = await response.json();
// console.log("Cliente creado:", data);

