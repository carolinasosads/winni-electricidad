import { Box, Grid, Typography, Divider, Button } from "@mui/material";
import { useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import ResenasHeader from "./ResenasHeader";
import ResenasFiltros from "./ResenasFiltros";
import ResenaCard from "./ResenaCard";
import ResenasCarrusel from "./ResenasCarrusel";
import ResenaModal from "./ResenaModal";

import { getResenasAprobadas } from "../../../services/resenaService";
import { getServiciosActivos } from "../../../services/authService";

import RateReviewIcon from "@mui/icons-material/RateReview";

export default function ResenasPage() {
  const rol = localStorage.getItem("rol");
  const navigate = useNavigate();

  const [resenas, setResenas] = useState([]);
  const [servicios, setServicios] = useState([]);
  const [servicioFiltro, setServicioFiltro] = useState("Todos");
  const [calificacionFiltro, setCalificacionFiltro] = useState("Todas");
  const [resenaSeleccionada, setResenaSeleccionada] = useState(null);

  const [mensajeError, setMensajeError] = useState(null);

  const resenasConImagen = useMemo(
    () => resenas.filter(r => r.imagenUrl),
    [resenas]
  );


  async function loadResenasAprobadas() {
    try {
      const data = await getResenasAprobadas();
      setResenas(data);
    } catch (err) {
      setMensajeError(err?.message || "Error al cargar las reseñas aprobadas.");
    }
  }

  const loadServicios = async () => {
    try {
      const data = await getServiciosActivos();
      setServicios(data.filter(s => s.titulo !== "Otro"));
    } catch (err) {
      setMensajeError(err?.message || "Error al cargar los servicios activos.");
    }
  };

  useEffect(() => { 
    loadServicios();
    loadResenasAprobadas();
  }, []);

  const promedio = useMemo(() => {
    if (resenas.length === 0) return 0;
    return (
      resenas.reduce((acc, r) => acc + r.calificacion, 0) / resenas.length
    ).toFixed(1);
  }, [resenas]);

  const resenasFiltradas = useMemo(() => {
    return resenas.filter(r => {
      const servicioOk =
        servicioFiltro === "Todos" || r.servicio?.titulo === servicioFiltro;

      const calificacionOk =
        calificacionFiltro === "Todas" ||
        r.calificacion === Number(calificacionFiltro);

      return servicioOk && calificacionOk;
    });
  }, [resenas, servicioFiltro, calificacionFiltro]);

  // TODO. AGREGAR EL ENDPOINT REAL DE ELIMINAR
  const eliminarResena = id => {
    if (!window.confirm("¿Eliminar esta reseña?")) return;
    setResenas(prev => prev.filter(r => r.id !== id));
  };

  return (
    <Box sx={{ maxWidth: 1200, mx: "auto", px: 2, py: 4 }}>
      <ResenasHeader promedio={promedio} total={resenas.length} />

      {rol === "Cliente" && (
        <Box
          sx={{
            mt: 4,
            mb: 3,
            p: 3,
            borderRadius: 2,
            bgcolor: "grey.50",
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
            flexWrap: "wrap",
            gap: 2
          }}
        >
          <Box>
            <Typography fontWeight={600}>
              ¿Ya usaste nuestros servicios?
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Dejá tu reseña y ayudá a otros clientes a elegir.
            </Typography>
          </Box>

          <Button
            variant="contained"
            startIcon={<RateReviewIcon />}
            onClick={() => navigate("/cliente/resenas/crear")}
          >
            Escribir reseña
          </Button>
        </Box>
      )}

      <ResenasFiltros
        servicio={servicioFiltro}
        calificacion={calificacionFiltro}
        servicios={servicios}
        onServicioChange={setServicioFiltro}
        onCalificacionChange={setCalificacionFiltro}
      />

      <Divider sx={{ mb: 3 }} />

      {resenasFiltradas.length === 0 ? (
        <Typography textAlign="center" color="text.secondary" mt={6}>
          No hay reseñas para mostrar.
        </Typography>
      ) : (
        <Grid container spacing={4} justifyContent="center">
          {resenasFiltradas.map(resena => (
            <Grid item xs={12} sm={10} md={6} key={resena.id}>
              <ResenaCard
                resena={resena}
                esAdmin={rol === "Administrador"}
                onEliminar={eliminarResena}
              />
            </Grid>
          ))}
        </Grid>

      )}

      <ResenasCarrusel
        resenas={resenasConImagen}
        onSelect={setResenaSeleccionada}
      />

      <ResenaModal
        resena={resenaSeleccionada}
        onClose={() => setResenaSeleccionada(null)}
      />
    </Box>
  );
}
