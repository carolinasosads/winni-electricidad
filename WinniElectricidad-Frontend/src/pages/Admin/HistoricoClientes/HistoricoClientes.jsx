import { Box, Typography, Paper } from "@mui/material";

export default function HistoricoClientes() {
  return (
    <Box sx={{ p: 3 }}>
      <Paper sx={{ p: 3 }}>
        <Typography variant="h5" fontWeight={700}>
          Historial de Clientes
        </Typography>
        <Typography sx={{ mt: 1 }} color="text.secondary">
          Próximamente podrás ver un listado histórico de los clientes y sus reservas.
        </Typography>
      </Paper>
    </Box>
  );
}