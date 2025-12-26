import { Stack, Typography } from "@mui/material";

export default function ServiciosHeader() {
  return (
    <Stack spacing={2} mb={5}>
      <Typography variant="h4" fontWeight={700}>
        Nuestros servicios
      </Typography>
      <Typography color="text.secondary">
        Conocé los servicios que ofrecemos y solicitá tu visita de presupuesto online.
      </Typography>
    </Stack>
  );
}
