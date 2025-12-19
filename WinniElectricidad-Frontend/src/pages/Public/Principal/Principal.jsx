import { useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import {
  Box,
  Typography,
  Button,
  Card,
  CardContent,
  CardMedia,
  Container,
} from "@mui/material";

import { getServiciosActivos } from "../../../services/authService";
import { SitemarkIcon } from "../../../components/CustomIcons/CustomIcons.jsx";

/* -----------------------------
   RESEÑAS (por ahora mock)
--------------------------------*/
const reseñasMock = [
  {
    nombre: "María G.",
    texto: "Excelente servicio, muy puntuales y prolijos. Recomiendo.",
    estrellas: 5,
  },
  {
    nombre: "Carlos R.",
    texto: "Me resolvieron una urgencia el mismo día. Muy buena atención.",
    estrellas: 5,
  },
  {
    nombre: "Lucía P.",
    texto: "Muy claros con el presupuesto y el trabajo. Todo impecable.",
    estrellas: 5,
  },
];

function Stars({ value = 5 }) {
  const n = Math.max(0, Math.min(5, Number(value) || 0));
  return (
    <Box sx={{ display: "flex", justifyContent: "center", gap: 0.25 }}>
      {Array.from({ length: 5 }).map((_, i) => (
        <Box
          key={i}
          component="span"
          sx={{
            fontSize: 14,
            lineHeight: 1,
            color: i < n ? "#F6B100" : "#D0D5DD",
          }}
        >
          ★
        </Box>
      ))}
    </Box>
  );
}

export default function PaginaPrincipal() {
  const [serviciosRaw, setServiciosRaw] = useState([]);
  const [error, setError] = useState(null);

  const servicios = useMemo(() => {
    const arr = Array.isArray(serviciosRaw) ? serviciosRaw : [];

    return arr
      .map((s) => {
        const idServicio = s?.id ?? s?.Id;
        const nombre = s?.titulo ?? s?.Titulo;
        const descripcion = s?.descripcion ?? s?.Descripcion;

        let imagenUrl = s?.imagenUrl ?? s?.ImagenUrl;
        if (typeof imagenUrl === "string") {
          imagenUrl = imagenUrl.trim();
          if (
            imagenUrl &&
            !imagenUrl.startsWith("http") &&
            !imagenUrl.startsWith("/")
          ) {
            imagenUrl = `/${imagenUrl}`;
          }
        } else {
          imagenUrl = null;
        }

        return { idServicio, nombre, descripcion, imagenUrl };
      })
      .filter((s) => s.idServicio != null);
  }, [serviciosRaw]);

  useEffect(() => {
    const controller = new AbortController();
    setError(null);

    getServiciosActivos(controller.signal)
      .then((data) => setServiciosRaw(data))
      .catch((e) => {
        if (e?.name === "AbortError") return;
        setError(e?.message || "Error al cargar servicios.");
      });

    return () => controller.abort();
  }, []);

  const reseñas = reseñasMock.slice(0, 3);

  return (
    <Box>
      {/* HEADER */}
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
            py: 1.25,
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
          }}
        >
          <SitemarkIcon />
        </Container>
      </Box>

      {/* HERO (compacto) */}
      <Box
        sx={{
          backgroundImage:
            "linear-gradient(rgba(0,0,0,.55), rgba(0,0,0,.55)), url('/hero-electricidad.jpg')",
          backgroundSize: "cover",
          backgroundPosition: "center",
          color: "white",
          py: { xs: 5, md: 6 },
          textAlign: "center",
        }}
      >
        <Container>
          <Typography variant="h4" sx={{ fontWeight: 800 }} gutterBottom>
            Winni Electricidad
          </Typography>

          <Typography variant="body1" sx={{ opacity: 0.95, mb: 2.5 }}>
            Creá tu cuenta y agendá tu servicio en minutos.
          </Typography>

          <Button
            component={Link}
            to="/registro"
            variant="contained"
            size="small"
            sx={{ mr: 1 }}
          >
            Registrarme
          </Button>

          <Button
            component={Link}
            to="/login"
            variant="outlined"
            size="small"
            color="inherit"
          >
            Iniciar sesión
          </Button>
        </Container>
      </Box>

      {/* SERVICIOS: 3 arriba centrados + 2 abajo centrados */}
      <Box sx={{ backgroundColor: "#f7f8fb", py: { xs: 5, md: 7 } }}>
        <Container>
          <Typography
            variant="overline"
            display="block"
            textAlign="center"
            sx={{ color: "text.secondary", letterSpacing: 1.4 }}
          >
            NUESTROS SERVICIOS
          </Typography>

          <Typography
            variant="h5"
            textAlign="center"
            sx={{ fontWeight: 800, mb: 4 }}
          >
            Qué servicio ofrecemos
          </Typography>

          {error && (
            <Typography color="error" textAlign="center" sx={{ mb: 2 }}>
              {error}
            </Typography>
          )}

          <Box
            sx={{
              maxWidth: { xs: "100%", md: 920 },
              mx: "auto",
              display: "flex",
              flexWrap: "wrap",
              justifyContent: "center",
              gap: 3,
            }}
          >
            {servicios.map((s) => (
              <Card
                key={s.idServicio}
                sx={{
                  width: { xs: "100%", sm: 280 },
                  height: 280,
                  borderRadius: 3,
                  border: "1px solid #eef0f4",
                  boxShadow: "0 10px 30px rgba(0,0,0,0.06)",
                  transition: "transform 140ms ease, box-shadow 140ms ease",
                  "&:hover": {
                    transform: "translateY(-3px)",
                    boxShadow: "0 16px 42px rgba(0,0,0,0.10)",
                  },
                  overflow: "hidden",
                  display: "flex",
                  flexDirection: "column",
                  alignItems: "center",
                  textAlign: "center",
                }}
              >
                <Box
                  sx={{
                    mt: 2.5,
                    width: 68,
                    height: 68,
                    borderRadius: 3,
                    overflow: "hidden",
                    boxShadow: "0 10px 24px rgba(0,0,0,0.08)",
                    backgroundColor: "#fff",
                  }}
                >
                  <CardMedia
                    component="img"
                    image={s.imagenUrl || "/servicio-default.jpg"}
                    alt={s.nombre || "Servicio"}
                    sx={{
                      width: "100%",
                      height: "100%",
                      objectFit: "cover",
                    }}
                  />
                </Box>

                <CardContent
                  sx={{
                    px: 2.5,
                    pt: 2,
                    pb: 2.5,
                    width: "100%",
                    display: "flex",
                    flexDirection: "column",
                    gap: 1,
                    flex: 1,
                  }}
                >
                  <Typography variant="subtitle1" sx={{ fontWeight: 800 }}>
                    {s.nombre || "Servicio"}
                  </Typography>

                  <Typography
                    variant="body2"
                    color="text.secondary"
                    sx={{
                      lineHeight: 1.55,
                      whiteSpace: "normal",
                      overflowWrap: "anywhere",
                      wordBreak: "break-word",
                      flex: 1,
                    }}
                  >
                    {s.descripcion || "Sin descripción."}
                  </Typography>
                </CardContent>
              </Card>
            ))}
          </Box>

          {/* RESEÑAS: fila con 3 cuadrados centrados */}
          <Box sx={{ mt: 6 }}>
            <Typography
              variant="overline"
              display="block"
              textAlign="center"
              sx={{ color: "text.secondary", letterSpacing: 1.4 }}
            >
              RESEÑAS
            </Typography>

            <Typography
              variant="h5"
              textAlign="center"
              sx={{ fontWeight: 800, mb: 3 }}
            >
              Lo que dicen nuestros clientes
            </Typography>

            <Box
              sx={{
                maxWidth: { xs: "100%", md: 920 },
                mx: "auto",
                display: "flex",
                justifyContent: "center",
                gap: 3,
                flexWrap: { xs: "wrap", md: "nowrap" }, // en desktop quedan 3 en fila
              }}
            >
              {reseñas.map((r, idx) => (
                <Card
                  key={idx}
                  sx={{
                    width: { xs: "100%", sm: 280 },
                    height: 280,
                    borderRadius: 3,
                    border: "1px solid #eef0f4",
                    boxShadow: "0 10px 30px rgba(0,0,0,0.06)",
                    overflow: "hidden",
                    display: "flex",
                    flexDirection: "column",
                    textAlign: "center",
                    p: 0,
                  }}
                >
                  <CardContent
                    sx={{
                      p: 3,
                      display: "flex",
                      flexDirection: "column",
                      gap: 1.25,
                      flex: 1,
                    }}
                  >
                    <Stars value={r.estrellas} />

                    <Typography
                      variant="body2"
                      color="text.secondary"
                      sx={{
                        lineHeight: 1.6,
                        whiteSpace: "normal",
                        overflowWrap: "anywhere",
                        wordBreak: "break-word",
                        flex: 1,
                      }}
                    >
                      “{r.texto}”
                    </Typography>

                    <Typography variant="subtitle2" sx={{ fontWeight: 800 }}>
                      {r.nombre}
                    </Typography>
                  </CardContent>
                </Card>
              ))}
            </Box>
          </Box>
        </Container>
      </Box>

      {/* CONTACTO */}
      <Box sx={{ py: 4 }}>
        <Container>
          <Typography
            variant="h6"
            textAlign="center"
            gutterBottom
            sx={{ fontWeight: 800 }}
          >
            Contacto
          </Typography>

          <Typography variant="body2" textAlign="center" color="text.secondary">
            📧 contacto@winnieelectricidad.com <br />
            🕒 Lunes a Viernes · 9 a 18 hs
          </Typography>
        </Container>
      </Box>
    </Box>
  );
}
