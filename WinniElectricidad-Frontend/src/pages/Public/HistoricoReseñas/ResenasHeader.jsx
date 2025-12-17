import { Stack, Typography, Rating, Box } from "@mui/material";

export default function ResenasHeader({ promedio, total }) {
  return (
    <Stack spacing={2} mb={5}>
      <Typography variant="h4" fontWeight={700}>
        Opiniones de nuestros clientes
      </Typography>

      <Typography color="text.secondary">
        ¡Conocé la experiencia de quienes ya confiaron en Winni Electricidad!
      </Typography>

      <Box
        sx={{
          display: "flex",
          alignItems: "center",
          gap: 2,
          mt: 1
        }}
      >
        <Typography variant="h3" fontWeight={700}>
          {promedio}
        </Typography>

        <Stack>
          <Rating
            value={Number(promedio)}
            precision={0.1}
            readOnly
            size="large"
            sx={{ color: "#F5A623" }}
          />
          <Typography variant="body2" color="text.secondary">
            {total} reseñas
          </Typography>
        </Stack>
      </Box>
    </Stack>
  );
}
