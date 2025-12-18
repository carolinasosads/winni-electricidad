import {
  Card,
  CardContent,
  Typography,
  Rating,
  Stack,
  Chip,
  IconButton,
  Tooltip
} from "@mui/material";
import DeleteOutlineIcon from "@mui/icons-material/DeleteOutline";

export default function ResenaCard({ resena, esAdmin, onEliminar }) {
  return (
    <Card
      sx={{
        position: "relative",
        height: "100%",
        borderRadius: 3,
        boxShadow: "0 8px 24px rgba(0,0,0,0.06)",
        transition: "transform 0.2s ease, box-shadow 0.2s ease",
        "&:hover": {
          transform: "translateY(-4px)",
          boxShadow: "0 12px 32px rgba(0,0,0,0.08)"
        }
      }}
    >
      {/* BOTÓN ADMIN */}
      {esAdmin && (
        <Tooltip title="Eliminar reseña">
          <IconButton
            onClick={() => onEliminar(resena)}
            size="small"
            sx={{
              position: "absolute",
              top: 14,
              right: 14,
              color: "grey.400",
              zIndex: 2,
              "&:hover": {
                color: "error.main",
                backgroundColor: "rgba(211,47,47,0.08)"
              }
            }}
          >
            <DeleteOutlineIcon fontSize="small" />
          </IconButton>
        </Tooltip>
      )}

      <CardContent sx={{ px: 4, pt: 6, pb: 4 }}>
        <Stack spacing={2} alignItems="center" textAlign="center">
          <Rating
            value={resena.calificacion}
            readOnly
            size="large"
            sx={{ color: "#F5A623" }}
          />

          <Typography
            variant="body1"
            sx={{ fontStyle: "italic", lineHeight: 1.7 }}
          >
            “{resena.descripcion}”
          </Typography>

          <Typography fontWeight={600}>
            {resena.cliente?.nombre ?? "Cliente"}
          </Typography>

          <Chip
            size="small"
            label={resena.servicio?.titulo ?? "Servicio"}
            sx={{ bgcolor: "grey.100", fontSize: "0.75rem" }}
          />

          <Typography variant="caption" color="text.secondary">
            {new Date(resena.fechaReseña).toLocaleDateString("es-UY")}
          </Typography>
        </Stack>
      </CardContent>
    </Card>
  );
}
