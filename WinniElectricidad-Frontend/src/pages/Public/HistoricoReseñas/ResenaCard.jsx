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
import DoNotDisturbOnIcon from '@mui/icons-material/DoNotDisturbOn';

export default function ResenaCard({ resena, esAdmin, onDesaprobar }) {
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
        <Tooltip title="Desaprobar reseña">
          <IconButton
            onClick={() => onDesaprobar(resena)}
            size="small"
            sx={{
              position: "absolute",
              top: 14,
              right: 14,
              zIndex: 3,
              color: "grey.500",
              bgcolor: "rgba(255,255,255,0.5)",
              backdropFilter: "blur(6px)",
              boxShadow: "0 2px 6px rgba(0,0,0,0.15)",
              transition: "all 0.2s ease",
              "&:hover": {
                color: "error.main",
                bgcolor: "rgba(255,255,255,0.85)"
              }
            }}
          >
            <DoNotDisturbOnIcon fontSize="small" />
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
