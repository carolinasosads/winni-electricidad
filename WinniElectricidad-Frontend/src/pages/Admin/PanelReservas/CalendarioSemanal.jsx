import { useMemo, useState } from "react";
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

const HOURS = [...Array(9)].map((_, i) => 9 + i);

export default function CalendarioSemanal({ reservas, onSelectReserva }) {
  const theme = useTheme();

  // Semana actualmente visible
  const [currentWeek, setCurrentWeek] = useState(new Date());

  const weekStart = startOfWeek(currentWeek, { weekStartsOn: 1 });
  const weekEnd = endOfWeek(currentWeek, { weekStartsOn: 1 });

  // Arreglo de lunes a domingo
  const days = useMemo(
    () => [...Array(7)].map((_, i) => addDays(weekStart, i)),
    [weekStart]
  );

  // Reservas dentro de esta semana
  const reservasSemana = useMemo(() => {
    return reservas.filter((r) => {
      const dt = parseISO(r.fechaReserva);
      return dt >= weekStart && dt <= weekEnd;
    });
  }, [reservas, weekStart, weekEnd]);

  // Reservas del día × hora
  function reservasDelDiaYHora(day, hour) {
    return reservasSemana.filter((r) => {
      const dt = parseISO(r.fechaReserva);
      return isSameDay(dt, day) && dt.getHours() === hour;
    });
  }

  // Estilo según el tipo de reserva
  function estiloReserva(reserva) {
    const fecha = new Date(reserva.fechaReserva);
    const hoy = new Date();
    const esFinalizada = reserva.estado === "Confirmada" && fecha < hoy;

    if (esFinalizada) {
      return {
        backgroundColor: "#BDBDBD",
        color: "white",
        fontFamily: "inherit",
        fontSize: "0.75rem",
        fontWeight: 600,
        borderRadius: 1,
        px: 1,
        py: 0.6,
        cursor: "pointer",
        "&:hover": {
          backgroundColor: "#9E9E9E",
        },
      };
    }

    if (reserva.estado === "Confirmada") {
      return {
        backgroundColor: theme.palette.primary.main,
        color: "white",
        fontFamily: "inherit",
        fontSize: "0.75rem",
        fontWeight: 600,
        borderRadius: 1,
        px: 1,
        py: 0.8,
        cursor: "pointer",
        transition: "box-shadow 0.2s ease, background-color 0.2s ease",
        "&:hover": {
          backgroundColor: theme.palette.primary.dark,
          boxShadow: "0px 3px 10px rgba(0,0,0,0.25)",
        },
      };
    }

    if (reserva.estado === "Pendiente") {
      return {
        backgroundColor: "white",
        color: theme.palette.primary.main,
        border: `2px solid ${theme.palette.primary.main}`,
        borderRadius: 1,
        fontFamily: "inherit",
        fontSize: "0.75rem",
        fontWeight: 600,
        px: 1,
        py: 0.8,
        cursor: "pointer",
        position: "relative",
        "&::after": {
          fontSize: "0.7rem",
          position: "absolute",
          top: -6,
          right: -6,
          backgroundColor: "white",
          border: `1px solid ${theme.palette.primary.main}`,
          borderRadius: "50%",
          width: 18,
          height: 18,
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
        },
        "&:hover": {
          backgroundColor: "#F5F9FF",
        },
      };
    }

    if (reserva.estado === "Cancelada") {
      return {
        backgroundColor: "#E0E0E0",
        color: "#555",
        fontFamily: "inherit",
        fontSize: "0.75rem",
        fontWeight: 600,
        borderRadius: 1,
        px: 1,
        py: 0.6,
        cursor: "default",
        "&:hover": {
          backgroundColor: "#D5D5D5",
        },
      };
    }

    return {
      backgroundColor: "#EEEEEE",
      color: "#555",
      borderRadius: 1,
      px: 1,
      py: 0.6,
      fontFamily: "inherit",
      fontSize: "0.75rem",
      fontWeight: 600,
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
        m: 0,
        maxWidth: "none",
      }}
    >
      {/* Header superior */}
      <Box
        sx={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          mb: 2,
          px: 1,
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

      {/* Encabezado de días */}
      <Box
        sx={{
          display: "flex",
          flexDirection: "row",
          alignItems: "center",
          pt: 2,
          pb: 1.5,
          borderBottom: "1px solid",
          borderColor: "divider",
          mb: 2,
          width: "100%",
        }}
      >
        {/* Columna vacía del horario */}
        <Box sx={{ flex: 1, minWidth: 0 }}></Box>

        {/* Días */}
        {days.map((day) => (
          <Box
            key={day}
            sx={{
              flex: 1,
              minWidth: 0,
              display: "flex",
              flexDirection: "column",
              alignItems: "center",
              justifyContent: "center",
              textTransform: "uppercase",
              gap: 0.6,
              px: 1,
            }}
          >
            <Typography
              variant="caption"
              fontWeight={600}
              color="text.secondary"
              sx={{
                letterSpacing: "0.12em",
                fontSize: "0.75rem",
              }}
            >
              {format(day, "eee", { locale: es })}
            </Typography>

            <Typography
              variant="body1"
              fontWeight={700}
              sx={{
                fontSize: "0.9rem",
                lineHeight: 1.1,
              }}
            >
              {format(day, "dd", { locale: es })}
            </Typography>
          </Box>
        ))}
      </Box>

      {/* Grilla de horas */}
      <Grid container direction="column" sx={{ width: "100%" }}>
        {HOURS.map((hour) => (
          <Grid
            item
            key={hour}
            sx={{
              display: "flex",
              borderBottom: "1px solid",
              borderColor: "divider",
              height: 90,
              width: "100%",
            }}
          >
            {/* Columna de la hora */}
            <Box
              sx={{
                flex: 1,
                minWidth: 0,
                display: "flex",
                justifyContent: "center",
                alignItems: "flex-start",
                pt: 1,
                borderRight: "1px solid",
                borderColor: "divider",
              }}
            >
              <Typography variant="body2">{hour}:00</Typography>
            </Box>

            {/* Celdas de cada día */}
            {days.map((day) => {
              const delDia = reservasDelDiaYHora(day, hour);

              return (
                <Box
                  key={day + hour}
                  sx={{
                    flex: 1,
                    minWidth: 0,
                    height: "100%",
                    position: "relative",
                    borderRight: "1px solid",
                    borderColor: "divider",
                    p: 0.5,
                  }}
                >
                  {delDia.map((r) => (
                    <Tooltip
                      key={r.id}
                      title={`${r.clienteNombre} – ${r.servicioNombre}`}
                    >
                      <Paper
                        sx={{
                          ...estiloReserva(r),
                          maxWidth: "95%",
                        }}
                        onClick={() => onSelectReserva(r)}
                      >
                        <Box
                          sx={{
                            display: "flex",
                            flexDirection: "column",
                            lineHeight: 1.1,
                          }}
                        >
                          <Typography
                            variant="body2"
                            sx={{ fontWeight: 600, fontSize: "0.75rem" }}
                          >
                            {r.clienteNombre}
                          </Typography>
                          <Typography
                            variant="caption"
                            sx={{
                              fontSize: "0.65rem",
                              opacity: 0.9,
                              mt: 0.3,
                              letterSpacing: "0.02em",
                            }}
                          >
                            {r.servicioNombre}
                          </Typography>
                          {r.estado === "Pendiente" && (
                            <Typography
                              sx={{
                                fontSize: "0.65rem",
                                fontWeight: 600,
                                mt: 0.4,
                                color: theme.palette.primary.main,
                                letterSpacing: "0.02em",
                                "@media (max-width: 600px)": {
                                  fontSize: "0.6rem",
                                },
                              }}
                            >
                              ⚠️ Requiere confirmación
                            </Typography>
                          )}
                        </Box>
                      </Paper>
                    </Tooltip>
                  ))}
                </Box>
              );
            })}
          </Grid>
        ))}
      </Grid>
    </Paper>
  );
}
