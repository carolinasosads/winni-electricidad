import { useState } from "react";
import {  Box,  Paper,  Typography,  Drawer,  IconButton,  Divider,  Alert,  List,  ListItemText,  Button,  Dialog,
  DialogTitle,  DialogContent,  DialogActions,  Chip,  TextField,  CircularProgress,} from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";

import {  crearPresupuestoParaReserva,  obtenerPresupuestoSegunReserva,  actualizarMontoPagadoPresupuesto,} from "../../../services/presupuestoService";

import ApiError from "../../../services/ApiError";

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

function formatearFecha(fechaIso) {
  if (!fechaIso) return "-";
  const fecha = new Date(fechaIso);
  if (isNaN(fecha.getTime())) return "-";
  return fecha.toLocaleString("es-UY", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

function getPresuMontoTotal(p) {
  const v = p?.montoTotal ?? p?.MontoTotal ?? p?.monto ?? p?.Monto;
  return v ?? "-";
}
function getPresuMontoPagado(p) {
  const v = p?.montoPagado ?? p?.MontoPagado ?? 0;
  return v ?? 0;
}
function getPresuFecha(p) {
  return (
    p?.fechaCreacion ??
    p?.FechaCreacion ??
    p?.fechaPresupuesto ??
    p?.FechaPresupuesto ??
    "-"
  );
}
function getPresuDetalleTrabajo(p) {
  return p?.descripcionTrabajo ?? p?.DescripcionTrabajo ?? "-";
}
function getPresuNotasInternas(p) {
  return p?.notasInternas ?? p?.NotasInternas ?? p?.notas ?? p?.Notas ?? "-";
}

function parseApiErrorMessage(err, fallback) {
  if (err instanceof ApiError) return err.message || fallback;
  return err?.message || fallback;
}

export default function DetalleClientes({  open,  onClose,  clienteSel,  detalle,  loadingDetalle,  errorDetalle,  mostrarReservas,
  onToggleReservas,  reservas,  loadingReservas,  errorReservas,}) {
  const direcciones = detalle?.direcciones ?? detalle?.Direcciones ?? [];

  const [openPresu, setOpenPresu] = useState(false);
  const [loadingPresu, setLoadingPresu] = useState(false);
  const [savingPresu, setSavingPresu] = useState(false);
  const [errorPresu, setErrorPresu] = useState(null);
  const [successPresu, setSuccessPresu] = useState(null);

  const [presuSel, setPresuSel] = useState(null);
  const [reservaSel, setReservaSel] = useState(null);

  const [montoPagadoEdit, setMontoPagadoEdit] = useState("");
  const [montoPagadoOriginal, setMontoPagadoOriginal] = useState("");

  const [montoTotalNuevo, setMontoTotalNuevo] = useState("");
  const [detalleTrabajoNuevo, setDetalleTrabajoNuevo] = useState("");
  const [notasInternasNuevo, setNotasInternasNuevo] = useState("");
  const [montoPagadoNuevo, setMontoPagadoNuevo] = useState("");

  const resetPresuUi = () => {
    setLoadingPresu(false);
    setSavingPresu(false);
    setErrorPresu(null);
    setSuccessPresu(null);

    setPresuSel(null);
    setReservaSel(null);

    setMontoPagadoEdit("");
    setMontoPagadoOriginal("");

    setMontoTotalNuevo("");
    setDetalleTrabajoNuevo("");
    setNotasInternasNuevo("");
    setMontoPagadoNuevo("");
  };

  const handleClosePresu = () => {
    setOpenPresu(false);
    resetPresuUi();
  };

  const handleVerPresupuesto = async (r) => {
    setReservaSel(r);
    setOpenPresu(true);

    setLoadingPresu(true);
    setSavingPresu(false);
    setErrorPresu(null);
    setSuccessPresu(null);
    setPresuSel(null);

    // reset creación
    setMontoTotalNuevo("");
    setDetalleTrabajoNuevo("");
    setNotasInternasNuevo("");
    setMontoPagadoNuevo("");

    const idReserva = getReservaId(r);
    if (!idReserva) {
      setErrorPresu("No se pudo determinar el ID de la reserva.");
      setLoadingPresu(false);
      return;
    }

    const tienePresupuesto = getTienePresupuesto(r);
    if (!tienePresupuesto) {
      setPresuSel(null); 
      setLoadingPresu(false);
      return;
    }

    try {
      const data = await obtenerPresupuestoSegunReserva(idReserva);

      setPresuSel(data);

      if (data) {
        const mp = String(getPresuMontoPagado(data) ?? 0);
        setMontoPagadoEdit(mp);
        setMontoPagadoOriginal(mp);
      } else {
        setMontoPagadoEdit("");
        setMontoPagadoOriginal("");
      }
    } catch (err) {
      if (err?.status >= 500) {
        setPresuSel(null);
      }
      setErrorPresu(
        parseApiErrorMessage(err, "Error al obtener el presupuesto.")
      );
    } finally {
      setLoadingPresu(false);
    }
  };

  // ---------- VALIDACIONES ----------
  const montoPagadoNumero = Number(montoPagadoEdit);
  const montoPagadoValido =
    montoPagadoEdit !== "" &&
    !Number.isNaN(montoPagadoNumero) &&
    montoPagadoNumero >= 0;

  const hayCambiosMontoPagado = montoPagadoEdit !== montoPagadoOriginal;

  const montoTotalNumero = Number(montoTotalNuevo);
  const montoTotalValido =
    montoTotalNuevo !== "" &&
    !Number.isNaN(montoTotalNumero) &&
    montoTotalNumero > 0;

  const montoPagadoNuevoNumero =
    montoPagadoNuevo === "" ? 0 : Number(montoPagadoNuevo);
  const montoPagadoNuevoValido =
    montoPagadoNuevo === "" ||
    (!Number.isNaN(montoPagadoNuevoNumero) && montoPagadoNuevoNumero >= 0);

  const detalleTrabajoValido = detalleTrabajoNuevo.trim().length >= 3;

  const montoPagadoNoSuperaTotal =
    montoPagadoNuevo === "" || montoPagadoNuevoNumero <= montoTotalNumero;

  const puedeCrearPresupuesto =
    !!reservaSel &&
    montoTotalValido &&
    detalleTrabajoValido &&
    montoPagadoNuevoValido &&
    montoPagadoNoSuperaTotal;

  // ---------- ACCIONES ----------
  const handleCancelarEdicionMontoPagado = () => {
    setMontoPagadoEdit(montoPagadoOriginal);
    setSuccessPresu(null);
    setErrorPresu(null);
  };

  const handleGuardarMontoPagado = async () => {
    if (!reservaSel) return;
    const idReserva = getReservaId(reservaSel);
    if (!idReserva) {
      setErrorPresu("No se pudo determinar el ID de la reserva.");
      return;
    }
    if (!montoPagadoValido) {
      setErrorPresu("Ingresá un monto pagado válido (>= 0).");
      return;
    }

    setSavingPresu(true);
    setErrorPresu(null);
    setSuccessPresu(null);

    try {
      const actualizado = await actualizarMontoPagadoPresupuesto(
        idReserva,
        montoPagadoNumero
      );

      setPresuSel(actualizado);

      const mp = String(getPresuMontoPagado(actualizado) ?? montoPagadoNumero);
      setMontoPagadoEdit(mp);
      setMontoPagadoOriginal(mp);

      setSuccessPresu("Monto pagado actualizado");
    } catch (err) {
      setErrorPresu(
        parseApiErrorMessage(err, "Error al actualizar el monto pagado.")
      );
    } finally {
      setSavingPresu(false);
    }
  };

  const handleCrearPresupuesto = async () => {
    if (!reservaSel) return;

    const idReserva = getReservaId(reservaSel);
    if (!idReserva) {
      setErrorPresu("No se pudo determinar el ID de la reserva.");
      return;
    }

    if (!puedeCrearPresupuesto) {
      setErrorPresu(
        "Completá el monto total, el detalle del trabajo y revisá el monto pagado."
      );
      return;
    }

    setSavingPresu(true);
    setErrorPresu(null);
    setSuccessPresu(null);

    try {
      const dto = {
        montoTotal: Number(montoTotalNumero),
        montoPagado: Number(montoPagadoNuevoNumero || 0),
        descripcionTrabajo: detalleTrabajoNuevo.trim(),
        notasInternas: (notasInternasNuevo || "").trim(),
      };

      const creado = await crearPresupuestoParaReserva(idReserva, dto);

      setPresuSel(creado);

      const mp = String(getPresuMontoPagado(creado) ?? dto.montoPagado ?? 0);
      setMontoPagadoEdit(mp);
      setMontoPagadoOriginal(mp);

      setSuccessPresu("Presupuesto creado correctamente ");
    } catch (err) {
      setErrorPresu(parseApiErrorMessage(err, "Error al crear el presupuesto."));
    } finally {
      setSavingPresu(false);
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
                <Box sx={{ display: "flex", gap: 1, alignItems: "center", mb: 2 }}>
                  <CircularProgress size={18} />
                  <Typography variant="body2">Cargando detalle…</Typography>
                </Box>
              )}

              {errorDetalle && <Alert severity="error">{errorDetalle}</Alert>}

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

                    {errorReservas && <Alert severity="error">{errorReservas}</Alert>}

                    {!loadingReservas && !errorReservas && reservas.length === 0 && (
                      <Alert severity="info">No hay reservas.</Alert>
                    )}

                    {!loadingReservas && !errorReservas && reservas.length > 0 && (
                      <List>
                        {reservas.map((r, i) => {
                          const idR = getReservaId(r) ?? i;
                          const fecha = formatearFecha(getReservaFecha(r));
                          const estado = getReservaEstado(r);

                          const tiene = getTienePresupuesto(r);
                          const monto = getMontoPresupuestado(r);

                          return (
                            <Paper key={idR} variant="outlined" sx={{ p: 1.5, mb: 1.2 }}>

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
                                  tiene ? `Presupuesto: $ ${monto}` : "Sin presupuesto"
                                }
                                color={tiene ? "success" : "default"}
                              />

                              <Box sx={{ display: "flex", gap: 1 }}>
                                <Button
                                  variant="outlined"
                                  onClick={() => handleVerPresupuesto(r)}
                                >
                                  {tiene ? "Ver presupuesto" : "Agregar presupuesto"}
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

      <Dialog open={openPresu} onClose={handleClosePresu} fullWidth>
        <DialogTitle>
          Presupuesto{" "}

        </DialogTitle>

        <DialogContent dividers>
          {loadingPresu && (
            <Box sx={{ display: "flex", gap: 1, alignItems: "center" }}>
              <CircularProgress size={18} />
              <Typography variant="body2">Cargando presupuesto…</Typography>
            </Box>
          )}

          {errorPresu && (
            <Alert severity="error" sx={{ mb: 1 }}>
              {errorPresu}
            </Alert>
          )}
          {successPresu && (
            <Alert severity="success" sx={{ mb: 1 }}>
              {successPresu}
            </Alert>
          )}

          {/* CREAR */}
          {!loadingPresu && presuSel === null && (
            <Box sx={{ display: "grid", gap: 1 }}>
              <TextField
                label="Monto total"
                required
                type="number"
                value={montoTotalNuevo}
                onChange={(e) => {
                  setMontoTotalNuevo(e.target.value);
                  setSuccessPresu(null);
                  setErrorPresu(null);
                }}
                inputProps={{ min: 0, step: "0.01" }}
                error={montoTotalNuevo !== "" && !montoTotalValido}
                helperText={
                  montoTotalNuevo === ""
                    ? "Obligatorio"
                    : !montoTotalValido
                      ? "Debe ser mayor a 0"
                      : ""
                }
              />

              <TextField
                label="Detalle del trabajo"
                value={detalleTrabajoNuevo}
                onChange={(e) => {
                  setDetalleTrabajoNuevo(e.target.value);
                  setSuccessPresu(null);
                  setErrorPresu(null);
                }}
                multiline
                minRows={2}
                error={detalleTrabajoNuevo.length > 500}
                helperText={
                  detalleTrabajoNuevo.length > 500
                    ? "No puede superar los 500 caracteres"
                    : ""
                }
              />


              <TextField
                label="Notas internas"
                value={notasInternasNuevo}
                onChange={(e) => {
                  setNotasInternasNuevo(e.target.value);
                  setSuccessPresu(null);
                  setErrorPresu(null);
                }}
                multiline
                minRows={2}
              />

              <TextField
                label="Monto pagado (opcional)"
                type="number"
                value={montoPagadoNuevo}
                onChange={(e) => {
                  setMontoPagadoNuevo(e.target.value);
                  setSuccessPresu(null);
                  setErrorPresu(null);
                }}
                inputProps={{ min: 0, step: "0.01" }}
                error={
                  (montoPagadoNuevo !== "" && !montoPagadoNuevoValido) ||
                  (montoPagadoNuevo !== "" && !montoPagadoNoSuperaTotal)
                }
                helperText={
                  montoPagadoNuevo !== "" && !montoPagadoNuevoValido
                    ? "Debe ser >= 0"
                    : montoPagadoNuevo !== "" && !montoPagadoNoSuperaTotal
                      ? "No puede ser mayor al monto total"
                      : ""
                }
              />
            </Box>
          )}

          {/* VER/EDITAR */}
          {!loadingPresu && presuSel && (
            <Box sx={{ display: "grid", gap: 1 }}>
              <Typography>
                <b>Monto total:</b> {getPresuMontoTotal(presuSel)}
              </Typography>

              <Typography>
                <b>Detalle del trabajo:</b> {getPresuDetalleTrabajo(presuSel)}
              </Typography>

              <Typography>
                <b>Fecha:</b> {formatearFecha(getPresuFecha(presuSel))}
              </Typography>

              <Typography>
                <b>Notas internas:</b> {getPresuNotasInternas(presuSel)}
              </Typography>

              <Divider sx={{ my: 1 }} />

              <TextField
                label="Monto pagado"
                type="number"
                value={montoPagadoEdit}
                onChange={(e) => {
                  setMontoPagadoEdit(e.target.value);
                  setSuccessPresu(null);
                  setErrorPresu(null);
                }}
                inputProps={{ min: 0, step: "0.01" }}
                error={montoPagadoEdit !== "" && !montoPagadoValido}
              />
            </Box>
          )}
        </DialogContent>

        <DialogActions>
          <Button onClick={handleClosePresu} disabled={savingPresu}>
            Cerrar
          </Button>

          {!loadingPresu && presuSel === null && (
            <Button
              variant="contained"
              onClick={handleCrearPresupuesto}
              disabled={savingPresu || !puedeCrearPresupuesto}
            >
              {savingPresu ? "Creando…" : "Crear presupuesto"}
            </Button>
          )}

          {!loadingPresu && presuSel && (
            <>
              <Button
                variant="outlined"
                onClick={handleCancelarEdicionMontoPagado}
                disabled={savingPresu || !hayCambiosMontoPagado}
              >
                Cancelar cambios
              </Button>

              <Button
                variant="contained"
                onClick={handleGuardarMontoPagado}
                disabled={
                  savingPresu ||
                  loadingPresu ||
                  !hayCambiosMontoPagado ||
                  !montoPagadoValido
                }
              >
                {savingPresu ? "Guardando…" : "Guardar cambios"}
              </Button>
            </>
          )}
        </DialogActions>
      </Dialog>
    </>
  );
}