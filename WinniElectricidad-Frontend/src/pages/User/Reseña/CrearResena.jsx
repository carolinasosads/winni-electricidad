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
  Paper
} from "@mui/material";
import AddPhotoAlternateIcon from "@mui/icons-material/AddPhotoAlternate";

import { crearResena } from "../../../services/resenaService";
import { getServiciosActivos } from "../../../services/authService";

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

  // Cargar servicios
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
        display: "flex",
        justifyContent: "center",
        mt: 4,
        px: 2,
        width: "100%",
      }}
    >
      <Paper
        elevation={3}
        sx={{
          width: "100%",
          maxWidth: 520,
          p: 4,
          borderRadius: 3,
        }}
      >
        <Typography variant="h5" sx={{ mb: 3, fontWeight: 600 }}>
          Publicar Reseña
        </Typography>

        {errorMsg && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {errorMsg}
          </Alert>
        )}

        {successMsg && (
          <Alert severity="success" sx={{ mb: 2 }}>
            {successMsg}
          </Alert>
        )}

        {/* Descripción */}
        <TextField
          label="Descripción"
          multiline
          rows={3}
          fullWidth
          value={descripcion}
          onChange={(e) => setDescripcion(e.target.value)}
          inputProps={{ maxLength: 500 }}
          required
          sx={{ mb: 2 }}
        />

        {/* Calificación */}
        <Typography>Calificación</Typography>
        <Rating
          value={calificacion}
          onChange={(e, newValue) => setCalificacion(newValue)}
          sx={{ mb: 2 }}
        />

        {/* Selección de servicio */}
        <TextField
          select
          label="Servicio"
          fullWidth
          required
          value={idServicio}
          onChange={(e) => setIdServicio(e.target.value)}
          sx={{ mb: 2 }}
        >
          {servicios.map((s) => (
            <MenuItem key={s.id} value={s.id}>
              {s.titulo}
            </MenuItem>
          ))}
          <MenuItem value="otro">Otro</MenuItem>
        </TextField>

        {/* Carga de imagen */}
        <Typography variant="body2" sx={{ mb: 1 }}>
          Formatos permitidos: JPG, JPEG, PNG
        </Typography>

        <Button
          variant="outlined"
          component="label"
          startIcon={<AddPhotoAlternateIcon />}
          sx={{ mb: 2 }}
        >
          Subir imagen
          <input type="file" accept=".jpg,.jpeg,.png" hidden onChange={handleImagen} />
        </Button>

        {preview && (
          <Box sx={{ mb: 2, textAlign: "center" }}>
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
          sx={{ py: 1.2, fontSize: "1rem" }}
        >
          {loading ? <CircularProgress size={24} /> : "Publicar Reseña"}
        </Button>
      </Paper>
    </Box>
  );
}
