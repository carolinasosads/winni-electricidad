import { useEffect, useState } from "react";
import {  crearUsuarioComoAdmin,  buscarUsuariosAdmin,  getServiciosActivos,  getDireccionesUsuarioAdmin,} from "../../../services/authService";

import {
  crearReservaAdmin,  crearReservaHistoricaAdmin,  getReservasPorClienteAdmin,} from "../../../services/reservaService";

import {
  crearPresupuestoParaReserva,  obtenerPresupuestoSegunReserva,  actualizarMontoPagadoPresupuesto,} from "../../../services/presupuestoService";

import {  Box,  Paper,  Typography,  Grid,  RadioGroup,  FormControlLabel,  Radio,  TextField,  Button,  Divider,  Alert,  FormControl,
  InputLabel,  Select,  MenuItem,  FormHelperText,  Chip,  Dialog,  DialogTitle,  DialogContent,  DialogActions,} from "@mui/material";

import ApiError from "../../../services/ApiError";

export default function AdminCrearReservaPage() {
  const [modoCliente, setModoCliente] = useState("existente"); // "existente" | "nuevo"

  // Cliente existente
  const [busqueda, setBusqueda] = useState("");
  const [resultados, setResultados] = useState([]);
  const [loadingBusqueda, setLoadingBusqueda] = useState(false);

  // Cliente nuevo
  const [nuevoCliente, setNuevoCliente] = useState({
    nombreCompleto: "",
    email: "",
    telefono: "",
    calle: "",
    esquina: "",
    numero: "",
    apto: "",
  });
  const [loadingNuevoCliente, setLoadingNuevoCliente] = useState(false);

  const [clienteSeleccionado, setClienteSeleccionado] = useState(null);

  const [direccionesCliente, setDireccionesCliente] = useState([]);
  const [loadingDirecciones, setLoadingDirecciones] = useState(false);

  // Reserva
  const [modoReserva, setModoReserva] = useState("nueva"); // "nueva" | "existente"
  const [reserva, setReserva] = useState({
    servicios: [],
    fechaHora: "",
    comentarios: "",
    tipoTrabajo: "instalacion", // "instalacion" | "mantenimiento"
    idDireccion: "",
  });
  const [loadingReserva, setLoadingReserva] = useState(false);

  const [reservasCliente, setReservasCliente] = useState([]);
  const [loadingReservasCliente, setLoadingReservasCliente] = useState(false);
  const [reservaSeleccionada, setReservaSeleccionada] = useState(null);

  const [reservaParaPresupuesto, setReservaParaPresupuesto] = useState(null);
  const [presupuesto, setPresupuesto] = useState({
    montoTotal: "",
    montoPagado: "",
    descripcionTrabajo: "",
    notasInternas: "",
  });
  const [loadingPresupuesto, setLoadingPresupuesto] = useState(false);

  // Modal de detalle presupuesto
  const [openDetallePresupuesto, setOpenDetallePresupuesto] = useState(false);
  const [detallePresupuesto, setDetallePresupuesto] = useState(null);
  const [loadingDetallePresupuesto, setLoadingDetallePresupuesto] =
    useState(false);

  //para editar el monto pagado 
  const [editMontoPagado, setEditMontoPagado] = useState("");
  const [loadingEditMontoPagado, setLoadingEditMontoPagado] = useState(false);
  const [errorEditMontoPagado, setErrorEditMontoPagado] = useState("");
  const [successEditMontoPagado, setSuccessEditMontoPagado] = useState("");

  const [serviciosOpts, setServiciosOpts] = useState([]);
  const [loadingServicios, setLoadingServicios] = useState(false);
  const [errorServicios, setErrorServicios] = useState("");
  const [errorServiciosSelect, setErrorServiciosSelect] = useState("");

  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const resetPresupuesto = () => {
    setPresupuesto({
      montoTotal: "",
      montoPagado: "",
      descripcionTrabajo: "",
      notasInternas: "",
    });
  };

  const getMontoPresupuestado = (r) =>
    r?.montoPresupuestado ?? r?.MontoPresupuestado ?? 0;

  const getTienePresupuesto = (r) =>
    r?.tienePresupuesto ?? r?.TienePresupuesto ?? false;

  const parseApiErrorMessage = (e, fallback) => {
    let msg = fallback;

    if (e instanceof ApiError) {
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
        msg = e.message || fallback;
      }
    } else if (e?.message) {
      msg = e.message;
    }

    return msg;
  };

  const normalizeToArray = (data) => {
    if (Array.isArray(data)) return data;
    if (Array.isArray(data?.reservas)) return data.reservas;
    if (Array.isArray(data?.$values)) return data.$values;
    return [];
  };

  const getClienteId = (cliente) => {
    return (
      cliente?.id ??
      cliente?.idUsuario ??
      cliente?.idUsuarioCliente ??
      cliente?.idCliente ??
      cliente?.idUsuarioBase ??
      null
    );
  };

  const labelById = (idStr) => {
    const found = serviciosOpts.find((s) => String(s.id) === String(idStr));
    return found?.label ?? `Servicio ${idStr}`;
  };

  const labelDireccion = (d) => {
    const calle = d.calle ?? "";
    const esquina = d.esquina ?? "";
    const numero = d.numero ? ` ${d.numero}` : "";
    const apto = d.apto ? `, Apto ${d.apto}` : "";
    return `${calle} y ${esquina}${numero}${apto}`;
  };

  // Carga servicios
  useEffect(() => {
    const ac = new AbortController();

    async function cargarServicios() {
      try {
        setLoadingServicios(true);
        setErrorServicios("");
        const data = await getServiciosActivos(ac.signal);

        const opts = (data ?? []).map((s) => ({
          id: s.id,
          label: s.titulo,
        }));
        setServiciosOpts(opts);
      } catch (e) {
        if (e?.name === "AbortError") return;
        console.error(e);
        setErrorServicios(e?.message ?? "Error al cargar servicios.");
      } finally {
        setLoadingServicios(false);
      }
    }

    cargarServicios();
    return () => ac.abort();
  }, []);

  const handleChangeModoCliente = (event) => {
    setModoCliente(event.target.value);

    setClienteSeleccionado(null);
    setResultados([]);
    setReservasCliente([]);
    setReservaSeleccionada(null);

    setDireccionesCliente([]);

    setModoReserva("nueva");
    setReservaParaPresupuesto(null);

    setReserva({
      servicios: [],
      fechaHora: "",
      comentarios: "",
      tipoTrabajo: "instalacion",
      idDireccion: "",
    });

    resetPresupuesto();
    setError("");
    setSuccess("");
  };

  const handleChangeModoReserva = (event) => {
    const nextMode = event.target.value;
    setModoReserva(nextMode);

    // reseteos
    setReservaSeleccionada(null);
    setReservaParaPresupuesto(null);
    resetPresupuesto();
    setError("");
    setSuccess("");
  };

  const handleBuscarClientes = async () => {
    setError("");
    setSuccess("");
    setResultados([]);

    if (!busqueda || busqueda.trim().length < 2) {
      setError("Ingrese al menos 2 caracteres para buscar.");
      return;
    }

    try {
      setLoadingBusqueda(true);
      const data = await buscarUsuariosAdmin(busqueda.trim());
      setResultados(Array.isArray(data) ? data : []);
    } catch (err) {
      console.error(err);
      setError(parseApiErrorMessage(err, "Error al buscar clientes."));
    } finally {
      setLoadingBusqueda(false);
    }
  };

  const cargarReservasDeCliente = async (clienteId) => {
    try {
      setLoadingReservasCliente(true);
      setReservasCliente([]);

      const data = await getReservasPorClienteAdmin(clienteId);

      const list = normalizeToArray(data);
      setReservasCliente(list);
    } catch (err) {
      console.error(err);
      setError(
        parseApiErrorMessage(err, "Error al cargar las reservas del cliente.")
      );
    } finally {
      setLoadingReservasCliente(false);
    }
  };

  const cargarDireccionesDeCliente = async (clienteId) => {
    const ac = new AbortController();
    try {
      setLoadingDirecciones(true);
      setDireccionesCliente([]);
      const dirs = await getDireccionesUsuarioAdmin(clienteId, ac.signal);
      setDireccionesCliente(Array.isArray(dirs) ? dirs : []);
    } catch (err) {
      console.error(err);
      setError(
        parseApiErrorMessage(err, "Error al cargar direcciones del cliente.")
      );
    } finally {
      setLoadingDirecciones(false);
    }
  };

  const handleSeleccionarCliente = async (cliente) => {
    const clienteId = getClienteId(cliente);

    if (!clienteId) {
      setError("No se pudo determinar el ID del cliente.");
      return;
    }

    const clienteNormalizado = {
      ...cliente,
      id: clienteId,
    };

    setClienteSeleccionado(clienteNormalizado);
    setSuccess(`Cliente seleccionado: ${cliente.nombreCompleto || cliente.email}`);
    setError("");

    setReservaSeleccionada(null);
    setModoReserva("nueva");

    setReservaParaPresupuesto(null);
    resetPresupuesto();

    setReserva((prev) => ({ ...prev, idDireccion: "" }));
    setDireccionesCliente([]);

    await Promise.all([
      cargarReservasDeCliente(clienteId),
      cargarDireccionesDeCliente(clienteId),
    ]);
  };

  // Nuevo cliente
  const handleChangeNuevoCliente = (field) => (event) => {
    setNuevoCliente((prev) => ({
      ...prev,
      [field]: event.target.value,
    }));
  };

  const handleCrearNuevoCliente = async () => {
    setError("");
    setSuccess("");

    if (!nuevoCliente.nombreCompleto || !nuevoCliente.email) {
      setError("Nombre completo y email son obligatorios para crear un cliente.");
      return;
    }

    if (!nuevoCliente.calle.trim() || !nuevoCliente.esquina.trim()) {
      setError("La dirección debe tener al menos calle y esquina.");
      return;
    }

    try {
      setLoadingNuevoCliente(true);

      const dto = {
        nombreCompleto: nuevoCliente.nombreCompleto,
        email: nuevoCliente.email,
        telefono: nuevoCliente.telefono,
        direcciones: [
          {
            calle: nuevoCliente.calle.trim(),
            esquina: nuevoCliente.esquina.trim(),
            numero: (nuevoCliente.numero || "").trim() || null,
            apto: (nuevoCliente.apto || "").trim() || null,
          },
        ],
      };

      const data = await crearUsuarioComoAdmin(dto);

      const cliente = {
        id: data?.id ?? data?.idUsuario ?? data?.idCliente ?? null,
        nombreCompleto: dto.nombreCompleto,
        email: data?.email ?? dto.email,
        telefono: dto.telefono,
      };

      setClienteSeleccionado(cliente);
      setSuccess(`Cliente creado y seleccionado: ${cliente.nombreCompleto}`);
      setModoCliente("existente");
      setResultados([]);

      setReservaSeleccionada(null);
      setModoReserva("nueva");
      setReservasCliente([]);
      setReservaParaPresupuesto(null);

      resetPresupuesto();

      if (cliente.id) {
        await Promise.all([
          cargarReservasDeCliente(cliente.id),
          cargarDireccionesDeCliente(cliente.id),
        ]);
      }
    } catch (err) {
      console.error(err);
      setError(parseApiErrorMessage(err, "Error al crear el cliente."));
    } finally {
      setLoadingNuevoCliente(false);
    }
  };

  // Reserva
  const handleChangeReserva = (field) => (event) => {
    setReserva((prev) => ({
      ...prev,
      [field]: event.target.value,
    }));
  };

  const handleChangeServicios = (event) => {
    const value = event.target.value;
    const next = Array.isArray(value) ? value : String(value).split(",");
    setReserva((prev) => ({ ...prev, servicios: next }));

    if (next.length === 0)
      setErrorServiciosSelect("Tenés que elegir al menos un servicio.");
    else setErrorServiciosSelect("");
  };

  const handleCrearReserva = async () => {
    setError("");
    setSuccess("");

    if (!clienteSeleccionado?.id) {
      setError("Debe seleccionar o crear un cliente primero.");
      return;
    }

    if (modoReserva === "existente") {
      if (!reservaSeleccionada) {
        setError("Debe seleccionar una reserva existente.");
        return;
      }

      setReservaParaPresupuesto(reservaSeleccionada);

      if (getTienePresupuesto(reservaSeleccionada)) {
        const montoP = getMontoPresupuestado(reservaSeleccionada);
        setPresupuesto((prev) => ({
          ...prev,
          montoTotal: String(montoP),
          montoPagado: "",
          descripcionTrabajo: "",
          notasInternas: "",
        }));
        setSuccess("Esta reserva ya tiene presupuesto.");
      } else {
        resetPresupuesto();
        setSuccess("Reserva seleccionada. Ahora podés cargar el presupuesto.");
      }
      return;
    }

    if (!reserva.servicios || reserva.servicios.length === 0) {
      setErrorServiciosSelect("Tenés que elegir al menos un servicio.");
      setError("Servicio(s) son obligatorios.");
      return;
    }

    if (!reserva.fechaHora) {
      setError("Fecha/hora es obligatoria.");
      return;
    }

    if (!reserva.idDireccion) {
      setError("Debe seleccionar una dirección.");
      return;
    }

    try {
      setLoadingReserva(true);

      const reservaDto = {
        fechaReserva: reserva.fechaHora,
        comentario: reserva.comentarios?.trim() || null,
        idDireccion: Number(reserva.idDireccion),
        idServicios: reserva.servicios.map(Number),
        tipoServicio: reserva.tipoTrabajo === "instalacion" ? 1 : 2,
      };

      const fechaSeleccionada = new Date(reservaDto.fechaReserva);
      const ahora = new Date();
      const esHistorica = fechaSeleccionada <= ahora;

      const reservaCreada = esHistorica
        ? await crearReservaHistoricaAdmin(clienteSeleccionado.id, reservaDto)
        : await crearReservaAdmin(clienteSeleccionado.id, reservaDto);

      setSuccess("Reserva creada correctamente.");
      setReserva((prev) => ({
        ...prev,
        fechaHora: "",
        comentarios: "",
      }));

      if (reservaCreada?.idReserva) {
        setReservaParaPresupuesto(reservaCreada);
        setReservaSeleccionada(reservaCreada);
      } else {
        await cargarReservasDeCliente(clienteSeleccionado.id);
      }

      resetPresupuesto();
    } catch (err) {
      console.error(err);
      setError(parseApiErrorMessage(err, "Error al crear la reserva."));
    } finally {
      setLoadingReserva(false);
    }
  };

  // Presupuesto
  const handleChangePresupuesto = (field) => (event) => {
    setPresupuesto((prev) => ({
      ...prev,
      [field]: event.target.value,
    }));
  };

  const handleCrearPresupuesto = async () => {
    setError("");
    setSuccess("");

    if (!reservaParaPresupuesto?.idReserva) {
      setError("Debe seleccionar una reserva para cargar el presupuesto.");
      return;
    }

    if (!presupuesto.montoTotal) {
      setError("El monto total es obligatorio.");
      return;
    }

    try {
      setLoadingPresupuesto(true);

      const dto = {
        montoTotal: Number(presupuesto.montoTotal),
        montoPagado: presupuesto.montoPagado ? Number(presupuesto.montoPagado) : 0,
        descripcionTrabajo: presupuesto.descripcionTrabajo,
        notasInternas: presupuesto.notasInternas,
      };

      await crearPresupuestoParaReserva(reservaParaPresupuesto.idReserva, dto);

      setSuccess("Presupuesto creado correctamente.");
      resetPresupuesto();

      if (clienteSeleccionado?.id) {
        await cargarReservasDeCliente(clienteSeleccionado.id);
      }
    } catch (err) {
      console.error(err);
      setError(parseApiErrorMessage(err, "Error al crear el presupuesto."));
    } finally {
      setLoadingPresupuesto(false);
    }
  };

  const handleVerDetallePresupuesto = async (res) => {
    setError("");
    setSuccess("");

    try {
      setLoadingDetallePresupuesto(true);
      setDetallePresupuesto(null);
      setErrorEditMontoPagado("");
      setSuccessEditMontoPagado("");

      const token = localStorage.getItem("token");

      const data = await obtenerPresupuestoSegunReserva(res.idReserva, token);

      setDetallePresupuesto(data);

      const mp = data?.montoPagado ?? data?.MontoPagado ?? 0;
      setEditMontoPagado(String(mp));

      setOpenDetallePresupuesto(true);
    } catch (err) {
      console.error(err);
      setError(
        parseApiErrorMessage(err, "Error al obtener el detalle del presupuesto.")
      );
    } finally {
      setLoadingDetallePresupuesto(false);
    }
  };

  const handleGuardarMontoPagado = async () => {
    setErrorEditMontoPagado("");
    setSuccessEditMontoPagado("");

    if (!detallePresupuesto) {
      setErrorEditMontoPagado("No hay detalle de presupuesto cargado.");
      return;
    }

    const idReserva =
      detallePresupuesto?.idReserva ?? detallePresupuesto?.IdReserva ?? null;

    if (!idReserva) {
      setErrorEditMontoPagado(
        "No se pudo determinar el idReserva del presupuesto."
      );
      return;
    }

    const monto = Number(editMontoPagado);

    if (Number.isNaN(monto)) {
      setErrorEditMontoPagado("El monto pagado debe ser un número.");
      return;
    }

    if (monto < 0) {
      setErrorEditMontoPagado("El monto pagado no puede ser negativo.");
      return;
    }

    const montoTotal = Number(
      detallePresupuesto?.montoTotal ?? detallePresupuesto?.MontoTotal ?? 0
    );

    if (montoTotal > 0 && monto > montoTotal) {
      setErrorEditMontoPagado(
        "El monto pagado no puede ser mayor al monto total."
      );
      return;
    }

    try {
      setLoadingEditMontoPagado(true);

      const token = localStorage.getItem("token");
      const updated = await actualizarMontoPagadoPresupuesto(
        idReserva,
        monto,
        token
      );

      setDetallePresupuesto((prev) => ({
        ...(prev || {}),
        ...updated,
      }));

      setSuccessEditMontoPagado("Monto pagado actualizado.");

      if (clienteSeleccionado?.id) {
        await cargarReservasDeCliente(clienteSeleccionado.id);
      }
    } catch (err) {
      console.error(err);
      setErrorEditMontoPagado(
        parseApiErrorMessage(err, "Error al actualizar el monto pagado.")
      );
    } finally {
      setLoadingEditMontoPagado(false);
    }
  };

  const reservaYaTienePresupuesto = getTienePresupuesto(reservaParaPresupuesto);

  const getStepFromMessage = (msg) => {
    const m = String(msg || "").toLowerCase();

    if (m.includes("presupuesto") || m.includes("monto")) return 3;

    if (
      m.includes("reserva") ||
      m.includes("servicio") ||
      m.includes("dirección") ||
      m.includes("fecha") ||
      m.includes("hora")
    )
      return 2;

    if (m.includes("cliente") || m.includes("buscar") || m.includes("búsq"))
      return 1;

    if (reservaParaPresupuesto) return 3;
    if (clienteSeleccionado) return 2;
    return 1;
  };

  const stepError = error ? getStepFromMessage(error) : null;
  const stepSuccess = success ? getStepFromMessage(success) : null;

  const RenderAlertsForStep = ({ step }) => (
    <>
      {error && stepError === step && (
        <Box sx={{ mb: 2 }}>
          <Alert severity="error">{error}</Alert>
        </Box>
      )}

      {success && stepSuccess === step && (
        <Box sx={{ mb: 2 }}>
          <Alert severity="success">{success}</Alert>
        </Box>
      )}
    </>
  );

  return (
    <Box sx={{ p: 3 }}>
      <Typography variant="h5" gutterBottom>
        Gestionar Presupuesto
      </Typography>

      {/* Paso 1: Cliente */}
      <Paper sx={{ p: 2, mb: 3 }}>
        <Typography variant="h6" gutterBottom>
          1. Cliente
        </Typography>

        <RenderAlertsForStep step={1} />

        <RadioGroup
          row
          value={modoCliente}
          onChange={handleChangeModoCliente}
          sx={{ mb: 2 }}
        >
          <FormControlLabel
            value="existente"
            control={<Radio />}
            label="Cliente existente"
          />
          <FormControlLabel
            value="nuevo"
            control={<Radio />}
            label="Nuevo cliente"
          />
        </RadioGroup>

        {modoCliente === "existente" && (
          <Box sx={{ mt: 1 }}>
            <Grid container spacing={2} alignItems="center">
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  label="Buscar por nombre, email o teléfono"
                  value={busqueda}
                  onChange={(e) => setBusqueda(e.target.value)}
                />
              </Grid>
              <Grid item xs={12} md="auto">
                <Button
                  variant="contained"
                  onClick={handleBuscarClientes}
                  disabled={loadingBusqueda}
                >
                  {loadingBusqueda ? "Buscando..." : "Buscar"}
                </Button>
              </Grid>
            </Grid>

            <Box sx={{ mt: 2 }}>
              {resultados.length === 0 && !loadingBusqueda && (
                <Typography variant="body2" color="text.secondary">
                  No hay resultados aún. Realiza una búsqueda.
                </Typography>
              )}

              {resultados.map((cliente) => {
                const id = getClienteId(cliente);
                return (
                  <Paper
                    key={id ?? `${cliente.email}-${cliente.nombreCompleto}`}
                    sx={{
                      p: 1.5,
                      mb: 1,
                      display: "flex",
                      justifyContent: "space-between",
                      alignItems: "center",
                    }}
                    variant={
                      clienteSeleccionado?.id === id ? "outlined" : "elevation"
                    }
                  >
                    <Box>
                      <Typography variant="subtitle1">
                        {cliente.nombreCompleto || "Sin nombre"}
                      </Typography>
                      <Typography variant="body2" color="text.secondary">
                        {cliente.email}{" "}
                        {cliente.telefono && `• ${cliente.telefono}`}
                      </Typography>
                    </Box>
                    <Button
                      size="small"
                      variant="text"
                      onClick={() => handleSeleccionarCliente(cliente)}
                    >
                      SELECCIONAR CLIENTE
                    </Button>
                  </Paper>
                );
              })}
            </Box>
          </Box>
        )}

        {modoCliente === "nuevo" && (
          <Box sx={{ mt: 1 }}>
            <Grid container spacing={2}>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  label="Nombre completo"
                  value={nuevoCliente.nombreCompleto}
                  onChange={handleChangeNuevoCliente("nombreCompleto")}
                  required
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  label="Email"
                  value={nuevoCliente.email}
                  onChange={handleChangeNuevoCliente("email")}
                  required
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  label="Teléfono"
                  value={nuevoCliente.telefono}
                  onChange={handleChangeNuevoCliente("telefono")}
                />
              </Grid>

              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  label="Calle"
                  value={nuevoCliente.calle}
                  onChange={handleChangeNuevoCliente("calle")}
                  required
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  label="Esquina"
                  value={nuevoCliente.esquina}
                  onChange={handleChangeNuevoCliente("esquina")}
                  required
                />
              </Grid>
              <Grid item xs={12} md={3}>
                <TextField
                  fullWidth
                  label="Número (opcional)"
                  value={nuevoCliente.numero}
                  onChange={handleChangeNuevoCliente("numero")}
                />
              </Grid>
              <Grid item xs={12} md={3}>
                <TextField
                  fullWidth
                  label="Piso / Apto (opcional)"
                  value={nuevoCliente.apto}
                  onChange={handleChangeNuevoCliente("apto")}
                />
              </Grid>
            </Grid>

            <Box sx={{ mt: 2 }}>
              <Button
                variant="contained"
                onClick={handleCrearNuevoCliente}
                disabled={loadingNuevoCliente}
              >
                {loadingNuevoCliente ? "Creando..." : "Crear cliente y usarlo"}
              </Button>
            </Box>
          </Box>
        )}

        {clienteSeleccionado && (
          <>
            <Divider sx={{ my: 2 }} />
            <Typography variant="body2" color="text.secondary">
              Cliente actual:{" "}
              <strong>
                {clienteSeleccionado.nombreCompleto || clienteSeleccionado.email}
              </strong>
            </Typography>
          </>
        )}
      </Paper>

      {/* Paso 2: Reserva */}
      <Paper sx={{ p: 2 }}>
        <Typography variant="h6" gutterBottom>
          2. Datos de la reserva
        </Typography>

        <RenderAlertsForStep step={2} />

        {!clienteSeleccionado && (
          <Alert severity="info" sx={{ mb: 2 }}>
            Primero seleccioná o creá un cliente para poder cargar la reserva.
          </Alert>
        )}

        {clienteSeleccionado && (
          <>
            <RadioGroup
              row
              value={modoReserva}
              onChange={handleChangeModoReserva}
              sx={{ mb: 2 }}
            >
              <FormControlLabel
                value="nueva"
                control={<Radio />}
                label="Crear nueva reserva"
              />
              <FormControlLabel
                value="existente"
                control={<Radio />}
                label="Usar reserva existente"
              />
            </RadioGroup>

            {/* Nueva Reserva */}
            {modoReserva === "nueva" && (
              <>
                <Grid container spacing={2} sx={{ mb: 2 }}>
                  <Grid item xs={12} md={6}>
                    <FormControl
                      fullWidth
                      disabled={loadingServicios}
                      error={!!errorServiciosSelect || !!errorServicios}
                    >
                      <InputLabel id="servicios-label">Servicios</InputLabel>
                      <Select
                        labelId="servicios-label"
                        label="Servicios"
                        multiple
                        value={reserva.servicios}
                        onChange={handleChangeServicios}
                        renderValue={(selected) => (
                          <Box sx={{ display: "flex", gap: 1, flexWrap: "wrap" }}>
                            {selected.map((idStr) => (
                              <Chip
                                key={idStr}
                                label={labelById(idStr)}
                                size="small"
                              />
                            ))}
                          </Box>
                        )}
                      >
                        {serviciosOpts.map((s) => (
                          <MenuItem key={s.id} value={String(s.id)}>
                            {s.label}
                          </MenuItem>
                        ))}
                      </Select>

                      {!!errorServiciosSelect && (
                        <FormHelperText>{errorServiciosSelect}</FormHelperText>
                      )}
                      {!errorServiciosSelect && !!errorServicios && (
                        <FormHelperText>{errorServicios}</FormHelperText>
                      )}
                      {!errorServiciosSelect && !errorServicios && (
                        <FormHelperText>
                          Elegí uno o más servicios (mínimo 1).
                        </FormHelperText>
                      )}
                    </FormControl>
                  </Grid>

                  <Grid item xs={12} md={6}>
                    <TextField
                      fullWidth
                      type="datetime-local"
                      label="Fecha y hora"
                      value={reserva.fechaHora}
                      onChange={handleChangeReserva("fechaHora")}
                      InputLabelProps={{ shrink: true }}
                    />
                  </Grid>
                </Grid>

                <Grid container spacing={2} sx={{ mb: 2 }}>
                  <Grid item xs={12} md={6}>
                    <FormControl fullWidth>
                      <InputLabel id="tipo-trabajo-label">
                        Tipo de trabajo
                      </InputLabel>
                      <Select
                        labelId="tipo-trabajo-label"
                        label="Tipo de trabajo"
                        value={reserva.tipoTrabajo}
                        onChange={handleChangeReserva("tipoTrabajo")}
                      >
                        <MenuItem value="instalacion">Instalación</MenuItem>
                        <MenuItem value="mantenimiento">Mantenimiento</MenuItem>
                      </Select>
                    </FormControl>
                  </Grid>

                  <Grid item xs={12} md={6}>
                    <FormControl
                      fullWidth
                      disabled={loadingDirecciones}
                      error={!loadingDirecciones && direccionesCliente.length === 0}
                    >
                      <InputLabel id="direccion-label">Dirección</InputLabel>
                      <Select
                        labelId="direccion-label"
                        label="Dirección"
                        value={reserva.idDireccion}
                        onChange={(e) =>
                          setReserva((prev) => ({
                            ...prev,
                            idDireccion: e.target.value,
                          }))
                        }
                      >
                        {direccionesCliente.map((d) => (
                          <MenuItem key={d.id} value={String(d.id)}>
                            {labelDireccion(d)}
                          </MenuItem>
                        ))}
                      </Select>

                      <FormHelperText>
                        {loadingDirecciones
                          ? "Cargando direcciones..."
                          : direccionesCliente.length === 0
                          ? "El cliente no tiene direcciones cargadas."
                          : "Seleccioná una dirección existente del cliente."}
                      </FormHelperText>
                    </FormControl>
                  </Grid>
                </Grid>

                <Grid container spacing={2}>
                  <Grid item xs={12}>
                    <TextField
                      fullWidth
                      label="Comentarios (opcional)"
                      multiline
                      rows={4}
                      value={reserva.comentarios}
                      onChange={handleChangeReserva("comentarios")}
                    />
                  </Grid>
                </Grid>
              </>
            )}

            {/* Reserva existente */}
            {modoReserva === "existente" && (
              <Box sx={{ mt: 1 }}>
                {loadingReservasCliente && (
                  <Typography variant="body2" color="text.secondary">
                    Cargando reservas del cliente...
                  </Typography>
                )}

                {!loadingReservasCliente && reservasCliente.length === 0 && (
                  <Typography variant="body2" color="text.secondary">
                    Este cliente no tiene reservas registradas.
                  </Typography>
                )}

                {!loadingReservasCliente &&
                  reservasCliente.map((res) => {
                    const tieneP = getTienePresupuesto(res);
                    const montoP = getMontoPresupuestado(res);

                    return (
                      <Paper
                        key={res.idReserva}
                        sx={{
                          p: 1.5,
                          mb: 1,
                          display: "flex",
                          justifyContent: "space-between",
                          alignItems: "center",
                          border:
                            reservaSeleccionada?.idReserva === res.idReserva
                              ? "2px solid #1976d2"
                              : "1px solid #e0e0e0",
                        }}
                      >
                        <Box>
                          <Typography variant="subtitle2">
                            {res.nombreServicio || "Servicio"}
                          </Typography>

                          <Typography variant="body2" color="text.secondary">
                            {res.fechaReserva &&
                              new Date(res.fechaReserva).toLocaleString("es-UY", {
                                day: "2-digit",
                                month: "2-digit",
                                year: "numeric",
                                hour: "2-digit",
                                minute: "2-digit",
                              })}
                          </Typography>

                          <Typography variant="body2" color="text.secondary">
                            Estado: {res.estado || "Sin estado"}
                          </Typography>

                          {res.direccionDescripcion && (
                            <Typography variant="body2" color="text.secondary">
                              Dirección: {res.direccionDescripcion}
                            </Typography>
                          )}

                          <Chip
                            size="small"
                            sx={{ mt: 1 }}
                            label={
                              tieneP ? `Presupuesto: $ ${montoP}` : "Sin presupuesto"
                            }
                            color={tieneP ? "success" : "default"}
                          />
                        </Box>

                        <Button
                          size="small"
                          variant="contained"
                          disabled={loadingDetallePresupuesto}
                          onClick={() => {
                            setError("");
                            setSuccess("");
                            setReservaSeleccionada(res);
                            setReservaParaPresupuesto(res);

                            if (tieneP) {
                              handleVerDetallePresupuesto(res);
                            } else {
                              resetPresupuesto();
                              setSuccess(
                                "Reserva seleccionada. Ahora podés cargar el presupuesto."
                              );
                            }
                          }}
                        >
                          {tieneP ? "VER DETALLE" : "AGREGAR PRESUPUESTO"}
                        </Button>
                      </Paper>
                    );
                  })}
              </Box>
            )}

            <Box sx={{ mt: 2 }}>
              <Button
                variant="contained"
                onClick={handleCrearReserva}
                disabled={loadingReserva}
              >
                {loadingReserva
                  ? modoReserva === "nueva"
                    ? "Creando reserva..."
                    : "Confirmando..."
                  : modoReserva === "nueva"
                  ? "Crear reserva"
                  : "Confirmar selección"}
              </Button>
            </Box>
          </>
        )}
      </Paper>

      {/* Paso 3: Presupuesto */}
      {clienteSeleccionado && reservaParaPresupuesto && (
        <Paper sx={{ p: 2, mt: 3 }}>
          <Typography variant="h6" gutterBottom>
            3. Presupuesto para la reserva
          </Typography>

          <RenderAlertsForStep step={3} />

          {reservaYaTienePresupuesto && (
            <Alert severity="info" sx={{ mb: 2 }}>
              Esta reserva ya tiene presupuesto.
            </Alert>
          )}

          <Grid container spacing={2} sx={{ mb: 2 }}>
            <Grid item xs={12} md={6}>
              <TextField
                fullWidth
                size="small"
                label="Monto total"
                type="number"
                value={presupuesto.montoTotal}
                onChange={handleChangePresupuesto("montoTotal")}
                inputProps={{ min: 0 }}
                disabled={reservaYaTienePresupuesto}
              />
            </Grid>

            <Grid item xs={12} md={6}>
              <TextField
                fullWidth
                size="small"
                label="Monto pagado (opcional)"
                type="number"
                value={presupuesto.montoPagado}
                onChange={handleChangePresupuesto("montoPagado")}
                inputProps={{ min: 0 }}
                disabled={reservaYaTienePresupuesto}
              />
            </Grid>
          </Grid>

          <Grid container spacing={2}>
            <Grid item xs={12} md={6}>
              <TextField
                fullWidth
                label="Descripción del trabajo"
                multiline
                minRows={4}
                value={presupuesto.descripcionTrabajo}
                onChange={handleChangePresupuesto("descripcionTrabajo")}
                disabled={reservaYaTienePresupuesto}
              />
            </Grid>

            <Grid item xs={12} md={4}>
              <TextField
                fullWidth
                label="Notas internas"
                multiline
                minRows={4}
                value={presupuesto.notasInternas}
                onChange={handleChangePresupuesto("notasInternas")}
                disabled={reservaYaTienePresupuesto}
              />
            </Grid>
          </Grid>

          {!reservaYaTienePresupuesto && (
            <Box sx={{ mt: 2 }}>
              <Button
                variant="contained"
                onClick={handleCrearPresupuesto}
                disabled={loadingPresupuesto}
              >
                {loadingPresupuesto ? "Creando presupuesto..." : "Crear presupuesto"}
              </Button>
            </Box>
          )}
        </Paper>
      )}

      {/* Modal de detalle del presupuesto y editr el monto pagado */}
      <Dialog
        open={openDetallePresupuesto}
        onClose={() => {
          setOpenDetallePresupuesto(false);
          setErrorEditMontoPagado("");
          setSuccessEditMontoPagado("");
        }}
        fullWidth
        maxWidth="sm"
      >
        <DialogTitle>Detalle del presupuesto</DialogTitle>

        <DialogContent dividers>
          {loadingDetallePresupuesto && (
            <Typography variant="body2" color="text.secondary">
              Cargando detalle...
            </Typography>
          )}

          {!loadingDetallePresupuesto && !detallePresupuesto && (
            <Alert severity="warning">
              No se pudo cargar el detalle del presupuesto.
            </Alert>
          )}

          {!loadingDetallePresupuesto && detallePresupuesto && (
            <Box sx={{ display: "flex", flexDirection: "column", gap: 1 }}>
              <Typography>
                <strong>Monto total:</strong> ${" "}
                {detallePresupuesto?.montoTotal ?? detallePresupuesto?.MontoTotal}
              </Typography>

              <TextField
                fullWidth
                size="small"
                label="Monto pagado"
                type="number"
                value={editMontoPagado}
                onChange={(e) => setEditMontoPagado(e.target.value)}
                inputProps={{ min: 0 }}
                sx={{ mt: 1 }}
              />

              {errorEditMontoPagado && (
                <Box sx={{ mt: 1 }}>
                  <Alert severity="error">{errorEditMontoPagado}</Alert>
                </Box>
              )}

              {successEditMontoPagado && (
                <Box sx={{ mt: 1 }}>
                  <Alert severity="success">{successEditMontoPagado}</Alert>
                </Box>
              )}

              {!!(detallePresupuesto?.descripcionTrabajo ?? detallePresupuesto?.DescripcionTrabajo) && (
                <Box sx={{ mt: 1 }}>
                  <Typography variant="subtitle2">Descripción del trabajo</Typography>
                  <Typography variant="body2" color="text.secondary">
                    {detallePresupuesto?.descripcionTrabajo ??
                      detallePresupuesto?.DescripcionTrabajo}
                  </Typography>
                </Box>
              )}

              {!!(detallePresupuesto?.notasInternas ?? detallePresupuesto?.NotasInternas) && (
                <Box sx={{ mt: 1 }}>
                  <Typography variant="subtitle2">Notas internas</Typography>
                  <Typography variant="body2" color="text.secondary">
                    {detallePresupuesto?.notasInternas ??
                      detallePresupuesto?.NotasInternas}
                  </Typography>
                </Box>
              )}

              {!!(detallePresupuesto?.notas ?? detallePresupuesto?.Notas) && (
                <Box sx={{ mt: 1 }}>
                  <Typography variant="subtitle2">Notas</Typography>
                  <Typography variant="body2" color="text.secondary">
                    {detallePresupuesto?.notas ?? detallePresupuesto?.Notas}
                  </Typography>
                </Box>
              )}
            </Box>
          )}
        </DialogContent>

        <DialogActions>
          <Button
            onClick={() => {
              setOpenDetallePresupuesto(false);
              setErrorEditMontoPagado("");
              setSuccessEditMontoPagado("");
            }}
          >
            Cerrar
          </Button>

          <Button
            variant="contained"
            onClick={handleGuardarMontoPagado}
            disabled={
              loadingEditMontoPagado ||
              loadingDetallePresupuesto ||
              !detallePresupuesto
            }
          >
            {loadingEditMontoPagado ? "Guardando..." : "Guardar monto pagado"}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}