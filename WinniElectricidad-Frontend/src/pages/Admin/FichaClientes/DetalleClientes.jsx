import { useState } from "react";
import {  Box, Paper,  Typography,  Drawer,  IconButton,  Divider,  CircularProgress,  Alert,  List,  ListItemText,  Button,  Dialog,
  DialogTitle,  DialogContent,  DialogActions,  Chip,} from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";

import { obtenerPresupuestoSegunReserva } from "../../../services/presupuestoService";

function getClienteId(c) {
  return c?.id ?? c?.Id ?? c?.idUsuario ?? c?.IdUsuario ?? null;
}
function getClienteNombre(c) {
  return c?.nombreCompleto ?? c?.NombreCompleto ?? "-";
}
function getClienteEmail(c) {
  return c?.email ?? c?.Email ?? "-";
}
function getClienteTelefono(c) {
  return c?.telefono ?? c?.Telefono ?? "-";
}
function formatDireccion(d) {
  if (!d) return "-";
  const calle = d.calle ?? d.Calle ?? "";
  const esquina = d.esquina ?? d.Esquina ?? "";
  const numero = d.numero ?? d.Numero ?? "";
  const apto = d.apto ?? d.Apto ?? "";
  return [calle, numero, esquina && `esq. ${esquina}`, apto && `apto ${apto}`]
    .filter(Boolean)
    .join(", ");
}

function getReservaId(r) {
  return r?.idReserva ?? r?.IdReserva ?? r?.id ?? r?.Id ?? null;
}
function getReservaFecha(r) {
  return (
    r?.fechaHora ?? r?.FechaHora ?? r?.fechaReserva ?? r?.FechaReserva ?? "-"
  );
}
function getReservaEstado(r) {
  return r?.estado ?? r?.Estado ?? "-";
}

function getTienePresupuesto(r) {
  return r?.tienePresupuesto ?? r?.TienePresupuesto ?? false;
}
function getMontoPresupuestado(r) {
  return r?.montoPresupuestado ?? r?.MontoPresupuestado ?? 0;
}

