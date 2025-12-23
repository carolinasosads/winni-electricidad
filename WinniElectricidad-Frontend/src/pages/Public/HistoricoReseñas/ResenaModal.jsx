import {
  Dialog,
  DialogContent,
  Typography,
  Rating,
  Stack,
  Chip,
  Box
} from "@mui/material";

export default function ResenaModal({ resena, onClose }) {
  if (!resena) return null;

  return (
    <Dialog
      open
      onClose={onClose}
      maxWidth="md"
      fullWidth
    >
      <DialogContent
        sx={{
          p: 0,
          overflowX: "hidden"
        }}
      >
        {resena.imagenUrl && (
          <Box
            component="img"
            src={`${import.meta.env.VITE_API_URL}${resena.imagenUrl}`}
            alt="Trabajo realizado"
            sx={{
              width: "100%",
              height: 360,
              objectFit: "cover"
            }}
            onError={(e) => (e.currentTarget.style.display = "none")}
          />
        )}

        <Stack spacing={2} sx={{ p: 4 }} textAlign="center">
          <Rating
            value={resena.calificacion}
            readOnly
            size="large"
            sx={{ color: "#F5A623" }}
          />

          <Typography
            sx={{
              fontStyle: "italic",
              lineHeight: 1.7,
              overflowWrap: "anywhere",
              wordBreak: "break-word",
              whiteSpace: "pre-wrap"
            }}
          >
            “{resena.descripcion}”
          </Typography>

          <Typography fontWeight={600}>
            {resena.cliente?.nombre ?? "Cliente"}
          </Typography>

          <Chip
            label={resena.servicio?.titulo ?? "Servicio"}
            size="small"
            sx={{ alignSelf: "center" }}
          />

          <Typography variant="caption" color="text.secondary">
            {new Date(resena.fechaReseña).toLocaleDateString("es-UY")}
          </Typography>
        </Stack>
      </DialogContent>
    </Dialog>
  );
}
