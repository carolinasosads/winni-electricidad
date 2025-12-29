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
  useMediaQuery,
} from "@mui/material";
import Alert from "@mui/material/Alert";
import { useTheme } from "@mui/material/styles";

import ChevronLeftIcon from "@mui/icons-material/ChevronLeft";
import ChevronRightIcon from "@mui/icons-material/ChevronRight";
import AgendaFilters from "./AgendaFilters";
import { CALENDAR_WIDTH, DAY_MIN_HEIGHT } from "./AgendaConstants";
import {
  getDireccionesUsuario,
  getHorariosDisponibles,
  crearReserva,
} from "../../../services/authService";
import { getServiciosActivos } from "../../../services/servicioService";
import ApiError from "../../../services/ApiError";

export default function AgendaPage({ onReserve }) {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down("sm"));

  const [currentMonth, setCurrentMonth] = useState(new Date());
  const [selectedDate, setSelectedDate] = useState(null);

  // filtros
  const [servicios, setServicios] = useState([]);
  const [tipoTrabajo, setTipoTrabajo] = useState("instalacion");
  const [direccionId, setDireccionId] = useState("");
  const [comentarios, setComentarios] = useState("");
  const [direccionesUsuario, setDireccionesUsuario] = useState([]);

  // servicios
  const [serviciosOpts, setServiciosOpts] = useState([]);
  const [loadingServicios, setLoadingServicios] = useState(true);
  const [errorServicios, setErrorServicios] = useState(null);

  // horarios disponibles
  const [disponibilidad, setDisponibilidad] = useState([]);
  const [loadingDisponibilidad, setLoadingDisponibilidad] = useState(true);
  const [errorDisponibilidad, setErrorDisponibilidad] = useState(null);

  // horario seleccionado
  const [selectedSlot, setSelectedSlot] = useState(null);

  // mensajes reserva
  const [reservaSuccess, setReservaSuccess] = useState("");
  const [reservaError, setReservaError] = useState("");

  // ---- carga inicial de datos ----
  useEffect(() => {
    const ac = new AbortController();

    async function cargarDatos() {
      try {
        setLoadingServicios(true);
        setErrorServicios(null);
        setLoadingDisponibilidad(true);
        setErrorDisponibilidad(null);

        const [dataServicios, dataDirecciones, dataDisponibilidad] =
          await Promise.all([
            getServiciosActivos(ac.signal),
            getDireccionesUsuario(ac.signal),
            getHorariosDisponibles(ac.signal),
          ]);

        // Servicios
        const optsServicios = (dataServicios ?? []).map((s) => ({
          id: s.id,
          label: s.titulo,
        }));
        setServiciosOpts(optsServicios);

        // Direcciones
        const optsDirecciones = (dataDirecciones ?? []).map((d) => ({
          id: d.id,
          label: `${d.calle} ${d.numero ?? ""}${
            d.apto ? `, Apto ${d.apto}` : ""
          }${d.esquina ? ` - Esq. ${d.esquina}` : ""}`.trim(),
        }));
        setDireccionesUsuario(optsDirecciones);
        if (optsDirecciones.length > 0) {
          setDireccionId(optsDirecciones[0].id);
        }

        // Disponibilidad (solo horas disponibles)
        setDisponibilidad(normalizarDisponibilidad(dataDisponibilidad));
      } catch (e) {
        if (e.name === "AbortError") return;

        const msg = e?.message ?? "Error al obtener datos iniciales";
        setErrorServicios(msg);
        setErrorDisponibilidad(msg);
      } finally {
        setLoadingServicios(false);
        setLoadingDisponibilidad(false);
      }
    }

    cargarDatos();

    return () => ac.abort();
  }, []);

  // ---- helpers de calendario ----
  const calendarDays = useMemo(() => {
    const start = startOfWeek(startOfMonth(currentMonth), { weekStartsOn: 1 });
    const end = endOfWeek(endOfMonth(currentMonth), { weekStartsOn: 1 });
    return eachDayOfInterval({ start, end });
  }, [currentMonth]);

  const monthAvailability = useMemo(
    () =>
      disponibilidad
        .filter((d) => isSameMonth(d.date, currentMonth))
        .map((d) => ({
          date: d.date,
          hasAvailability: (d.slots?.length ?? 0) > 0,
          availableCount: d.slots?.length ?? 0,
          slots: d.slots ?? [],
        })),
    [disponibilidad, currentMonth]
  );

  const infoFor = (date) =>
    monthAvailability.find((d) => isSameDay(d.date, date)) || {
      date,
      hasAvailability: false,
      availableCount: 0,
      slots: [],
    };

  const slots = useMemo(() => {
    if (!selectedDate) return [];
    const info = infoFor(selectedDate);
    return (info.slots ?? []).map((time) => ({ time }));
  }, [selectedDate, monthAvailability]);

  const mobileAvailableDays = useMemo(
    () =>
      disponibilidad
        .filter((d) => d.slots.length > 0)
        .sort((a, b) => a.date - b.date)
        .slice(0, 14)
        .map((d) => ({
          date: d.date,
          availableCount: d.slots.length,
        })),
    [disponibilidad]
  );

  // ---- reservar ----
  const handleReserve = async () => {
    if (!selectedDate || !selectedSlot) return;

    // limpio mensajes previos
    setReservaSuccess("");
    setReservaError("");

    // validación rápida front
    if (!servicios || servicios.length === 0) {
      setReservaError("Debe seleccionar al menos un servicio.");
      return;
    }

    const payload = {
      fechaReserva: combinarFechaYHora(selectedDate, selectedSlot),
      tipoServicio: tipoTrabajo === "instalacion" ? 1 : 2,
      idDireccion: Number(direccionId),
      idServicios: servicios.map((s) => s.id),
      comentario: comentarios?.trim() || null,
    };

    try {
      const reservaCreada = await crearReserva(payload);

      if (onReserve) onReserve(reservaCreada);

      const fechaStr = format(selectedDate, "PPP", { locale: es });
      setReservaSuccess(`Reserva creada con éxito para ${fechaStr} ${selectedSlot}.`);
      setReservaError("");

      // limpiar campos
      setSelectedSlot(null);
      setComentarios("");

      // volver a pedir disponibilidad para refrescar el calendario
      try {
        const dataDisponibilidadActualizada = await getHorariosDisponibles();
        setDisponibilidad(normalizarDisponibilidad(dataDisponibilidadActualizada));
      } catch (e) {
        console.error("No se pudo actualizar la disponibilidad:", e);
      }
    } catch (e) {
      console.error("Error al crear la reserva:", e);

      let msg = "No se pudo crear la reserva. Intentá nuevamente.";

      if (e instanceof ApiError) {
        // puede venir con JSON adentro del message
        try {
          const parsed = JSON.parse(e.message);
          if (parsed?.errors) {
            msg = Object.values(parsed.errors).flat().join(" ");
          } else if (parsed?.message) {
            msg = parsed.message;
          } else {
            msg = e.message;
          }
        } catch {
          msg = e.message || msg;
        }
      } else if (e?.message) {
        msg = e.message;
      }

      setReservaError(msg);
      setReservaSuccess("");
    }
  };

  const noHayHorariosDisponibles =
    !loadingDisponibilidad &&
    disponibilidad.length === 0;

  // ---- render ----
  return (
    <Box sx={{ bgcolor: (t) => t.palette.background.default }}>
      <Box sx={{ p: { xs: 2, sm: 3 }, maxWidth: 1000, mx: "auto" }}>
        <Box
          sx={{
            maxWidth: CALENDAR_WIDTH,
            mx: "auto",
            mb: 3,
            textAlign: { xs: "center", sm: "left" },
          }}
        >
          <Typography variant="h4" fontWeight={700} gutterBottom>
            Reservas para visitas de presupuesto
          </Typography>

          <Typography variant="subtitle1" color="text.secondary" gutterBottom>
            Desde este panel podés reservar un turno para que uno de nuestros técnicos visite tu domicilio y realice un presupuesto sin costo.
          </Typography>

          <Alert severity="warning" variant="outlined">
            Importante: esta reserva es solo para la visita de presupuesto. No se agendan instalaciones ni reparaciones por este medio.
          </Alert>
        </Box>

        <Grid container spacing={3} justifyContent="center">
          <Grid item xs={12} md={8} lg={8}>
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

            {isMobile ? (
              <Paper variant="outlined" sx={{ p: 2, mt: 2 }}>
                <Typography variant="h6" fontWeight={700} gutterBottom>
                  Días disponibles
                </Typography>

                <Box sx={{ display: "flex", flexDirection: "column", gap: 1 }}>
                  {mobileAvailableDays.length === 0 ? (
                    <Typography color="text.secondary">
                      No hay días disponibles.
                    </Typography>
                  ) : (
                    mobileAvailableDays.map((d) => {
                      const selected =
                        selectedDate && isSameDay(selectedDate, d.date);

                      return (
                        <Button
                          key={d.date.toISOString()}
                          variant={selected ? "contained" : "outlined"}
                          onClick={() => {
                            setSelectedDate(d.date);
                            setSelectedSlot(null);
                            setReservaSuccess("");
                            setReservaError("");
                          }}
                          sx={{
                            justifyContent: "space-between",
                            textTransform: "none",
                          }}
                        >
                          <Typography
                            sx={{
                              fontWeight: selected ? 700 : 600,
                              color: selected ? "inherit" : "primary.main",
                            }}
                          >
                            {format(d.date, "EEE d MMM", { locale: es })}
                          </Typography>

                          <Chip
                            size="small"
                            label={`${d.availableCount} turnos`}
                            variant="outlined"
                            sx={{
                              color: selected ? "inherit" : "primary.main",
                              borderColor: selected
                                ? "transparent"
                                : "primary.light",
                            }}
                          />
                        </Button>
                      );
                    })
                  )}
                </Box>
              </Paper>
            ) : (
              <Paper
                variant="outlined"
                sx={{
                  p: 2,
                  mt: 2,
                  mx: "auto",
                  width: CALENDAR_WIDTH,
                  overflow: "hidden",
                }}
              >
                <Box
                  sx={{
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "space-between",
                    mb: 1,
                  }}
                >
                  <IconButton size="small" onClick={() => setCurrentMonth(addMonths(currentMonth, -1))}>
                    <ChevronLeftIcon />
                  </IconButton>

                  <Typography variant="h6" fontWeight={700}>
                    {format(currentMonth, "LLLL yyyy", { locale: es })}
                  </Typography>

                  <IconButton size="small" onClick={() => setCurrentMonth(addMonths(currentMonth, 1))}>
                    <ChevronRightIcon />
                  </IconButton>
                </Box>

                <Box
                  sx={{
                    display: "grid",
                    gridTemplateColumns: "repeat(7, 1fr)",
                    textAlign: "center",
                    mb: 1,
                    columnGap: 1,
                  }}
                >
                  {["lun", "mar", "mié", "jue", "vie", "sáb", "dom"].map((d) => (
                    <Box key={d}>
                      <Typography variant="caption" fontWeight={700} color="text.secondary">
                        {d.toUpperCase()}
                      </Typography>
                    </Box>
                  ))}
                </Box>

                <Box
                  sx={{
                    display: "grid",
                    gridTemplateColumns: "repeat(7, 1fr)",
                    gap: 1,
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
                    const isAvailable = info.hasAvailability && inThisMonth;

                    return (
                      <Box key={date.toISOString()}>
                        <Button
                          variant={isSelected ? "outlined" : "text"}
                          onClick={() => {
                            setSelectedDate(date);
                            setSelectedSlot(null);
                            setReservaSuccess("");
                            setReservaError("");
                          }}
                          disabled={!isAvailable}
                          sx={{
                            width: "100%",
                            minHeight: DAY_MIN_HEIGHT.md,
                            borderRadius: 2,

                            /* SELECCION */
                            borderColor: isSelected ? "primary.main" : "divider",
                            bgcolor: isSelected ? "rgba(25,118,210,0.06)" : "background.paper",

                            color: "text.primary",

                            opacity: inThisMonth ? 1 : 0.45,
                            display: "flex",
                            flexDirection: "column",
                            alignItems: "center",
                            justifyContent: "space-between",
                            textTransform: "none",

                            /* HOVER */
                            "&:hover": isAvailable && {
                              bgcolor: "rgba(25,118,210,0.08)",
                            },

                            "& .MuiTouchRipple-child": {
                              backgroundColor: "rgba(25,118,210,0.35)",
                            },

                            "&:disabled": {
                              opacity: 0.35,
                            },
                          }}
                        >
                          <Typography
                            variant="body2"
                            sx={{
                              fontWeight: isSelected ? 700 : 500,

                              color: isAvailable
                                ? isSelected
                                  ? "primary.main"
                                  : "primary.main"
                                : "text.disabled",
                            }}
                          >
                            {format(date, "d", { locale: es })}
                          </Typography>

                          <Chip
                            size="small"
                            label={info.hasAvailability ? `${info.availableCount} turnos` : "—"}
                            variant="outlined"
                            sx={{
                              color: info.hasAvailability
                                ? "primary.main"
                                : "text.disabled",
                              borderColor: info.hasAvailability
                                ? "primary.light"
                                : "divider",
                            }}
                          />
                        </Button>
                      </Box>
                    );
                  })}
                </Box>
              </Paper>
            )}

            {/* HORARIOS DISPONIBLES */}
            <Paper
              variant="outlined"
              sx={{ p: 2, mt: 2 }}
            > 
              <Typography variant="h6" fontWeight={700} gutterBottom>
                Horarios disponibles
              </Typography>

              {reservaSuccess && (
                <Alert
                  severity="success"
                  variant="outlined"
                  sx={{ mb: 1 }}
                  onClose={() => setReservaSuccess("")}
                >
                  {reservaSuccess}
                </Alert>
              )}

              {reservaError && (
                <Alert
                  severity="error"
                  variant="outlined"
                  sx={{ mb: 1 }}
                  onClose={() => setReservaError("")}
                >
                  {reservaError}
                </Alert>
              )}

              {loadingDisponibilidad && (
                <Typography color="text.secondary" sx={{ mb: 1 }}>
                  Cargando disponibilidad...
                </Typography>
              )}

              {!loadingDisponibilidad && (
                <>
                  {errorDisponibilidad ? (
                    <Typography color="error" sx={{ mb: 2 }}>
                      No se pudo cargar la disponibilidad. Intentá nuevamente más tarde.
                    </Typography>
                  ) : noHayHorariosDisponibles ? (
                    <Typography color="text.secondary" sx={{ mb: 2 }}>
                      No hay horarios disponibles en este momento.
                    </Typography>
                  ) : !selectedDate ? (
                    <Typography color="text.secondary" sx={{ mb: 2 }}>
                      Seleccioná un día del calendario.
                    </Typography>
                  ) : null}
                </>
              )}

              {selectedDate && (
                <>
                  <Typography variant="body2" color="text.secondary">
                    {format(selectedDate, "PPPP", { locale: es })}
                  </Typography>

                  <Divider sx={{ my: 1.5 }} />

                  {slots.length === 0 ? (
                    <Typography color="text.secondary" sx={{ mb: 2 }}>
                      No hay horarios para{" "}
                      <strong>{format(selectedDate, "PPP", { locale: es })}</strong>. Probá otro día.
                    </Typography>
                  ) : (
                    <Box sx={{ display: "flex", flexWrap: "wrap", gap: 1, mb: 2 }}>
                      {slots.map((s) => (
                        <Button
                          key={s.time}
                          size="small"
                          variant={selectedSlot === s.time ? "contained" : "outlined"}
                          onClick={() => setSelectedSlot(s.time)}
                          sx={{ borderRadius: 2 }}
                        >
                          {s.time}
                        </Button>
                      ))}
                    </Box>
                  )}

                  <TextField
                    label="Comentarios (opcional)"
                    placeholder="Ej: timbre roto, preferencia por la tarde, etc."
                    fullWidth
                    multiline
                    minRows={3}
                    value={comentarios}
                    onChange={(e) => setComentarios(e.target.value)}
                    sx={{ mb: 2 }}
                  />

                  {selectedSlot && (
                    <Button
                      variant="contained"
                      color="primary"
                      fullWidth
                      sx={{ mt: 0, borderRadius: 2 }}
                      onClick={handleReserve}
                    >
                      Confirmar reserva
                    </Button>
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

// ---- helpers ----
function combinarFechaYHora(fecha, horaStr) {
  const [hours, minutes] = horaStr.split(":").map(Number);

  const year = fecha.getFullYear();
  const month = String(fecha.getMonth() + 1).padStart(2, "0");
  const day = String(fecha.getDate()).padStart(2, "0");
  const hh = String(hours).padStart(2, "0");
  const mm = String(minutes).padStart(2, "0");

  return `${year}-${month}-${day}T${hh}:${mm}:00`;
}

function normalizarDisponibilidad(dataDisponibilidad) {
  return (dataDisponibilidad ?? []).map((d) => ({
    date: new Date(d.fecha),
    slots: (d.horas ?? []).filter((h) => h.disponible).map((h) => h.hora),
  }));
}
