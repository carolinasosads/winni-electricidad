import {
  Card,
  CardContent,
  Typography,
  Stack,
  TextField,
  Button,
  Box,
  MenuItem
} from "@mui/material";
import { useRef, useState } from "react";
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

  const [imagenFile, setImagenFile] = useState(null);
  const [preview, setPreview] = useState(null);

  const fileInputRef = useRef(null);

  const handleImageClick = () => {
    fileInputRef.current?.click();
  };

  const handleImageChange = (e) => {
    const file = e.target.files?.[0];
    if (!file) return;

    setImagenFile(file);
    setPreview(URL.createObjectURL(file));
  };

  const handleSubmit = () => {
    onCrear({
      titulo,
      descripcion,
      icono,
      imagen: imagenFile,
      activo: true
    });
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
          height: 150,
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
        {preview ? (
          <Box
            component="img"
            src={preview}
            alt="Preview"
            sx={{
              width: "100%",
              height: "100%",
              objectFit: "cover"
            }}
          />
        ) : (
          <Typography color="text.secondary">
            Click para subir imagen
          </Typography>
        )}

        <input
          ref={fileInputRef}
          type="file"
          accept="image/*"
          hidden
          onChange={handleImageChange}
        />
      </Box>

      <Box
        sx={{
          position: "absolute",
          top: 150 - 28,
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
          pt: 5,
          pb: 4,
          flexGrow: 1
        }}
      >
        <Stack spacing={2} sx={{ height: "100%", width: "100%" }}>
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
            disabled={!titulo || !descripcion || !imagenFile}
            sx={{ fontWeight: 600 }}
          >
            Agregar servicio
          </Button>
        </Stack>
      </CardContent>
    </Card>
  );
}
