import { Stack, Typography, Rating, Box, Chip, Tooltip } from "@mui/material";
import { keyframes } from "@mui/system";
import AutoAwesomeOutlinedIcon from "@mui/icons-material/AutoAwesomeOutlined";

export default function ResenasHeader({ promedio, total }) {
  const shimmer = keyframes`
    0% {
      opacity: 0;
      transform: translateX(-60%);
    }
    40% {
      opacity: 0.35;
    }
    60% {
      opacity: 0.35;
    }
    100% {
      opacity: 0;
      transform: translateX(60%);
    }
  `;

  return (
    <Stack spacing={2} mb={5}>
      <Box
        sx={{
          display: "flex",
          alignItems: "center",
          gap: 1.5,
          flexWrap: "wrap"
        }}
      >
        <Typography variant="h4" fontWeight={700}>
          Opiniones de nuestros clientes
        </Typography>

        <Tooltip
          arrow
          placement="right"
          title="Las reseñas son moderadas por un sistema inteligente para garantizar un espacio respetuoso."
        >
          <Chip
            icon={
              <AutoAwesomeOutlinedIcon
                sx={{ fontSize: 16, color: "#0d47a1" }}
              />
            }
            label="Moderadas por IA"
            size="small"
            color="primary"
            variant="outlined"
            sx={{
              position: "relative",
              overflow: "hidden",

              "&::before": {
                content: '""',
                position: "absolute",
                top: 0,
                left: "-50%",
                width: "200%",
                height: "100%",
                background:
                  "linear-gradient(120deg, transparent, rgba(25,118,210,0.3), transparent)",
                animation: `${shimmer} 3s ease-in-out infinite`,
                pointerEvents: "none"
              }
            }}
          />
        </Tooltip>
      </Box>

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
