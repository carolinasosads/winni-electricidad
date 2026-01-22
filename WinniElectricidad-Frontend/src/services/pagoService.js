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

export const crearPago = async (pago) => {
    try{
        const res = await fetch(`${urlAPIPresupuesto}pagar`, {
            method: "POST",
            headers: getAuthHeaders(),
            body: JSON.stringify(pago),
        });

       const data = await handleJsonOrText(res);

        if (!res.ok) {
        const message = data?.message || "Error al crear el pago.";
        throw new ApiError(message, res.status);
        }

        return data; 


    }catch(err){
        if (err?.name === "AbortError") return; 
        if (err instanceof ApiError) throw err;
        console.error(err);
        throw new ApiError("Error al crear el pago.");
    }
}