export default function DetalleClientes({
  open,  onClose,  clienteSel,  detalle,  loadingDetalle,  errorDetalle,
  mostrarReservas,  onToggleReservas,  reservas,  loadingReservas,  errorReservas,}) {
  const direcciones = detalle?.direcciones ?? detalle?.Direcciones ?? [];

  const [openPresu, setOpenPresu] = useState(false);
  const [loadingPresu, setLoadingPresu] = useState(false);
  const [errorPresu, setErrorPresu] = useState(null);
  const [presuSel, setPresuSel] = useState(null);
  const [reservaSel, setReservaSel] = useState(null);

  const handleVerPresupuesto = async (r) => {
    setReservaSel(r);
    setOpenPresu(true);

    setLoadingPresu(true);
    setErrorPresu(null);
    setPresuSel(null);

    const idReserva = getReservaId(r);
    if (!idReserva) {
      setErrorPresu("No se pudo determinar el ID de la reserva.");
      setLoadingPresu(false);
      return;
    }

    try {
      const data = await obtenerPresupuestoSegunReserva(idReserva);
      setPresuSel(data); 
    } catch (err) {
      setErrorPresu(err?.message || "Error al obtener el presupuesto.");
    } finally {
      setLoadingPresu(false);
    }
  };

  return (
    <>
      <Drawer
        anchor="right"
        open={open}
        onClose={onClose}
        PaperProps={{ sx: { width: { xs: "100%", sm: 520 } } }}
      >
        <Box sx={{ p: 2, display: "flex", gap: 1 }}>
          <Box sx={{ flex: 1 }}>
            <Typography variant="h6" fontWeight={700}>
              Detalle del cliente
            </Typography>
          </Box>
          <IconButton onClick={onClose}>
            <CloseIcon />
          </IconButton>
        </Box>

        <Divider />

        <Box sx={{ p: 2 }}>
          {!clienteSel ? (
            <Alert severity="info">Seleccioná un cliente.</Alert>
          ) : (
            <>
              <Paper sx={{ p: 2, mb: 2 }}>
                <Typography fontWeight={700}>Datos personales</Typography>
                <Typography>
                  <b>Nombre:</b> {getClienteNombre(clienteSel)}
                </Typography>
                <Typography>
                  <b>Email:</b> {getClienteEmail(clienteSel)}
                </Typography>
                <Typography>
                  <b>Teléfono:</b> {getClienteTelefono(clienteSel)}
                </Typography>
              </Paper>

              {loadingDetalle && (
                <Box
                  sx={{
                    display: "flex",
                    gap: 1,
                    alignItems: "center",
                    mb: 2,
                  }}
                >
                  <CircularProgress size={18} />
                  <Typography variant="body2">Cargando detalle…</Typography>
                </Box>
              )}

              {errorDetalle && <Alert severity="error">{errorDetalle}</Alert>}

              {/* Direcciones */}
              <Paper sx={{ p: 2, mb: 2 }}>
                <Typography fontWeight={700}>
                  Direcciones ({direcciones.length})
                </Typography>

                {direcciones.length === 0 ? (
                  <Alert severity="info" sx={{ mt: 1 }}>
                    No tiene direcciones.
                  </Alert>
                ) : (
                  <List>
                    {direcciones.map((d, i) => (
                      <ListItemText key={i} primary={formatDireccion(d)} />
                    ))}
                  </List>
                )}
              </Paper>

              {/* Reservas*/}
              <Paper sx={{ p: 2 }}>
                <Box
                  sx={{
                    display: "flex",
                    justifyContent: "space-between",
                    alignItems: "center",
                    gap: 1,
                  }}
                >
                  <Typography fontWeight={700}>
                    Reservas {mostrarReservas ? `(${reservas.length})` : ""}
                  </Typography>

                  <Button variant="contained" onClick={onToggleReservas}>
                    {mostrarReservas ? "Ocultar reservas" : "Mostrar reservas"}
                  </Button>
                </Box>

                {mostrarReservas && (
                  <Box sx={{ mt: 2 }}>
                    {loadingReservas && (
                      <Box sx={{ display: "flex", gap: 1, alignItems: "center" }}>
                        <CircularProgress size={18} />
                        <Typography variant="body2">Cargando reservas…</Typography>
                      </Box>
                    )}

                    {errorReservas && (
                      <Alert severity="error">{errorReservas}</Alert>
                    )}

                    {!loadingReservas &&
                      !errorReservas &&
                      reservas.length === 0 && (
                        <Alert severity="info">No hay reservas.</Alert>
                      )}

                    {!loadingReservas &&
                      !errorReservas &&
                      reservas.length > 0 && (
                        <List>
                          {reservas.map((r, i) => {
                            const idR = getReservaId(r) ?? i;
                            const fecha = getReservaFecha(r);
                            const estado = getReservaEstado(r);

                            const tiene = getTienePresupuesto(r);
                            const monto = getMontoPresupuestado(r);

                            return (
                              <Paper
                                key={idR}
                                variant="outlined"
                                sx={{ p: 1.5, mb: 1.2 }}
                              >
                                <Typography sx={{ fontWeight: 700 }}>
                                  Reserva #{getReservaId(r) ?? "-"}
                                </Typography>

                                <Typography variant="body2">
                                  <b>Fecha:</b> {fecha}
                                </Typography>

                                <Typography variant="body2" sx={{ mb: 1 }}>
                                  <b>Estado:</b> {estado}
                                </Typography>

                                <Chip
                                  size="small"
                                  sx={{ mb: 1 }}
                                  label={
                                    tiene
                                      ? `Presupuesto: $ ${monto}`
                                      : "Sin presupuesto"
                                  }
                                  color={tiene ? "success" : "default"}
                                />

                                <Box sx={{ display: "flex", gap: 1 }}>
                                  <Button
                                    variant="outlined"
                                    onClick={() => handleVerPresupuesto(r)}
                                    disabled={!tiene}
                                  >
                                    Ver presupuesto
                                  </Button>
                                </Box>
                              </Paper>
                            );
                          })}
                        </List>
                      )}
                  </Box>
                )}
              </Paper>
            </>
          )}
        </Box>
      </Drawer>

      {/* Modal presupuesto*/}
      <Dialog open={openPresu} onClose={() => setOpenPresu(false)} fullWidth>
        <DialogTitle>
          Presupuesto{" "}
          {reservaSel ? `de Reserva #${getReservaId(reservaSel)}` : ""}
        </DialogTitle>

        <DialogContent dividers>
          {loadingPresu && (
            <Box sx={{ display: "flex", gap: 1, alignItems: "center" }}>
              <CircularProgress size={18} />
              <Typography variant="body2">Cargando presupuesto…</Typography>
            </Box>
          )}

          {errorPresu && <Alert severity="error">{errorPresu}</Alert>}

          {!loadingPresu && !errorPresu && presuSel === null && (
            <Alert severity="info">
              Esta reserva no tiene presupuesto asociado.
            </Alert>
          )}

          {!loadingPresu && !errorPresu && presuSel && (
            <Box sx={{ display: "grid", gap: 1 }}>
              <Typography>
                <b>ID Presupuesto:</b> {presuSel?.id ?? presuSel?.Id ?? "-"}
              </Typography>

              <Typography>
                <b>Monto total:</b>{" "}
                {presuSel?.montoTotal ?? presuSel?.MontoTotal ?? "-"}
              </Typography>

              <Typography>
                <b>Fecha:</b>{" "}
                {presuSel?.fechaCreacion ?? presuSel?.FechaCreacion ?? "-"}
              </Typography>

              <Typography>
                <b>Notas internas:</b>{" "}
                {presuSel?.notasInternas ?? presuSel?.NotasInternas ?? "-"}
              </Typography>
            </Box>
          )}
        </DialogContent>

        <DialogActions>
          <Button onClick={() => setOpenPresu(false)}>Cerrar</Button>
        </DialogActions>
      </Dialog>
    </>
  );
}