import { Navigate } from "react-router-dom";

export function ProtectedRoute({ children, allowedRoles }) {
  const token = localStorage.getItem("token");
  const rol = localStorage.getItem("rol");

  if (!token) {
    return <Navigate to="/" replace />;
  }

  if (Array.isArray(allowedRoles) && allowedRoles.length > 0) {
    const ok = allowedRoles.includes(rol);
    if (!ok) {
      return <Navigate to="/" replace />;
    }
  }

  return children;
}
