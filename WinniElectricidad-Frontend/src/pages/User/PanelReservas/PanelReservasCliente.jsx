import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import {  getMisReservasCliente, aprobarReserva,  cancelarReserva,  modificarReserva,} from "../../../services/reservaService";
import ReservaDetailModalCliente from "./ReservaDetailModalCliente";
import CalendarioSemanal from "../../Admin/PanelReservas/CalendarioSemanal";
import CalendarioDiarioMobile from "../../Admin/PanelReservas/CalendarioDiarioMobile";
import ModificarReservaModal from "../../Admin/PanelReservas/ModificarReservaModal";

import {  Box,  Typography,  Drawer,  IconButton,  useMediaQuery,  Alert,  Card,  CardActionArea,  CardContent,  Chip,} from "@mui/material";

import MenuIcon from "@mui/icons-material/Menu";

const ordenarPorHorario = (reservas) => {
  if (!Array.isArray(reservas)) return [];
  return [...reservas].sort((a, b) => new Date(a.fechaReserva) - new Date(b.fechaReserva));
};

const filtrarPorMesYAnio = (reservas, mes, anio) => {
  if (!Array.isArray(reservas)) return [];
  return reservas.filter((r) => {
    const d = new Date(r.fechaReserva);
    return d.getMonth() + 1 === mes && d.getFullYear() === anio;
  });
};

const fechaNormalizada = (fechaIso) => {
  const d = new Date(fechaIso);
  return d.toLocaleDateString("es-UY", { day: "2-digit", month: "2-digit", year: "numeric" });
};

const tituloReserva = (r) => {
  const lista = Array.isArray(r?.servicios) ? r.servicios : [];
  const serviciosTexto =
    lista.length > 0 ? lista.join(", ") : (r?.nombreServicio || "").trim() || "Servicio no especificado";

  const fecha = fechaNormalizada(r?.fechaReserva);
  return `${serviciosTexto} · ${fecha}`;
};

function ColumnaReservas({ titulo, chipColor, items, onSelect }) {
  return (
    <Box
      sx={{
        border: "1px solid",
        borderColor: "divider",
        borderRadius: 2,
        p: 2,
        minHeight: 300,
      }}
    >
      <Box sx={{ display: "flex", alignItems: "center", gap: 1, mb: 2 }}>
        <Typography variant="h6" fontWeight={700}>
          {titulo}
        </Typography>
        <Chip size="small" label={items.length} color={chipColor} />
      </Box>

      {items.length === 0 ? (
        <Typography color="text.secondary">No hay reservas.</Typography>
      ) : (
        <Box sx={{ display: "flex", flexDirection: "column", gap: 1.5 }}>
          {items.map((r) => (
            <Card key={r.idReserva} variant="outlined" sx={{ borderRadius: 2 }}>
              <CardActionArea onClick={() => onSelect(r)}>
                <CardContent sx={{ py: 1.25 }}>
                  <Typography fontWeight={700} sx={{ lineHeight: 1.2 }}>
                    {tituloReserva(r)}
                  </Typography>
                </CardContent>
              </CardActionArea>
            </Card>
          ))}
        </Box>
      )}
    </Box>
  );
}

