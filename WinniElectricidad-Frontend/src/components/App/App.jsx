import { Routes, Route, Navigate } from "react-router-dom";

import Registro from "../../pages/Public/Registro/Registro.jsx";
import Login from "../../pages/Public/Login/Login.jsx";
import ResetPassword from "../../pages/Public/Login/ResetPassword";
import ResenaPage from "../../pages/Public/HistoricoReseñas/ResenasPage.jsx";

import { PrivateRoute } from "../routes/PrivateRoute.jsx";
import { ProtectedRoute } from "../routes/ProtectedRoute.jsx";

import MainLayout from "../../layout/MainLayout.jsx";

import AgendaPage from "../../pages/User/Agenda/AgendaPage.jsx";
import CrearResena from "../../pages/User/Reseña/CrearResena.jsx";

import DashboardAdmin from "../../pages/Admin/Dashboard/DashboardAdmin.jsx"
import HistoricoClientes from "../../pages/Admin/HistoricoClientes/HistoricoClientes.jsx"
import PanelReservas from "../../pages/Admin/PanelReservas/PanelReservas.jsx"

import AdminCrearReservaPage from "../../pages/Admin/CrearCliente/AdminCrearReserva.jsx";

import FichaClientes from "../../pages/Admin/FichaClientes/FichaClientes.jsx"

import PaginaPrincipal from "../../pages/Public/Principal/Principal.jsx"
import ServiciosPage from "../../pages/Public/Servicio/ServiciosPage.jsx";

export default function App() {
  return (
    <Routes>
      {/* Página principal pública */}
      <Route path="/" element={<PaginaPrincipal />} />

      {/* Públicas */}
      <Route path="/registro" element={<Registro />} />
      <Route path="/login" element={<Login />} />
      <Route path="/reset-password" element={<ResetPassword />} />
      <Route path="/resenas" element={<ResenaPage />} />
      <Route path="/servicios" element={<ServiciosPage />} />

      {/* Usuario Cliente */}
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
        <Route path="resenas" element={<ResenaPage />} />
        <Route path="resenas/crear" element={<CrearResena />} />
        <Route path="servicios" element={<ServiciosPage />} /> 
      </Route>

      {/* Usuario Administrador */}
      <Route
        path="/admin"
        element={
          <ProtectedRoute allowedRoles={["Administrador"]}>
            <MainLayout />
          </ProtectedRoute>
        }
      >
        <Route index element={<DashboardAdmin />} />
        <Route path="resenas" element={<ResenaPage />} />
        <Route path="historico" element={<HistoricoClientes />} />
        <Route path="panel-reservas" element={<PanelReservas />} />
        <Route path="crear-reserva" element={<AdminCrearReservaPage />} />
        <Route path="ficha-clientes" element={<FichaClientes />} />
        <Route path="servicios" element={<ServiciosPage />} />       
      </Route>

      {/* Catch-all */}
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}