import { useEffect, useMemo, useState } from "react";
import {
  addMonths,
  eachDayOfInterval,
  endOfMonth,
  endOfWeek,
  format,
  isSameDay,
  isSameMonth,
  startOfMonth,
  startOfWeek,
} from "date-fns";
import { es } from "date-fns/locale";

// MUI
import {
  Box,
  Paper,
  Grid,
  Typography,
  IconButton,
  Button,
  Chip,
  Divider,
  TextField,
} from "@mui/material";
import ChevronLeftIcon from "@mui/icons-material/ChevronLeft";
import ChevronRightIcon from "@mui/icons-material/ChevronRight";
import AgendaFilters from "./AgendaFilters";
import { CALENDAR_WIDTH, DAY_MIN_HEIGHT, direccionesUsuario } from "./AgendaConstants";
import { getServiciosActivos } from "../../../services/authService";

/** ------------------ MOCKS SIN BACKEND (slots y disponibilidad) ------------------ */
function mockSlotsFor(date) {
  const seed = date.getDate() * (date.getMonth() + 1);
  const base = ["09:00","09:30","10:00","10:30","11:00","15:00","15:30","16:00","16:30"];
  return base
    .map((t, i) => ({ time: t, available: ((seed + i * 3) % 5) !== 0 }))
    .filter(s => s.available);
}

function mockMonthAvailability(currentMonth) {
  const start = startOfMonth(currentMonth);
  const end = endOfMonth(currentMonth);
  const days = eachDayOfInterval({ start, end });
  return days.map(d => {
    const slots = mockSlotsFor(d);
    return {
      date: d,
      hasAvailability: slots.length > 0,
      availableCount: slots.length,
    };
  });
}

