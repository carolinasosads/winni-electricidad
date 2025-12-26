import {  Box,  Modal,  Paper,  Typography,  Divider,  Button,  IconButton,} from "@mui/material";

import CloseIcon from "@mui/icons-material/Close";
import CheckCircleOutline from "@mui/icons-material/CheckCircleOutline";
import HighlightOff from "@mui/icons-material/HighlightOff";
import EditCalendar from "@mui/icons-material/EditCalendar";

export default function ReservaDetailModalCliente({
  open,
  onClose,
  reserva,
  onAceptar,
  onCancelar,
  onModificar,
}) {
  if (!reserva) return null;

  const baseBtnStyle = {
    fontWeight: 400,
    py: 1.2,
    textTransform: "uppercase",
    borderRadius: 2,
  };
  const btnStyle = { ...baseBtnStyle };
  const btnStyleOutline = { ...baseBtnStyle, borderWidth: 2 };

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
            <Typography variant="h6" fontWeight={600} sx={{ textTransform: "none" }}>
              Reserva
            </Typography>

            <IconButton onClick={onClose}>
              <CloseIcon />
            </IconButton>
          </Box>

          <Divider sx={{ mb: 2 }} />

          {/* DETALLE */}
          <Box
            sx={{
              lineHeight: 1.8,
              display: "flex",
              flexDirection: "column",
              gap: 1.2,
            }}
          >
            {/* Sección: Reserva */}
            <Typography
              variant="subtitle2"
              sx={{ fontWeight: 700, color: "primary.main", mt: 1 }}
            >
              Detalle de la Reserva
            </Typography>

            <Box sx={{ pl: 1 }}>
              <Typography>
                <strong>Servicio:</strong>{" "}
                {reserva.servicios?.map((s) => s.titulo).join(", ")}
              </Typography>

              <Typography>
                <strong>Dirección:</strong>{" "}
                {[
                  reserva.direccion?.calle?.trim(),
                  reserva.direccion?.numero?.trim() || null,
                  reserva.direccion?.apto ? `Apto ${reserva.direccion.apto}` : null,
                  reserva.direccion?.esquina
                    ? `Esq. ${reserva.direccion.esquina}`
                    : null,
                ]
                  .filter(Boolean)
                  .join(", ")}
              </Typography>

              <Typography>
                <strong>Fecha:</strong>{" "}
                {new Date(reserva.fechaReserva).toLocaleString("es-UY")}
              </Typography>

              <Typography>
                <strong>Estado:</strong> {reserva.estado}
              </Typography>

              <Typography>
                <strong>Comentario:</strong>{" "}
                {reserva.comentario || "Sin comentarios"}
              </Typography>
            </Box>
          </Box>

          {/* BOTONES */}
          <Box sx={{ mt: 3, display: "flex", flexDirection: "column", gap: 1.5 }}>
            {(() => {
              const estado = (reserva.estado || "").toLowerCase();
              const fecha = new Date(reserva.fechaReserva);
              const hoy = new Date();

              if (fecha < hoy) {
                return (
                  <Typography color="text.secondary">
                    Esta reserva ya pasó. No se pueden realizar acciones.
                  </Typography>
                );
              }

              // CLIENTE: SOLO PENDIENTE -> aceptar/modificar/cancelar
              if (estado === "pendiente") {
                return (
                  <>
                    <Button
                      variant="contained"
                      color="success"
                      startIcon={<CheckCircleOutline />}
                      sx={btnStyle}
                      onClick={() => onAceptar(reserva)}
                    >
                      Aceptar
                    </Button>

                    <Button
                      variant="outlined"
                      color="primary"
                      startIcon={<EditCalendar />}
                      sx={btnStyleOutline}
                      onClick={() => onModificar(reserva)}
                    >
                      Modificar
                    </Button>

                    <Button
                      variant="outlined"
                      color="error"
                      startIcon={<HighlightOff />}
                      sx={btnStyleOutline}
                      onClick={() => onCancelar(reserva)}
                    >
                      Cancelar
                    </Button>
                  </>
                );
              }

              return (
                <Typography color="text.secondary">
                  Esta reserva no está pendiente. Solo podés gestionar reservas en estado{" "}
                  <strong>Pendiente</strong>.
                </Typography>
              );
            })()}
          </Box>
        </Paper>
      </Box>
    </Modal>
  );
}
