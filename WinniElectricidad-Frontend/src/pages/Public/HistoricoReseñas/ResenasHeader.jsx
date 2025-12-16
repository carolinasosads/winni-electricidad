import { Stack, Typography, Rating } from "@mui/material";

export default function ResenasHeader({ promedio, total }) {
  return (
    <Stack spacing={1} mb={4}>
      <Typography variant="h4" fontWeight={600}>
        Opiniones de nuestros clientes
      </Typography>

      <Typography color="text.secondary">
        Conocé la experiencia de quienes ya confiaron en Winni Electricidad
      </Typography>

      <Stack direction="row" spacing={1} alignItems="center">
        <Typography variant="h5" fontWeight={600}>
          {promedio}
        </Typography>
        <Rating value={Number(promedio)} precision={0.1} readOnly />
        <Typography color="text.secondary">
          ({total} reseñas)
        </Typography>
      </Stack>
    </Stack>
  );
}
