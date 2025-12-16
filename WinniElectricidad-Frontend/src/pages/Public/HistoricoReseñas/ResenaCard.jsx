import {
  Card,
  CardContent,
  CardMedia,
  Typography,
  Rating,
  Stack,
  Chip,
  Button
} from "@mui/material";
import DeleteOutlineIcon from "@mui/icons-material/DeleteOutline";

export default function ResenaCard({ resena, esAdmin, onEliminar }) {
  return (
    <Card sx={{ height: "100%" }}>
      {resena.imagenUrl && (
        <CardMedia
          component="img"
          height="180"
          image={resena.imagenUrl}
          alt="Imagen reseña"
        />
      )}

      <CardContent>
        <Stack spacing={1}>
          <Stack direction="row" justifyContent="space-between">
            <Typography fontWeight={600}>
              {resena.usuario}
            </Typography>
            <Chip
              size="small"
              label={resena.servicio}
              variant="outlined"
            />
          </Stack>

          <Rating value={resena.calificacion} readOnly />

          <Typography variant="body2">
            {resena.descripcion}
          </Typography>

          <Typography variant="caption" color="text.secondary">
            {new Date(resena.fecha).toLocaleDateString("es-UY")}
          </Typography>

          {esAdmin && (
            <Button
              size="small"
              color="error"
              startIcon={<DeleteOutlineIcon />}
              sx={{ alignSelf: "flex-end", mt: 1 }}
              onClick={() => onEliminar(resena.id)}
            >
              Eliminar reseña
            </Button>
          )}
        </Stack>
      </CardContent>
    </Card>
  );
}
