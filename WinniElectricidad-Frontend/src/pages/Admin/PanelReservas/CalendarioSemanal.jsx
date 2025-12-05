import { useMemo, useState, useEffect } from "react";
import {
  startOfWeek,
  endOfWeek,
  addDays,
  format,
  isSameDay,
  parseISO,
} from "date-fns";
import { es } from "date-fns/locale";
import {
  Box,
  Grid,
  Paper,
  Typography,
  Tooltip,
  IconButton,
} from "@mui/material";
import ChevronLeftIcon from "@mui/icons-material/ChevronLeft";
import ChevronRightIcon from "@mui/icons-material/ChevronRight";
import { useTheme } from "@mui/material/styles";

const SLOTS = [
  { key: "09:00", hour: 9, minute: 0, label: "09:00" },
  { key: "10:30", hour: 10, minute: 30, label: "10:30" },
  { key: "12:00", hour: 12, minute: 0, label: "12:00" },
  { key: "13:30", hour: 13, minute: 30, label: "13:30" },
  { key: "15:00", hour: 15, minute: 0, label: "15:00" },
  { key: "16:30", hour: 16, minute: 30, label: "16:30" },
];

export default function CalendarioSemanal({
  reservas,
  onSelectReserva,
  onCambioSemana,
}) {
  const theme = useTheme();
  const rojoCancelada = "#d32f2f";

  const [currentWeek, setCurrentWeek] = useState(new Date());

  const weekStart = startOfWeek(currentWeek, { weekStartsOn: 1 });
  const weekEnd = endOfWeek(currentWeek, { weekStartsOn: 1 });

  const days = useMemo(
    () => Array.from({ length: 7 }, (_, i) => addDays(weekStart, i)),
    [weekStart]
  );

  const reservasSemana = useMemo(() => {
    return reservas.filter((r) => {
      const dt = parseISO(r.fechaReserva);
      return dt >= weekStart && dt <= weekEnd;
    });
  }, [reservas, weekStart, weekEnd]);

  useEffect(() => {
    onCambioSemana?.(weekStart);
  }, [weekStart, onCambioSemana]);

  function reservasDelDiaYSlot(day, slot) {
    return reservasSemana.filter((r) => {
      const dt = parseISO(r.fechaReserva);
      return (
        isSameDay(dt, day) &&
        dt.getHours() === slot.hour &&
        dt.getMinutes() === slot.minute
      );
    });
  }

  const prioridadEstado = {
    Pendiente: 1,
    Confirmada: 1,
    Finalizada: 2,
    Cancelada: 3,
  };

  function estiloReservaBase(r) {
    const fechaReserva = new Date(r.fechaReserva);
    const esFinalizada = fechaReserva < new Date();

    // CANCELADA
    if (r.estado === "Cancelada") {
      return {
        border: `2px solid ${rojoCancelada}`,
        borderRadius: 2,
        color: rojoCancelada,
        fontSize: "0.70rem",
        fontWeight: 600,
        textDecoration: "line-through",
        px: 1,
        py: 0.5,
        cursor: "pointer",

        backgroundColor: "#f7dede",
        backgroundImage:
          "repeating-linear-gradient(135deg, #f7dede 0px, #f7dede 6px, #f2c1c1 6px, #f2c1c1 12px)",
      };
    }

    // FINALIZADA
    if (esFinalizada) {
      return {
        backgroundColor: "#BDBDBD",
        color: "white",
        borderRadius: 2,
        px: 1,
        py: 0.6,
        fontWeight: 600,
        cursor: "pointer",
      };
    }

    // CONFIRMADA
    if (r.estado === "Confirmada") {
      return {
        backgroundColor: "#E8F5E9",
        border: "1px solid #4CAF50",
        color: "#2E7D32",
        borderRadius: 2,
        px: 1,
        py: 0.6,
        cursor: "pointer",
        fontWeight: 600,
        boxShadow: "0 1px 2px rgba(0,0,0,0.10)",
      };
}

    // PENDIENTE
    if (r.estado === "Pendiente") {
      return {
        backgroundColor: "#fff",
        color: theme.palette.primary.main,
        border: `2px solid ${theme.palette.primary.main}`,
        borderRadius: 2,
        px: 1,
        py: 0.6,
        cursor: "pointer",
        boxShadow: "0 1px 3px rgba(0,0,0,0.08)",
      };
    }

    // DEFAULT
    return {
      backgroundColor: "#EEEEEE",
      color: "#555",
      borderRadius: 2,
      px: 1,
      py: 0.6,
      cursor: "pointer",
    };
  }

  return (
    <Paper
      variant="outlined"
      sx={{
        width: "100%",
        p: 2,
        boxSizing: "border-box",
        overflowX: "hidden",
      }}
    >
      {/* HEADER */}
      <Box
        sx={{
          display: "flex",
          justifyContent: "space-between",
          mb: 2,
          alignItems: "center",
        }}
      >
        <IconButton onClick={() => setCurrentWeek(addDays(currentWeek, -7))}>
          <ChevronLeftIcon />
        </IconButton>

        <Typography variant="h5" fontWeight={700}>
          Semana del {format(weekStart, "dd 'de' MMMM yyyy", { locale: es })}
        </Typography>

        <IconButton onClick={() => setCurrentWeek(addDays(currentWeek, 7))}>
          <ChevronRightIcon />
        </IconButton>
      </Box>

      {/* HEADER DÍAS */}
      <Box
        sx={{
          display: "grid",
          gridTemplateColumns: "80px repeat(7, 1fr)",
          borderBottom: "1px solid #ddd",
          pb: 1,
        }}
      >
        <Box />
        {days.map((day) => (
          <Box
            key={day}
            sx={{
              textAlign: "center",
              textTransform: "uppercase",
            }}
          >
            <Typography variant="caption" fontWeight={600}>
              {format(day, "eee", { locale: es })}
            </Typography>
            <Typography fontWeight={700}>{format(day, "dd")}</Typography>
          </Box>
        ))}
      </Box>

      {/* GRILLA */}
      <Grid container direction="column">
        {SLOTS.map((slot) => (
          <Grid
            key={slot.key}
            item
            sx={{
              display: "grid",
              gridTemplateColumns: "80px repeat(7, 1fr)",
              borderBottom: "1px solid #eee",
              height: 95,
            }}
          >
            {/* HORA */}
            <Box
              sx={{
                p: 1,
                borderRight: "1px solid #eee",
                textAlign: "center",
              }}
            >
              <Typography variant="body2">{slot.label}</Typography>
            </Box>

            {/* COLUMNAS DE DÍAS */}
            {days.map((day) => {
              let delDia = reservasDelDiaYSlot(day, slot);

              delDia = delDia.sort(
                (a, b) => prioridadEstado[a.estado] - prioridadEstado[b.estado]
              );

              const hayActivas = delDia.some((x) => x.estado !== "Cancelada");

              return (
                <Box
                  key={slot.key + day}
                  sx={{
                    borderRight: "1px solid #eee",
                    position: "relative",
                    p: 0.5,
                  }}
                >
                  {delDia.map((r) => {
                    const esCancelada = r.estado === "Cancelada";

                    const topPosition =
                      esCancelada && hayActivas ? 50 : 4;

                    return (
                      <Tooltip
                        key={r.id}
                        title={`${r.cliente?.nombre} — ${r.servicios
                          ?.map((s) => s.titulo)
                          .join(", ")}`}
                      >
                        <Paper
                          onClick={() => onSelectReserva(r)}
                          sx={{
                            ...estiloReservaBase(r),
                            position: "absolute",
                            top: topPosition,
                            left: 4,
                            right: 4,
                            overflow: "hidden",
                            whiteSpace: "nowrap",
                            textOverflow: "ellipsis",
                            cursor: "pointer",

                            zIndex: esCancelada ? 1 : 10,

                            opacity: esCancelada && hayActivas ? 0.35 : 1,
                          }}
                        >
                          {/* Nombre */}
                          <Typography
                            sx={{
                              fontWeight: 600,
                              fontSize: esCancelada ? "0.7rem" : "0.78rem",
                              textDecoration: esCancelada
                                ? "line-through"
                                : "none",
                              overflow: "hidden",
                              textOverflow: "ellipsis",
                            }}
                          >
                            {r.cliente?.nombre}
                          </Typography>

                          {/* Servicios */}
                          {!esCancelada && (
                            <Typography
                              sx={{
                                fontSize: "0.65rem",
                                opacity: 0.9,
                                overflow: "hidden",
                                textOverflow: "ellipsis",
                              }}
                            >
                              {r.servicios?.map((s) => s.titulo).join(", ")}
                            </Typography>
                          )}

                          {/* Pendiente */}
                          {r.estado === "Pendiente" && new Date(r.fechaReserva) >= new Date() && (
                            <Typography
                              sx={{
                                fontSize: "0.65rem",
                                fontWeight: 600,
                                mt: 0.4,
                                color: theme.palette.primary.main,
                              }}
                            >
                              ⚠ Requiere confirmación
                            </Typography>
                          )}

                          {/* Finalizada */}
                          {new Date(r.fechaReserva) < new Date() && r.estado !== "Cancelada" && (
                            <Typography
                              sx={{
                                fontSize: "0.63rem",
                                fontWeight: 600,
                                mt: 0.3,
                                color: "#555",
                                opacity: 0.8,
                              }}
                            >
                              Reserva pasada
                            </Typography>
                          )}
                        </Paper>
                      </Tooltip>
                    );
                  })}
                </Box>
              );
            })}
          </Grid>
        ))}
      </Grid>
    </Paper>
  );
}
