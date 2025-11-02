
import React from "react";
import { Routes, Route, Navigate} from "react-router-dom";

import Registro from "../../pages/Public/Registro/Registro.jsx";
import Login from "../../pages/Public/Login/Login.jsx";


export default function App() {
  return (
    <Routes>
      {/* si no tiiene nada va al login */}
      <Route path="/" element={<Navigate to="/login" replace />} />

      {/*Paginas publicas */}
      <Route path="/registro" element={<Registro />} />
      <Route path="/login" element={<Login />} />

      {/*Aca iria alguna pagina privada que todavia no tenemos
      <Route
        path="/dashboard"
        element={
          <PrivateRoute>
            <Dashboard />
          </PrivateRoute>
        }
      />
      */}
 
      {/* catch-all por si entra a algo que no existe */}
      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  )
}
