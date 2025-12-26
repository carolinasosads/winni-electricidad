import { useEffect, useMemo, useState } from "react";
import {
  getReservasPorEstado,
  getReservasFinalizadas,
  aprobarReserva,
  cancelarReserva,
  modificarReserva,
} from "../../../services/reservaService";

import ReservaDetailModalCliente from "./ReservaDetailModalCliente";
import CalendarioSemanal from "../../Admin/PanelReservas/CalendarioSemanal";
import EstadoList from "../../Admin/PanelReservas/EstadoList";
import ModificarReservaModal from "../../Admin/PanelReservas/ModificarReservaModal";

import {
  Box,
  Typography,
  Drawer,
  IconButton,
  useMediaQuery,
  Alert,
} from "@mui/material";
import MenuIcon from "@mui/icons-material/Menu";

// ---- Helper para ordenar por horario ----
const ordenarPorHorario = (reservas) => {
  if (!Array.isArray(reservas)) return [];
  return [...reservas].sort(
    (a, b) => new Date(a.fechaReserva) - new Date(b.fechaReserva)
  );
};

const filtrarPorMesYAnio = (reservas, mes, anio) => {
  if (!Array.isArray(reservas)) return [];
  return reservas.filter((r) => {
    const d = new Date(r.fechaReserva);
    return d.getMonth() + 1 === mes && d.getFullYear() === anio;
  });
};

