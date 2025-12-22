import {  Box,  List,  ListItemButton,  ListItemText,  Divider,  Chip,  Typography,} from "@mui/material";

function getClienteId(c) {
  return c?.id ?? c?.Id ?? c?.idUsuario ?? c?.IdUsuario ?? null;
}
function getClienteNombre(c) {
  return c?.nombreCompleto ?? c?.NombreCompleto ?? "-";
}
function getClienteEmail(c) {
  return c?.email ?? c?.Email ?? "-";
}

export default function ListadoClientes({ clientes, onSelect }) {
  return (
    <>
      <Box sx={{ p: 2, borderBottom: "1px solid", borderColor: "divider" }}>
        <Typography sx={{ fontWeight: 600 }}>
          Resultados ({clientes.length})
        </Typography>
      </Box>

      <List disablePadding>
        {clientes.map((c, idx) => {
          const id = getClienteId(c) ?? `idx-${idx}`;
          return (
            <Box key={id}>
              <ListItemButton onClick={() => onSelect(c)}>
                <ListItemText
                  primary={getClienteNombre(c)}
                  secondary={getClienteEmail(c)}
                />
                <Chip label={`ID ${getClienteId(c) ?? "-"}`} size="small" />
              </ListItemButton>
              <Divider />
            </Box>
          );
        })}
      </List>
    </>
  );
}