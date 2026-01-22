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
  Chip
} from "@mui/material";
import { Autocomplete } from "@mui/material";

import { getServiciosActivos } from "../../../services/servicioService";

const MENSAJES_BASE = {
  Climatización: {
    Verano:
      "Antes de la temporada de calor, es un buen momento para realizar el mantenimiento de tu aire acondicionado. Una limpieza y revisión a tiempo mejora el rendimiento, reduce el consumo eléctrico y evita fallas en los días de mayor uso.",
    Invierno:
      "De cara a la temporada de frío, recomendamos revisar y mantener tu sistema de losa radiante o calefacción. Un control preventivo ayuda a asegurar un funcionamiento correcto, seguro y eficiente durante el invierno."
  },

  Electricidad: {
    Verano:
      "Durante el verano aumenta el uso de instalaciones eléctricas en exteriores, como luces de piscina, bombas o tomas al aire libre. Un mantenimiento preventivo ayuda a evitar fallas, cortes y riesgos eléctricos.",
    Invierno:
      "En invierno es recomendable revisar el estado general de la instalación eléctrica, especialmente ante un mayor uso de calefacción y artefactos eléctricos. Un control preventivo mejora la seguridad y el correcto funcionamiento del sistema."
  },

  Sanitaria:
    "Es un buen momento para revisar y mantener tu instalación sanitaria. Un control preventivo permite detectar pérdidas, obstrucciones o desgastes antes de que se conviertan en un problema mayor.",

  Riego:
    "Para asegurar un riego eficiente y evitar desperdicios de agua, recomendamos realizar un mantenimiento del sistema de riego. Revisar válvulas, cañerías y programadores ayuda a que el sistema funcione correctamente durante todo el año.",

  Otros:
    "Te recordamos que es recomendable realizar el mantenimiento correspondiente al servicio contratado, para asegurar su correcto funcionamiento y prevenir inconvenientes futuros."
};

/* =========================
   PREVIEW HTML
========================= */
const buildEmailPreviewHtml = ({ titulo, mensaje }) => `
  <div style="font-family: Arial, sans-serif; background-color:#f4f6f8; padding:16px;">
    <div style="
      max-width:600px;
      margin:0 auto;
      background-color:#ffffff;
      border-radius:8px;
      overflow:hidden;
      box-shadow:0 2px 6px rgba(0,0,0,0.05);
    ">
      <div style="background-color:#1f3a5f; color:#ffffff; padding:16px 24px;">
        <h2 style="margin:0; font-size:20px;">Winni Electricidad</h2>
      </div>

      <div style="padding:24px; color:#333;">
        <h3 style="margin-top:0; color:#1f3a5f;">${titulo}</h3>

        <p>Hola <strong>Cliente</strong>,</p>

        <p>${mensaje || ""}</p>

        <p style="margin-top:24px;">
          ¡Gracias por confiar en <strong>Winni Electricidad</strong>!
        </p>

        <hr style="margin:24px 0; border:none; border-top:1px solid #e0e0e0;" />

        <p style="font-size:12px; color:#777;">
          Este es un recordatorio automático enviado por Winni Electricidad.
        </p>
      </div>
    </div>
  </div>
`;

/* =========================
   MOCK CLIENTES
========================= */
const CLIENTES_DEL_SERVICIO_MOCK = [
  {
    id: 1,
    nombreCompleto: "Juan Pérez",
    email: "juan@mail.com",
    telefono: "099123456"
  }
];

const TODOS_LOS_CLIENTES_MOCK = [
  ...CLIENTES_DEL_SERVICIO_MOCK,
  {
    id: 2,
    nombreCompleto: "María López",
    email: "maria@mail.com",
    telefono: "098987654"
  }
];