export default function PanelReservasCliente() {
  const [diaSeleccionado, setDiaSeleccionado] = useState(new Date());
  const [mesActual, setMesActual] = useState(new Date().getMonth() + 1);
  const [anioActual, setAnioActual] = useState(new Date().getFullYear());

  const [pendientes, setPendientes] = useState([]);
  const [confirmadas, setConfirmadas] = useState([]);
  const [canceladas, setCanceladas] = useState([]);
  const [finalizadas, setFinalizadas] = useState([]);

  const [selectedReserva, setSelectedReserva] = useState(null);
  const [openModificar, setOpenModificar] = useState(false);

  const [mensajeOk, setMensajeOk] = useState(null);
  const [mensajeError, setMensajeError] = useState(null);

  const isMobile = useMediaQuery("(max-width:900px)");
  const [openDrawer, setOpenDrawer] = useState(false);

  const prevSnapshotRef = useRef(new Map());
  const accionDelClienteRef = useRef(false);

  const [modalFeedback, setModalFeedback] = useState(null);
  const [isWorking, setIsWorking] = useState(false);

  const todasLasReservasDelCliente = useMemo(() => {
    const all = [
      ...(Array.isArray(pendientes) ? pendientes : []),
      ...(Array.isArray(confirmadas) ? confirmadas : []),
      ...(Array.isArray(canceladas) ? canceladas : []),
      ...(Array.isArray(finalizadas) ? finalizadas : []),
    ];

    const map = new Map();
    for (const r of all) map.set(r.idReserva, r);
    return Array.from(map.values());
  }, [pendientes, confirmadas, canceladas, finalizadas]);

  const reservasMes = useMemo(() => {
    return filtrarPorMesYAnio(todasLasReservasDelCliente, mesActual, anioActual);
  }, [todasLasReservasDelCliente, mesActual, anioActual]);

  const reservasMesConTitulo = useMemo(() => {
    return (Array.isArray(reservasMes) ? reservasMes : []).map((r) => ({
      ...r,
      titulo: tituloReserva(r),
      title: tituloReserva(r),
      nombreServicio:
        Array.isArray(r?.servicios) && r.servicios.length > 0
          ? r.servicios.join(", ")
          : (r?.nombreServicio || "").trim() || "Servicio no especificado",
    }));
  }, [reservasMes]);

  const reservasMesParaCalendario = useMemo(() => {
    const normalizarServicios = (r) => {
      if (Array.isArray(r?.servicios) && r.servicios.length > 0 && typeof r.servicios[0] === "object") {
        return r.servicios;
      }

      if (Array.isArray(r?.servicios) && r.servicios.length > 0 && typeof r.servicios[0] === "string") {
        return r.servicios.filter(Boolean).map((s) => ({ titulo: s }));
      }

      const ns = (r?.nombreServicio || "").trim();
      return ns ? [{ titulo: ns }] : [{ titulo: "Servicio no especificado" }];
    };

    const normalizarCliente = (r) => {
      return { nombre: tituloReserva(r) };
    };

    return (Array.isArray(reservasMes) ? reservasMes : []).map((r) => ({
      ...r,
      cliente: r?.cliente?.nombre ? r.cliente : normalizarCliente(r),
      servicios: normalizarServicios(r),
    }));
  }, [reservasMes]);

  const buildSnapshot = useCallback((arr) => {
    const m = new Map();
    for (const r of arr) {
      m.set(r.idReserva, {
        estado: (r.estado || "").toLowerCase(),
        fechaIso: r.fechaReserva ? new Date(r.fechaReserva).toISOString() : null,
      });
    }
    return m;
  }, []);

  const detectarCambiosYNotificar = useCallback((prevMap, nextArr) => {
    if (accionDelClienteRef.current) {
      accionDelClienteRef.current = false;
      return;
    }

    if (!prevMap || prevMap.size === 0) return;

    const cambios = [];
    for (const r of nextArr) {
      const prev = prevMap.get(r.idReserva);
      if (!prev) continue;

      const estadoAhora = (r.estado || "").toLowerCase();
      const fechaAhoraIso = r.fechaReserva ? new Date(r.fechaReserva).toISOString() : null;

      const cambioEstado = prev.estado !== estadoAhora;
      const cambioFecha = prev.fechaIso !== fechaAhoraIso;

      if (!cambioEstado && !cambioFecha) continue;

      if (prev.estado === "pendiente" && estadoAhora === "confirmada") {
        cambios.push({
          tipo: "confirmada",
          msg: `✅ Tu reserva "${tituloReserva(r)}" quedó confirmada.`,
        });
        continue;
      }

      if (prev.estado === "confirmada" && estadoAhora === "pendiente") {
        cambios.push({
          tipo: "cambio",
          msg: `⚠️ Hubo un cambio sugerido en "${tituloReserva(r)}". Quedó en Pendiente y necesitás aprobarla.`,
        });
        continue;
      }

      if (cambioFecha) {
        cambios.push({
          tipo: "reprogramada",
          msg: `🗓️ Se actualizó la fecha de "${tituloReserva(r)}". Revisala y confirmá si corresponde.`,
        });
      }
    }

    if (cambios.length > 0) {
      setMensajeOk(cambios[0].msg);
      setMensajeError(null);
    }
  }, []);

  const recargarTodo = useCallback(async () => {
    const all = await getMisReservasCliente();
    const arr = Array.isArray(all) ? all : [];
    const ahora = new Date();

    detectarCambiosYNotificar(prevSnapshotRef.current, arr);

    const ca = arr.filter((r) => r.estado === "Cancelada");
    const f = arr.filter((r) => new Date(r.fechaReserva) < ahora && r.estado !== "Cancelada");

    const activas = arr.filter((r) => new Date(r.fechaReserva) >= ahora && r.estado !== "Cancelada");
    const p = activas.filter((r) => r.estado === "Pendiente");
    const c = activas.filter((r) => r.estado === "Confirmada");

    setPendientes(ordenarPorHorario(p));
    setConfirmadas(ordenarPorHorario(c));
    setCanceladas(ordenarPorHorario(ca));
    setFinalizadas(ordenarPorHorario(f));

    prevSnapshotRef.current = buildSnapshot(arr);
    return arr;
  }, [buildSnapshot, detectarCambiosYNotificar]);

  useEffect(() => {
    (async () => {
      try {
        await recargarTodo();
      } catch {
        setMensajeError("Error al cargar tus reservas. Por favor, intente nuevamente.");
        setMensajeOk(null);
      }
    })();
  }, [recargarTodo]);

  const refrescarSelectedReserva = (arr, idReserva) => {
    const updated = (Array.isArray(arr) ? arr : []).find((x) => x.idReserva === idReserva) || null;
    if (updated) setSelectedReserva(updated);
  };

  const limpiarMensajesGlobales = () => {
    setMensajeOk(null);
    setMensajeError(null);
  };

  const handleAceptar = async (reserva) => {
    if (!reserva?.idReserva) return;

    try {
      accionDelClienteRef.current = true;
      limpiarMensajesGlobales();
      setIsWorking(true);
      setModalFeedback(null);

      const resp = await aprobarReserva(reserva.idReserva);

      const arr = await recargarTodo();
      refrescarSelectedReserva(arr, reserva.idReserva);

      setModalFeedback({ severity: "success", text: resp?.message || "Reserva aceptada." });
    } catch (err) {
      setModalFeedback({ severity: "error", text: err?.message || "Error al aceptar la reserva." });
    } finally {
      setIsWorking(false);
    }
  };

  const handleCancelar = async (reserva) => {
    if (!reserva?.idReserva) return;

    try {
      accionDelClienteRef.current = true;
      limpiarMensajesGlobales();
      setIsWorking(true);
      setModalFeedback(null);

      const resp = await cancelarReserva(reserva.idReserva);

      const arr = await recargarTodo();
      refrescarSelectedReserva(arr, reserva.idReserva);

      setModalFeedback({ severity: "success", text: resp?.message || "Reserva cancelada." });
    } catch (err) {
      setModalFeedback({ severity: "error", text: err?.message || "Error al cancelar la reserva." });
    } finally {
      setIsWorking(false);
    }
  };

  const handleModificarClick = (reserva) => {
    setModalFeedback(null);
    setSelectedReserva(reserva);
    setOpenModificar(true);
  };

  const handleModificarReserva = async (nuevaFecha) => {
    try {
      if (!selectedReserva?.idReserva) return;

      accionDelClienteRef.current = true;
      limpiarMensajesGlobales();
      setIsWorking(true);
      setModalFeedback(null);

      const resp = await modificarReserva(selectedReserva.idReserva, nuevaFecha);

      setOpenModificar(false);

      const arr = await recargarTodo();
      refrescarSelectedReserva(arr, selectedReserva.idReserva);

      setModalFeedback({ severity: "success", text: resp?.message || "Reserva modificada." });
    } catch (err) {
      setModalFeedback({ severity: "error", text: err?.message || "Error al modificar la reserva." });
    } finally {
      setIsWorking(false);
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

  const columnas = (
    <Box sx={{ display: "grid", gridTemplateColumns: "repeat(4, 1fr)", gap: 2 }}>
      <ColumnaReservas
        titulo="Pendientes"
        chipColor="warning"
        items={pendientes}
        onSelect={(r) => {
          setModalFeedback(null);
          setSelectedReserva(r);
        }}
      />
      <ColumnaReservas
        titulo="Confirmadas"
        chipColor="primary"
        items={confirmadas}
        onSelect={(r) => {
          setModalFeedback(null);
          setSelectedReserva(r);
        }}
      />
      <ColumnaReservas
        titulo="Canceladas"
        chipColor="error"
        items={canceladas}
        onSelect={(r) => {
          setModalFeedback(null);
          setSelectedReserva(r);
        }}
      />
      <ColumnaReservas
        titulo="Finalizadas"
        chipColor="success"
        items={finalizadas}
        onSelect={(r) => {
          setModalFeedback(null);
          setSelectedReserva(r);
        }}
      />
    </Box>
  );

  return (
    <Box
      sx={{
        p: 2,
        width: "100%",
        maxWidth: "100%",
        overflowX: "hidden",
        boxSizing: "border-box",
      }}
    >
      <Typography variant="h4" fontWeight={700} sx={{ mb: 2 }}>
        Mis Reservas
      </Typography>

      {mensajeError && (
        <Alert severity="error" sx={{ mb: 2 }} onClose={() => setMensajeError(null)}>
          {mensajeError}
        </Alert>
      )}

      {mensajeOk && (
        <Alert severity="success" sx={{ mb: 2 }} onClose={() => setMensajeOk(null)}>
          {mensajeOk}
        </Alert>
      )}

      {!isMobile && (
        <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
          <CalendarioSemanal
            reservas={reservasMesParaCalendario}
            onSelectReserva={(r) => {
              setModalFeedback(null);
              setSelectedReserva(r);
            }}
            onCambioSemana={handleCambioSemana}
          />
          {columnas}
        </Box>
      )}

      {isMobile && (
        <>
          <Box sx={{ mb: 2 }}>
            <IconButton
              onClick={() => setOpenDrawer(true)}
              sx={{ borderRadius: 2, px: 1.5, border: "1px solid", borderColor: "divider", gap: 1 }}
            >
              <MenuIcon fontSize="small" />
              <Typography variant="button">Abrir listas</Typography>
            </IconButton>
          </Box>

          <CalendarioDiarioMobile
            diaSeleccionado={diaSeleccionado}
            setDiaSeleccionado={setDiaSeleccionado}
            reservas={reservasMesParaCalendario}
            onSelectReserva={(r) => {
              setModalFeedback(null);
              setSelectedReserva(r);
            }}
          />

          <Drawer
            anchor="left"
            open={openDrawer}
            onClose={() => setOpenDrawer(false)}
            variant="temporary"
            PaperProps={{
              sx: {
                width: "80vw",
                maxWidth: 320,
                p: 2,
                pt: 3,
              },
            }}
          >
            <Typography variant="h6" fontWeight={700} sx={{ mb: 2 }}>
              Listas de Reservas
            </Typography>

            <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
              <ColumnaReservas
                titulo="Pendientes"
                chipColor="warning"
                items={pendientes}
                onSelect={(r) => {
                  setModalFeedback(null);
                  setSelectedReserva(r);
                  setOpenDrawer(false);
                }}
              />
              <ColumnaReservas
                titulo="Confirmadas"
                chipColor="primary"
                items={confirmadas}
                onSelect={(r) => {
                  setModalFeedback(null);
                  setSelectedReserva(r);
                  setOpenDrawer(false);
                }}
              />
              <ColumnaReservas
                titulo="Canceladas"
                chipColor="error"
                items={canceladas}
                onSelect={(r) => {
                  setModalFeedback(null);
                  setSelectedReserva(r);
                  setOpenDrawer(false);
                }}
              />
              <ColumnaReservas
                titulo="Finalizadas"
                chipColor="success"
                items={finalizadas}
                onSelect={(r) => {
                  setModalFeedback(null);
                  setSelectedReserva(r);
                  setOpenDrawer(false);
                }}
              />
            </Box>
          </Drawer>
        </>
      )}

      <ReservaDetailModalCliente
        reserva={selectedReserva}
        open={!!selectedReserva}
        onClose={() => {
          setModalFeedback(null);
          setSelectedReserva(null);
        }}
        onAceptar={handleAceptar}
        onCancelar={handleCancelar}
        onModificar={handleModificarClick}
        feedback={modalFeedback}
        onClearFeedback={() => setModalFeedback(null)}
        isWorking={isWorking}
      />

      <ModificarReservaModal
        open={openModificar}
        onClose={() => setOpenModificar(false)}
        reserva={selectedReserva}
        reservasMes={reservasMesConTitulo}
        onSubmit={handleModificarReserva}
      />
    </Box>
  );
}
