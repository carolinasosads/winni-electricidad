import {
  Box,
  Autocomplete,
  TextField,
  RadioGroup,
  FormControlLabel,
  Radio,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
} from "@mui/material";

export default function AgendaFilters({
  width,
  servicioOpciones = [],
  direccionesUsuario = [],
  servicios,
  setServicios,
  tipoTrabajo,
  setTipoTrabajo,
  direccionId,
  setDireccionId,
}) {
  const hasDirecciones = (direccionesUsuario?.length ?? 0) > 0;

  return (
    <Box
      sx={{
        display: "grid",
        gridTemplateColumns: { xs: "1fr", md: "1.4fr 1fr 1fr" },
        gap: 2,
        mb: 2,
        width,
        mx: "auto",
      }}
    >
      {/* Servicios (multi-picklist) */}
      <Autocomplete
        multiple
        options={servicioOpciones}
        getOptionLabel={(o) => o.label}
        isOptionEqualToValue={(o, v) => o.id === v.id}
        value={servicios}
        onChange={(_, val) => setServicios(val)}
        disableCloseOnSelect
        filterSelectedOptions
        renderInput={(params) => <TextField {...params} label="Servicios" />}
        />

      {/* Tipo de trabajo */}
      <FormControl>
        <RadioGroup
          row
          value={tipoTrabajo}
          onChange={(e) => setTipoTrabajo(e.target.value)}
          aria-label="tipo-trabajo"
          name="tipo-trabajo"
        >
          <FormControlLabel value="instalacion" control={<Radio />} label="Instalación" />
          <FormControlLabel value="mantenimiento" control={<Radio />} label="Mantenimiento" />
        </RadioGroup>
      </FormControl>

      {/* Direcciones */}
      <FormControl disabled={!hasDirecciones}>
        <InputLabel id="direccion-label">Dirección</InputLabel>
        <Select
          labelId="direccion-label"
          label="Dirección"
          value={hasDirecciones ? direccionId : ""}
          onChange={(e) => setDireccionId(e.target.value)}
        >
          {hasDirecciones ? (
            direccionesUsuario.map((d) => (
              <MenuItem key={d.id} value={d.id}>
                {d.label}
              </MenuItem>
            ))
          ) : (
            <MenuItem value="" disabled>
              Sin direcciones cargadas
            </MenuItem>
          )}
        </Select>
      </FormControl>
    </Box>
  );
}