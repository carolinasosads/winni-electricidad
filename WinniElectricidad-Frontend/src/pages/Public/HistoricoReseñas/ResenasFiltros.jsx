import {
  Stack,
  Typography,
  Select,
  MenuItem
} from "@mui/material";
import FilterAltOutlinedIcon from "@mui/icons-material/FilterAltOutlined";

export default function ResenasFiltros({
  servicio,
  calificacion,
  onServicioChange,
  onCalificacionChange
}) {
  return (
    <Stack
      direction={{ xs: "column", sm: "row" }}
      spacing={2}
      mb={4}
      alignItems="center"
    >
      <Stack direction="row" spacing={1} alignItems="center">
        <FilterAltOutlinedIcon fontSize="small" />
        <Typography fontWeight={500}>Filtrar por</Typography>
      </Stack>

      <Select
        size="small"
        value={servicio}
        onChange={e => onServicioChange(e.target.value)}
      >
        <MenuItem value="Todos">Todos los servicios</MenuItem>
        <MenuItem value="Electricidad">Electricidad</MenuItem>
        <MenuItem value="Sanitaria">Sanitaria</MenuItem>
        <MenuItem value="Climatización">Climatización</MenuItem>
        <MenuItem value="Riego">Riego</MenuItem>
      </Select>

      <Select
        size="small"
        value={calificacion}
        onChange={e => onCalificacionChange(e.target.value)}
      >
        <MenuItem value="Todas">Todas las calificaciones</MenuItem>
        {[5, 4, 3, 2, 1].map(v => (
          <MenuItem key={v} value={v}>
            {v} estrellas
          </MenuItem>
        ))}
      </Select>
    </Stack>
  );
}
