import {
  Box,
  Typography,
  Paper,
  Stack,
  IconButton,
  Button,
} from "@mui/material";
import { useTheme } from "@mui/material/styles";
import ChevronLeftIcon from "@mui/icons-material/ChevronLeft";
import ChevronRightIcon from "@mui/icons-material/ChevronRight";
import TodayIcon from "@mui/icons-material/Today";

const HORAS = [
  { label: "09:00", hour: 9, minute: 0 },
  { label: "10:30", hour: 10, minute: 30 },
  { label: "12:00", hour: 12, minute: 0 },
  { label: "13:30", hour: 13, minute: 30 },
  { label: "15:00", hour: 15, minute: 0 },
  { label: "16:30", hour: 16, minute: 30 },
];

export default function CalendarioDiarioMobile({
  diaSeleccionado,
  setDiaSeleccionado,
  reservas = [],
  onSelectReserva,
}) {
  const theme = useTheme();
  const rojoCancelada = "#d32f2f";

  if (!diaSeleccionado) return null;

  const mismoDia = (a, b) =>
    a.getFullYear() === b.getFullYear() &&
    a.getMonth() === b.getMonth() &&
    a.getDate() === b.getDate();

  const reservasDelDia = reservas.filter((r) =>
    mismoDia(new Date(r.fechaReserva), diaSeleccionado)
  );

  function reservaDelSlot(slot) {
    return reservasDelDia.find((r) => {
      const d = new Date(r.fechaReserva);
      return d.getHours() === slot.hour && d.getMinutes() === slot.minute;
    });
  }

  function estiloReservaBase(r) {
    const fechaReserva = new Date(r.fechaReserva);
    const esFinalizada = fechaReserva < new Date();

    if (r.estado === "Cancelada") {
      return {
        border: `2px solid ${rojoCancelada}`,
        color: rojoCancelada,
        textDecoration: "line-through",
        backgroundColor: "#f7dede",
        backgroundImage:
          "repeating-linear-gradient(135deg, #f7dede 0px, #f7dede 6px, #f2c1c1 6px, #f2c1c1 12px)",
      };
    }

    if (esFinalizada) {
      return {
        backgroundColor: "#BDBDBD",
        color: "white",
      };
    }

    if (r.estado === "Confirmada") {
      return {
        backgroundColor: "#E8F5E9",
        border: "1px solid #4CAF50",
        color: "#2E7D32",
      };
    }

    if (r.estado === "Pendiente") {
      return {
        backgroundColor: "#fff",
        color: theme.palette.primary.main,
        border: `2px solid ${theme.palette.primary.main}`,
      };
    }

    return {};
  }

  return (
    <Box
      sx={{
        width: "100%",
        maxWidth: "100%",
        overflowX: "hidden",
        boxSizing: "border-box",
      }}
    >
      {/* Header */}
      <Box
        sx={{
          px: 1,
          display: "flex",
          alignItems: "center",
          justifyContent: "space-between",
          mb: 2,
        }}
      >
        <IconButton
          onClick={() =>
            setDiaSeleccionado(
              new Date(diaSeleccionado.getTime() - 86400000)
            )
          }
        >
          <ChevronLeftIcon />
        </IconButton>

        <Box sx={{ textAlign: "center", px: 1 }}>
          <Typography fontWeight={700}>
            {diaSeleccionado.toLocaleDateString("es-UY", {
              weekday: "long",
              day: "numeric",
              month: "long",
              year: "numeric",
            })}
          </Typography>

          <Button
            size="small"
            startIcon={<TodayIcon />}
            onClick={() => setDiaSeleccionado(new Date())}
          >
            Hoy
          </Button>
        </Box>

        <IconButton
          onClick={() =>
            setDiaSeleccionado(
              new Date(diaSeleccionado.getTime() + 86400000)
            )
          }
        >
          <ChevronRightIcon />
        </IconButton>
      </Box>

      <Stack spacing={2} sx={{ px: 1 }}>
        {HORAS.map((slot) => {
          const reserva = reservaDelSlot(slot);

          return (
            <Box key={slot.label} sx={{ width: "100%" }}>
              <Typography fontWeight={600} sx={{ mb: 0.5 }}>
                {slot.label}
              </Typography>

              {!reserva && (
                <Typography variant="body2" color="text.secondary">
                  Sin reservas
                </Typography>
              )}

              {reserva && (
                <Paper
                  onClick={() => onSelectReserva(reserva)}
                  sx={{
                    p: 1.5,
                    borderRadius: 2,
                    fontWeight: 600,
                    cursor: "pointer",
                    width: "100%",
                    boxSizing: "border-box",
                    wordBreak: "break-word",
                    ...estiloReservaBase(reserva),
                  }}
                >
                  <Typography fontWeight={600}>
                    {reserva.cliente?.nombre}
                  </Typography>

                  {!["Cancelada"].includes(reserva.estado) && (
                    <Typography fontSize="0.75rem">
                      {reserva.servicios?.map((s) => s.titulo).join(", ")}
                    </Typography>
                  )}

                  {reserva.estado === "Pendiente" &&
                    new Date(reserva.fechaReserva) >= new Date() && (
                      <Typography fontSize="0.7rem" fontWeight={600} mt={0.4}>
                        ⚠ Requiere confirmación
                      </Typography>
                    )}

                  {new Date(reserva.fechaReserva) < new Date() &&
                    reserva.estado !== "Cancelada" && (
                      <Typography fontSize="0.7rem" mt={0.3}>
                        Reserva pasada
                      </Typography>
                    )}
                </Paper>
              )}
            </Box>
          );
        })}
      </Stack>
    </Box>
  );
}
