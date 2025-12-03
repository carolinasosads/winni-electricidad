import {
  Box,
  Modal,
  Paper,
  Typography,
  Divider,
  Button,
  IconButton,
  List,
  ListItemButton,
  ListItemText,
} from "@mui/material";

import CloseIcon from "@mui/icons-material/Close";
import ScheduleIcon from "@mui/icons-material/Schedule";
import ErrorOutlineIcon from "@mui/icons-material/ErrorOutline";

import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFnsV3";
import { DatePicker } from "@mui/x-date-pickers";
import { es } from "date-fns/locale";

import { useState, useMemo } from "react";

const HOURS = [
  "09:00",
  "10:30",
  "12:00",
  "13:30",
  "15:00",
  "16:30",
];

export default function ModalModificarReserva({
  open,
  onClose,
  reserva,
  reservasMes,
  onSubmit,
}) {
  if (!reserva) return null;

  const originalDate = new Date(reserva.fechaReserva);

  const [fecha, setFecha] = useState(originalDate);
  const [horaSeleccionada, setHoraSeleccionada] = useState(null);

  const buildDateTime = (date, timeStr) => {
    const [hh, mm] = timeStr.split(":").map(Number);
    return new Date(date.getFullYear(), date.getMonth(), date.getDate(), hh, mm);
  };

  // horas ocupadas para el día elegido
  const horasOcupadas = useMemo(() => {
    return reservasMes
      .filter(r => {
        const d = new Date(r.fechaReserva);
        return (
          d.getFullYear() === fecha.getFullYear() &&
          d.getMonth() === fecha.getMonth() &&
          d.getDate() === fecha.getDate() &&
          r.id !== reserva.id
        );
      })
      .map(r => {
        const d = new Date(r.fechaReserva);
        const hh = d.getHours().toString().padStart(2, "0");
        const mm = d.getMinutes().toString().padStart(2, "0");
        return `${hh}:${mm}`;
      });
  }, [fecha, reservasMes, reserva.id]);

  const handleGuardar = () => {
    if (!horaSeleccionada) return;

    const nuevaFecha = buildDateTime(fecha, horaSeleccionada);
    onSubmit(nuevaFecha);
  };

  return (
    <Modal open={open} onClose={onClose}>
      <Box
        sx={{
          width: "95%",
          maxWidth: 480,
          mx: "auto",
          mt: "10vh",
        }}
      >
        <Paper
          sx={{
            p: 3,
            borderRadius: 3,
            boxShadow: "0px 8px 30px rgba(0,0,0,0.25)",
          }}
        >
          {/* Header */}
          <Box sx={{ display: "flex", justifyContent: "space-between", mb: 2 }}>
            <Typography variant="h6" fontWeight={600}>
              Sugerir nueva fecha
            </Typography>

            <IconButton onClick={onClose}>
              <CloseIcon />
            </IconButton>
          </Box>

          <Divider sx={{ mb: 2 }} />

          {/* Fecha */}
          <LocalizationProvider dateAdapter={AdapterDateFns} adapterLocale={es}>
            <DatePicker
              label="Nueva fecha"
              value={fecha}
              onChange={(v) => setFecha(v)}
            />
          </LocalizationProvider>

          <Typography sx={{ mt: 3, mb: 1 }} fontWeight={600}>
            Seleccionar hora
          </Typography>

          {/* Horas */}
          <List sx={{ maxHeight: 240, overflowY: "auto" }}>
            {HOURS.map((h) => {
              const ocupada = horasOcupadas.includes(h);

              return (
                <ListItemButton
                  key={h}
                  selected={horaSeleccionada === h}
                  onClick={() => setHoraSeleccionada(h)}
                  sx={{
                    borderRadius: 2,
                    mb: 1,
                    bgcolor:
                      horaSeleccionada === h ? "primary.light" : "transparent",
                  }}
                >
                  <ListItemText
                    primary={
                      <Box
                        sx={{
                          display: "flex",
                          alignItems: "center",
                          gap: 1,
                          color: ocupada ? "error.main" : "success.main",
                          fontWeight: 600,
                        }}
                      >
                        {ocupada ? (
                          <ErrorOutlineIcon color="error" />
                        ) : (
                          <ScheduleIcon color="success" />
                        )}
                        {h}
                      </Box>
                    }
                    secondary={
                      ocupada ? "Ocupado" : "Disponible"
                    }
                  />
                </ListItemButton>
              );
            })}
          </List>

          <Button
            variant="contained"
            fullWidth
            sx={{ mt: 3, py: 1.3, borderRadius: 2 }}
            disabled={!horaSeleccionada}
            onClick={handleGuardar}
          >
            Guardar sugerencia
          </Button>
        </Paper>
      </Box>
    </Modal>
  );
}
