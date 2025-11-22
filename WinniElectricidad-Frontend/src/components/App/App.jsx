import { Routes, Route, Navigate } from "react-router-dom";

import Registro from "../../pages/Public/Registro/Registro.jsx";
import Login from "../../pages/Public/Login/Login.jsx";
import ResetPassword from "../../pages/Public/Login/ResetPassword";

import { PrivateRoute } from "../routes/PrivateRoute.jsx";
import { ProtectedRoute } from "../routes/ProtectedRoute.jsx";

import AgendaLayout from "../../pages/User/Agenda/AgendaLayout.jsx";
import AgendaPage from "../../pages/User/Agenda/AgendaPage.jsx"; 

export default function App() {
  return (
    <Routes>
      <Route path="/" element={<Navigate to="/login" replace />} />

      {/* públicas */}
      <Route path="/registro" element={<Registro />} />
      <Route path="/login" element={<Login />} />
      <Route path="/reset-password" element={<ResetPassword />} />

      {/* solo UsuarioCliente */}
      <Route
        path="/agenda"
        element={
          <ProtectedRoute allowedRoles={["Cliente"]}>
            <AgendaLayout />
          </ProtectedRoute>
        }
      >
        <Route index element={<AgendaPage />} />
      </Route>

      {/* catch-all */}
      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  );
}