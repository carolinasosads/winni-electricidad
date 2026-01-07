import {
  Card,
  CardContent,
  Typography,
  Stack,
  Button,
  IconButton,
  Tooltip,
  Box
} from "@mui/material";
import VisibilityOffOutlinedIcon from "@mui/icons-material/VisibilityOffOutlined";
import VisibilityOutlinedIcon from "@mui/icons-material/VisibilityOutlined";
import BoltOutlinedIcon from "@mui/icons-material/BoltOutlined";
import PlumbingOutlinedIcon from "@mui/icons-material/PlumbingOutlined";
import AcUnitOutlinedIcon from "@mui/icons-material/AcUnitOutlined";
import WaterOutlinedIcon from "@mui/icons-material/WaterOutlined";
import BuildOutlinedIcon from "@mui/icons-material/BuildOutlined";

export default function ServicioCard({
  servicio,
  rol,
  onToggleActivo,
  onSolicitar
}) {
  const esAdmin = rol === "Administrador";
  const esCliente = rol === "Cliente";

  const iconPorServicio = {
    Electricidad: <BoltOutlinedIcon />,
    Sanitaria: <PlumbingOutlinedIcon />,
    Climatización: <AcUnitOutlinedIcon />,
    Riego: <WaterOutlinedIcon />,
    Otro: <BuildOutlinedIcon />
  };

  // ✅ Imagen servida desde /public (funciona igual en local y prod)
  const imageUrl = servicio.imagenUrl;

  console.log("[ServicioCard imagen]", imageUrl);

  return (
    <Card
      sx={{
        width: "100%",
        height: "100%",
        display: "flex",
        flexDirection: "column",
        position: "relative",
        borderRadius: 3,
        boxShadow: "0 8px 24px rgba(0,0,0,0.06)",
        transition: "transform .2s ease, box-shadow .2s ease",
        "&:hover": {
          transform: "translateY(-4px)",
          boxShadow: "0 12px 32px rgba(0,0,0,0.08)"
        },
        opacity: servicio.activo === false ? 0.5 : 1
      }}
    >
      {esAdmin && (
        <Tooltip
          title={servicio.activo ? "Desactivar servicio" : "Activar servicio"}
        >
          <IconButton
            size="small"
            onClick={() => onToggleActivo(servicio)}
            sx={{
              position: "absolute",
              top: 14,
              right: 14,
              zIndex: 3,
              color: "grey.500",
              bgcolor: "rgba(255,255,255,0.55)",
              backdropFilter: "blur(6px)",
              boxShadow: "0 2px 6px rgba(0,0,0,0.15)",
              transition: "all 0.2s ease",
              "&:hover": {
                color: servicio.activo ? "error.main" : "success.main",
                bgcolor: "rgba(255,255,255,0.85)"
              }
            }}
          >
            {servicio.activo ? (
              <VisibilityOffOutlinedIcon fontSize="small" />
            ) : (
              <VisibilityOutlinedIcon fontSize="small" />
            )}
          </IconButton>
        </Tooltip>
      )}

      <Box
        component="img"
        src={imageUrl}
        alt={servicio.titulo}
        sx={{
          width: "100%",
          height: 350,
          objectFit: "cover",
          borderTopLeftRadius: 12,
          borderTopRightRadius: 12
        }}
        onError={(e) => {
          console.error("❌ Error cargando imagen:", imageUrl);
          e.currentTarget.src = "/servicios/servicio-default.jpg";
        }}
      />

      <Box
        sx={{
          position: "absolute",
          top: 350 - 28,
          left: "50%",
          transform: "translateX(-50%)",
          width: 56,
          height: 56,
          borderRadius: "50%",
          bgcolor: "background.paper",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          color: "primary.main",
          boxShadow: "0 8px 24px rgba(0,0,0,0.12)",
          zIndex: 2
        }}
      >
        {iconPorServicio[servicio.titulo] ?? <BuildOutlinedIcon />}
      </Box>

      <CardContent
        sx={{
          px: 3,
          pt: 5,
          pb: 4,
          flexGrow: 1
        }}
      >
        <Stack spacing={2} sx={{ height: "100%", width: "100%" }}>
          <Typography variant="h6" fontWeight={600} align="center">
            {servicio.titulo}
          </Typography>

          <Typography
            variant="body2"
            color="text.secondary"
            align="center"
            sx={{
              display: "-webkit-box",
              WebkitLineClamp: 3,
              WebkitBoxOrient: "vertical",
              overflow: "hidden"
            }}
          >
            {servicio.descripcion}
          </Typography>

          <Box sx={{ flexGrow: 1 }} />

          {esCliente && (
            <Button
              variant="text"
              size="small"
              onClick={() => onSolicitar(servicio)}
              sx={{ alignSelf: "center", fontWeight: 600 }}
            >
              Solicitar presupuesto
            </Button>
          )}
        </Stack>
      </CardContent>
    </Card>
  );
}
