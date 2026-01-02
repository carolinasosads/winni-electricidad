import { useState, useEffect } from "react";
import {
  Box,
  Button,
  TextField,
  Typography,
  MenuItem,
  Rating,
  CircularProgress,
  Alert,
  Divider
} from "@mui/material";
import AddPhotoAlternateIcon from "@mui/icons-material/AddPhotoAlternate";

import { crearResena } from "../../../services/resenaService";
import { getServiciosActivos } from "../../../services/servicioService";

export default function CrearResena() {
  const [descripcion, setDescripcion] = useState("");
  const [calificacion, setCalificacion] = useState(0);
  const [servicios, setServicios] = useState([]);
  const [idServicio, setIdServicio] = useState("");
  const [imagen, setImagen] = useState(null);
  const [preview, setPreview] = useState(null);

  const [loading, setLoading] = useState(false);
  const [errorMsg, setErrorMsg] = useState("");
  const [successMsg, setSuccessMsg] = useState("");

  useEffect(() => {
    getServiciosActivos().then((res) => {
      setServicios(res);
    });
  }, []);

  const formatosPermitidos = ["image/jpeg", "image/png", "image/jpg"];

  const handleImagen = (e) => {
    const file = e.target.files[0];
    if (!file) return;

    if (!formatosPermitidos.includes(file.type)) {
      setErrorMsg("Formato no permitido. Solo JPG y PNG.");
      return;
    }

    setImagen(file);
    setPreview(URL.createObjectURL(file));
    setErrorMsg("");
  };

  const handleSubmit = async () => {
    if (!descripcion.trim()) {
      setErrorMsg("La descripción es obligatoria.");
      return;
    }
    if (calificacion < 1 || calificacion > 5) {
      setErrorMsg("Debes seleccionar una calificación de 1 a 5 estrellas.");
      return;
    }
    if (!idServicio) {
      setErrorMsg("Debes seleccionar un servicio.");
      return;
    }

    setLoading(true);
    setErrorMsg("");
    setSuccessMsg("");

    const formData = new FormData();
    formData.append("Descripcion", descripcion);
    formData.append("Calificacion", calificacion);
    formData.append("IdServicio", idServicio);

    if (imagen) {
      formData.append("imagen", imagen);
    }

    try {
      await crearResena(formData);
      setSuccessMsg("¡Reseña publicada con éxito!");

      setDescripcion("");
      setCalificacion(0);
      setIdServicio("");
      setImagen(null);
      setPreview(null);
    } catch (error) {
      setErrorMsg(error.message || "Error al enviar reseña.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box
      sx={{
        width: "100%",
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
          maxWidth: 520,
          display: "flex",
          flexDirection: "column",
          gap: 2.5,

          px: { xs: 2, sm: 3 },
          py: { xs: 3, sm: 4 },

          backgroundColor: { xs: "grey.50", sm: "transparent" },
          borderRadius: { xs: 2, sm: 0 },
        }}
      >
        {/* Acento superior */}
        <Box
          sx={{
            width: 48,
            height: 4,
            backgroundColor: "primary.main",
            borderRadius: 2,
          }}
        />

        <Typography variant="h5" fontWeight={600}>
          Publicar Reseña
        </Typography>

        <Divider />

        {errorMsg && <Alert severity="error">{errorMsg}</Alert>}
        {successMsg && <Alert severity="success">{successMsg}</Alert>}

        <TextField
          label="Descripción"
          multiline
          rows={3}
          fullWidth
          value={descripcion}
          onChange={(e) => setDescripcion(e.target.value)}
          inputProps={{ maxLength: 500 }}
          required
        />

        <Typography variant="caption" color="text.secondary">
          Las reseñas son moderadas por un sistema inteligente para garantizar un espacio respetuoso.
        </Typography>

        <Box>
          <Typography fontWeight={500}>Calificación</Typography>
          <Rating
            value={calificacion}
            onChange={(e, newValue) => setCalificacion(newValue)}
          />
        </Box>

        <TextField
          select
          label="Servicio"
          fullWidth
          required
          value={idServicio}
          onChange={(e) => setIdServicio(e.target.value)}
        >
          {servicios.map((s) => (
            <MenuItem key={s.id} value={s.id}>
              {s.titulo}
            </MenuItem>
          ))}
        </TextField>

        <Typography variant="body2">
          Formatos permitidos: JPG, JPEG, PNG
        </Typography>

        <Button
          variant="outlined"
          component="label"
          startIcon={<AddPhotoAlternateIcon />}
        >
          Subir imagen
          <input
            type="file"
            accept=".jpg,.jpeg,.png"
            hidden
            onChange={handleImagen}
          />
        </Button>

        {preview && (
          <Box textAlign="center">
            <img
              src={preview}
              alt="Preview"
              style={{
                width: 120,
                height: 120,
                borderRadius: 8,
                objectFit: "cover",
              }}
            />
          </Box>
        )}

        <Button
          variant="contained"
          fullWidth
          onClick={handleSubmit}
          disabled={loading}
          sx={{
            py: 1.4,
            fontSize: "1rem",
            mt: 2,
          }}
        >
          {loading ? <CircularProgress size={24} /> : "Publicar Reseña"}
        </Button>
      </Box>
    </Box>
  );
}