export default function PanelReservasCliente() {
  // ---- Estados ----
  const [mesActual, setMesActual] = useState(new Date().getMonth() + 1);
  const [anioActual, setAnioActual] = useState(new Date().getFullYear());

  // reservas por estado (del cliente)
  const [pendientes, setPendientes] = useState([]);
  const [confirmadas, setConfirmadas] = useState([]);
  const [canceladas, setCanceladas] = useState([]);
  const [finalizadas, setFinalizadas] = useState([]);

  const [selectedReserva, setSelectedReserva] = useState(null);
  const [openModificar, setOpenModificar] = useState(false);

  const [mensajeOk, setMensajeOk] = useState(null);
  const [mensajeError, setMensajeError] = useState(null);

  // mobile / desktop
  const isMobile = useMediaQuery("(max-width:900px)");
  const [openDrawer, setOpenDrawer] = useState(false);

  // ✅ Fuente única para el calendario: SOLO reservas del cliente (por estado)
  const todasLasReservasDelCliente = useMemo(() => {
    const all = [
      ...(Array.isArray(pendientes) ? pendientes : []),
      ...(Array.isArray(confirmadas) ? confirmadas : []),
      ...(Array.isArray(canceladas) ? canceladas : []),
      ...(Array.isArray(finalizadas) ? finalizadas : []),
    ];

    // Evitar duplicados por si el backend repite algo:
    const map = new Map();
    for (const r of all) map.set(r.idReserva, r);
    return Array.from(map.values());
  }, [pendientes, confirmadas, canceladas, finalizadas]);

  // ✅ Calendario filtrado por mes/año, pero SIEMPRE desde reservas del cliente
  const reservasMes = useMemo(() => {
    return filtrarPorMesYAnio(todasLasReservasDelCliente, mesActual, anioActual);
  }, [todasLasReservasDelCliente, mesActual, anioActual]);

  // ---- Cargar reservas del cliente por estado ----
  async function recargarTodo() {
    try {
      // Esto debería venir filtrado por usuario logueado.
      const [p, c, ca, f] = await Promise.all([
        getReservasPorEstado("Pendiente"),
        getReservasPorEstado("Confirmada"),
        getReservasPorEstado("Cancelada"),
        getReservasFinalizadas(),
      ]);

      setPendientes(ordenarPorHorario(p));
      setConfirmadas(ordenarPorHorario(c));
      setCanceladas(ordenarPorHorario(ca));
      setFinalizadas(ordenarPorHorario(f));

      setMensajeError(null);
    } catch (error) {
      setMensajeError(
        "Error al cargar tus reservas. Por favor, intente nuevamente."
      );
    }
  }

  useEffect(() => {
    recargarTodo();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  // ----- Acciones (solo para Pendiente) -----
  const handleAceptar = async (reserva) => {
    try {
      const resp = await aprobarReserva(reserva.idReserva);

      setSelectedReserva(null);
      await recargarTodo();
      setMensajeOk(resp.message || "Reserva aceptada.");
      setMensajeError(null);
    } catch (err) {
      setMensajeError(err?.message || "Error al aceptar la reserva.");
      setMensajeOk(null);
    }
  };

  const handleCancelar = async (reserva) => {
    try {
      const resp = await cancelarReserva(reserva.idReserva);

      setSelectedReserva(null);
      await recargarTodo();
      setMensajeOk(resp.message || "Reserva cancelada.");
      setMensajeError(null);
    } catch (err) {
      setMensajeError(err?.message || "Error al cancelar la reserva.");
      setMensajeOk(null);
    }
  };

  const handleModificarClick = (reserva) => {
    setSelectedReserva(reserva);
    setOpenModificar(true);
  };

  const handleModificarReserva = async (nuevaFecha) => {
    try {
      const resp = await modificarReserva(selectedReserva.idReserva, nuevaFecha);

      setOpenModificar(false);
      setSelectedReserva(null);

      await recargarTodo();
      setMensajeOk(resp.message || "Reserva modificada.");
      setMensajeError(null);
    } catch (err) {
      setMensajeError(err?.message || "Error al modificar la reserva.");
      setMensajeOk(null);
    }
  };

  const handleCambioSemana = (fechaInicioSemana) => {
    const nuevoMes = fechaInicioSemana.getMonth() + 1;
    const nuevoAnio = fechaInicioSemana.getFullYear();

    if (nuevoMes !== mesActual || nuevoAnio !== anioActual) {
      setMesActual(nuevoMes);
      setAnioActual(nuevoAnio);
    }
  };

  return (
    <Box sx={{ p: 2, width: "100%" }}>
      <Typography variant="h4" fontWeight={700} sx={{ mb: 2 }}>
        Mis Reservas
      </Typography>

      {/* MENSAJES */}
      {mensajeError && (
        <Alert
          severity="error"
          sx={{ mb: 2 }}
          onClose={() => setMensajeError(null)}
        >
          {mensajeError}
        </Alert>
      )}

      {mensajeOk && (
        <Alert
          severity="success"
          sx={{ mb: 2 }}
          onClose={() => setMensajeOk(null)}
        >
          {mensajeOk}
        </Alert>
      )}

      {/* ---------- DESKTOP ---------- */}
      {!isMobile && (
        <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
          <CalendarioSemanal
            reservas={reservasMes}
            onSelectReserva={setSelectedReserva}
            onCambioSemana={handleCambioSemana}
          />

          <Box
            sx={{
              display: "grid",
              gridTemplateColumns: "repeat(4, 1fr)",
              gap: 2,
            }}
          >
            <EstadoList
              titulo="Pendientes"
              color="warning.main"
              items={pendientes}
              onSelect={setSelectedReserva}
            />
            <EstadoList
              titulo="Confirmadas"
              color="primary.main"
              items={confirmadas}
              onSelect={setSelectedReserva}
            />
            <EstadoList
              titulo="Canceladas"
              color="error.main"
              items={canceladas}
              onSelect={setSelectedReserva}
            />
            <EstadoList
              titulo="Finalizadas"
              color="success.main"
              items={finalizadas}
              onSelect={setSelectedReserva}
            />
          </Box>
        </Box>
      )}

      {/* ---------- MOBILE ---------- */}
      {isMobile && (
        <>
          <Box sx={{ mb: 2 }}>
            <IconButton
              onClick={() => setOpenDrawer(true)}
              sx={{
                borderRadius: 2,
                px: 1.5,
                border: "1px solid",
                borderColor: "divider",
                gap: 1,
              }}
            >
              <MenuIcon fontSize="small" />
              <Typography variant="button">Abrir panel lateral</Typography>
            </IconButton>
          </Box>

          <CalendarioSemanal
            reservas={reservasMes}
            onSelectReserva={setSelectedReserva}
            onCambioSemana={handleCambioSemana}
          />

          <Drawer
            anchor="left"
            open={openDrawer}
            onClose={() => setOpenDrawer(false)}
            variant="temporary"
            PaperProps={{
              sx: { width: 300, p: 2, pt: 3 },
            }}
          >
            <Typography variant="h6" fontWeight={700} sx={{ mb: 2 }}>
              Listas de Reservas
            </Typography>

            <EstadoList
              titulo="Pendientes"
              color="warning.main"
              items={pendientes}
              onSelect={(r) => {
                setSelectedReserva(r);
                setOpenDrawer(false);
              }}
            />

            <EstadoList
              titulo="Confirmadas"
              color="primary.main"
              items={confirmadas}
              onSelect={(r) => {
                setSelectedReserva(r);
                setOpenDrawer(false);
              }}
            />

            <EstadoList
              titulo="Canceladas"
              color="error.main"
              items={canceladas}
              onSelect={(r) => {
                setSelectedReserva(r);
                setOpenDrawer(false);
              }}
            />

            <EstadoList
              titulo="Finalizadas"
              color="success.main"
              items={finalizadas}
              onSelect={(r) => {
                setSelectedReserva(r);
                setOpenDrawer(false);
              }}
            />
          </Drawer>
        </>
      )}

      {/* Modal detalle reserva (cliente) */}
      <ReservaDetailModalCliente
        reserva={selectedReserva}
        open={!!selectedReserva}
        onClose={() => setSelectedReserva(null)}
        onAceptar={handleAceptar}
        onCancelar={handleCancelar}
        onModificar={handleModificarClick}
      />

      {/* Modal modificar reserva */}
      <ModificarReservaModal
        open={openModificar}
        onClose={() => setOpenModificar(false)}
        reserva={selectedReserva}
        reservasMes={reservasMes}
        onSubmit={handleModificarReserva}
      />
    </Box>
  );
}
