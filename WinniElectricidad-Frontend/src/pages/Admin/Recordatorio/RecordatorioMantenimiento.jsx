import { useEffect, useMemo, useState } from "react";
import {
  Box,
  Button,
  TextField,
  Typography,
  MenuItem,
  RadioGroup,
  FormControlLabel,
  Radio,
  Divider,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Chip,
  Alert,
  CircularProgress,
  Autocomplete
} from "@mui/material";
import { createFilterOptions } from "@mui/material/Autocomplete";

import { getServiciosActivos } from "../../../services/servicioService";
import {
  listarClientesAdmin,
  listarClientesAdminSegunServicio
} from "../../../services/clienteService";

import { enviarRecordatorios } from "../../../services/recordatorioService";

const MENSAJES_BASE = {
  Climatización: {
    Verano:
      "Antes de que empiecen los días de más calor, recomendamos realizar el mantenimiento del aire acondicionado con tiempo. Hacerlo de forma anticipada permite coordinar mejor la agenda y evitar esperas o imprevistos cuando la demanda aumenta.",
    Invierno:
      "Antes de que llegue el frío fuerte, es recomendable revisar el sistema de calefacción o losa radiante. Anticiparse al invierno ayuda a asegurar disponibilidad y evita contratiempos en los momentos de mayor uso."
  },

  Electricidad: {
    Verano:
      "Con la llegada del verano suele aumentar la demanda de trabajos eléctricos, especialmente en exteriores. Revisar la instalación con anticipación permite prevenir fallas y coordinar los trabajos sin urgencias de último momento.",
    Invierno:
      "En invierno se incrementa el uso de estufas y artefactos eléctricos. Realizar una revisión antes de la temporada ayuda a evitar sobrecargas y a planificar los trabajos con mayor previsión."
  },

  Sanitaria:
    "Revisar la instalación sanitaria con tiempo permite detectar pérdidas u obstrucciones antes de que se transformen en una urgencia. La planificación anticipada evita imprevistos y facilita una mejor coordinación de los trabajos.",

  Riego:
    "Antes de los períodos de mayor uso del riego, es conveniente realizar una revisión general del sistema. Anticiparse permite ajustar el funcionamiento y evitar reparaciones urgentes cuando la demanda es más alta.",

  Otros:
    "Realizar el mantenimiento del servicio contratado de forma anticipada permite planificar mejor los trabajos y evitar demoras o urgencias innecesarias durante los momentos de mayor demanda."
};

const buildEmailPreviewHtml = ({ titulo, mensaje }) => `
  <div style="font-family: Arial, sans-serif; background-color:#f4f6f8; padding:16px; box-sizing:border-box;">
    <div style="
      max-width:600px;
      margin:0 auto;
      background-color:#ffffff;
      border-radius:8px;
      overflow:hidden;
      box-shadow:0 2px 6px rgba(0,0,0,0.05);
    ">
      <!-- Header -->
      <div style="background-color:#1f3a5f; color:#ffffff; padding:16px 24px;">
        <h2 style="margin:0; font-size:20px;">Winni Electricidad</h2>
        <p style="margin:4px 0 0; font-size:13px; opacity:0.9;">
          Servicios técnicos y mantenimiento
        </p>
      </div>

      <!-- Body -->
      <div style="padding:24px; color:#333;">
        <h3 style="margin-top:0; color:#1f3a5f;">
          Mantenimiento recomendado antes de la temporada - ${titulo}
        </h3>

        <p>Hola 👋,</p>

        <p style="line-height:1.5; font-size:15px;">
          ${mensaje || ""}
        </p>

        <!-- Bloque informativo -->
        <div style="
          background:#f5f7fa;
          padding:12px;
          border-left:4px solid #1f3a5f;
          margin:24px 0;
          font-size:14px;
        ">
          Coordinar estos trabajos con anticipación ayuda a evitar esperas y a asegurar disponibilidad
          durante los momentos de mayor demanda.
        </div>

        <p>
          Si querés coordinar una revisión o tenés alguna consulta, quedamos a las órdenes.
        </p>

        <p>
          Saludos,<br/>
          <strong>Winni Electricidad</strong>
        </p>

        <hr style="margin:24px 0; border:none; border-top:1px solid #e0e0e0;" />

        <p style="font-size:12px; color:#777;">
          Este es un recordatorio automático enviado por Winni Electricidad.
        </p>
      </div>
    </div>
  </div>
`;