export default function AgendaPage({ onReserve }) {
  const [currentMonth, setCurrentMonth] = useState(new Date());
  const [selectedDate, setSelectedDate] = useState(null);

  // filtros
  const [servicios, setServicios] = useState([]); // [{id,label}, ...] seleccionados
  const [tipoTrabajo, setTipoTrabajo] = useState("instalacion");
  const [direccionId, setDireccionId] = useState(direccionesUsuario[0]?.id ?? "");
  const [comentarios, setComentarios] = useState("");

  // opciones desde API
  const [serviciosOpts, setServiciosOpts] = useState([]);          // ← FALTABA
  const [loadingServicios, setLoadingServicios] = useState(true);  // ← FALTABA
  const [errorServicios, setErrorServicios] = useState(null);      // ← FALTABA

  useEffect(() => {
    const ac = new AbortController();
    (async () => {
      try {
        setLoadingServicios(true);
        setErrorServicios(null);
        

      const data = await getServiciosActivos(ac.signal); //ver si se usa
            const opts = (data ?? []).map(s => ({
        id: s.id,
        label: s.titulo, // ← el nombre real del servicio
      }));
        
        setServiciosOpts(opts);
      } catch (e) {
        setErrorServicios(e?.message ?? "Error al obtener servicios");
      } finally {
        setLoadingServicios(false);
      }
    })();
    return () => ac.abort();
  }, []);

  const calendarDays = useMemo(() => {
    const start = startOfWeek(startOfMonth(currentMonth), { weekStartsOn: 1 });
    const end = endOfWeek(endOfMonth(currentMonth), { weekStartsOn: 1 });
    return eachDayOfInterval({ start, end });
  }, [currentMonth]);

  const monthAvailability = useMemo(
    () => mockMonthAvailability(currentMonth),
    [currentMonth]
  );

  const infoFor = (date) =>
    monthAvailability.find((d) => isSameDay(d.date, date)) || {
      date,
      hasAvailability: false,
      availableCount: 0,
    };

  const slots = selectedDate ? mockSlotsFor(selectedDate) : [];

  const handleReserve = (slot) => {
    const payload = {
      date: selectedDate,
      time: slot.time,
      servicios: servicios.map((s) => s.id),
      tipo: tipoTrabajo,
      direccionId,
      comentarios,
    };
    if (onReserve) onReserve(payload);
    else alert(
      `Reserva (mock): ${format(selectedDate, "PPP", { locale: es })} ${slot.time}\n` +
      JSON.stringify(payload, null, 2)
    );
  };

  return (
    <Box sx={{ minHeight: "100vh", bgcolor: (t) => t.palette.background.default }}>
      <Box sx={{ p: { xs: 2, sm: 3 }, flex: 1 }}>
        <Typography variant="h4" fontWeight={700} gutterBottom>
          Reservas
        </Typography>

        <Grid container spacing={3}>
          {/* Columna izquierda: filtros + calendario */}
          <Grid item xs={12} md={7} lg={8}>
            <AgendaFilters
              width={CALENDAR_WIDTH}
              servicioOpciones={serviciosOpts}
              direccionesUsuario={direccionesUsuario}
              servicios={servicios}
              setServicios={setServicios}
              tipoTrabajo={tipoTrabajo}
              setTipoTrabajo={setTipoTrabajo}
              direccionId={direccionId}
              setDireccionId={setDireccionId}
              loadingServicios={loadingServicios}
              errorServicios={errorServicios}
            />

            <Paper
              variant="outlined"
              sx={{
                p: 2,
                mx: "auto",
                width: CALENDAR_WIDTH,
                overflow: "hidden",
              }}
            >
              {/* Header de mes */}
              <Box
                sx={{
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "space-between",
                  mb: 1,
                }}
              >
                <IconButton
                  size="small"
                  onClick={() => setCurrentMonth(addMonths(currentMonth, -1))}
                >
                  <ChevronLeftIcon />
                </IconButton>

                <Typography variant="h6" fontWeight={700}>
                  {format(currentMonth, "LLLL yyyy", { locale: es })}
                </Typography>

                <IconButton
                  size="small"
                  onClick={() => setCurrentMonth(addMonths(currentMonth, 1))}
                >
                  <ChevronRightIcon />
                </IconButton>
              </Box>

              {/* Nombres de días */}
              <Grid container columns={7} spacing={1} sx={{ textAlign: "center", mb: 1 }}>
                {["lun", "mar", "mié", "jue", "vie", "sáb", "dom"].map((d) => (
                  <Grid key={d} item xs={1}>
                    <Typography variant="caption" fontWeight={700} color="text.secondary">
                      {d.toUpperCase()}
                    </Typography>
                  </Grid>
                ))}
              </Grid>

              {/* Grilla de días */}
              <Grid
                container
                columns={7}
                spacing={1}
                sx={{
                  minHeight: {
                    xs: 6 * DAY_MIN_HEIGHT.xs,
                    sm: 6 * DAY_MIN_HEIGHT.sm,
                    md: 6 * DAY_MIN_HEIGHT.md,
                  },
                }}
              >
                {calendarDays.map((date) => {
                  const inThisMonth = isSameMonth(date, currentMonth);
                  const info = infoFor(date);
                  const isSelected = !!selectedDate && isSameDay(selectedDate, date);

                  return (
                    <Grid key={date.toISOString()} item xs={1}>
                      <Button
                        variant={isSelected ? "outlined" : "text"}
                        onClick={() => setSelectedDate(date)}
                        disabled={!info.hasAvailability || !inThisMonth}
                        sx={{
                          width: "100%",
                          minHeight: DAY_MIN_HEIGHT.md, // alto fijo por día (elige el que quieras)
                          borderRadius: 2,
                          borderColor: isSelected ? "primary.main" : "divider",
                          bgcolor: "background.paper",
                          color: "text.primary",
                          opacity: inThisMonth ? 1 : 0.45,
                          display: "flex",
                          flexDirection: "column",
                          alignItems: "center",
                          justifyContent: "space-between",
                          textTransform: "none",
                          "&:disabled": { opacity: 0.35 },
                        }}
                      >
                        <Typography variant="body2">
                          {format(date, "d", { locale: es })}
                        </Typography>
                        <Chip
                          size="small"
                          label={info.hasAvailability ? `${info.availableCount} turnos` : "—"}
                          variant="outlined"
                          sx={{ borderColor: info.hasAvailability ? "primary.light" : "divider" }}
                        />
                      </Button>
                    </Grid>
                  );
                })}
              </Grid>
            </Paper>
          </Grid>

          {/* Columna derecha: horarios */}
          <Grid item xs={12} md={5} lg={4}>
            <Paper variant="outlined" sx={{ p: 2, maxHeight: 420, overflow: "auto" }}>
              <Typography variant="h6" fontWeight={700} gutterBottom>
                Horarios disponibles
              </Typography>

              <TextField
                label="Comentarios (opcional)"
                placeholder="Ej: timbre roto, preferencia por la tarde, etc."
                fullWidth
                multiline
                minRows={3}
                value={comentarios}
                onChange={(e) => setComentarios(e.target.value)}
                sx={{ mb: 2 }}
                disabled={!selectedDate}
              />

              {!selectedDate && (
                <Typography color="text.secondary">
                  Seleccioná un día del calendario.
                </Typography>
              )}

              {selectedDate && (
                <>
                  <Typography variant="body2" color="text.secondary">
                    {format(selectedDate, "PPPP", { locale: es })}
                  </Typography>
                  <Divider sx={{ my: 1.5 }} />

                  {slots.length === 0 ? (
                    <Typography color="text.secondary">
                      No hay horarios para{" "}
                      <strong>{format(selectedDate, "PPP", { locale: es })}</strong>. Probá otro día.
                    </Typography>
                  ) : (
                    <Box sx={{ display: "flex", flexWrap: "wrap", gap: 1 }}>
                      {slots.map((s) => (
                        <Button
                          key={s.time}
                          size="small"
                          variant="outlined"
                          onClick={() => handleReserve(s)}
                          sx={{ borderRadius: 2 }}
                        >
                          {s.time}
                        </Button>
                      ))}
                    </Box>
                  )}
                </>
              )}
            </Paper>
          </Grid>
        </Grid>
      </Box>
    </Box>
  );
}
