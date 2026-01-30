import { useState, useRef } from "react";
import {
  Box,
  Card,
  CardContent,
  Typography,
  IconButton,
  Stack,
  Divider,
  useMediaQuery
} from "@mui/material";
import { useTheme } from "@mui/material/styles";

import ChevronLeftRoundedIcon from "@mui/icons-material/ChevronLeftRounded";
import ChevronRightRoundedIcon from "@mui/icons-material/ChevronRightRounded";

export default function PresupuestoCarrusel({
  presupuestos,
  seleccionadoId,
  onSeleccionar,
  bloqueado
}) {
  const theme = useTheme();
  const esMobile = useMediaQuery(theme.breakpoints.down("sm"));

  const [index, setIndex] = useState(0);
  const touchStartX = useRef(null);

  if (!presupuestos || presupuestos.length === 0) return null;

  const presupuesto = presupuestos[index];
  const saldoPendiente = presupuesto.montoTotal - (presupuesto.montoPagado ?? 0);

  const formatearFecha = (isoDate) => {
    if (!isoDate) return "";
    const date = new Date(isoDate);

    return date.toLocaleDateString("es-UY", {
      day: "2-digit",
      month: "2-digit",
      year: "numeric",
    });
  };

  const goPrev = () => {
    setIndex((i) => (i - 1 + presupuestos.length) % presupuestos.length);
  };

  const goNext = () => {
    setIndex((i) => (i + 1) % presupuestos.length);
  };

  const handleTouchStart = (e) => {
    if (bloqueado) return;
    touchStartX.current = e.touches[0].clientX;
  };

  const handleTouchEnd = (e) => {
    if (bloqueado) return;
    if (touchStartX.current === null) return;
    const delta = e.changedTouches[0].clientX - touchStartX.current;
    if (Math.abs(delta) > 50) {
      if (delta > 0) {
        goPrev();
      } else {
        goNext();
      }
    }

    touchStartX.current = null;
  };

  return (
    <Box sx={{ width: "100%" }}>
      <Box
        sx={{
          display: "flex",
          alignItems: "stretch",
          width: "100%",
          gap: 1,
        }}
        onTouchStart={esMobile ? handleTouchStart : undefined}
        onTouchEnd={esMobile ? handleTouchEnd : undefined}
      >
        {!esMobile && presupuestos.length > 1 && (
          <Box sx={{ display: "flex", alignItems: "center" }}>
            <IconButton
              onClick={goPrev}
              disabled={bloqueado}
              sx={{
                bgcolor: "background.paper",
                boxShadow: 2,
                "&:hover": { bgcolor: "grey.100" },
              }}
            >
              <ChevronLeftRoundedIcon />
            </IconButton>
          </Box>
        )}

        <Card
          onClick={() => onSeleccionar(presupuesto)}
          sx={{
            flex: 1,
            opacity: bloqueado ? 0.9 : 1,
            cursor: bloqueado ? "default" : "pointer",
            borderRadius: 3,
            border:
              seleccionadoId === presupuesto.id
                ? "2px solid"
                : "1px solid",
            borderColor:
              seleccionadoId === presupuesto.id
                ? "primary.main"
                : "divider",
            boxShadow: "0 8px 24px rgba(0,0,0,0.06)",
            transition: "all .2s ease",
            "&:hover": {
              boxShadow: "0 12px 32px rgba(0,0,0,0.08)",
            },
          }}
        >
          <CardContent>
            <Stack spacing={1.2}>
              <Typography fontWeight={600}>
                Reserva
              </Typography>

              <Typography variant="body2" color="text.secondary">
                {formatearFecha(presupuesto.reserva.fechaReserva)}
              </Typography>

              <Typography variant="body2" color="text.secondary">
                Dirección: {presupuesto.reserva.direccion}
              </Typography>

              <Typography variant="body2" color="text.secondary">
                Servicio/s: {presupuesto.reserva.servicios.join(", ")}
              </Typography>

              <Divider sx={{ my: 1 }} />

              <Box
                sx={{
                  bgcolor: "grey.50",
                  borderRadius: 2,
                  px: 2,
                  py: 1.5,
                }}
              >
                <Stack spacing={0.5}>
                  <Typography variant="body2">
                    Total: ${presupuesto.montoTotal}
                  </Typography>

                  <Typography variant="body2" color="primary.main">
                    Pagado: ${presupuesto.montoPagado}
                  </Typography>

                  <Typography variant="h6" fontWeight={700}>
                    Restante: ${saldoPendiente}
                  </Typography>
                </Stack>
              </Box>
            </Stack>
          </CardContent>
        </Card>

        {!esMobile && presupuestos.length > 1 && (
          <Box sx={{ display: "flex", alignItems: "center" }}>
            <IconButton
              onClick={goNext}
              disabled={bloqueado}
              sx={{
                bgcolor: "background.paper",
                boxShadow: 2,
                "&:hover": { bgcolor: "grey.100" },
              }}
            >
              <ChevronRightRoundedIcon />
            </IconButton>
          </Box>
        )}
      </Box>

      {presupuestos.length > 1 && (
        <Box
          sx={{
            display: "flex",
            justifyContent: "center",
            gap: 1,
            mt: 1.5,
          }}
        >
          {presupuestos.map((_, i) => (
            <Box
              key={i}
              sx={{
                width: 6,
                height: 6,
                borderRadius: "50%",
                bgcolor:
                  i === index ? "primary.main" : "grey.400",
                transition: "all .2s ease",
              }}
            />
          ))}
        </Box>
      )}
    </Box>
  );
}
