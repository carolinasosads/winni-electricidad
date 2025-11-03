import React from "react";
import Button from "@mui/material/Button";
import { logout } from '../../../services/authService.js';

export default function Principal() {
  const handleLogout = () => {
    logout();
    window.location.href = "/login";
  };

  return (
    <div style={{ display: "flex", justifyContent: "center", marginTop: "50px" }}>
      <Button
        type="button"
        fullWidth
        variant="contained"
        color="primary"
        onClick={handleLogout}
      >
        Cerrar Sesión
      </Button>
    </div>
  );
}
