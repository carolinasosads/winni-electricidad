import { Box, Typography, Card, CardActionArea } from "@mui/material";
import EventAvailableIcon from "@mui/icons-material/EventAvailable";
import RateReviewIcon from "@mui/icons-material/RateReview";
import BuildIcon from "@mui/icons-material/Build";
import { useNavigate } from "react-router-dom";
import ListAltIcon from "@mui/icons-material/ListAlt";

export default function ClienteDashboard() {
  const navigate = useNavigate();

  const acciones = [
    {
      titulo: "Agendar presupuesto",
      descripcion:
        "Solicitá una visita de presupuesto.",
      icono: EventAvailableIcon,
      ruta: "/cliente/agenda",
    },
    {
      titulo: "Servicios",
      descripcion: "Explorá los servicios disponibles.",
      icono: BuildIcon,
      ruta: "/cliente/servicios",
    },
    {
      titulo: "Reseñas",
      descripcion: "Ver o crear una reseña.",
      icono: RateReviewIcon,
      ruta: "/cliente/resenas",
    },
    {
      titulo: "Mis reservas",
      descripcion: "Consultá el estado y el historial de tus reservas.",
      icono: ListAltIcon,
      ruta: "/cliente/mis-reservas",
    },

  ];

  return (
    <Box
      sx={{
        width: "100%",
        maxWidth: "100vw",
        overflowX: "hidden",
        px: { xs: 2, sm: 0 },
        py: { xs: 3, sm: 5 },
        display: "flex",
        justifyContent: "center",
        boxSizing: "border-box",
      }}
    >
      <Box
        sx={{
          width: "100%",
          maxWidth: 900,
          display: "flex",
          flexDirection: "column",
          gap: 4,
          boxSizing: "border-box",
        }}
      >
        {/* Header */}
        <Box>
          <Box
            sx={{
              width: 48,
              height: 4,
              backgroundColor: "primary.main",
              borderRadius: 2,
              mb: 1.5,
            }}
          />

          <Typography variant="h4" fontWeight={600}>
            Hola!
          </Typography>

          <Typography color="text.secondary" mt={1}>
            Desde acá podés consultar disponibilidad, explorar servicios y compartir tu experiencia con Winni Electricidad.
          </Typography>
        </Box>

        {/* Acciones */}
        <Box
          sx={{
            display: "grid",
            gridTemplateColumns: {
              xs: "1fr",
              sm: "repeat(3, 1fr)",
            },
            gap: 3,
            width: "100%",
          }}
        >
          {acciones.map((accion) => {
            const Icono = accion.icono;

            return (
              <Card
                key={accion.titulo}
                elevation={0}
                sx={{
                  display: "flex",
                  borderRadius: 2,
                  border: "1px solid",
                  borderColor: "divider",
                  backgroundColor: "background.paper",
                  minWidth: 0,
                  boxSizing: "border-box",
                  transition: "transform .2s ease, box-shadow .2s ease",

                  "&:hover": {
                    boxShadow: 2,
                    transform: "translateY(-2px)",
                  },
                }}
              >
                <CardActionArea
                  onClick={() => navigate(accion.ruta)}
                  TouchRippleProps={{
                    sx: {
                      color: "rgba(25,118,210,0.25)",
                    },
                  }}
                  sx={{
                    flex: 1,
                    p: { xs: 2, sm: 3 },
                    display: "flex",
                    flexDirection: "column",
                    alignItems: "flex-start",
                    gap: 2,
                    minWidth: 0,

                    "&:hover, &:active": {
                      backgroundColor: "rgba(25,118,210,0.06)",
                    },

                    "& .MuiCardActionArea-focusHighlight": {
                      backgroundColor: "transparent",
                    },
                  }}
                >
                  <Icono
                    sx={{
                      fontSize: 32,
                      color: "primary.main",
                      flexShrink: 0,
                    }}
                  />

                  <Typography fontWeight={600}>
                    {accion.titulo}
                  </Typography>

                  <Typography
                    variant="body2"
                    color="text.secondary"
                  >
                    {accion.descripcion}
                  </Typography>
                </CardActionArea>
              </Card>
            );
          })}
        </Box>
      </Box>
    </Box>
  );
}
