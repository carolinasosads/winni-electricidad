import { Box, Grid, Typography, Divider } from "@mui/material";
import { useEffect, useMemo, useState } from "react";
import ResenasHeader from "./ResenasHeader";
import ResenasFiltros from "./ResenasFiltros";
import ResenaCard from "./ResenaCard";

// reemplazá por tu auth real
const useAuth = () => ({
  role: "Administrador" // "Usuario" | "Administrador" | null
});

// reemplazá por tu service real
const fetchResenas = async () => [
  {
    id: 1,
    usuario: "María González",
    fecha: "2024-11-12",
    descripcion: "Excelente servicio, muy prolijos y puntuales.",
    calificacion: 5,
    servicio: "Electricidad",
    imagenUrl: "/resenas/ejemplo1.jpg"
  },
  {
    id: 2,
    usuario: "Juan Pérez",
    fecha: "2024-10-03",
    descripcion: "Buen trabajo, aunque demoró un poco más de lo esperado.",
    calificacion: 4,
    servicio: "Sanitaria",
    imagenUrl: null
  }
];

export default function ResenasPage() {
  const { role } = useAuth();

  const [resenas, setResenas] = useState([]);
  const [servicioFiltro, setServicioFiltro] = useState("Todos");
  const [calificacionFiltro, setCalificacionFiltro] = useState("Todas");

  useEffect(() => {
    fetchResenas().then(setResenas);
  }, []);

  // ⭐ Promedio general (AC3)
  const promedio = useMemo(() => {
    if (resenas.length === 0) return 0;
    return (
      resenas.reduce((acc, r) => acc + r.calificacion, 0) / resenas.length
    ).toFixed(1);
  }, [resenas]);

  // 🔍 Filtros (AC2)
  const resenasFiltradas = useMemo(() => {
    return resenas.filter(r => {
      const servicioOk =
        servicioFiltro === "Todos" || r.servicio === servicioFiltro;

      const calificacionOk =
        calificacionFiltro === "Todas" ||
        r.calificacion === Number(calificacionFiltro);

      return servicioOk && calificacionOk;
    });
  }, [resenas, servicioFiltro, calificacionFiltro]);

  // 🗑️ Eliminar (AC4 + AC5)
  const eliminarResena = id => {
    if (!window.confirm("¿Eliminar esta reseña?")) return;
    setResenas(prev => prev.filter(r => r.id !== id));
  };

  return (
    <Box sx={{ maxWidth: 1200, mx: "auto", px: 2, py: 4 }}>
      <ResenasHeader promedio={promedio} total={resenas.length} />

      <ResenasFiltros
        servicio={servicioFiltro}
        calificacion={calificacionFiltro}
        onServicioChange={setServicioFiltro}
        onCalificacionChange={setCalificacionFiltro}
      />

      <Divider sx={{ mb: 3 }} />

      {resenasFiltradas.length === 0 ? (
        <Typography textAlign="center" color="text.secondary" mt={6}>
          No hay reseñas para mostrar.
        </Typography>
      ) : (
        <Grid container spacing={3}>
          {resenasFiltradas.map(resena => (
            <Grid item xs={12} md={6} key={resena.id}>
              <ResenaCard
                resena={resena}
                esAdmin={role === "Administrador"}
                onEliminar={eliminarResena}
              />
            </Grid>
          ))}
        </Grid>
      )}
    </Box>
  );
}
