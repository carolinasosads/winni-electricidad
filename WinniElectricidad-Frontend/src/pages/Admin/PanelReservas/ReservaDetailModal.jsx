import {
  Box,
  Modal,
  Paper,
  Typography,
  Divider,
  Button,
  IconButton,
} from "@mui/material";

import CloseIcon from "@mui/icons-material/Close";
import CheckCircleOutline from "@mui/icons-material/CheckCircleOutline";
import HighlightOff from "@mui/icons-material/HighlightOff";
import EditCalendar from "@mui/icons-material/EditCalendar";

export default function ReservaDetailModal({
  open,
  onClose,
  reserva,
  onAprobar,
  onRechazar,
  onSugerirCambio,
}) {
  if (!reserva) return null;

  const btnStyle = {
    fontWeight: 400,
    py: 1.2,
    textTransform: "uppercase",
    borderRadius: 2,
  };

  const btnStyleOutline = {
    fontWeight: 400,
    py: 1.2,
    textTransform: "uppercase",
    borderRadius: 2,
    borderWidth: 2,
  };


  return (
    <Modal open={open} onClose={onClose}>
      <Box
        sx={{
          width: "95%",
          maxWidth: 520,
          mx: "auto",
          mt: "8vh",
          outline: "none",
        }}
      >
        <Paper
          sx={{
            p: 3,
            borderRadius: 3,
            boxShadow: "0px 8px 30px rgba(0,0,0,0.25)",
            animation: "fadeIn .25s ease-out",
          }}
        >
          {/* HEADER */}
          <Box sx={{ display: "flex", justifyContent: "space-between", mb: 2 }}>
            <Typography
              variant="h6"
              fontWeight={600} 
              sx={{ textTransform: "none" }}
            >
              Detalle de Reserva
            </Typography>

            <IconButton onClick={onClose}>
              <CloseIcon />
            </IconButton>
          </Box>

          <Divider sx={{ mb: 2 }} />

          {/* DETALLE */}
          <Box sx={{ lineHeight: 1.8 }}>
            <Typography>
              <strong>Cliente:</strong> {reserva.clienteNombre}
            </Typography>
            <Typography>
              <strong>Servicio:</strong> {reserva.servicioNombre}
            </Typography>
            <Typography>
              <strong>Dirección:</strong> {reserva.direccion}
            </Typography>
            <Typography>
              <strong>Fecha:</strong> {new Date(reserva.fechaReserva).toLocaleString("es-UY")}
            </Typography>
            <Typography>
              <strong>Estado:</strong> {reserva.estado}
            </Typography>
          </Box>

          {/* BOTONES */}
          <Box sx={{ mt: 3, display: "flex", flexDirection: "column", gap: 1.5 }}>
            {(() => {
              const estado = reserva.estado.toLowerCase();
              const fecha = new Date(reserva.fechaReserva);
              const hoy = new Date();

              if (fecha < hoy) {
                return (
                  <Typography color="text.secondary">
                    Esta reserva ya pasó. No se pueden realizar acciones.
                  </Typography>
                );
              }

              if (estado === "pendiente") {
                return (
                  <>
                    <Button
                      variant="contained"
                      color="success"
                      startIcon={<CheckCircleOutline />}
                      sx={btnStyle}
                      onClick={onAprobar}
                    >
                      Aprobar
                    </Button>

                    <Button
                      variant="outlined"
                      color="error"
                      startIcon={<HighlightOff />}
                      sx={btnStyleOutline}
                      onClick={onRechazar}
                    >
                      Rechazar
                    </Button>

                    <Button
                      variant="outlined"
                      color="primary"
                      startIcon={<EditCalendar />}
                      sx={btnStyleOutline}
                      onClick={() => onSugerirCambio(reserva)}
                    >
                      Sugerir modificación
                    </Button>
                  </>
                );
              }

              if (estado === "confirmada") {
                return (
                  <>
                    <Button
                      variant="outlined"
                      color="error"
                      startIcon={<HighlightOff />}
                      sx={btnStyleOutline}
                      onClick={onRechazar}
                    >
                      Cancelar
                    </Button>

                    <Button
                      variant="outlined"
                      color="primary"
                      startIcon={<EditCalendar />}
                      sx={btnStyleOutline}
                      onClick={() => onSugerirCambio(reserva)}
                    >
                      Sugerir modificación
                    </Button>
                  </>
                );
              }

              if (estado === "cancelada") {
                return (
                  <>
                    <Button
                      variant="outlined"
                      color="primary"
                      startIcon={<EditCalendar />}
                      sx={btnStyleOutline}
                      onClick={() => onSugerirCambio(reserva)}
                    >
                      Sugerir modificación
                    </Button>
                  </>
                );
              }

              return null;
            })()}

          </Box>
        </Paper>
      </Box>
    </Modal>
  );
}
