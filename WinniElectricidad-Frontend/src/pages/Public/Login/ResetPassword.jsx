import React, {useEffect, useState} from "react";
import {
  Container,
  Typography,
  TextField,
  Button,
  Alert,
  Box,
  CircularProgress,
} from "@mui/material";
import { useSearchParams, useNavigate } from "react-router-dom";
import { resetPassword } from "../../../services/authService";

export default function ResetPassword() {
  const [searchParams] = useSearchParams();
  const token = searchParams.get("token");
  const navigate = useNavigate();
  
  const [tokenMissing, setTokenMissing] = useState(false);
  const [password, setPassword] = useState("");
  const [confirm, setConfirm] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(false);

  useEffect(() => {
      setTokenMissing(!token || token.trim().length === 0);
  }, [token]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);

    if (tokenMissing) {
      setError("El enlace de restablecimiento no es válido o ya expiró.");
      return;
    }

    if (password.trim().length < 6) {
      setError("La contraseña debe tener al menos 6 caracteres.");
      return;
    }

    if (password !== confirm) {
      setError("Las contraseñas no coinciden.");
      return;
    }

    setLoading(true);
    try {
      await resetPassword(password, token);
      setSuccess(true);
      setTimeout(() => navigate("/login"), 2000);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  if (tokenMissing && !success) {
    return (
        <Container
            maxWidth="sm"
            sx={{
                mt: 8,
                display: "flex",
                flexDirection: "column",
                alignItems: "center",
                gap: 2,
            }}
        >
            <Typography variant="h5" gutterBottom>
                Restablecer contraseña
            </Typography>
            <Alert severity="error">
                El enlace de restablecimiento no es válido o ha expirado.
            </Alert>
            <Box sx={{ display: "flex", gap: 1 }}>
                <Button variant="outlined" onClick={() => navigate("/login")}>
                    Ir al login
                </Button>
                <Button
                    variant="contained"
                    onClick={() => navigate("/forgot-password")}
                >
                    Solicitar nuevo enlace
                </Button>
            </Box>
        </Container>
    );
  }
    
  return (
    <Container
        maxWidth="sm"
        sx={{
            mt: 8,
            display: "flex",
            flexDirection: "column",
            alignItems: "center",
        }}
    >
        <Typography variant="h5" gutterBottom>
            Restablecer contraseña
        </Typography>
        <Typography variant="body2" sx={{ mb: 3 }}>
            Ingresa y confirma tu nueva contraseña para completar el proceso.
        </Typography>

        {success ? (
            <Alert severity="success">
                Contraseña actualizada correctamente. Redirigiendo al login...
            </Alert>
        ) : (
            <Box
                component="form"
                onSubmit={handleSubmit}
                sx={{ width: "100%", display: "flex", flexDirection: "column", gap: 2 }}
            >
                <TextField
                    label="Nueva contraseña"
                    type="password"
                    fullWidth
                    required
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    sx={{ borderRadius: 4 }}
                />
                <TextField
                    label="Confirmar contraseña"
                    type="password"
                    fullWidth
                    required
                    value={confirm}
                    onChange={(e) => setConfirm(e.target.value)}
                    sx={{ borderRadius: 4 }}
                />
                {error && <Alert severity="error">{error}</Alert>}
                <Button
                    type="submit"
                    variant="contained"
                    disabled={loading || tokenMissing}
                    sx={{ mt: 1, borderRadius: 4 }}
                >
                    {loading ? (
                        <Box sx={{ display: "flex", alignItems: "center" }}>
                            <CircularProgress size={18} sx={{ mr: 1 }} />
                            Procesando...
                        </Box>
                    ) : (
                        "Restablecer contraseña"
                    )}
                </Button>
            </Box>
        )}
    </Container>
  );
}