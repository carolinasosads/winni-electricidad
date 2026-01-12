import { Box, Typography, Dialog, DialogTitle, DialogContent, DialogActions, Button, Alert, Toolbar, IconButton, Chip } from "@mui/material";
import Divider from '@mui/material/Divider';
import ChevronLeftRoundedIcon from "@mui/icons-material/ChevronLeftRounded";
import ChevronRightRoundedIcon from "@mui/icons-material/ChevronRightRounded";
import StarIcon from "@mui/icons-material/Star";
import StarBorderIcon from "@mui/icons-material/StarBorder";
import HowToRegIcon from '@mui/icons-material/HowToReg';
import { TextField } from "@mui/material";

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
  crearServicio,
  editarServicio
} from "../../../services/servicioService";

export default function ServiciosPage() {
  const rol = localStorage.getItem("rol");
  const estaLogueado = Boolean(localStorage.getItem("token"));

  const navigate = useNavigate();

  const [servicios, setServicios] = useState([]);
  const [servicioAccion, setServicioAccion] = useState(null);

  const [errorMsg, setErrorMsg] = useState("");
  const [successMsg, setSuccessMsg] = useState("");
  const [errorEditar, setErrorEditar] = useState("");

  const [servicioSeleccionado, setServicioSeleccionado] = useState(null);
  const [imgModalIndex, setImgModalIndex] = useState(0);

  const [servicioEditando, setServicioEditando] = useState(null);
  const [imagenesEditando, setImagenesEditando] = useState([]);

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
              try{
                setErrorMsg("");
                setSuccessMsg("");
                const formData = new FormData();

                formData.append("Titulo", nuevoServicio.titulo);
                formData.append("Descripcion", nuevoServicio.descripcion);

                nuevoServicio.imagenes.forEach((img) => {
                  formData.append("imagenes", img);
                });

                await crearServicio(formData);
                await reloadServicios();
                setSuccessMsg("¡Servicio creado con éxito!");
              } catch (error) {
                setErrorMsg(error.message || "Error al crear el servicio.");
              }
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
            onOpenGaleria={(index) => {
              setServicioSeleccionado(servicio);
              setImgModalIndex(index);
            }}
            onEditar={(servicio) => {
              setServicioEditando(servicio);
              setImagenesEditando((servicio.imagenes || []).map(img => ({
                ...img,
                nueva: false
              })));
            }}
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

      <Dialog
        open={Boolean(servicioEditando)}
        onClose={() => {
          setServicioEditando(null);
          setErrorEditar("");
        }}
        fullWidth
        maxWidth="md"
      >

        <DialogTitle><strong>Editar servicio</strong></DialogTitle>

        {servicioEditando && (
          <DialogContent
            sx={{
              maxHeight: "70vh",
              overflowY: "auto",
              display: "flex",
              flexDirection: "column",
              gap: 3
            }}
          >
            {errorEditar && (
              <Alert severity="error" sx={{ mb: 2 }}>
                {errorEditar}
              </Alert>
            )}
            <Typography
              variant="subtitle1"
              sx={{ fontWeight: 500, color: "text.secondary" }}
            >
              Datos del servicio
            </Typography>
            <Divider />
            <TextField
              label="Título"
              value={servicioEditando.titulo}
              onChange={(e) =>
                setServicioEditando({
                  ...servicioEditando,
                  titulo: e.target.value
                })
              }
            />

            <TextField
              label="Descripción"
              multiline
              minRows={3}
              value={servicioEditando.descripcion}
              onChange={(e) =>
                setServicioEditando({
                  ...servicioEditando,
                  descripcion: e.target.value
                })
              }
            />

            <Typography
              variant="subtitle1"
              sx={{ fontWeight: 500, color: "text.secondary", mt: 2 }}
            >
              Imágenes del servicio
            </Typography>

          <Divider />

          <Box
            sx={{
              display: "grid",
              gridTemplateColumns: "repeat(auto-fill, minmax(160px, 1fr))",
              gap: 2
            }}
          >
            {imagenesEditando.map((img, index) => (
              <Box
                key={img.id ?? index}
                sx={{
                  borderRadius: 2,
                  overflow: "hidden",
                  position: "relative",
                  cursor: img.esPrincipal ? "default" : "pointer",
                  border: img.esPrincipal
                    ? "2px solid #1976d2"
                    : "1px solid rgba(0,0,0,0.12)",
                  "&:hover .principal-overlay": {
                    opacity: img.esPrincipal ? 0 : 1
                  }
                }}
                onClick={() => {
                  if (img.esPrincipal) return;

                  setImagenesEditando((prev) =>
                    prev.map((i, iIdx) => ({
                      ...i,
                      esPrincipal: iIdx === index
                    }))
                  );
                }}
              >
                {/* Imagen */}
                <Box
                  component="img"
                  src={img.url}
                  sx={{
                    width: "100%",
                    height: 120,
                    objectFit: "cover",
                    display: "block"
                  }}
                />

                <IconButton
                  size="small"
                  onClick={(e) => {
                    e.stopPropagation();
                    setImagenesEditando((prev) =>
                      prev.map((i, iIdx) => ({
                        ...i,
                        esPrincipal: iIdx === index
                      }))
                    );
                  }}
                  sx={{
                    position: "absolute",
                    bottom: 6,
                    right: 6,
                    width: 26,
                    height: 26,
                    bgcolor: "rgba(255,255,255,0.85)",
                    backdropFilter: "blur(4px)",
                    transition: "background-color 0.2s ease",
                    "&:hover": {
                      bgcolor: "rgba(255,255,255,1)"
                    }
                  }}
                >
                  {img.esPrincipal ? (
                    <StarIcon sx={{ fontSize: 18, color: "#1976d2" }} />
                  ) : (
                    <StarBorderIcon sx={{ fontSize: 18, color: "rgba(0,0,0,0.5)" }} />
                  )}
                </IconButton>

                {/* Chip principal */}
                {img.esPrincipal && (
                  <Chip
                    label="Principal"
                    size="small"
                    color="primary"
                    sx={{
                      position: "absolute",
                      bottom: 6,
                      left: 6,
                      fontSize: "0.7rem"
                    }}
                  />
                )}

                {/* Eliminar */}
                <IconButton
                  size="small"
                  onClick={(e) => {
                    e.stopPropagation();
                    setImagenesEditando((prev) => {
                      const eraPrincipal = prev[index]?.esPrincipal;
                      const next = prev.filter((_, iIdx) => iIdx !== index);

                      if (eraPrincipal && next.length > 0) {
                        return next.map((img, i) => ({ ...img, esPrincipal: i === 0 }));
                      }

                      return next;
                    });
                  }}
                  sx={{
                    position: "absolute",
                    top: 6,
                    right: 6,
                    width: 24,
                    height: 24,
                    borderRadius: "50%",
                    bgcolor: "rgba(0,0,0,0.6)",
                    color: "white",
                    "&:hover": {
                      bgcolor: "rgba(0,0,0,0.8)"
                    }
                  }}
                >
                  ✕
                </IconButton>
              </Box>
            ))}
          </Box>
          <Button
            variant="outlined"
            component="label"
            sx={{ alignSelf: "flex-start", mt: 2 }}
          >
            Agregar imágenes
            <input
              type="file"
              hidden
              multiple
              accept="image/*"
              onChange={(e) => {
                const files = Array.from(e.target.files);

                const nuevas = files.map((file) => ({
                  file,
                  url: URL.createObjectURL(file),
                  esPrincipal: false,
                  nueva: true
                }));

                setImagenesEditando((prev) => [...prev, ...nuevas]);
              }}
            />
          </Button>
          </DialogContent>
        )}

        <DialogActions>
          <Button onClick={() => setServicioEditando(null)}>
            Cancelar
          </Button>

          <Button
            variant="contained"
            onClick={async () => {
              try {
                const formData = new FormData();

                // datos básicos 
                formData.append("Titulo", servicioEditando.titulo);
                formData.append("Descripcion", servicioEditando.descripcion);

                // separar existentes y nuevas
                const existentes = imagenesEditando.filter((i) => i.nueva === false);
                const nuevas = imagenesEditando.filter((i) => i.nueva === true);

                // ImagenesExistentes: lista de urls
                existentes.forEach((img) => {
                  formData.append("ImagenesExistentes", img.url);
                });

                // UrlPrincipalExistente: URL
                const principalExistente = existentes.find((i) => i.esPrincipal);
                if (principalExistente) {
                  formData.append("UrlPrincipalExistente", principalExistente.url);
                }

                // Archivos nuevos
                nuevas.forEach((img) => {
                  formData.append("imagenes", img.file);
                });

                // IndexPrincipalNueva: índice dentro de las imagenes nuevas
                const idxPrincipalNueva = nuevas.findIndex((i) => i.esPrincipal);
                if (idxPrincipalNueva !== -1) {
                  formData.append("IndexPrincipalNueva", String(idxPrincipalNueva));
                }

                await editarServicio(servicioEditando.id, formData);

                setSuccessMsg("¡Servicio editado con éxito!");
                setServicioEditando(null);
                await reloadServicios();
              } catch (err) {
                setErrorEditar(err.message || "Error al editar el servicio");
              }
            }}
          >
            Guardar cambios
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog
        open={Boolean(servicioSeleccionado)}
        onClose={() => setServicioSeleccionado(null)}
        fullWidth
        maxWidth="md"
        PaperProps={{
          sx: {
            background: "transparent",
            boxShadow: "none",
            overflow: "visible"
          }
        }}
        slotProps={{
          backdrop: {
            sx: {
              backgroundColor: "rgba(0,0,0,0.35)",
              backdropFilter: "blur(8px)"
            }
          }
        }}
      >
        <DialogContent
          sx={{
            p: 0,
            position: "relative",
          }}
        >
          {servicioSeleccionado && (
            <Box
              sx={{
                position: "relative",
                width: "100%",
                height: "80vh",
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                "&:hover .modal-arrow": {
                  opacity: 1
                }
              }}
            >
              <Box
                component="img"
                src={
                  servicioSeleccionado.imagenes[imgModalIndex]?.url ||
                  "/servicios/servicio-default.jpg"
                }
                sx={{
                  maxWidth: "100%",
                  maxHeight: "100%",
                  objectFit: "contain"
                }}
              />

              <IconButton
                className="modal-arrow"
                onClick={() =>
                  setImgModalIndex((i) =>
                    i === 0
                      ? servicioSeleccionado.imagenes.length - 1
                      : i - 1
                  )
                }
                sx={{
                  position: "absolute",
                  left: 24,
                  top: "50%",
                  transform: "translateY(-50%)",
                  color: "white",
                  opacity: 0,
                  transition: "opacity .25s ease, transform .15s ease",
                  filter: "drop-shadow(0 4px 8px rgba(0,0,0,.6))",
                  "&:hover": {
                    transform: "translateY(-50%) scale(1.1)"
                  }
                }}
              >
                <ChevronLeftRoundedIcon sx={{ fontSize: 30 }} />
              </IconButton>

              <IconButton
                className="modal-arrow"
                onClick={() =>
                  setImgModalIndex((i) =>
                    i === servicioSeleccionado.imagenes.length - 1
                      ? 0
                      : i + 1
                  )
                }
                sx={{
                  position: "absolute",
                  right: 24,
                  top: "50%",
                  transform: "translateY(-50%)",
                  color: "white",
                  opacity: 0,
                  transition: "opacity .25s ease, transform .15s ease",
                  filter: "drop-shadow(0 4px 8px rgba(0,0,0,.6))",
                  "&:hover": {
                    transform: "translateY(-50%) scale(1.1)"
                  }
                }}
              >
                <ChevronRightRoundedIcon sx={{ fontSize: 30 }} />
              </IconButton>
            </Box>
          )}
        </DialogContent>
      </Dialog>
    </Box>
  );
}
