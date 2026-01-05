import { Box, Typography, Card, CardActionArea } from "@mui/material";
import EventNoteIcon from "@mui/icons-material/EventNote";
import AddCircleOutlineIcon from "@mui/icons-material/AddCircleOutline";
import PeopleAltIcon from "@mui/icons-material/PeopleAlt";
import BuildIcon from "@mui/icons-material/Build";
import RateReviewIcon from "@mui/icons-material/RateReview";
import { useNavigate } from "react-router-dom";

export default function DashboardAdmin() {
  const navigate = useNavigate();

  const acciones = [
    {
      titulo: "Panel de reservas",
      descripcion: "Gestioná y visualizá todas las reservas.",
      icono: EventNoteIcon,
      ruta: "/admin/panel-reservas",
    },
    {
      titulo: "Gestión de presupuestos",
      descripcion: "Agregá un nuevo presupuesto a una reserva y cliente preexistente o creala de cero.",
      icono: AddCircleOutlineIcon,
      ruta: "/admin/crear-reserva",
    },
    {
      titulo: "Ficha de clientes",
      descripcion: "Consultá información de los clientes.",
      icono: PeopleAltIcon,
      ruta: "/admin/ficha-clientes",
    },
    {
      titulo: "Servicios",
      descripcion: "Administrá los servicios disponibles.",
      icono: BuildIcon,
      ruta: "/admin/servicios",
    },
    {
      titulo: "Reseñas",
      descripcion: "Moderá y revisá reseñas de clientes.",
      icono: RateReviewIcon,
      ruta: "/admin/resenas",
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
          maxWidth: 1100,
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
            Panel de administración
          </Typography>

          <Typography color="text.secondary" mt={1}>
            Desde acá podés gestionar reservas, presupuestos, clientes, servicios y reseñas del sistema.
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
