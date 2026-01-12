import {
  Card,
  CardContent,
  Typography,
  Stack,
  TextField,
  Button,
  Box,
  MenuItem,
  CircularProgress
} from "@mui/material";
import { useRef, useState, useEffect } from "react";
import AddOutlinedIcon from "@mui/icons-material/AddOutlined";

import BoltOutlinedIcon from "@mui/icons-material/BoltOutlined";
import PlumbingOutlinedIcon from "@mui/icons-material/PlumbingOutlined";
import AcUnitOutlinedIcon from "@mui/icons-material/AcUnitOutlined";
import WaterOutlinedIcon from "@mui/icons-material/WaterOutlined";
import ConstructionOutlinedIcon from "@mui/icons-material/ConstructionOutlined";
import HandymanOutlinedIcon from "@mui/icons-material/HandymanOutlined";
import HomeRepairServiceOutlinedIcon from "@mui/icons-material/HomeRepairServiceOutlined";
import BuildOutlinedIcon from "@mui/icons-material/BuildOutlined";

const ICONOS_SERVICIO = [
  { key: "Electricidad", label: "Electricidad", icon: <BoltOutlinedIcon /> },
  { key: "Sanitaria", label: "Sanitaria", icon: <PlumbingOutlinedIcon /> },
  { key: "Climatización", label: "Climatización", icon: <AcUnitOutlinedIcon /> },
  { key: "Riego", label: "Riego", icon: <WaterOutlinedIcon /> },
  { key: "Mantenimiento", label: "Mantenimiento", icon: <HandymanOutlinedIcon /> },
  { key: "Construcción", label: "Construcción / Obra", icon: <ConstructionOutlinedIcon /> },
  { key: "Reparaciones", label: "Reparaciones", icon: <HomeRepairServiceOutlinedIcon /> },
  { key: "Otro", label: "Otro", icon: <BuildOutlinedIcon /> }
];

export default function ServicioCreateCard({ onCrear }) {
  const [titulo, setTitulo] = useState("");
  const [descripcion, setDescripcion] = useState("");
  const [icono, setIcono] = useState("Otro");

  const [imagenes, setImagenes] = useState([]);
  const [imagenActiva, setImagenActiva] = useState(0);
  const [previews, setPreviews] = useState([]);

  const [creando, setCreando] = useState(false);

  const fileInputRef = useRef(null);

  useEffect(() => {
    return () => previews.forEach(URL.revokeObjectURL);
  }, [previews]);

  const handleImageClick = () => {
    fileInputRef.current?.click();
  };

  const handleImageChange = (e) => {
    const files = Array.from(e.target.files);
    if (!files.length) return;

    setImagenes(files);
    setPreviews(files.map(f => URL.createObjectURL(f)));
    setImagenActiva(0);
  };

  const handleSubmit = async () => {
    try{
      setCreando(true);
      await onCrear({
        titulo,
        descripcion,
        icono,
        imagenes,
        activo: true
      });

      setTitulo("");
      setDescripcion("");
      setIcono("Otro");
      setImagenes([]);
      setPreviews([]);
      setImagenActiva(0);

      if (fileInputRef.current) {
        fileInputRef.current.value = "";
      }
    } finally {
      setCreando(false);
    }
  };

  return (
    <Card
      sx={{
        width: "100%",
        height: "100%",
        display: "flex",
        flexDirection: "column",
        position: "relative",
        borderRadius: 3,
        boxShadow: "0 8px 24px rgba(0,0,0,0.06)",
        border: "1px solid",
        borderColor: "divider",
        transition: "box-shadow .2s ease, border-color .2s ease",
        "&:hover": {
        borderColor: "primary.main",
        boxShadow: "0 12px 32px rgba(0,0,0,0.08)"
        }
      }}
    >
      <Box
        onClick={handleImageClick}
        sx={{
          width: "100%",
          height: 180,
          cursor: "pointer",
          bgcolor: "grey.100",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          borderTopLeftRadius: 12,
          borderTopRightRadius: 12,
          overflow: "hidden",
          position: "relative"
        }}
      >
        {previews.length > 0 ? (
          <Box
            component="img"
            src={previews[imagenActiva]}
            alt="Preview principal"
            sx={{ width: "100%", height: "100%", objectFit: "cover" }}
          />
        ) : (
          <Typography color="text.secondary">
            Click para subir imágenes
          </Typography>
        )}

        <input
          ref={fileInputRef}
          type="file"
          accept="image/*"
          multiple
          hidden
          onChange={handleImageChange}
        />
      </Box>

      {previews.length > 1 && (
          <Stack
            direction="row"
            spacing={1}
            justifyContent="center"
            sx={{ px: 2, pt: 1, pb: 0 }}
          >
            {previews.map((src, i) => (
              <Box
                key={i}
                component="img"
                src={src}
                onClick={() => setImagenActiva(i)}
                sx={{
                  width: 44,
                  height: 44,
                  objectFit: "cover",
                  borderRadius: 1,
                  cursor: "pointer",
                  border: i === imagenActiva ? "2px solid" : "1px solid",
                  borderColor: i === imagenActiva ? "primary.main" : "divider",
                  opacity: i === imagenActiva ? 1 : 0.6,
                  transition: "all .2s"
                }}
              />
            ))}
          </Stack>
        )}

      <Box
        sx={{
          position: "absolute",
          top: 180 - 28,
          left: "50%",
          transform: "translateX(-50%)",
          width: 56,
          height: 56,
          borderRadius: "50%",
          bgcolor: "primary.main",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          color: "white",
          boxShadow: "0 8px 24px rgba(0,0,0,0.12)",
          zIndex: 2
        }}
      >
        <AddOutlinedIcon />
      </Box>

      <CardContent
        sx={{
          px: 3,
          pt: 4,
          pb: 4,
          flexGrow: 1
        }}
      >
        <Stack spacing={2} sx={{ height: "100%", width: "100%" }}>
          <Typography
            variant="caption"
            color="text.secondary"
            textAlign="center"
            mt={1}
          >
            La primera imagen será la imagen principal del servicio
          </Typography>
          <TextField
            label="Título del servicio"
            value={titulo}
            onChange={(e) => setTitulo(e.target.value)}
            fullWidth
            size="small"
          />

          <TextField
            label="Descripción"
            value={descripcion}
            onChange={(e) => setDescripcion(e.target.value)}
            multiline
            rows={2}
            fullWidth
            size="small"
          />

          <TextField
            select
            label="Ícono del servicio"
            value={icono}
            onChange={(e) => setIcono(e.target.value)}
            fullWidth
            size="small"
          >
            {ICONOS_SERVICIO.map((op) => (
              <MenuItem key={op.key} value={op.key}>
                <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                  {op.icon}
                  <Typography variant="body2">{op.label}</Typography>
                </Box>
              </MenuItem>
            ))}
          </TextField>

          <Button
            variant="contained"
            onClick={handleSubmit}
            disabled={creando || !titulo || !descripcion || imagenes.length === 0}
            sx={{ fontWeight: 600 }}
          >
            {creando ? <CircularProgress size={24} /> : "Crear"}
          </Button>
        </Stack>
      </CardContent>
    </Card>
  );
}
