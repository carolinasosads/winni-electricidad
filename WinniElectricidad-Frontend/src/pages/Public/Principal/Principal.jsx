import { useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import {  Box,  Typography,  Button,  Card,  CardContent,  CardMedia,  Container,  Rating,  Toolbar,  CssBaseline,} from "@mui/material";

import { getServiciosActivos } from "../../../services/servicioService";
import { getResenasAprobadas } from "../../../services/resenaService";

import { SitemarkIcon } from "../../../components/CustomIcons/CustomIcons.jsx";
import AppHeader from "/src/components/Header/Header.jsx";
import Sidebar from "/src/components/SideBar/SideBar.jsx"; 

export default function PaginaPrincipal() {
  const [serviciosRaw, setServiciosRaw] = useState([]);
  const [error, setError] = useState(null);

  const [resenasRaw, setResenasRaw] = useState([]);
  const [errorResenas, setErrorResenas] = useState(null);

  const [estaLogueado, setEstaLogueado] = useState(
    Boolean(localStorage.getItem("token"))
  );

  const [sidebarExpanded, setSidebarExpanded] = useState(true);

  useEffect(() => {
    const handler = () => {
      const token = localStorage.getItem("token");
      setEstaLogueado(Boolean(token));
    };
    window.addEventListener("auth-change", handler);
    handler();
    return () => window.removeEventListener("auth-change", handler);
  }, []);

 useEffect(() => {
  const body = document.body;
  const html = document.documentElement;

  let prevBodyOverflow;
  let prevHtmlOverflow;
  let prevBodyPaddingRight;
  let hasModified = false;

  if (estaLogueado) {
    prevBodyOverflow = body.style.overflow;
    prevHtmlOverflow = html.style.overflow;
    prevBodyPaddingRight = body.style.paddingRight;

    body.style.overflow = "auto";
    html.style.overflow = "auto";

    hasModified = true;
  }

  return () => {
    if (hasModified) {
      body.style.overflow = prevBodyOverflow;
      html.style.overflow = prevHtmlOverflow;
      body.style.paddingRight = prevBodyPaddingRight;
    }
  };
}, [estaLogueado, sidebarExpanded]);

  const servicios = useMemo(() => {
    const arr = Array.isArray(serviciosRaw) ? serviciosRaw : [];

    const imagenesPorServicio = {
      electricidad: "/servicios/electricidad.jpeg",
      sanitaria: "/servicios/sanitaria.jpeg",
      climatizacion: "/servicios/climatizacion.jpeg",
      "climatización": "/servicios/climatizacion.jpeg",
      riego: "/servicios/riego.jpg",
      otro: "/servicios/otros.jpeg",
      otros: "/servicios/otros.jpeg",
    };

    return arr
      .map((s) => {
        const idServicio = s?.id ?? s?.Id;
        const nombre = s?.titulo ?? s?.Titulo;
        const descripcion = s?.descripcion ?? s?.Descripcion;

        let imagenUrl = s?.imagenUrl ?? s?.ImagenUrl ?? null;

        if (!imagenUrl && typeof nombre === "string") {
          const key = nombre.trim().toLowerCase();
          imagenUrl = imagenesPorServicio[key] ?? null;
        }

        if (typeof imagenUrl === "string") {
          imagenUrl = imagenUrl.trim();

          if (
            imagenUrl &&
            !imagenUrl.startsWith("http") &&
            !imagenUrl.startsWith("/")
          ) {
            imagenUrl = `/${imagenUrl}`;
          }
        }

        if (!imagenUrl) imagenUrl = "/servicio-default.jpg";

        return { idServicio, nombre, descripcion, imagenUrl };
      })
      .filter((s) => s.idServicio != null);
  }, [serviciosRaw]);

  const reseñas = useMemo(() => {
    const arr = Array.isArray(resenasRaw) ? resenasRaw : [];

    return arr.map((r, idx) => {
      const nombre =
        r?.cliente?.nombre ??
        r?.cliente?.Nombre ??
        r?.nombreCliente ??
        r?.NombreCliente ??
        r?.nombre ??
        r?.Nombre ??
        r?.autor ??
        r?.Autor ??
        "Cliente";

      const texto =
        r?.descripcion ??
        r?.Descripcion ??
        r?.texto ??
        r?.Texto ??
        r?.detalle ??
        r?.Detalle ??
        "";

      const estrellas =
        r?.calificacion ??
        r?.Calificacion ??
        r?.estrellas ??
        r?.Estrellas ??
        r?.puntuacion ??
        r?.Puntuacion ??
        0;

        let imagenUrl =
          r?.imagenUrl ??
          r?.ImagenUrl ??
          r?.urlImagen ??
          r?.UrlImagen ??
          r?.imagen ??
          r?.Imagen ??
          null;

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

        return {
          id: r?.id ?? r?.Id ?? `${nombre}-${idx}`,
          nombre,
          texto,
          estrellas: Number(estrellas) || 0,
          imagenUrl,
        };
      })
      .filter((x) => (x.texto || "").trim().length > 0)
      .slice(0, 3);
  }, [resenasRaw]);

  useEffect(() => {
    const controller = new AbortController();
    setError(null);

    getServiciosActivos(controller.signal)
      .then((data) => {
        const lista =
          (Array.isArray(data) && data) ||
          data?.servicios ||
          data?.Servicios ||
          data?.items ||
          data?.Items ||
          data?.data ||
          data?.Data ||
          data?.$values ||
          [];
        setServiciosRaw(Array.isArray(lista) ? lista : []);
      })
      .catch((e) => {
        if (e?.name === "AbortError") return;
        setError(e?.message || "Error al cargar servicios.");
      });

    return () => controller.abort();
  }, []);

  useEffect(() => {
    const controller = new AbortController();
    setErrorResenas(null);

    getResenasAprobadas(controller.signal)
      .then((data) => {
        const lista =
          (Array.isArray(data) && data) ||
          data?.resenas ||
          data?.Resenas ||
          data?.items ||
          data?.Items ||
          data?.data ||
          data?.Data ||
          data?.$values ||
          [];

        setResenasRaw(Array.isArray(lista) ? lista : []);
      })
      .catch((e) => {
        if (e?.name === "AbortError") return;
        setErrorResenas(e?.message || "Error al cargar reseñas.");
      });

    return () => controller.abort();
  }, []);

  return (
    <Box sx={{ display: "flex", width: "100%" }}>
      <CssBaseline />

      {estaLogueado && (
        <Sidebar expanded={sidebarExpanded} setExpanded={setSidebarExpanded} />
      )}

      <Box
        component="main"
        sx={{
          flexGrow: 1,
          minWidth: 0,
          width: "100%",
          minHeight: "100vh",
          overflowY: "auto",
        }}
      >
        <AppHeader
          logo={<SitemarkIcon />}
          title=""
          showMenuButton={estaLogueado}
          menuOpen={sidebarExpanded}
          onToggleMenu={setSidebarExpanded}
          homeHref="/"
        />
        <Toolbar />

        <Box
          sx={{
            backgroundImage:
              "linear-gradient(135deg, #0f2027, #203a43, #2c5364)",
            color: "white",
            py: { xs: 10, md: 14 },
            textAlign: "center",
            boxShadow: "inset 0 -60px 100px rgba(0,0,0,0.35)",
          }}
        >
          <Container maxWidth="md">
            <Typography
              variant="h2"
              sx={{
                fontWeight: 900,
                mb: 2,
                fontSize: { xs: "2.6rem", sm: "3.2rem", md: "3.8rem" },
              }}
            >
              Winni Electricidad
            </Typography>

            {!estaLogueado && (
              <Typography
                variant="h6"
                sx={{
                  opacity: 0.95,
                  mb: 4,
                  fontSize: { xs: "1.1rem", sm: "1.25rem", md: "1.35rem" },
                }}
              >
                Creá tu cuenta y agendá tu servicio en minutos.
              </Typography>
            )}

            {!estaLogueado && (
              <Box sx={{ display: "flex", justifyContent: "center", gap: 2 }}>
                <Button
                  component={Link}
                  to="/registro"
                  variant="contained"
                  size="large"
                  sx={{ px: 4, py: 1.4, fontSize: "0.95rem", fontWeight: 700 }}
                >
                  Registrarme
                </Button>

                <Button
                  component={Link}
                  to="/login"
                  variant="outlined"
                  size="large"
                  color="inherit"
                  sx={{
                    px: 4,
                    py: 1.4,
                    fontSize: "0.95rem",
                    fontWeight: 700,
                    borderColor: "rgba(255,255,255,0.85)",
                  }}
                >
                  Iniciar sesión
                </Button>
              </Box>
            )}
          </Container>
        </Box>

          {/* servicios y reseñas */}
          <Box sx={{ backgroundColor: "#f7f8fb", py: { xs: 5, md: 7 } }}>
              <Container maxWidth="lg">
                  {/* SERVICIOS */}
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
                      Qué servicios ofrecemos
                  </Typography>

                  {error && (
                      <Typography color="error" textAlign="center" sx={{ mb: 2 }}>
                          {error}
                      </Typography>
                  )}

                  <Box
                      sx={{
                          maxWidth: { xs: "100%", md: 980 },
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
                                  width: { xs: "100%", sm: 300 },
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
                              }}
                          >
                              <CardMedia
                                  component="img"
                                  image={s.imagenUrl || "/servicio-default.jpg"}
                                  alt={s.nombre || "Servicio"}
                                  sx={{ width: "100%", height: 170, objectFit: "cover" }}
                              />

                              <CardContent
                                  sx={{
                                      px: 2.5,
                                      pt: 2,
                                      pb: 2.5,
                                      display: "flex",
                                      flexDirection: "column",
                                      gap: 1,
                                      textAlign: "center",
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
                                      }}
                                  >
                                      {s.descripcion || "Sin descripción."}
                                  </Typography>
                              </CardContent>
                          </Card>
                      ))}
                  </Box>

                  {/* RESEÑAS */}
                  <Box sx={{ mt: 7 }}>
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

                      {errorResenas && (
                          <Typography color="error" textAlign="center" sx={{ mb: 2 }}>
                              {errorResenas}
                          </Typography>
                      )}

                      <Box
                          sx={{
                              maxWidth: { xs: "100%", md: 980 },
                              mx: "auto",
                              display: "flex",
                              flexWrap: "wrap",
                              justifyContent: "center",
                              gap: 3,
                          }}
                      >
                          {reseñas.slice(0, 3).map((r) => (
                              <Card
                                  key={r.id}
                                  sx={{
                                      width: { xs: "100%", sm: 300 },
                                      borderRadius: 4,
                                      boxShadow: "0 10px 30px rgba(0,0,0,0.08)",
                                  }}
                              >
                                  <CardContent sx={{ textAlign: "center" }}>
                                      <Typography variant="subtitle1" sx={{ fontWeight: 800 }}>
                                          {r.nombre}
                                      </Typography>

                                      <Box sx={{ display: "flex", justifyContent: "center", mt: 1, mb: 1 }}>
                                          <Rating value={Number(r.estrellas) || 0} readOnly />
                                      </Box>

                                      <Typography
                                          variant="body2"
                                          color="text.secondary"
                                          sx={{
                                              lineHeight: 1.6,
                                              whiteSpace: "normal",
                                              overflowWrap: "anywhere",
                                              wordBreak: "break-word",
                                          }}
                                      >
                                          “{r.texto}”
                                      </Typography>
                                  </CardContent>
                              </Card>
                          ))}
                      </Box>

                      {!errorResenas && reseñas.length === 0 && (
                          <Typography color="text.secondary" textAlign="center" sx={{ mt: 2 }}>
                              Aún no hay reseñas para mostrar.
                          </Typography>
                      )}
                  </Box>
              </Container>
          </Box>

          {/* CONTACTO */}
          <Box sx={{ py: 6, backgroundColor: "white" }}>
              <Container maxWidth="md">
                  <Typography
                      variant="h4"
                      textAlign="center"
                      gutterBottom
                      sx={{ fontWeight: 900 }}
                  >
                      Contacto
                  </Typography>

                  <Typography textAlign="center" sx={{ color: "text.secondary" }}>
                      <span aria-hidden="true">📧</span> Email: washivillanueva@gmail.com
                      <br />
                      <span aria-hidden="true">🕒</span> Atención: Lunes a Viernes de 9 a 18 hs
                  </Typography>
              </Container>
          </Box>
      </Box>
    </Box>
  );
}
