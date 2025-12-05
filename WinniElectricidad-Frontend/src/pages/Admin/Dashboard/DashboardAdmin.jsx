import { Box, Typography, Paper } from "@mui/material";

export default function DashboardAdmin() {
  return (
    <Box sx={{ p: 3 }}>
      <Paper sx={{ p: 3 }}>
        <Typography variant="h5" fontWeight={700}>
          Panel de Administración
        </Typography>
        <Typography sx={{ mt: 1 }} color="text.secondary">
          Esta es la vista principal del administrador.
        </Typography>
      </Paper>
    </Box>
  );
}