/* =========================
   COMPONENTE
========================= */
export default function RecordatorioMantenimiento() {
  const [servicios, setServicios] = useState([]);
  const [idServicio, setIdServicio] = useState("");
  const [temporada, setTemporada] = useState("Verano");
  const [mensaje, setMensaje] = useState("");

  // destinatarios
  const [modoEnvio, setModoEnvio] = useState("todosServicio");
  const [origenClientes, setOrigenClientes] = useState("servicio");
  const [clientesSeleccionados, setClientesSeleccionados] = useState([]);

  const [confirmOpen, setConfirmOpen] = useState(false);

  /* cargar servicios */
  useEffect(() => {
    getServiciosActivos().then(setServicios).catch(console.error);
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
  const clientesDisponibles = useMemo(() => {
    if (modoEnvio !== "manual") return [];

    return origenClientes === "servicio"
      ? CLIENTES_DEL_SERVICIO_MOCK
      : TODOS_LOS_CLIENTES_MOCK;
  }, [modoEnvio, origenClientes]);

  const cantidadDestinatarios = useMemo(() => {
    if (modoEnvio === "todosServicio") {
      return CLIENTES_DEL_SERVICIO_MOCK.length;
    }
    return clientesSeleccionados.length;
  }, [modoEnvio, clientesSeleccionados]);

  const tituloPreview = useMemo(() => {
    if (!servicioSeleccionado) return "Recordatorio de mantenimiento";
    const base = `Recordatorio de mantenimiento - ${servicioSeleccionado.titulo}`;
    return esClimatizacion ? `${base} (${temporada})` : base;
  }, [servicioSeleccionado, esClimatizacion, temporada]);

  return (
    <Box sx={{ maxWidth: 520, mx: "auto", py: 4, px: 2 }}>
      <Typography variant="h5" fontWeight={600}>
        Enviar recordatorios
      </Typography>

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

      <Typography fontWeight={500} sx={{ mt: 3 }}>
        Destinatarios
      </Typography>

      <Box sx={{ mt: 4 }}>
  <Typography fontWeight={600} mb={1}>
    Destinatarios
  </Typography>

  <RadioGroup
        value={modoEnvio}
        onChange={(e) => {
        setModoEnvio(e.target.value);
        setClientesSeleccionados([]);
        }}
    >
        <FormControlLabel
        value="todosServicio"
        control={<Radio />}
        label="Todos los clientes que contrataron el servicio"
        />

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
            ml: 3,
            p: 2,
            borderRadius: 2,
            backgroundColor: "grey.50",
            border: "1px solid",
            borderColor: "grey.200"
        }}
        >
        <Typography variant="body2" fontWeight={500} mb={1}>
            Origen de clientes
        </Typography>

        <RadioGroup
            value={origenClientes}
            onChange={(e) => {
            setOrigenClientes(e.target.value);
            setClientesSeleccionados([]);
            }}
        >
            <FormControlLabel
            value="servicio"
            control={<Radio size="small" />}
            label="Clientes que contrataron el servicio"
            />
            <FormControlLabel
            value="todos"
            control={<Radio size="small" />}
            label="Cualquier cliente"
            />
        </RadioGroup>

        <Autocomplete
            multiple
            options={clientesDisponibles}
            value={clientesSeleccionados}
            onChange={(e, v) => setClientesSeleccionados(v)}
            getOptionLabel={(o) =>
            `${o.nombreCompleto} (${o.telefono})`
            }
            isOptionEqualToValue={(o, v) => o.id === v.id}
            renderTags={(value, getTagProps) =>
            value.map((o, i) => (
                <Chip
                label={o.nombreCompleto}
                {...getTagProps({ index: i })}
                key={o.id}
                />
            ))
            }
            renderInput={(params) => (
            <TextField
                {...params}
                label="Seleccionar clientes"
                placeholder="Buscar por nombre, email o teléfono"
                fullWidth
            />
            )}
        />
        </Box>
    )}
    </Box>

      {/* PREVIEW */}
      <Box sx={{ mt: 4 }}>
        <Typography fontWeight={500} mb={1}>
          Vista previa del correo
        </Typography>

        <Box sx={{ border: "1px solid #ddd", borderRadius: 2 }}>
          <div
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
        onClick={() => setConfirmOpen(true)}
      >
        Enviar
      </Button>

      <Dialog open={confirmOpen} onClose={() => setConfirmOpen(false)}>
        <DialogTitle>Confirmar envío</DialogTitle>
        <DialogContent>
          <Typography>
            ¿Estás seguro que querés enviar este recordatorio a{" "}
            <strong>{cantidadDestinatarios}</strong> cliente/s?
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setConfirmOpen(false)}>Cancelar</Button>
          <Button variant="contained">Confirmar</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
