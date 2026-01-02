import { useEffect, useState } from "react";
import {
  getReservasPorMesYAnio,
  getReservasPorEstado,
  getReservasFinalizadas,
  aprobarReserva,
  cancelarReserva,
  modificarReserva
} from "../../../services/reservaService";

import CalendarioSemanal from "./CalendarioSemanal";
import CalendarioDiarioMobile from "./CalendarioDiarioMobile";
import ReservaDetailModal from "./ReservaDetailModal";
import ModificarReservaModal from "./ModificarReservaModal";
import EstadoList from "./EstadoList";

import {
  Box,
  Typography,
  Drawer,
  IconButton,
  useMediaQuery,
  Alert
} from "@mui/material";
import MenuIcon from "@mui/icons-material/Menu";

// ---- Helper para ordenar por horario ----
const ordenarPorHorario = (reservas) => {
  if (!Array.isArray(reservas)) return [];
  return [...reservas].sort(
    (a, b) => new Date(a.fechaReserva) - new Date(b.fechaReserva)
  );
};

export default function PanelReservas() {
  // ---- Estados ----
  const [diaSeleccionado, setDiaSeleccionado] = useState(new Date());
  const [mesActual, setMesActual] = useState(new Date().getMonth() + 1);
  const [anioActual, setAnioActual] = useState(new Date().getFullYear());

  const [reservasMes, setReservasMes] = useState([]);
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

  // ---- Cargar reservas de todos los estados y cargar reservas del mes para el calendario ----
  async function recargarTodo() {
    try {
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

      const data = await getReservasPorMesYAnio(mesActual, anioActual);
      setReservasMes(data);
    } catch (error) {
      setMensajeError(
        "Error al cargar las reservas. Por favor, intente nuevamente."
      );
    }
  }

  useEffect(() => {
    recargarTodo();
  }, []);

  // ---- Cargar reservas de un nuevo mes al calendario ----
  useEffect(() => {
    async function loadMes() {
      try {
        const data = await getReservasPorMesYAnio(mesActual, anioActual);
        setReservasMes(data);
      } catch (err) {
        setMensajeError(err?.message || "Error al cargar las reservas del mes.");
      }
    }

    loadMes();
  }, [mesActual, anioActual]);

  // ----- Handlers de acciones -----
  const handleAprobar = async (reserva) => {
    try {
      const resp = await aprobarReserva(reserva.idReserva);

      setSelectedReserva(null);
      recargarTodo();
      setMensajeOk(resp.message);
      setMensajeError(null);
    } catch (err) {
      setMensajeError(err?.message || "Error al aprobar la reserva.");
      setMensajeOk(null);
    }
  };

  const handleCancelar = async (reserva) => {
    try {
      const resp = await cancelarReserva(reserva.idReserva);

      setSelectedReserva(null);
      recargarTodo();
      setMensajeOk(resp.message);
      setMensajeError(null);
    } catch (err) {
      setMensajeError(err?.message || "Error al cancelar la reserva.");
      setMensajeOk(null);
    }
  };

  const handleSugerirCambio = async (reserva) => {
    setSelectedReserva(reserva);
    setOpenModificar(true);
  };

  const handleModificarReserva = async (nuevaFecha) => {
    try {
      const resp = await modificarReserva(
        selectedReserva.idReserva,
        nuevaFecha
      );

      setOpenModificar(false);
      setSelectedReserva(null);

      recargarTodo();
      setMensajeOk(resp.message);
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
        Panel de Reservas
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
              <Typography variant="button">
                Abrir panel lateral
              </Typography>
            </IconButton>
          </Box>

          {isMobile ? (
            <CalendarioDiarioMobile
              diaSeleccionado={diaSeleccionado}
              setDiaSeleccionado={setDiaSeleccionado}
              reservas={reservasMes}
              onSelectReserva={setSelectedReserva}
            />
          ) : (
            <CalendarioSemanal
              reservas={reservasMes}
              onSelectReserva={setSelectedReserva}
              onCambioSemana={handleCambioSemana}
            />
          )}

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

      {/* Modal detalle reserva */}
      <ReservaDetailModal
        reserva={selectedReserva}
        open={!!selectedReserva}
        onClose={() => setSelectedReserva(null)}
        onAprobar={handleAprobar}
        onCancelar={handleCancelar}
        onSugerirCambio={handleSugerirCambio}
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
