import React from "react";
import { Routes, Route, Navigate} from "react-router-dom";

import Registro from "../../pages/Public/Registro/Registro.jsx";
import Login from "../../pages/Public/Login/Login.jsx";
import Principal from "../../pages/Public/Principal/Principal.jsx";
import ResetPassword from "../../pages/Public/Login/ResetPassword";
import { PrivateRoute } from "../routes/PrivateRoute.jsx";


export default function App() {
  return (
    <Routes>
      {/* si no tiene nada va al login */}
      <Route path="/" element={<Navigate to="/login" replace />} />

      {/*Paginas publicas */}
      <Route path="/registro" element={<Registro />} />
      <Route path="/login" element={<Login />} />
      <Route path="/reset-password" element={<ResetPassword />} />

      {/*Paginas privadas */}
      <Route
        path="/principal"
        element={
          <PrivateRoute>
            <Principal />
          </PrivateRoute>
        }
      />
      
 
      {/* catch-all por si entra a algo que no existe */}
      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  )
}
