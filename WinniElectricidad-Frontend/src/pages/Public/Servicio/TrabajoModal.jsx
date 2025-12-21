import { Dialog, DialogContent, Box, Typography } from "@mui/material";

export default function TrabajoModal({ trabajo, onClose }) {
  if (!trabajo) return null;

  return (
    <Dialog open onClose={onClose} maxWidth="md" fullWidth>
      <DialogContent sx={{ p: 0 }}>
        <Box
          component="img"
          src={`${import.meta.env.VITE_API_URL}${trabajo.imagenUrl}`}
          alt="Trabajo realizado"
          sx={{
            width: "100%",
            height: 420,
            objectFit: "cover"
          }}
        />

        {trabajo.descripcion && (
          <Typography sx={{ p: 3 }}>
            {trabajo.descripcion}
          </Typography>
        )}
      </DialogContent>
    </Dialog>
  );
}
