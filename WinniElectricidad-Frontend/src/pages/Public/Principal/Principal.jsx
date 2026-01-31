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
  Rating,
  Toolbar,
  CssBaseline,
  TextField,
  Alert,
  CircularProgress,
} from "@mui/material";

import { getServiciosActivos } from "../../../services/servicioService";
import { getResenasDestacadas } from "../../../services/resenaService";
import { crearConsulta } from "../../../services/consultaService";

import { SitemarkIcon } from "../../../components/CustomIcons/CustomIcons.jsx";
import AppHeader from "/src/components/Header/Header.jsx";
import Sidebar from "/src/components/SideBar/SideBar.jsx";
import WhatsAppIcon from "@mui/icons-material/WhatsApp";
import IconButton from "@mui/material/IconButton";
import LinkedInIcon from "@mui/icons-material/LinkedIn";

export default function PaginaPrincipal() {
  const [serviciosRaw, setServiciosRaw] = useState([]);
  const [error, setError] = useState(null);

  const [resenasRaw, setResenasRaw] = useState([]);
  const [errorResenas, setErrorResenas] = useState(null);

  const [estaLogueado, setEstaLogueado] = useState(
    Boolean(localStorage.getItem("token"))
  );

  const [sidebarExpanded, setSidebarExpanded] = useState(true);

  const [consultaNombre, setConsultaNombre] = useState("");
  const [consultaEmail, setConsultaEmail] = useState("");
  const [consultaTelefono, setConsultaTelefono] = useState("");
  const [consultaMensaje, setConsultaMensaje] = useState("");

  const [enviandoConsulta, setEnviandoConsulta] = useState(false);
  const [consultaOk, setConsultaOk] = useState(null);
  const [consultaError, setConsultaError] = useState(null);

  const WHATSAPP_LINK =
  "https://wa.me/59894224578?text=Hola%20Winni%20Electricidad,%20te%20contacto%20desde%20la%20web%20para%20...";

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
    return arr
      .map((s) => {
        const idServicio = s?.id ?? s?.Id;
        const nombre = s?.titulo ?? s?.Titulo;
        const descripcion = s?.descripcion ?? s?.Descripcion;
        const imagenPrincipal =
          Array.isArray(s.imagenes) &&
          s.imagenes.find((i) => i.esPrincipal)?.url;
        const imagenUrl = imagenPrincipal || "/servicios/servicio-default.jpg";
        return { idServicio, nombre, descripcion, imagenUrl };
      })
      .filter((s) => s.idServicio != null);
  }, [serviciosRaw]);

  const reseñas = useMemo(() => {
    const arr = Array.isArray(resenasRaw) ? resenasRaw : [];
    return arr
      .map((r, idx) => {
        const nombre =
          r?.cliente?.nombre ??
          r?.nombre ??
          r?.autor ??
          "Cliente";
        const texto = r?.descripcion ?? "";
        const estrellas = r?.calificacion ?? 0;
        return {
          id: r?.id ?? idx,
          nombre,
          texto,
          estrellas: Number(estrellas) || 0,
        };
      })
      .filter((x) => (x.texto || "").trim().length > 0)
      .slice(0, 3);
  }, [resenasRaw]);

  useEffect(() => {
    const controller = new AbortController();
    getServiciosActivos(controller.signal)
      .then((data) => setServiciosRaw(data ?? []))
      .catch((e) => {
        if (e?.name === "AbortError") return;
        setError("Error al cargar servicios.");
      });
    return () => controller.abort();
  }, []);

  useEffect(() => {
    const controller = new AbortController();
    getResenasDestacadas(controller.signal)
      .then((data) => setResenasRaw(data ?? []))
      .catch((e) => {
        if (e?.name === "AbortError") return;
        setErrorResenas("Error al cargar reseñas.");
      });

    return () => controller.abort();
  }, []);

  const handleEnviarConsulta = async (e) => {
    e.preventDefault();
    setConsultaOk(null);
    setConsultaError(null);

    const mensaje = consultaMensaje.trim();

    if (estaLogueado) {
      const idCliente = localStorage.getItem("idCliente");

      if (!idCliente) {
        setConsultaError(
          "No pude identificar tu usuario logueado (idCliente). Cerrá sesión e iniciá de nuevo."
        );
        return;
      }

      if (!mensaje) {
        setConsultaError("Completá el Mensaje.");
        return;
      }

      setEnviandoConsulta(true);
      try {
        await crearConsulta({
          idCliente: Number(idCliente),
          mensaje,
        });

        setConsultaOk("¡Listo! Tu consulta fue enviada. Te vamos a responder a la brevedad.");
        setConsultaMensaje("");
      } catch (err) {
        setConsultaError(err?.message || "Ocurrió un error al enviar la consulta.");
      } finally {
        setEnviandoConsulta(false);
      }

      return;
    }

    const nombre = consultaNombre.trim();
    const email = consultaEmail.trim();

    const telefonoSoloDigitos = (consultaTelefono ?? "").replace(/\D/g, "").trim();

    if (!nombre || !email || !mensaje) {
      setConsultaError("Completá nombre, email, teléfono y mensaje.");
      return;
    }

    const emailRegex = /\S+@\S+\.\S+/;
    if (!emailRegex.test(email)) {
      setConsultaError("Ingresá un email válido.");
      return;
    }

    if (telefonoSoloDigitos.length < 8 || consultaTelefono == "") {
      setConsultaError("Ingresá un teléfono válido (mínimo 8 dígitos).");
      return;
    }

    if (telefonoSoloDigitos.length > 9) {
      setConsultaError("Ingresá un teléfono válido (máximo 9 dígitos).");
      return;
    }

    setEnviandoConsulta(true);
    try {
      await crearConsulta({
        nombre,
        email,
        telefono: telefonoSoloDigitos,
        mensaje,
      });

      setConsultaOk("¡Listo! Tu consulta fue enviada. Te vamos a responder a la brevedad.");
      setConsultaNombre("");
      setConsultaEmail("");
      setConsultaTelefono("");
      setConsultaMensaje("");
    } catch (err) {
      setConsultaError(err?.message || "Ocurrió un error al enviar la consulta.");
    } finally {
      setEnviandoConsulta(false);
    }
  };

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
            position: "relative",
            minHeight: { xs: "70vh", md: "80vh" },
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            textAlign: "center",
            backgroundImage: `url("/banner/banner-casa.png")`,
            backgroundSize: "cover",
            backgroundPosition: "center",
            backgroundRepeat: "no-repeat",
          }}
        >
        
          <style>
            {`
              @keyframes boltGlow {
                0%   { filter: drop-shadow(0 0 0 rgba(2,136,209,0)); }
                40%  { filter: drop-shadow(0 0 8px rgba(79,195,247,0.6)); }
                60%  { filter: drop-shadow(0 0 14px rgba(2,136,209,0.9)); }
                100% { filter: drop-shadow(0 0 0 rgba(2,136,209,0)); }
              }
            `}
          </style>

          <Box
            sx={{
              position: "absolute",
              inset: 0,
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
              pointerEvents: "none",
              zIndex: 1,
            }}
          >
            <Box
              sx={{
                width: { xs: "90vw", sm: "70vw", md: 520 },
                aspectRatio: "1 / 1",
                borderRadius: "50%",
                background: `
                  radial-gradient(
                    circle at center,
                    rgba(8,32,64,0.32) 0%,
                    rgba(8,32,64,0.30) 35%,
                    rgba(8,32,64,0.18) 55%,
                    rgba(8,32,64,0.08) 70%,
                    rgba(8,32,64,0.0) 100%
                  )
                `,
                backdropFilter: "blur(2.5px)",
                WebkitBackdropFilter: "blur(3px)",
              }}
            />
          </Box>

          <Container maxWidth="md" sx={{ position: "relative", zIndex: 2 }}>
            <Box
              sx={{
                display: "flex",
                flexDirection: "column",
                alignItems: "center",
                mb: 3,
              }}
            >
              <Box
                component="svg"
                viewBox="0 0 64 64"
                sx={{
                  width: { xs: 56, sm: 64, md: 72 },
                  height: { xs: 56, sm: 64, md: 72 },
                  animation: "boltGlow 3.5s ease-in-out infinite",
                }}
              >
                <defs>
                  <linearGradient id="boltGradient" x1="0" y1="0" x2="1" y2="1">
                    <stop offset="0%" stopColor="#4fc3f7" />
                    <stop offset="100%" stopColor="#0288d1" />
                  </linearGradient>
                </defs>
                <path
                  d="M36 2 L12 36 H30 L24 62 L52 26 H34 Z"
                  fill="url(#boltGradient)"
                />
              </Box>

              <Box sx={{ textAlign: "center", mt: 1 }}>
                <Typography
                  sx={{
                    fontWeight: 900,
                    letterSpacing: "0.08em",
                    fontSize: { xs: "2.2rem", sm: "2.6rem", md: "3rem" },
                    lineHeight: 1,
                    textTransform: "uppercase",
                    color: "#ffffff",
                    textShadow: "0 2px 8px rgba(0,0,0,0.35)",
                  }}
                >
                  Winni
                </Typography>

                <Typography
                  sx={{
                    fontWeight: 600,
                    letterSpacing: "0.35em",
                    fontSize: { xs: "0.85rem", sm: "0.95rem" },
                    opacity: 0.85,
                    mt: 0.4,
                    textTransform: "uppercase",
                    color: "#ffffff",
                    textShadow: "0 2px 8px rgba(0,0,0,0.35)",
                  }}
                >
                  Electricidad
                </Typography>
              </Box>
            </Box>

            {!estaLogueado && (
              <>
                <Typography
                  sx={{
                    fontSize: { xs: "1.05rem", sm: "1.15rem" },
                    maxWidth: 520,
                    mx: "auto",
                    mb: 2,
                    lineHeight: 1.55,
                    fontWeight: 500,
                    color: "rgba(255,255,255,0.9)",
                    textShadow: "0 1px 6px rgba(0,0,0,0.3)"
                  }}
                >
                  Servicio técnico profesional para tu hogar, edificio o empresa.
                </Typography>

                <Typography
                  sx={{
                    fontSize: { xs: "0.9rem", sm: "0.95rem" },
                    mb: 3,
                    fontWeight: 500,
                    color: "rgba(255,255,255,0.9)",
                    textShadow: "0 1px 6px rgba(0,0,0,0.3)"
                  }}
                >
                  Creá tu cuenta y agendá tu servicio en minutos.
                </Typography>

                <Box
                  sx={{
                    display: "flex",
                    justifyContent: "center",
                    gap: 2,
                    flexWrap: "wrap",
                    mt: 0.5,
                  }}
                >
                  <Button
                    component={Link}
                    to="/registro"
                    variant="contained"
                    size="medium"
                    sx={{
                      px: 4,
                      py: 1.2,
                      fontSize: "0.95rem",
                      fontWeight: 700,
                      borderRadius: 999,
                      boxShadow: "0 6px 16px rgba(2,136,209,0.25)",
                      background: "linear-gradient(90deg, #0288d1, #0277bd)",
                      textTransform: "uppercase",
                      letterSpacing: "0.03em",
                    }}
                  >
                    Crear cuenta
                  </Button>

                  <Button
                    component={Link}
                    to="/login"
                    variant="outlined"
                    size="medium"
                    sx={{
                      px: 4,
                      py: 1.2,
                      fontSize: "0.95rem",
                      fontWeight: 600,
                      borderRadius: 999,
                      borderWidth: 1.5,
                      borderColor: "#ffffff",
                      color: "#ffffff",
                      textTransform: "uppercase",
                      letterSpacing: "0.03em",
                      "&:hover": {
                        borderWidth: 1.5,
                        backgroundColor: "rgba(2,136,209,0.05)",
                      },
                    }}
                  >
                    Iniciar sesión
                  </Button>
                </Box>
              </>
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
              sx={{ color: "#4a6a85", letterSpacing: 1.4 }}
            >
              NUESTROS SERVICIOS
            </Typography>

            <Typography
              variant="h5"
              textAlign="center"
              sx={{
                fontWeight: 900,
                mb: 4,
                color: "#0f2a44",
              }}
            >
              Qué servicios ofrecemos
            </Typography>

            <Box
              sx={{
                width: 60,
                height: 3,
                mx: "auto",
                mb: 4,
                borderRadius: 2,
                background:
                  "linear-gradient(90deg, #0B4F9F, #1FA2FF)",
              }}
            />

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
                  component={Link}
                  to="/servicios"
                  sx={{
                    textDecoration: "none",
                    color: "inherit",

                    "&:hover": {
                      textDecoration: "none",
                    },

                    "&:visited": {
                      textDecoration: "none",
                    },

                    "&:active": {
                      textDecoration: "none",
                    },

                    "& *": {
                      textDecoration: "none !important",
                    },
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
                    image={s.imagenUrl || "/servicios/servicio-default.jpg"}
                    alt={s.nombre || "Servicio"}
                    sx={{ width: "100%", height: 260, objectFit: "cover" }}
                    onError={(e) => {
                      e.target.src = "/servicios/servicio-default.jpg";
                    }}
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
                    <Typography variant="subtitle1" sx={{ fontWeight: 800, color: "#0f2a44" }}>
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
                sx={{ color: "#4a6a85", letterSpacing: 1.4 }}
              >
                RESEÑAS
              </Typography>

              <Typography
                variant="h5"
                textAlign="center"
                sx={{
                  fontWeight: 900,
                  mb: 3,
                  color: "#0f2a44",
                }}
              >
                Lo que dicen nuestros clientes
              </Typography>

              <Box
                sx={{
                  width: 60,
                  height: 3,
                  mx: "auto",
                  mb: 4,
                  borderRadius: 2,
                  background:
                    "linear-gradient(90deg, #0B4F9F, #1FA2FF)",
                }}
              />

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
                {reseñas.map((r) => (
                  <Card
                    key={r.id}
                    component={Link}
                    to="/resenas"
                    sx={{
                      textDecoration: "none",
                      color: "inherit",

                      "&:hover": {
                        textDecoration: "none",
                      },

                      "&:visited": {
                        textDecoration: "none",
                      },

                      "&:active": {
                        textDecoration: "none",
                      },

                      "& *": {
                        textDecoration: "none !important",
                      },
                      width: { xs: "100%", sm: 300 },
                      borderRadius: 4,
                      boxShadow: "0 10px 30px rgba(0,0,0,0.08)",
                      transition: "transform 140ms ease, box-shadow 140ms ease",
                      "&:hover": {
                        transform: "translateY(-3px)",
                        boxShadow: "0 16px 42px rgba(0,0,0,0.10)",
                      },
                      "& *": {
                        textDecoration: "none",
                      },
                    }}
                  >
                    <CardContent sx={{ textAlign: "center" }}>
                      <Typography variant="subtitle1" sx={{ fontWeight: 800, color: "#0f2a44" }}>
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

        <Box sx={{ py: 6, backgroundColor: "white" }}>
          <Container maxWidth="md">
            <Typography
              variant="h4"
              textAlign="center"
              gutterBottom
              sx={{
                fontWeight: 900,
                color: "#0f2a44",
              }}
            >
              Consultas generales
            </Typography>


            <Typography textAlign="center" sx={{ color: "#4a6a85", mb: 3 }}>
              Si querés contactarte con nosotros, escribinos acá.
            </Typography>

            {consultaOk && (
              <Alert severity="success" sx={{ mb: 2 }}>
                {consultaOk}
              </Alert>
            )}

            {consultaError && (
              <Alert severity="error" sx={{ mb: 2 }}>
                {consultaError}
              </Alert>
            )}

            <Box
              component="form"
              onSubmit={handleEnviarConsulta}
              sx={{
                display: "flex",
                flexDirection: "column",
                gap: 2,
                p: 3,
                borderRadius: 3,
                border: "1px solid #eef0f4",
                boxShadow: "0 10px 30px rgba(0,0,0,0.06)",
              }}
            >
              {!estaLogueado && (
                <>
                  <TextField
                    label="Nombre"
                    value={consultaNombre}
                    onChange={(e) => setConsultaNombre(e.target.value)}
                    fullWidth
                    required
                  />

                  <TextField
                    label="Email para contactarte"
                    type="email"
                    value={consultaEmail}
                    onChange={(e) => setConsultaEmail(e.target.value)}
                    fullWidth
                    required
                  />

                  <TextField
                    label="Teléfono"
                    value={consultaTelefono}
                    onChange={(e) => {
                      setConsultaTelefono(e.target.value);
                    }}
                    fullWidth
                    required
                    inputProps={{
                      inputMode: "tel",
                      pattern: "[0-9\\s-]*",
                    }}
                    helperText="Ingresá 8 o 9 dígitos (ej: 091234567 o 29001234)"
                  />
                </>
              )}

              <TextField
                label="Mensaje"
                value={consultaMensaje}
                onChange={(e) => setConsultaMensaje(e.target.value)}
                fullWidth
                multiline
                minRows={4}
                required
              />

              <Button
                type="submit"
                variant="contained"
                disabled={enviandoConsulta}
                sx={{
                  px: 4,
                  py: 1.3,
                  fontSize: "0.9rem",
                  fontWeight: 800,
                  borderRadius: 999,
                  textTransform: "uppercase",
                  letterSpacing: "0.04em",
                  alignSelf: "center",
                  minWidth: 220,
                  background: "linear-gradient(90deg, #0288d1, #0277bd)",
                  boxShadow: "0 6px 16px rgba(2,136,209,0.25)",
                  "&:hover": {
                    background: "linear-gradient(90deg, #0277bd, #01579b)",
                    boxShadow: "0 8px 20px rgba(2,136,209,0.35)",
                  },
                }}
              >
                {enviandoConsulta ? (
                  <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                    <CircularProgress size={18} sx={{ color: "white" }} />
                    Enviando…
                  </Box>
                ) : (
                  "Enviar consulta"
                )}
              </Button>
            </Box>
          </Container>
        </Box>
        <Box
          sx={{
            py: 3,
            textAlign: "center",
          }}
        >
          <Typography
            variant="caption"
            sx={{
              color: "#8aa0b3",
              fontSize: "0.75rem",
              letterSpacing: "0.02em",
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
              gap: 0.5,
              flexWrap: "wrap",
            }}
          >
            Desarrollado por{" "}
            <Box
              component="a"
              href="https://www.linkedin.com/in/carolina-sosa-067a4b341"
              target="_blank"
              rel="noopener noreferrer"
              sx={{
                color: "#6f8fa8",
                textDecoration: "none",
                fontWeight: 600,
                display: "inline-flex",
                alignItems: "center",
                gap: 0.4,
                "&:hover": {
                  color: "#0B4F9F",
                  textDecoration: "underline",
                },
              }}
            >
              Carolina Sosa
              <LinkedInIcon sx={{ fontSize: 14 }} />
            </Box>
            -
            <Box
              component="a"
              href="https://www.linkedin.com/in/sofía-villanueva-acosta"
              target="_blank"
              rel="noopener noreferrer"
              sx={{
                color: "#6f8fa8",
                textDecoration: "none",
                fontWeight: 600,
                display: "inline-flex",
                alignItems: "center",
                gap: 0.4,
                "&:hover": {
                  color: "#0B4F9F",
                  textDecoration: "underline",
                },
              }}
            >
              Sofía Villanueva
              <LinkedInIcon sx={{ fontSize: 14 }} />
            </Box>
          </Typography>
        </Box>
      </Box>
      

      <IconButton
        component="a"
        href={WHATSAPP_LINK}
        target="_blank"
        rel="noopener noreferrer"
        aria-label="Contactar por WhatsApp"
        sx={{
          position: "fixed",
          bottom: 24,
          right: 24,
          backgroundColor: "#25D366",
          color: "white",
          width: 56,
          height: 56,
          boxShadow: "0 8px 24px rgba(0,0,0,0.25)",
          "&:hover": {
            backgroundColor: "#1ebe5d",
          },
          zIndex: 2000,
        }}
      >
        <WhatsAppIcon sx={{ fontSize: 30 }} />
      </IconButton>
    </Box>
  );
}

