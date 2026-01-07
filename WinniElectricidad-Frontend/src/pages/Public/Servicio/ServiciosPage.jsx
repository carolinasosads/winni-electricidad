import { Box, Typography, Dialog, DialogTitle, DialogContent, DialogActions, Button, Alert, Toolbar } from "@mui/material";
import HowToRegIcon from '@mui/icons-material/HowToReg';
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

import AppHeader from "../../../components/Header/Header.jsx";
import { SitemarkIcon } from "../../../components/CustomIcons/CustomIcons.jsx";

import ServiciosCard from "./ServiciosCard";
import ServicioCreateCard from "./ServicioCreateCard";
import ServiciosHeader from "./ServiciosHeader";

import {
  getServiciosActivos,
  getServicios,
  desactivarServicio,
  activarServicio,
  crearServicio
} from "../../../services/servicioService";

export default function ServiciosPage() {
  const rol = localStorage.getItem("rol");
  const estaLogueado = Boolean(localStorage.getItem("token"));

  const navigate = useNavigate();

  const [servicios, setServicios] = useState([]);
  const [servicioAccion, setServicioAccion] = useState(null);

  const [errorMsg, setErrorMsg] = useState("");
  const [successMsg, setSuccessMsg] = useState("");

  const serviciosNormalizados = servicios.map(s => {
    const imagenPrincipal =
      Array.isArray(s.imagenes) &&
      s.imagenes.find(i => i.esPrincipal)?.url;

    return {
      ...s,
      imagenUrl: imagenPrincipal || "/servicios/servicio-default.jpg"
    };
  });

  const loadServicios = async (signal) => {
    try {
      const data =
        rol === "Administrador"
          ? await getServicios(signal)
          : await getServiciosActivos(signal);

      setServicios(data);
    } catch (err) {
      if (err.name === "AbortError") return;
      setErrorMsg(err?.message || "Error al cargar los servicios.");
    }
  };

  const reloadServicios = async () => {
    try {
      setErrorMsg("");
      await loadServicios();
    } catch (err) {
      setErrorMsg(err?.message || "Error al cargar los servicios.");
    }
  };

  useEffect(() => {
    setErrorMsg("");
    setSuccessMsg("");
    const ac = new AbortController();
    loadServicios(ac.signal);
    return () => ac.abort();
  }, []);

  const confirmarToggle = async () => {
    try {
      setErrorMsg("");
      setSuccessMsg("");

      if (servicioAccion.activo) {
        await desactivarServicio(servicioAccion.id);
        setSuccessMsg("¡Servicio desactivado con éxito!");
      } else {
        await activarServicio(servicioAccion.id);
        setSuccessMsg("¡Servicio activado con éxito!");
      }

      await reloadServicios();
      setServicioAccion(null);
    } catch (err) {
      setErrorMsg(err?.message || "Error al modificar el servicio.");
    }
  };

  return (
    <Box sx={{ maxWidth: 1200, mx: "auto", px: 2, py: 4 }}>
      {!estaLogueado && (
        <>
          <AppHeader
            logo={<SitemarkIcon />}
            showBackButton
            homeHref="/"
          />
          <Toolbar />
        </>
      )}
      <ServiciosHeader />

      {errorMsg && (
        <Alert severity="error" sx={{ mb: 3 }}>
          {errorMsg}
        </Alert>
      )}
      
      {successMsg && (
        <Alert severity="success" sx={{ mb: 3 }}>
          {successMsg}
        </Alert>
      )}

      {!rol && (
        <Box
          sx={{
            mt: 4,
            mb: 3,
            p: 3,
            borderRadius: 2,
            bgcolor: "grey.50",
            display: "flex",
            justifyContent: "space-between",
            flexWrap: "wrap",
            gap: 2
          }}
        >
          <Box>
            <Typography fontWeight={600}>
              ¿Querés solicitar un presupuesto?
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Registrate y agendá tu servicio de forma rápida.
            </Typography>
          </Box>

          <Button
            variant="contained"
            startIcon={<HowToRegIcon />}
            onClick={() => navigate("/registro")}
          >
            Registrarme
          </Button>
        </Box>
        )}

      <Box
        sx={{
          display: "grid",
          gap: 4,
          gridTemplateColumns: {
            xs: "1fr",
            sm: "repeat(2, 1fr)",
            md: "repeat(3, 1fr)"
          }
        }}
      >
        {rol === "Administrador" && (
          <ServicioCreateCard
            onCrear={async (nuevoServicio) => {
              await crearServicio(nuevoServicio);
              await reloadServicios();
              setSuccessMsg("¡Servicio creado con éxito!");
            }}
          />
        )}
        {serviciosNormalizados.map(servicio => (
          <ServiciosCard
            key={servicio.id}
            servicio={servicio}
            rol={rol}
            onToggleActivo={setServicioAccion}
            onSolicitar={() => navigate("/cliente/agenda")}
          />
        ))}
      </Box>

      <Dialog
        open={Boolean(servicioAccion)}
        onClose={() => setServicioAccion(null)}
      >
        <DialogTitle>
          {servicioAccion?.activo ? "Desactivar servicio" : "Activar servicio"}
        </DialogTitle>
        <DialogContent>
          <Typography>
            ¿Seguro que querés{" "}
            {servicioAccion?.activo ? "desactivar" : "activar"} este servicio?
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setServicioAccion(null)}>Cancelar</Button>
          <Button variant="contained" onClick={confirmarToggle}>
            Confirmar
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
