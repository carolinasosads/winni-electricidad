// src/components/Common/LogoutButton.jsx
import * as React from "react";
import Button from "@mui/material/Button";
import { useNavigate } from "react-router-dom";
import { logout } from "../../../services/authService";

export default function LogoutButton({ children = "Cerrar sesión", ...props }) {
  const navigate = useNavigate();
  const handleLogout = () => {
    logout();
    navigate("/login", { replace: true });
  };

  return (
    <Button
      variant="outlined"
      size="small"
      onClick={handleLogout}
      sx={{ textTransform: 'none', borderRadius: 2 }}
      {...props}
    >
      {children}
    </Button>
  );
}