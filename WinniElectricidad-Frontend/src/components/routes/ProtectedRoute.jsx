import { Navigate, useLocation } from "react-router-dom";

export function ProtectedRoute({ children, allowedRoles }) {
  const location = useLocation();
  const token = localStorage.getItem("token");
  const rol = localStorage.getItem("rol");

  if (!token) {
    return <Navigate to="/login" replace state={{ from: location }} />;
  }

  if (Array.isArray(allowedRoles) && allowedRoles.length > 0) {
    const ok = allowedRoles.includes(rol);
    if (!ok) {
      if(rol === "Administrador"){
        return <Navigate to="/admin" replace />;
      } else {
        return <Navigate to="/cliente" replace />;
      }      
    }
  }

  return children;
}
