import { Navigate } from "react-router-dom";

export function PrivateRoute({ children }) {
  const token = localStorage.getItem("token");

  // Si no está logueado, siempre manda a la página principal
  return token ? children : <Navigate to="/" replace />;
}