export default function RecordatorioMantenimiento() {
  const [servicios, setServicios] = useState([]);
  const [idServicio, setIdServicio] = useState("");
  const [temporada, setTemporada] = useState("Verano");
  const [mensaje, setMensaje] = useState("");
  const [autocompleteInput, setAutocompleteInput] = useState("");
  const [mensajeError, setMensajeError] = useState("");
  const [mensajeConfirmacion, setMensajeConfirmacion] = useState("");
  const [enviando, setEnviando] = useState(false);

  // destinatarios
  const [modoEnvio, setModoEnvio] = useState("todosServicio");
  const [origenClientes, setOrigenClientes] = useState("servicio");
  const [clientesSeleccionados, setClientesSeleccionados] = useState([]);
  const [clientesDisponibles, setClientesDisponibles] = useState([]);
  const [clientesServicio, setClientesServicio] = useState([]);

  const [confirmOpen, setConfirmOpen] = useState(false);

  const filterClientes = createFilterOptions({
    stringify: (option) =>
      `${option.nombreCompleto} ${option.email} ${option.telefono}`
  });

  /* cargar servicios */
  useEffect(() => {
    const cargarServicios = async () => {
      try {
        setMensajeError("");
        const data = await getServiciosActivos();
        setServicios(data ?? []);
      } catch (err) {
        console.error(err);
        setServicios([]);
        setMensajeError(
          "No se pudieron cargar los servicios. Verificá tu conexión o intentá nuevamente."
        );
      }
    };

    cargarServicios();
  }, []);

  const servicioSeleccionado = useMemo(() => {
    const idNum = Number(idServicio);
    if (!idNum) return null;
    return servicios.find((s) => s.id === idNum) ?? null;
  }, [idServicio, servicios]);

  const esClimatizacion = useMemo(() => {
    return (servicioSeleccionado?.titulo ?? "")
      .toLowerCase()
      .includes("climat");
  }, [servicioSeleccionado]);

  /* precarga mensaje */
  useEffect(() => {
    if (!servicioSeleccionado) {
      setMensaje("");
      return;
    }

    const titulo = servicioSeleccionado.titulo;

    if (titulo === "Climatización") {
      setMensaje(MENSAJES_BASE.Climatización[temporada]);
      return;
    }

    if (titulo === "Electricidad") {
      setMensaje(
        MENSAJES_BASE.Electricidad[temporada] ??
          MENSAJES_BASE.Electricidad.Invierno
      );
      return;
    }

    setMensaje(MENSAJES_BASE[titulo] ?? "");
  }, [servicioSeleccionado, temporada]);

  /* clientes disponibles */
  useEffect(() => {
    if (modoEnvio !== "manual") {
      setClientesDisponibles([]);
      return;
    }

    const ac = new AbortController();

    const cargarClientes = async () => {
      setMensajeError("");

      try {
        if (origenClientes === "servicio") {
          if (!idServicio) {
            setClientesDisponibles([]);
            return;
          }

          const idNum = Number(idServicio);

          const clientes = await listarClientesAdminSegunServicio(idNum, ac.signal);
          setClientesDisponibles(clientes ?? []);
        } else {
          const clientes = await listarClientesAdmin(ac.signal);
          setClientesDisponibles(clientes ?? []);
        }

        setClientesSeleccionados([]);
        setAutocompleteInput("");
      } catch (err) {
        if (err.name !== "AbortError") {
          console.error(err);
          setClientesDisponibles([]);
          setMensajeError(
            "No se pudieron cargar los clientes. Intentá nuevamente más tarde."
          );
        }
      }
    };

    cargarClientes();

    return () => ac.abort();
  }, [modoEnvio, origenClientes, idServicio]);

  useEffect(() => {
    if (modoEnvio !== "todosServicio" || !idServicio) {
      setClientesServicio([]);
      return;
    }

    const ac = new AbortController();

    const cargarClientesServicio = async () => {
      try {
        setMensajeError("");
        const idNum = Number(idServicio);
        const clientes = await listarClientesAdminSegunServicio(
          idNum,
          ac.signal
        );
        setClientesServicio(clientes ?? []);
      } catch (err) {
        if (err.name !== "AbortError") {
          console.error(err);
          setClientesServicio([]);
          setMensajeError(
            "No se pudieron cargar los clientes de este servicio. Intentá nuevamente."
          )
        }
      }
    };

    cargarClientesServicio();

    return () => ac.abort();
  }, [modoEnvio, idServicio]);

  const emailsParaEnviar = useMemo(() => {
    if (modoEnvio === "todosServicio") {
      return clientesServicio.map(c => c.email);
    }

    return clientesSeleccionados.map(c => c.email);
  }, [modoEnvio, clientesServicio, clientesSeleccionados]);

  const cantidadDestinatarios = emailsParaEnviar.length;

  const tituloPreview = servicioSeleccionado ? servicioSeleccionado.titulo : "[nombre-del-servicio]";

  const handleEnviar = async () => {
    if (!servicioSeleccionado) return;

    if (!mensaje.trim()) {
      setMensajeError("El mensaje no puede estar vacío.");
      return;
    }

    if (emailsParaEnviar.length === 0) {
      setMensajeError("No hay destinatarios para enviar el recordatorio.");
      return;
    }

    const payload = {
      tituloServicio: servicioSeleccionado.titulo,
      texto: mensaje,
      emailClientesParaEnviar: emailsParaEnviar
    };

    try {
      setMensajeError("");
      setEnviando(true);

      await enviarRecordatorios(payload);

      setConfirmOpen(false);
      setMensajeConfirmacion(
        "¡Recordatorios enviados correctamente a todos los destinatarios!"
      );
    } catch (err) {
      console.error(err);

      setMensajeError(
        err?.message ||
          "Ocurrió un error al enviar el recordatorio. Intentá nuevamente."
      );
    } finally {
      setEnviando(false);
    }
  };

  const cantidadClientesServicio = useMemo(() => {
    return clientesServicio.length;
  }, [clientesServicio]);

  return (
    <Box
      sx={{
        maxWidth: 520,
        width: "100%",
        mx: "auto",
        py: 4,
        px: { xs: 1.5, sm: 2 },
        boxSizing: "border-box"
      }}
    >
      <Typography variant="h5" fontWeight={600}>
        Enviar recordatorios
      </Typography>

      {mensajeError && (
        <Box sx={{ mt: 2 }}>
          <Alert severity="error" onClose={() => setMensajeError("")}>
            {mensajeError}
          </Alert>
        </Box>
      )}

      {mensajeConfirmacion && (
        <Alert severity="success" sx={{ mb: 3 }}>
          {mensajeConfirmacion}
        </Alert>
      )}

      <Divider sx={{ my: 2 }} />

      <TextField
        select
        label="Servicio"
        fullWidth
        value={idServicio}
        onChange={(e) => setIdServicio(e.target.value)}
      >
        {servicios.map((s) => (
          <MenuItem key={s.id} value={s.id}>
            {s.titulo}
          </MenuItem>
        ))}
      </TextField>

      {esClimatizacion && (
        <RadioGroup
          row
          value={temporada}
          onChange={(e) => setTemporada(e.target.value)}
        >
          <FormControlLabel value="Verano" control={<Radio />} label="Verano" />
          <FormControlLabel
            value="Invierno"
            control={<Radio />}
            label="Invierno"
          />
        </RadioGroup>
      )}

      <TextField
        label="Mensaje"
        multiline
        rows={6}
        fullWidth
        value={mensaje}
        onChange={(e) => setMensaje(e.target.value)}
        sx={{ mt: 2 }}
      />

      <Box sx={{ mt: 4 }}>
        <Typography fontWeight={600} mb={1}>
          Destinatarios
        </Typography>

        <RadioGroup
          value={modoEnvio}
          onChange={(e) => {
            setModoEnvio(e.target.value);
            setClientesSeleccionados([]);
            setAutocompleteInput("");
          }}
        >
          <FormControlLabel
            value="todosServicio"
            control={<Radio />}
            label="Todos los clientes que contrataron el servicio"
          />

          {modoEnvio === "todosServicio" && idServicio && (
            <Typography
              variant="caption"
              color={cantidadClientesServicio === 0 ? "warning.main" : "text.secondary"}
              sx={{ ml: 4, mt: 0.5, display: "block" }}
            >
              {cantidadClientesServicio === 0 ? (
                "No hay clientes registrados para este servicio."
              ) : (
                <>
                  Se enviará a {cantidadClientesServicio} cliente
                  {cantidadClientesServicio !== 1 && "s"} que contrataron{" "}
                  <strong>{servicioSeleccionado?.titulo}</strong>
                </>
              )}
            </Typography>
          )}

          <FormControlLabel
            value="manual"
            control={<Radio />}
            label="Seleccionar clientes manualmente"
          />
        </RadioGroup>

        {/* BLOQUE MANUAL */}
        {modoEnvio === "manual" && (
          <Box
            sx={{
              mt: 2,
              p: 2,
              width: "100%",
              maxWidth: 520,
              mx: "auto",
              borderRadius: 2,
              backgroundColor: "grey.50",
              border: "1px solid",
              borderColor: "grey.200",
              minHeight: 360,
              boxSizing: "border-box"
            }}
          >
            {/* Paso 1 */}
            <Typography variant="body2" fontWeight={600} mb={0.5}>
              1. ¿Desde dónde querés seleccionar?
            </Typography>

            <Typography
              variant="caption"
              color="text.secondary"
              mb={1}
              display="block"
            >
              Elegí el grupo de clientes desde el cual vas a seleccionar.
            </Typography>

            <RadioGroup
              value={origenClientes}
              onChange={(e) => {
                setOrigenClientes(e.target.value);
                setClientesSeleccionados([]);
                setAutocompleteInput("");
              }}
            >
              <FormControlLabel
                value="servicio"
                control={<Radio size="small" />}
                label="Clientes que ya contrataron este servicio"
              />
              <FormControlLabel
                value="todos"
                control={<Radio size="small" />}
                label="Cualquier cliente"
              />
            </RadioGroup>

            <Divider sx={{ my: 2 }} />

            {/* Paso 2 */}
            <Typography variant="body2" fontWeight={600} mb={0.5}>
              2. Seleccioná los clientes
            </Typography>

            <Typography
              variant="caption"
              color="text.secondary"
              mb={1}
              display="block"
            >
              Podés buscar por nombre, email o teléfono.
            </Typography>

            <Box
              sx={{
                mb: 2,
                minHeight: origenClientes === "todos" ? 64 : "auto"
              }}
            >
              {origenClientes === "todos" ? (
                <>
                  <Typography
                    variant="caption"
                    color="text.secondary"
                    display="block"
                    mb={1}
                  >
                    Esta opción incluye a todos los clientes del sistema, hayan
                    contratado este servicio previamente o no.
                  </Typography>

                  <Box sx={{ display: "flex", gap: 2, flexWrap: "wrap" }}>
                    <Button
                      size="small"
                      variant="text"
                      onClick={() => setClientesSeleccionados(clientesDisponibles)}
                      sx={{
                        textTransform: "none",
                        fontWeight: 500
                      }}
                    >
                      Seleccionar todos
                    </Button>

                    <Button
                      size="small"
                      variant="text"
                      color="inherit"
                      onClick={() => setClientesSeleccionados([])}
                      sx={{
                        textTransform: "none",
                        color: "text.secondary"
                      }}
                    >
                      Limpiar selección
                    </Button>
                  </Box>
                </>
              ) : null}
            </Box>

            <Autocomplete
              sx={{
                "& .MuiAutocomplete-inputRoot": {
                  flexWrap: "wrap"
                },
                "& .MuiAutocomplete-root": { maxWidth: "100%" }
              }}
              multiple
              options={clientesDisponibles}
              value={clientesSeleccionados}
              inputValue={autocompleteInput}
              onInputChange={(e, value) => {
                setAutocompleteInput(value);
              }}
              onChange={(e, v) => {
                setClientesSeleccionados(v);
                setAutocompleteInput("");
              }}
              filterSelectedOptions
              filterOptions={filterClientes}
              getOptionLabel={(o) => o.nombreCompleto}
              isOptionEqualToValue={(o, v) => o.idUsuario === v.idUsuario}
              noOptionsText={
                origenClientes === "servicio" && !idServicio
                  ? "Seleccioná un servicio para poder ver los clientes que lo contrataron"
                  : clientesDisponibles.length === 0
                    ? "No hay clientes disponibles para este criterio"
                    : clientesSeleccionados.length === clientesDisponibles.length
                      ? "Todos los clientes disponibles ya fueron seleccionados"
                      : "No hay clientes disponibles"
              }
              renderTags={(value, getTagProps) =>
                value.map((o, i) => (
                  <Chip
                    key={o.idUsuario}
                    sx={{ maxWidth: "100%" }}
                    label={o.nombreCompleto}
                    {...getTagProps({ index: i })}
                  />
                ))
              }
              renderInput={(params) => (
                <TextField
                  {...params}
                  label="Seleccionar clientes"
                  placeholder="Ej: Juan, 099…, mail@…"
                  fullWidth
                />
              )}
              renderOption={(props, option) => (
                <li {...props} key={option.idUsuario}>
                  <Box>
                    <Typography variant="body2">
                      {option.nombreCompleto}
                    </Typography>
                    <Typography variant="caption" color="text.secondary">
                      {option.email} · {option.telefono}
                    </Typography>
                  </Box>
                </li>
              )}
            />
          </Box>
        )}
      </Box>

      {/* PREVIEW */}
      <Box sx={{ mt: 4 }}>
        <Typography fontWeight={600} mb={1}>
          Vista previa del correo
        </Typography>

        <Box
          sx={{
            border: "1px solid #ddd",
            borderRadius: 2,
            overflowX: "hidden",
            maxWidth: "100%"
          }}
        >
          <div
            style={{ maxWidth: "100%", overflowX: "hidden" }}
            dangerouslySetInnerHTML={{
              __html: buildEmailPreviewHtml({
                titulo: tituloPreview,
                mensaje
              })
            }}
          />
        </Box>
      </Box>

      <Button
        variant="contained"
        fullWidth
        sx={{ mt: 3 }}
        disabled={!servicioSeleccionado || cantidadDestinatarios === 0}
        onClick={() => {
          setMensajeError("");
          setConfirmOpen(true);
        }}
      >
        Enviar
      </Button>

      <Dialog open={confirmOpen} onClose={() => setConfirmOpen(false)}>
        <DialogTitle>Confirmar envío</DialogTitle>
        {mensajeError && (
          <Alert severity="error" onClose={() => setMensajeError("")}>
            {mensajeError}
          </Alert>
        )}
        <DialogContent>
          <Typography mb={1}>
            Estás por enviar el siguiente recordatorio:
          </Typography>

          <Box sx={{ mb: 2 }}>
            <Typography variant="body2">
              <strong>Servicio:</strong> {servicioSeleccionado?.titulo}
            </Typography>

            <Typography variant="body2">
              <strong>Destinatarios:</strong> {cantidadDestinatarios}
            </Typography>
          </Box>

          <Divider sx={{ my: 1.5 }} />

          <Typography variant="body2" mb={1}>
            <strong>Mensaje:</strong>
          </Typography>

          <Typography
            variant="body2"
            sx={{
              backgroundColor: "grey.100",
              p: 1.5,
              borderRadius: 1,
              fontSize: 14
            }}
          >
            {mensaje}
          </Typography>

          <Divider sx={{ my: 2 }} />

          <Typography variant="caption" color="text.secondary">
            Se enviará a:
            <br />
            {emailsParaEnviar.slice(0, 3).join(", ")}
            {emailsParaEnviar.length > 3 && "…"}
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setConfirmOpen(false)}>Cancelar</Button>
          <Button
            variant="contained"
            onClick={handleEnviar}
            disabled={enviando || !mensaje.trim()}
          >
            {enviando ? <CircularProgress size={24} color="inherit" /> : "Confirmar"}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
