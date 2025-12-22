import { Box, Typography, Dialog, DialogTitle, DialogContent, DialogActions, Button } from "@mui/material";
import HowToRegIcon from '@mui/icons-material/HowToReg';
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

import ServiciosCard from "./ServiciosCard";
import ServiciosHeader from "./ServiciosHeader";

import {
  getServiciosActivos,
  getServicios,
  desactivarServicio,
  activarServicio
} from "../../../services/servicioService";

export default function ServiciosPage() {
  const rol = localStorage.getItem("rol");
  const navigate = useNavigate();

  const [servicios, setServicios] = useState([]);
  const [servicioAccion, setServicioAccion] = useState(null);

  const loadServicios = async (signal) => {
    const data =
      rol === "Administrador"
        ? await getServicios(signal)
        : await getServiciosActivos(signal);

    setServicios(data);
  };

  useEffect(() => {
    const ac = new AbortController();
    loadServicios(ac.signal);
    return () => ac.abort();
  }, []);

  const confirmarToggle = async () => {
    if (servicioAccion.activo) {
      await desactivarServicio(servicioAccion.id);
    } else {
      await activarServicio(servicioAccion.id);
    }

    await loadServicios();
    setServicioAccion(null);
  };

  return (
    <Box sx={{ maxWidth: 1200, mx: "auto", px: 2, py: 4 }}>
      <ServiciosHeader />

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
        {servicios.map(servicio => (
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
