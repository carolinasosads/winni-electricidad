import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { Box, Typography, Button, Grid, Card, CardContent, CardMedia,Container,} from "@mui/material";

import { getServiciosActivos } from "../../../services/authService";

import { SitemarkIcon } from "../../../components/CustomIcons/CustomIcons.jsx";

/* -----------------------------
   MOCK DE RESEÑAS
--------------------------------*/
const reviews = [
  { nombre: "María G.", texto: "Excelente servicio, muy puntuales y prolijos." },
  { nombre: "Carlos R.", texto: "Me resolvieron una urgencia el mismo día." },
  { nombre: "Lucía P.", texto: "Muy claros con el presupuesto y el trabajo." },
];

export default function HomePublica() {
  const [servicios, setServicios] = useState([]);
  const [error, setError] = useState(null);

  useEffect(() => {
    const controller = new AbortController();

    getServiciosActivos(controller.signal)
      .then(setServicios)
      .catch((e) => setError(e.message));

    return () => controller.abort();
  }, []);

  return (
    <Box>
      <Box
        sx={{
          position: "sticky",
          top: 0,
          zIndex: 10,
          backgroundColor: "white",
          borderBottom: "1px solid #eee",
        }}
      >
        <Container
          sx={{
            py: 1.5,
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
            gap: 2,
          }}
        >
          <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
            <SitemarkIcon />
          </Box>
          </Container>
      </Box>

      {/* HERO */}
      <Box
        sx={{
          backgroundImage:
            "linear-gradient(rgba(0,0,0,.6), rgba(0,0,0,.6)), url('/hero-electricidad.jpg')",
          backgroundSize: "cover",
          backgroundPosition: "center",
          color: "white",
          py: 10,
          textAlign: "center",
        }}
      >
        <Typography variant="h3" gutterBottom>
          Electricistas profesionales a tu alcance
        </Typography>

        <Typography variant="h6" sx={{ mb: 4 }}>
          Creá tu cuenta y agendá tu servicio en minutos
        </Typography>

        <Button
          component={Link}
          to="/registro"
          variant="contained"
          color="primary"  
          sx={{ mr: 2 }}
        >
          Registrarme
        </Button>

        <Button
          component={Link}
          to="/login"
          variant="outlined"
          color="inherit"
          // Si querés que este también sea azul, usá:
          // color="primary"
          // variant="contained"
        >
          Iniciar sesión
        </Button>
      </Box>

      {/* RESEÑAS */}
      <Container sx={{ py: 8 }}>
        <Typography variant="h4" textAlign="center" gutterBottom>
          Lo que dicen nuestros clientes
        </Typography>

        <Grid container spacing={3} sx={{ mt: 3 }}>
          {reviews.map((r, i) => (
            <Grid item xs={12} md={4} key={i}>
              <Card sx={{ height: "100%" }}>
                <CardContent>
                  <Typography variant="body1">“{r.texto}”</Typography>
                  <Typography
                    variant="subtitle2"
                    sx={{ mt: 2, fontWeight: "bold" }}
                  >
                    {r.nombre}
                  </Typography>
                </CardContent>
              </Card>
            </Grid>
          ))}
        </Grid>
      </Container>

      {/* SERVICIOS */}
      <Box sx={{ backgroundColor: "#f5f5f5", py: 8 }}>
        <Container>
          <Typography variant="h4" textAlign="center" gutterBottom>
            Nuestros servicios
          </Typography>

          {error && (
            <Typography color="error" textAlign="center">
              {error}
            </Typography>
          )}

          <Grid container spacing={4} sx={{ mt: 3 }}>
            {servicios.map((s) => (
              <Grid item xs={12} sm={6} md={4} key={s.idServicio}>
                <Card sx={{ height: "100%" }}>
                  <CardMedia
                    component="img"
                    height="160"
                    image={s.imagenUrl || "/servicio-default.jpg"}
                    alt={s.nombre}
                  />
                  <CardContent>
                    <Typography variant="h6">{s.nombre}</Typography>
                    <Typography variant="body2" color="text.secondary">
                      {s.descripcion}
                    </Typography>
                  </CardContent>
                </Card>
              </Grid>
            ))}
          </Grid>
        </Container>
      </Box>

      {/* CONTACTO */}
      <Box sx={{ py: 6 }}>
        <Container>
          <Typography variant="h4" textAlign="center" gutterBottom>
            Contacto
          </Typography>

          <Typography textAlign="center">
            📧 Email: contacto@winnieelectricidad.com <br />
            🕒 Atención: Lunes a Viernes de 9 a 18 hs (o le agrego otro horario/ninguno?)
          </Typography>
        </Container>
      </Box>
    </Box>
  );
}
