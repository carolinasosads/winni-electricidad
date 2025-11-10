import { Routes, Route, Navigate } from "react-router-dom";

import Registro from "../../pages/Public/Registro/Registro.jsx";
import Login from "../../pages/Public/Login/Login.jsx";
import Principal from "../../pages/Public/Principal/Principal.jsx";
import ResetPassword from "../../pages/Public/Login/ResetPassword";
import Dashboard from "../../pages/Admin/Dashboard/Dashboard.jsx";

import EmployeeList from "../MUI/CitasList.jsx";
import EmployeeShow from "../../components/MUI/EmployeeShow.jsx";
import EmployeeEdit from "../../components/MUI/EmployeeEdit.jsx";

import { PrivateRoute } from "../routes/PrivateRoute.jsx";

import AgendaLayout from "../../pages/User/Agenda/AgendaLayout.jsx";
import AgendaPage from "../../pages/User/Agenda/AgendaPage.jsx"; 

export default function App() {
  return (
    <Routes>
      <Route path="/" element={<Navigate to="/login" replace />} />

      {/* Públicas */}
      <Route path="/registro" element={<Registro />} />
      <Route path="/login" element={<Login />} />
      <Route path="/reset-password" element={<ResetPassword />} />

{/*Que no se pueda ingresar por url falta */}
      <Route path="/agenda" element={<AgendaLayout />}>
        <Route index element={<AgendaPage />} />
      </Route>

      <Route
        path="/dashboard"
        element={
          <PrivateRoute>
            <Dashboard />
          </PrivateRoute>
        }
      >

        {/*Cambiar una vezz que tengamos el back */}
        <Route index element={<EmployeeList />} />
        <Route path="employees" element={<EmployeeList />} />
        <Route path="employees/:employeeId" element={<EmployeeShow />} />
        <Route path="employees/:employeeId/edit" element={<EmployeeEdit />} />
        <Route path="*" element={<EmployeeList />} />
      </Route>

      <Route
        path="/principal"
        element={
          <PrivateRoute>
            <Principal />
          </PrivateRoute>
        }
      />

    { /*Si ponen algo por url que no existe va para el login */}
      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  );
}