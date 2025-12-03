import { useEffect, useState } from "react";
import {
  getReservasPorMesYAnio,
  getReservasPorEstado,
  getReservasFinalizadas
  // TODO:
  // aprobarReserva,
  // rechazarReserva,
  // sugerirModificacion,
  // cancelarReservaConfirmada,
} from "../../../services/reservaService";

import CalendarioSemanal from "./CalendarioSemanal";
import ReservaDetailModal from "./ReservaDetailModal";
import ModificarReservaModal from "./ModificarReservaModal";
import EstadoList from "./EstadoList";

import {
  Box,
  Typography,
  Drawer,
  IconButton,
  useMediaQuery,
} from "@mui/material";
import MenuIcon from "@mui/icons-material/Menu";

export default function PanelReservas() {
  // ---- Estados ----
  const [mesActual, setMesActual] = useState(new Date().getMonth() + 1);
  const [anioActual, setAnioActual] = useState(new Date().getFullYear());

  const [reservasMes, setReservasMes] = useState([]);
  const [pendientes, setPendientes] = useState([]);
  const [confirmadas, setConfirmadas] = useState([]);
  const [canceladas, setCanceladas] = useState([]);
  const [finalizadas, setFinalizadas] = useState([]);

  const [selectedReserva, setSelectedReserva] = useState(null);
  const [openModificar, setOpenModificar] = useState(false);

  // mobile / desktop
  const isMobile = useMediaQuery("(max-width:900px)");
  const [openDrawer, setOpenDrawer] = useState(false);

  // ---- Cargar reservas de todos los estados ----
  useEffect(() => {
    async function loadEstados() {
      const [p, c, ca, f] = await Promise.all([
        getReservasPorEstado("Pendiente"),
        getReservasPorEstado("Confirmada"),
        getReservasPorEstado("Cancelada"),
        getReservasFinalizadas(),
      ]);

      setPendientes(p);
      setConfirmadas(c);
      setCanceladas(ca);
      setFinalizadas(f);
    }

    loadEstados();
  }, []);

  // ---- Cargar reservas del mes para el calendario ----
  useEffect(() => {
    async function loadMes() {
      const data = await getReservasPorMesYAnio(mesActual, anioActual);
      setReservasMes(data);
    }

    loadMes();
  }, [mesActual, anioActual]);

  // ----- Handlers de acciones ) -----
  const handleAprobar = async (reserva) => {
    console.log("Aprobando.", reserva);
    // await aprobarReserva(reserva.id);

    setSelectedReserva(null);
    // await loadEstados();
    // await loadMes();
  };

  const handleCancelar = async (reserva) => {
    console.log("Cancelando.", reserva);
    // await cancelarReserva(reserva.id);

    setSelectedReserva(null);
    // await loadEstados();
    // await loadMes();
  };

  const handleRechazar = async (reserva) => {
    console.log("Rechazando.", reserva);
    // await rechazarReserva(reserva.id);

    setSelectedReserva(null);
    // await loadEstados();
    // await loadMes();
  };

  const handleSugerirCambio = async (reserva) => {
    setSelectedReserva(reserva);
    setOpenModificar(true); // abre modal de modificación
  };

  const handleModificarReserva = async (nuevaFecha) => {
    console.log("Modificando reserva a:", nuevaFecha);

    // await reservaService.modificarReserva(selectedReserva.id, nuevaFecha);

    setOpenModificar(false);
    setSelectedReserva(null);

    // recargar listas creo
    // await loadEstados();
    // await loadMes();
  };

  const handleEditarPresupuesto = async (reserva) => {
    console.log("Editar presupuesto.", reserva);
    // abrir modal presupuesto
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
      <Typography variant="h4" fontWeight={700} sx={{ mb: 3 }}>
        Panel de Reservas
      </Typography>

      {/* ---------- DESKTOP: listas fijas + calendario ---------- */}
      {!isMobile && (
        <Box
          sx={{
            display: "flex",
            flexDirection: "column",
            gap: 3,
            width: "100%",
          }}
        >
          {/* Calendario arriba */}
          <Box sx={{ width: "100%" }}>
            <CalendarioSemanal
              reservas={reservasMes}
              onSelectReserva={setSelectedReserva}
              onCambioSemana={handleCambioSemana}
            />
          </Box>

          {/* Listas abajo */}
          <Box
            sx={{
              width: "100%",
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

      {/* ---------- MOBILE: calendario + drawer lateral ---------- */}
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
              sx: {
                width: 300,
                p: 2,
                boxSizing: "border-box",
                pt: 3,
              },
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
        onRechazar={handleRechazar}
        onSugerirCambio={handleSugerirCambio}
        onEditarPresupuesto={handleEditarPresupuesto}
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
