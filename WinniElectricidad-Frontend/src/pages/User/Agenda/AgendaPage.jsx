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
import { CALENDAR_WIDTH, DAY_MIN_HEIGHT } from "./AgendaConstants";
import {getServiciosActivos, getDireccionesUsuario, getHorariosDisponibles, } from "../../../services/authService";

export default function AgendaPage({ onReserve }) {
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

  useEffect(() => {
    const ac = new AbortController();

    (async () => {
      try {
        setLoadingServicios(true);
        setErrorServicios(null);
        setLoadingDisponibilidad(true);
        setErrorDisponibilidad(null);

        // Pedimos servicios, direcciones y disponibilidad en paralelo
        const [dataServicios, dataDirecciones, dataDisponibilidad] =
          await Promise.all([
            getServiciosActivos(ac.signal),
            getDireccionesUsuario(ac.signal),
            getHorariosDisponibles(ac.signal),
          ]);

        // ----- Servicios -----
        const optsServicios = (dataServicios ?? []).map((s) => ({
          id: s.id,
          label: s.titulo,
        }));
        setServiciosOpts(optsServicios);

        // ----- Direcciones -----
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

        // ----- Disponibilidad -----
        const disponibilidadNormalizada = (dataDisponibilidad ?? []).map(
          (d) => ({
            date: new Date(d.fecha), 
            slots: (d.horas ?? [])
              .filter((h) => h.disponible)
              .map((h) => h.hora), 
          })
        );

        setDisponibilidad(disponibilidadNormalizada);
      } catch (e) {
        if (e.name === "AbortError") {
          console.log("Petición cancelada");
          return;
        }

        const msg = e?.message ?? "Error al obtener datos iniciales";
        setErrorServicios(msg);
        setErrorDisponibilidad(msg);
      } finally {
        setLoadingServicios(false);
        setLoadingDisponibilidad(false);
      }
    })();

    return () => ac.abort();
  }, []);

  const calendarDays = useMemo(() => {
    const start = startOfWeek(startOfMonth(currentMonth), { weekStartsOn: 1 });
    const end = endOfWeek(endOfMonth(currentMonth), { weekStartsOn: 1 });
    return eachDayOfInterval({ start, end });
  }, [currentMonth]);

  // Disponibilidad agregada por día del mes actual
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
    else
      alert(
        `Reserva (mock): ${format(selectedDate, "PPP", {
          locale: es,
        })} ${slot.time}\n` + JSON.stringify(payload, null, 2)
      );
  };

  return (
    <Box
      sx={{ minHeight: "100vh", bgcolor: (t) => t.palette.background.default }}
    >
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
                  onClick={() =>
                    setCurrentMonth(addMonths(currentMonth, -1))
                  }
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
              <Grid
                container
                columns={7}
                spacing={1}
                sx={{ textAlign: "center", mb: 1 }}
              >
                {["lun", "mar", "mié", "jue", "vie", "sáb", "dom"].map((d) => (
                  <Grid key={d} item xs={1}>
                    <Typography
                      variant="caption"
                      fontWeight={700}
                      color="text.secondary"
                    >
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
                  const isSelected =
                    !!selectedDate && isSameDay(selectedDate, date);

                  return (
                    <Grid key={date.toISOString()} item xs={1}>
                      <Button
                        variant={isSelected ? "outlined" : "text"}
                        onClick={() => setSelectedDate(date)}
                        disabled={!info.hasAvailability || !inThisMonth}
                        sx={{
                          width: "100%",
                          minHeight: DAY_MIN_HEIGHT.md,
                          borderRadius: 2,
                          borderColor: isSelected
                            ? "primary.main"
                            : "divider",
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
                          label={
                            info.hasAvailability
                              ? `${info.availableCount} turnos`
                              : "—"
                          }
                          variant="outlined"
                          sx={{
                            borderColor: info.hasAvailability
                              ? "primary.light"
                              : "divider",
                          }}
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
            <Paper
              variant="outlined"
              sx={{ p: 2, maxHeight: 420, overflow: "auto" }}
            >
              <Typography variant="h6" fontWeight={700} gutterBottom>
                Horarios disponibles
              </Typography>

              {loadingDisponibilidad && (
                <Typography color="text.secondary" sx={{ mb: 1 }}>
                  Cargando disponibilidad...
                </Typography>
              )}

              {!loadingDisponibilidad && errorDisponibilidad && (
                <Typography color="error" sx={{ mb: 1 }}>
                  {errorDisponibilidad}
                </Typography>
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
                      <strong>
                        {format(selectedDate, "PPP", { locale: es })}
                      </strong>
                      . Probá otro día.
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
