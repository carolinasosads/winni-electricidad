import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Typography,
  Box,
  Alert,
  MenuItem,
  Select,
  FormControl,
} from "@mui/material";
import { useState, useMemo } from "react";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { es } from "date-fns/locale";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";

export default function ModificarReservaModal({
  open,
  onClose,
  reserva,
  reservasMes,
  onSubmit,
}) {
  if (!reserva) return null;

  const fechaInicial = new Date(reserva.fechaReserva);
  const [fecha, setFecha] = useState(fechaInicial);
  const [errorBack, setErrorBack] = useState(null);

  const SLOTS = [
    { key: "09:00", hour: 9, minute: 0 },
    { key: "10:30", hour: 10, minute: 30 },
    { key: "12:00", hour: 12, minute: 0 },
    { key: "13:30", hour: 13, minute: 30 },
    { key: "15:00", hour: 15, minute: 0 },
    { key: "16:30", hour: 16, minute: 30 },
  ];

  const horaActual = `${String(fecha.getHours()).padStart(2, "0")}:${String(
    fecha.getMinutes()
  ).padStart(2, "0")}`;

  const handleModificar = () => {
    setErrorBack(null);

    const yyyy = fecha.getFullYear();
    const MM = String(fecha.getMonth() + 1).padStart(2, "0");
    const dd = String(fecha.getDate()).padStart(2, "0");
    const hh = String(fecha.getHours()).padStart(2, "0");
    const mm = String(fecha.getMinutes()).padStart(2, "0");

    const fechaLocalString = `${yyyy}-${MM}-${dd}T${hh}:${mm}:00`;

    onSubmit(fechaLocalString).catch((err) => {
      setErrorBack(err.message || "Error modificando la reserva.");
    });
  };

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
      <DialogTitle>Modificar Reserva</DialogTitle>

      <DialogContent dividers>
        {/* Cliente */}
        <Typography variant="body1" sx={{ mb: 1.5 }}>
          Cliente: <strong>{reserva.cliente?.nombre}</strong>
        </Typography>

        {/* Fecha actual */}
        <Typography variant="body2" sx={{ mb: 3 }}>
          Fecha actual:{" "}
          <strong>
            {new Date(reserva.fechaReserva).toLocaleString("es-UY")}
          </strong>
        </Typography>

        {/* Fecha + Hora en una fila */}
        <LocalizationProvider dateAdapter={AdapterDateFns} adapterLocale={es}>
          <Box
            sx={{
              display: "flex",
              gap: 2,
              mt: 1,
              alignItems: "flex-start",
            }}
          >
            {/* FECHA */}
            <Box sx={{ flex: 1 }}>
              <Typography fontWeight={600} sx={{ mb: 1 }}>
                Nueva fecha
              </Typography>

              <DatePicker
                value={fecha}
                onChange={(newDate) => {
                  if (!newDate) return;

                  const updated = new Date(newDate);
                  updated.setHours(fecha.getHours());
                  updated.setMinutes(fecha.getMinutes());
                  setFecha(updated);
                  setErrorBack(null);
                }}
                disablePast
                slotProps={{
                  textField: { fullWidth: true, size: "small" },
                }}
              />
            </Box>

            {/* HORA */}
            <Box sx={{ width: 160 }}>
              <Typography fontWeight={600} sx={{ mb: 1 }}>
                Hora
              </Typography>

              <FormControl fullWidth size="small">
                <Select
                  value={horaActual}
                  onChange={(e) => {
                    const selected = SLOTS.find((s) => s.key === e.target.value);
                    const updated = new Date(fecha);
                    updated.setHours(selected.hour);
                    updated.setMinutes(selected.minute);
                    setFecha(updated);
                    setErrorBack(null);
                  }}
                >
                  {SLOTS.map((s) => (
                    <MenuItem key={s.key} value={s.key}>
                      {s.key}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Box>
          </Box>
        </LocalizationProvider>

        {errorBack && (
          <Alert severity="error" sx={{ mt: 2 }}>
            {errorBack}
          </Alert>
        )}
      </DialogContent>

      <DialogActions>
        <Button onClick={onClose}>Cancelar</Button>
        <Button variant="contained" onClick={handleModificar}>
          Guardar cambios
        </Button>
      </DialogActions>
    </Dialog>
  );
}
