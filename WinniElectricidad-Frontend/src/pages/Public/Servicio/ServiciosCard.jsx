import {
  Card,
  CardContent,
  Typography,
  Stack,
  Button,
  IconButton,
  Tooltip,
  Box
} from "@mui/material";
import VisibilityOffOutlinedIcon from "@mui/icons-material/VisibilityOffOutlined";
import VisibilityOutlinedIcon from "@mui/icons-material/VisibilityOutlined";
import { useState } from "react";
import TrabajosPorServicioCarrusel from "./TrabajosPorServicioCarrusel";
import TrabajoModal from "./TrabajoModal";

export default function ServicioCard({
  servicio,
  rol,
  onToggleActivo,
  onSolicitar
}) {
  const [trabajoSeleccionado, setTrabajoSeleccionado] = useState(null);

  const esAdmin = rol === "Administrador";
  const esCliente = rol === "Cliente";

  return (
    <Card
      sx={{
        width: "100%",
        height: "100%",
        display: "flex",
        flexDirection: "column",
        position: "relative",
        borderRadius: 3,
        boxShadow: "0 8px 24px rgba(0,0,0,0.06)",
        transition: "transform .2s ease, box-shadow .2s ease",
        "&:hover": {
          transform: "translateY(-4px)",
          boxShadow: "0 12px 32px rgba(0,0,0,0.08)"
        },
        opacity: servicio.activo === false ? 0.5 : 1
      }}
    >
      {/* BOTÓN ADMIN */}
      {esAdmin && (
        <Tooltip title={servicio.activo ? "Desactivar servicio" : "Activar servicio"}>
          <IconButton
            size="small"
            onClick={() => onToggleActivo(servicio)}
            sx={{
              position: "absolute",
              top: 14,
              right: 14,
              zIndex: 2,
              color: "grey.500",
              "&:hover": {
                color: servicio.activo ? "error.main" : "success.main",
                bgcolor: "rgba(0,0,0,0.04)"
              }
            }}
          >
            {servicio.activo ? (
              <VisibilityOffOutlinedIcon fontSize="small" />
            ) : (
              <VisibilityOutlinedIcon fontSize="small" />
            )}
          </IconButton>
        </Tooltip>
      )}

      {/* IMAGEN */}
      <Box
        component="img"
        src={`${import.meta.env.VITE_API_URL}${servicio.imagenUrl}`}
        alt={servicio.titulo}
        sx={{
          width: "100%",
          height: 180,
          objectFit: "cover"
        }}
        onError={(e) => (e.currentTarget.style.display = "none")}
      />

      <CardContent sx={{ px: 3, pt: 3, pb: 4, flexGrow: 1 }}>
        <Stack spacing={2} sx={{ height: "100%", width: "100%" }}>
          <Typography variant="h6" fontWeight={600}>
            {servicio.titulo}
          </Typography>

          <Typography
            variant="body2"
            color="text.secondary"
            sx={{
              display: "-webkit-box",
              WebkitLineClamp: 3,
              WebkitBoxOrient: "vertical",
              overflow: "hidden"
            }}
          >
            {servicio.descripcion}
          </Typography>

          <Box sx={{ flexGrow: 1 }} />

          {esCliente && (
            <Button
              variant="contained"
              fullWidth
              onClick={() => onSolicitar(servicio)}
            >
              Solicitar presupuesto
            </Button>
          )}

          <TrabajosPorServicioCarrusel
            trabajos={servicio.trabajos}
            onSelect={setTrabajoSeleccionado}
          />

          <TrabajoModal
            trabajo={trabajoSeleccionado}
            onClose={() => setTrabajoSeleccionado(null)}
          />
        </Stack>
      </CardContent>
    </Card>
  );
}
