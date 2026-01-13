import {
  Card,
  CardContent,
  Typography,
  Stack,
  Button,
  IconButton,
  Tooltip,
  Box,
  useMediaQuery
} from "@mui/material";
import { useTheme } from "@mui/material/styles";
import { useRef, useState, useEffect} from "react";

import VisibilityOffOutlinedIcon from "@mui/icons-material/VisibilityOffOutlined";
import VisibilityOutlinedIcon from "@mui/icons-material/VisibilityOutlined";
import BoltOutlinedIcon from "@mui/icons-material/BoltOutlined";
import PlumbingOutlinedIcon from "@mui/icons-material/PlumbingOutlined";
import AcUnitOutlinedIcon from "@mui/icons-material/AcUnitOutlined";
import WaterOutlinedIcon from "@mui/icons-material/WaterOutlined";
import BuildOutlinedIcon from "@mui/icons-material/BuildOutlined";
import ChevronLeftRoundedIcon from "@mui/icons-material/ChevronLeftRounded";
import ChevronRightRoundedIcon from "@mui/icons-material/ChevronRightRounded";
import EditIcon from '@mui/icons-material/Edit';

export default function ServicioCard({
  servicio,
  rol,
  onToggleActivo,
  onSolicitar,
  onOpenGaleria,
  onEditar
}) {
  const esAdmin = rol === "Administrador";
  const esCliente = rol === "Cliente";

  const theme = useTheme();
  const esMobile = useMediaQuery(theme.breakpoints.down("sm"));

  const imagenes = servicio.imagenes ?? [];
  const total = imagenes.length;
  const tieneCarrusel = total > 1;

  const [index, setIndex] = useState(0);
  const [clicked, setClicked] = useState(null);
  const touchStartX = useRef(null);

  const iconPorServicio = {
    Electricidad: <BoltOutlinedIcon />,
    Sanitaria: <PlumbingOutlinedIcon />,
    Climatización: <AcUnitOutlinedIcon />,
    Riego: <WaterOutlinedIcon />,
    Otro: <BuildOutlinedIcon />
  };

  const goPrev = () => {
    setIndex((i) => (i - 1 + total) % total);
  };

  const goNext = () => {
    setIndex((i) => (i + 1) % total);
  };

  const handleTouchStart = (e) => {
    touchStartX.current = e.touches[0].clientX;
  };

  const handleTouchEnd = (e) => {
    if (touchStartX.current === null) return;
    const delta = e.changedTouches[0].clientX - touchStartX.current;
    if (Math.abs(delta) > 50) delta > 0 ? goPrev() : goNext();
    touchStartX.current = null;
  };

  const imageUrl =
    imagenes[index]?.url ||
    servicio.imagenUrl ||
    "/servicios/servicio-default.jpg";

  const arrowBaseStyles = {
    position: "absolute",
    top: "50%",
    transform: "translateY(-50%)",
    color: "white",
    opacity: 0,
    transition: "opacity .25s ease, transform .15s ease",
    filter: "drop-shadow(0 4px 8px rgba(0,0,0,.6))",
    "&:hover": {
      transform: "translateY(-50%) scale(1.15)"
    }
  };

  useEffect(() => {
    if (!imagenes.length) return;

    const principalIndex = imagenes.findIndex(i => i.esPrincipal);
    setIndex(principalIndex >= 0 ? principalIndex : 0);
  }, [imagenes]);

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
        <Box
          sx={{
            position: "absolute",
            top: 14,
            right: 14,
            zIndex: 3,
            display: "flex",
            flexDirection: "column",
            gap: 1
          }}
        >
          <Tooltip title={servicio.activo ? "Desactivar servicio" : "Activar servicio"}>
            <IconButton
              size="small"
              onClick={() => onToggleActivo(servicio)}
              sx={{
                bgcolor: "rgba(255,255,255,0.7)",
                backdropFilter: "blur(6px)"
              }}
            >
              {servicio.activo
                ? <VisibilityOffOutlinedIcon fontSize="small" />
                : <VisibilityOutlinedIcon fontSize="small" />
              }
            </IconButton>
          </Tooltip>

          <Tooltip title="Editar servicio">
            <IconButton
              size="small"
              onClick={() => onEditar(servicio)}
              sx={{
                bgcolor: "rgba(255,255,255,0.7)",
                backdropFilter: "blur(6px)"
              }}
            >
              <EditIcon fontSize="small" />
            </IconButton>
          </Tooltip>
        </Box>
      )}

      <Box
        sx={{
          position: "relative",
          height: 350,
          overflow: "hidden",
          borderTopLeftRadius: 12,
          borderTopRightRadius: 12,
          "&:hover .carousel-arrow": {
            opacity: 1
          }
        }}
        onTouchStart={tieneCarrusel && esMobile ? handleTouchStart : undefined}
        onTouchEnd={tieneCarrusel && esMobile ? handleTouchEnd : undefined}
        onClick={() => onOpenGaleria?.(index)}
      >
        <Box
          key={index}
          component="img"
          src={imageUrl}
          alt={servicio.titulo}
          sx={{
            width: "100%",
            height: "100%",
            objectFit: "cover",
          }}
          onError={(e) => {
            e.currentTarget.src = "/servicios/servicio-default.jpg";
          }}
        />

        {tieneCarrusel && !esMobile && (
          <>
            <IconButton
              className="carousel-arrow"
              onClick={(e) => {
                e.stopPropagation();
                setClicked("prev");
                goPrev();
                setTimeout(() => setClicked(null), 150);
              }}
              sx={{
                ...arrowBaseStyles,
                left: 12,
                transform:
                  clicked === "prev"
                    ? "translateY(-50%) scale(0.9)"
                    : "translateY(-50%)"
              }}
            >
              <ChevronLeftRoundedIcon sx={{ fontSize:30 }} />
            </IconButton>

            <IconButton
              className="carousel-arrow"
              onClick={(e) => {
                e.stopPropagation();
                setClicked("next");
                goNext();
                setTimeout(() => setClicked(null), 150);
              }}
              sx={{
                ...arrowBaseStyles,
                right: 12,
                transform:
                  clicked === "next"
                    ? "translateY(-50%) scale(0.9)"
                    : "translateY(-50%)"
              }}
            >
              <ChevronRightRoundedIcon sx={{ fontSize: 30 }} />
            </IconButton>
          </>
        )}

        {tieneCarrusel && (
          <Box
            sx={{
              position: "absolute",
              bottom: 12,
              right: 14,
              display: "flex",
              gap: 0.75,
              zIndex: 3
            }}
          >
            {imagenes.map((_, i) => (
              <Box
                key={i}
                onClick={(e) => {
                  e.stopPropagation();
                  setIndex(i);
                }}
                sx={{
                  width: 6,
                  height: 6,
                  borderRadius: "50%",
                  bgcolor: i === index ? "rgba(255,255,255,0.95)" : "rgba(255,255,255,0.45)",
                  cursor: "pointer",
                  transition: "all .2s ease",
                  "&:hover": {
                    bgcolor: "white"
                  }
                }}
              />
            ))}
          </Box>
        )}
      </Box>

      <Box
        sx={{
          position: "absolute",
          top: 322,
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

      <CardContent sx={{ px: 3, pt: 5, pb: 4, flexGrow: 1 }}>
        <Stack spacing={2}>
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