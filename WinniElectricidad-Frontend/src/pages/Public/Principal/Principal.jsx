import React from "react";
import Button from "@mui/material/Button";
import { logout } from "../../../services/authService.js";
import { useNavigate } from "react-router-dom";

export default function Principal() {
  const navigate = useNavigate();
  const handleLogout = () => {
    logout();
    navigate("/login", { replace: true });
  };

  return (
    <div style={{ display: "flex", justifyContent: "center", marginTop: 50 }}>
      <Button type="button" fullWidth variant="contained" onClick={handleLogout}>
        Cerrar Sesión
      </Button>
    </div>
  );
}
