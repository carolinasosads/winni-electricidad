import { Box, Typography, TextField, Button } from "@mui/material";

export default function FiltroClientes({  query,  onChangeQuery,  onLimpiar,  onVerTodos,}) {
  return (
    <>
      <Typography sx={{ mb: 1, fontWeight: 600 }}>Buscar cliente</Typography>

      <Box sx={{ display: "flex", gap: 1, alignItems: "center" }}>
        <TextField
          fullWidth
          label="Nombre, email o telefono."
          value={query}
          onChange={(e) => onChangeQuery(e.target.value)}
        />

        <Button variant="outlined" onClick={onLimpiar} disabled={!query}>
          Limpiar
        </Button>

        <Button variant="contained" onClick={onVerTodos}>
          Ver todos
        </Button>
      </Box>
    </>
  );
}