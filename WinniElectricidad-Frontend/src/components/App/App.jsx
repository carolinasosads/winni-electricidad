import { Routes, Route, Navigate } from "react-router-dom";

import Registro from "../../pages/Public/Registro/Registro.jsx";
import Login from "../../pages/Public/Login/Login.jsx";
import ResetPassword from "../../pages/Public/Login/ResetPassword";

import { PrivateRoute } from "../routes/PrivateRoute.jsx";
import { ProtectedRoute } from "../routes/ProtectedRoute.jsx";

import MainLayout from "../../layout/MainLayout.jsx";

import AgendaPage from "../../pages/User/Agenda/AgendaPage.jsx";

import DashboardAdmin from "../../pages/Admin/Dashboard/DashboardAdmin.jsx"
import HistoricoClientes from "../../pages/Admin/HistoricoClientes/HistoricoClientes.jsx"
import PanelReservas from "../../pages/Admin/PanelReservas/PanelReservas.jsx"


export default function App() {
  return (
    <Routes>
      <Route path="/" element={<Navigate to="/login" replace />} />

      {/* públicas */}
      <Route path="/registro" element={<Registro />} />
      <Route path="/login" element={<Login />} />
      <Route path="/reset-password" element={<ResetPassword />} />

      {/* UsuarioCliente */}
      <Route
        path="/cliente"
        element={
          <ProtectedRoute allowedRoles={["Cliente"]}>
            <MainLayout />
          </ProtectedRoute>
        }
      >
        <Route index element={<AgendaPage />} />
        <Route path="agenda" element={<AgendaPage />} />
      </Route>

      {/* UsuarioAdministrador*/}
      <Route
        path="/admin"
        element={
          <ProtectedRoute allowedRoles={["Administrador"]}>
            <MainLayout />
          </ProtectedRoute>
        }
      >
        <Route index element={<DashboardAdmin />} />
        <Route path="historico" element={<HistoricoClientes />} />
        <Route path="panel-reservas" element={<PanelReservas />} />
      </Route>

      {/* catch-all */}
      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  );
}