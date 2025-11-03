import ApiError from "./ApiError";

//Login, Logout, Registro y recuperacion de contraseñas
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

    const { token, role } = data;

    localStorage.setItem("token", token);
    localStorage.setItem("role", role);
    
    return data;
  
}

//TODO: A chequear si funciona y si guardamos el rol
export const logout = () => {
  localStorage.removeItem("token");
  localStorage.removeItem("rol");
};