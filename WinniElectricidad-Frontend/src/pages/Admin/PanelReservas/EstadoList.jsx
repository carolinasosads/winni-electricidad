import { Box, Typography, Paper, Chip, Stack } from "@mui/material";
import AccessTimeIcon from "@mui/icons-material/AccessTime";
import CheckCircleIcon from "@mui/icons-material/CheckCircle";
import CancelIcon from "@mui/icons-material/Cancel";
import HistoryIcon from "@mui/icons-material/History";

export default function EstadoList({ titulo, color, items = [], onSelect }) {
  // Iconos por tipo de estado
  const iconPorTitulo = {
    Pendientes: <AccessTimeIcon sx={{ color: "#1976d2" }} />,
    Confirmadas: <CheckCircleIcon sx={{ color: "#2e7d32" }} />,
    Canceladas: <CancelIcon sx={{ color: "#d32f2f" }} />,
    Finalizadas: <HistoryIcon sx={{ color: "#616161" }} />,
  };

  return (
    <Paper
      elevation={0}
      sx={{
        p: 2,
        borderRadius: 2,
        border: "1px solid",
        borderColor: "divider",
        mb: 2,
      }}
    >
      {/* --- Título + badge de cantidad --- */}
      <Stack
        direction="row"
        alignItems="center"
        spacing={1.2}
        sx={{ mb: 1 }}
      >
        {iconPorTitulo[titulo]}

        <Typography
          variant="h6"
          fontWeight={700}
          sx={{ fontSize: "1.1rem" }}
        >
          {titulo}
        </Typography>

        <Chip
          label={items.length}
          size="small"
          sx={{
            bgcolor: color + "22",
            color: color.replace(".main", ""),
            fontWeight: 600,
            ml: "auto",
          }}
        />
      </Stack>

      <Box sx={{ borderBottom: "1px solid", borderColor: "divider", mb: 1 }} />

      {/* --- Lista de items --- */}
      <Stack spacing={1}>
        {items.length === 0 && (
          <Typography
            variant="body2"
            color="text.secondary"
            sx={{ fontStyle: "italic", py: 0.5 }}
          >
            No hay reservas.
          </Typography>
        )}

        {items.map((r) => (
          <Paper
            key={r.id}
            onClick={() => onSelect(r)}
            sx={{
              p: 1.2,
              cursor: "pointer",
              borderRadius: 2,
              transition: "0.15s",
              "&:hover": {
                backgroundColor: "#f5f8ff",
                transform: "scale(1.01)",
              },
            }}
          >
            <Typography fontWeight={600}>
              {r.cliente?.nombre ?? r.clienteNombre ?? "Sin nombre"}
            </Typography>
            <Typography variant="body2" sx={{ color: "text.secondary" }}>
              {new Date(r.fechaReserva).toLocaleString("es-UY")}
            </Typography>
          </Paper>
        ))}
      </Stack>
    </Paper>
  );
}