import { useEffect, useMemo, useState } from "react";
import { Box, Paper, Typography, Alert, CircularProgress } from "@mui/material";

import ApiError from "../../../services/ApiError";
import { buscarUsuariosAdmin } from "../../../services/authService";
import { listarClientesAdmin, obtenerDetalleClienteAdmin } from "../../../services/clienteService";
import { getReservasPorClienteAdmin } from "../../../services/reservaService";

import FiltroClientes from "./FiltroClientes";
import ListadoClientes from "./ListadoClientes";
import DetalleClientes from "./DetalleClientes";

function getIdUsuario(c) {
  return (
    c?.IdUsuario ??
    c?.idUsuario ??
    c?.Id ??      
    c?.id ??
    c?.usuarioId ??
    null
  );
}

export default function FichaClientes() {
  const [query, setQuery] = useState("");

  const [clientes, setClientes] = useState([]);
  const [loadingClientes, setLoadingClientes] = useState(false);
  const [errorClientes, setErrorClientes] = useState(null);

  const [modoListadoTotal, setModoListadoTotal] = useState(false);

  const [openDetalle, setOpenDetalle] = useState(false);
  const [clienteSel, setClienteSel] = useState(null);

  const [detalle, setDetalle] = useState(null);
  const [loadingDetalle, setLoadingDetalle] = useState(false);
  const [errorDetalle, setErrorDetalle] = useState(null);

  // reservas on-demand
  const [mostrarReservas, setMostrarReservas] = useState(false);
  const [reservasCliente, setReservasCliente] = useState([]);
  const [loadingReservas, setLoadingReservas] = useState(false);
  const [errorReservas, setErrorReservas] = useState(null);

  useEffect(() => {
    if (!modoListadoTotal) return;

    const ac = new AbortController();
    setLoadingClientes(true);
    setErrorClientes(null);

    (async () => {
      try {
        const data = await listarClientesAdmin(ac.signal);
        setClientes(Array.isArray(data) ? data : []);
      } catch (err) {
        if (ac.signal.aborted) return;
        setClientes([]);
        setErrorClientes(err?.message || "Ocurrió un error al cargar clientes.");
      } finally {
        if (!ac.signal.aborted) setLoadingClientes(false);
      }
    })();

    return () => ac.abort();
  }, [modoListadoTotal]);

  useEffect(() => {
    let alive = true;
    setErrorClientes(null);

    const q = query.trim();

    if (!q) {
      setClientes([]);
      setModoListadoTotal(false);
      return;
    }

    if (q.length < 2) {
      setClientes([]);
      setModoListadoTotal(false);
      return;
    }

    setModoListadoTotal(false);
    setLoadingClientes(true);

    const t = setTimeout(async () => {
      try {
        const data = await buscarUsuariosAdmin(q);
        if (!alive) return;
        setClientes(Array.isArray(data) ? data : []);
      } catch (err) {
        if (!alive) return;
        if (err instanceof ApiError) setErrorClientes(err.message);
        else setErrorClientes("Ocurrió un error al buscar clientes.");
        setClientes([]);
      } finally {
        if (alive) setLoadingClientes(false);
      }
    }, 350);

    return () => {
      alive = false;
      clearTimeout(t);
    };
  }, [query]);

  useEffect(() => {
    if (!clienteSel) return;

    const idUsuario = getIdUsuario(clienteSel);
    if (!idUsuario) {
      setDetalle(null);
      setErrorDetalle("No se pudo determinar el ID del usuario.");
      return;
    }

    const ac = new AbortController();

    setMostrarReservas(false);
    setReservasCliente([]);
    setErrorReservas(null);
    setLoadingReservas(false);

    setLoadingDetalle(true);
    setErrorDetalle(null);
    setDetalle(null);

    (async () => {
      try {
        const data = await obtenerDetalleClienteAdmin(idUsuario, ac.signal);
        if (!ac.signal.aborted) setDetalle(data);
      } catch (err) {
        if (!ac.signal.aborted) {
          setErrorDetalle(err?.message || "Ocurrió un error al cargar el detalle.");
        }
      } finally {
        if (!ac.signal.aborted) setLoadingDetalle(false);
      }
    })();

    return () => ac.abort();
  }, [clienteSel]);

  const handleMostrarReservas = async () => {
    if (!clienteSel) return;

    const idUsuario = getIdUsuario(clienteSel);
    if (!idUsuario) {
      setErrorReservas("No se pudo determinar el ID del usuario.");
      return;
    }

    if (mostrarReservas) {
      setMostrarReservas(false);
      return;
    }

    setMostrarReservas(true);
    setLoadingReservas(true);
    setErrorReservas(null);
    setReservasCliente([]);

    try {
      const data = await getReservasPorClienteAdmin(idUsuario);
      setReservasCliente(Array.isArray(data) ? data : []);
    } catch (err) {
      setErrorReservas(err?.message || "Error cargando reservas del cliente.");
      setReservasCliente([]);
    } finally {
      setLoadingReservas(false);
    }
  };

  const resultados = useMemo(
    () => (Array.isArray(clientes) ? clientes : []),
    [clientes]
  );

  return (
    <Box sx={{ p: 2 }}>
      <Typography variant="h5" sx={{ mb: 2, fontWeight: 700 }}>
        Ficha de clientes
      </Typography>

      <Alert severity="info" sx={{ mb: 2 }}>
        Esta es la ficha de clientes. Acá podés ver toda la información relacionada a un
        cliente: datos personales, direcciones, reservas y presupuestos.
      </Alert>

      <Paper sx={{ p: 2, mb: 2 }}>
        <FiltroClientes
          query={query}
          onChangeQuery={setQuery}
          onLimpiar={() => {
            setQuery("");
            setModoListadoTotal(false);
            setClientes([]);
          }}
          onVerTodos={() => {
            setQuery("");
            setModoListadoTotal(true);
          }}
        />

        <Box sx={{ mt: 2 }}>
          {loadingClientes && (
            <Box sx={{ display: "flex", gap: 1, alignItems: "center" }}>
              <CircularProgress size={18} />
              <Typography variant="body2">
                {query.trim() ? "Buscando…" : "Cargando…"}
              </Typography>
            </Box>
          )}

          {errorClientes && <Alert severity="error">{errorClientes}</Alert>}

          {!loadingClientes && !errorClientes && resultados.length === 0 && (
            <Alert severity="info">
              Usá el buscador (mínimo 2 caracteres) o tocá <b>“Ver todos”</b> para mostrar clientes.
            </Alert>
          )}
        </Box>
      </Paper>

      <Paper sx={{ p: 0 }}>
        <ListadoClientes
          clientes={resultados}
          onSelect={(c) => {
            setClienteSel(c);
            setOpenDetalle(true);
          }}
        />
      </Paper>

      <DetalleClientes
        open={openDetalle}
        onClose={() => setOpenDetalle(false)}
        clienteSel={clienteSel}
        detalle={detalle}
        loadingDetalle={loadingDetalle}
        errorDetalle={errorDetalle}
        mostrarReservas={mostrarReservas}
        onToggleReservas={handleMostrarReservas}
        reservas={reservasCliente}
        loadingReservas={loadingReservas}
        errorReservas={errorReservas}
      />
    </Box>
  );
}