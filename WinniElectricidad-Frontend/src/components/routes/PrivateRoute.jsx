import { Navigate } from "react-router-dom";

export function PrivateRoute({ children }) {
  const token = localStorage.getItem("token");
//Si tiene token te deja acceder y sino te manda al login
  return token ? children : <Navigate to="/login" />;
}