import { useEffect, useRef, useState } from "react";
import {  Card,  CardContent,  Typography,  Rating,  Chip,  IconButton,  Tooltip,
  Box,  Stack,  Menu,  MenuItem,  ListItemIcon,  ListItemText} from "@mui/material";

import DoNotDisturbOnIcon from "@mui/icons-material/DoNotDisturbOn";
import ShareIcon from "@mui/icons-material/Share";

import FacebookIcon from "@mui/icons-material/Facebook";
import WhatsAppIcon from "@mui/icons-material/WhatsApp";
import XIcon from "@mui/icons-material/X";

const MAX_LINES = 3;

export default function ResenaCard({ resena, esAdmin, onDesaprobar, onVerMas }) {
  const textRef = useRef(null);
  const [tieneOverflow, setTieneOverflow] = useState(false);

  const [anchorEl, setAnchorEl] = useState(null);
  const openMenu = Boolean(anchorEl);

  useEffect(() => {
    const el = textRef.current;
    if (!el) return;

    const medir = () => {
      const overflow = el.scrollHeight - el.clientHeight > 1;
      setTieneOverflow(overflow);
    };

    medir();

    const ro = new ResizeObserver(medir);
    ro.observe(el);

    return () => ro.disconnect();
  }, [resena?.descripcion]);

  const resenaId =
    resena?.idReseña ??
    resena?.idResena ??
    resena?.id ??
    resena?.resenaId ??
    null;

  const anchorId = `resena-${resenaId}`;
  const urlCompartir = `${window.location.origin}/resenas#${anchorId}`;

  const textoCompartir = `Mirá esta reseña: "${resena?.descripcion ?? ""}"`;

  const shareUrls = {
    facebook: `https://www.facebook.com/sharer/sharer.php?u=${encodeURIComponent(
      urlCompartir
    )}`,
    whatsapp: `https://wa.me/?text=${encodeURIComponent(
      `${textoCompartir}\n${urlCompartir}`
    )}`,
    x: `https://twitter.com/intent/tweet?text=${encodeURIComponent(
      textoCompartir
    )}&url=${encodeURIComponent(urlCompartir)}`
  };

  const abrirShare = (url) => {
    window.open(url, "_blank", "noopener,noreferrer");
    setAnchorEl(null);
  };

  return (
    <Card
      sx={{
        position: "relative",
        borderRadius: 3,
        height: "100%",
        boxShadow: "0 8px 24px rgba(0,0,0,0.06)",
        width: "100%",
        minWidth: 0
      }}
    >
      {esAdmin && (
        <Tooltip title="Desaprobar reseña">
          <IconButton
            size="small"
            onClick={() => onDesaprobar(resena)}
            sx={{
              position: "absolute",
              top: 14,
              right: 14,
              zIndex: 2,
              color: "grey.500",
              "&:hover": {
                color: "error.main",
                bgcolor: "rgba(0,0,0,0.04)"
              }
            }}
          >
            <DoNotDisturbOnIcon fontSize="small" />
          </IconButton>
        </Tooltip>
      )}

      <CardContent
        sx={{
          px: { xs: 2, sm: 4 },
          pt: { xs: 4, sm: 6 },
          pb: { xs: 3, sm: 4 },
          textAlign: "center"
        }}
      >
        <Rating value={resena.calificacion} readOnly size="large" />

        <Box
          sx={{
            mt: 2,
            mb: 3,
            position: "relative",
            px: 1
          }}
        >
          <Typography
            ref={textRef}
            variant="body1"
            sx={{
              fontStyle: "italic",
              lineHeight: 1.7,

              display: "-webkit-box",
              WebkitLineClamp: MAX_LINES,
              WebkitBoxOrient: "vertical",
              overflow: "hidden",

              overflowWrap: "anywhere",
              wordBreak: "break-word"
            }}
          >
            “{resena.descripcion}”
          </Typography>

          {tieneOverflow && (
            <>
              <Box
                sx={{
                  position: "absolute",
                  left: 0,
                  right: 0,
                  bottom: 0,
                  height: 28,
                  pointerEvents: "none",
                  background:
                    "linear-gradient(to bottom, rgba(255,255,255,0), rgba(255,255,255,1))"
                }}
              />

              <Typography
                component="span"
                onClick={() => onVerMas(resena)}
                sx={{
                  position: "absolute",
                  right: 6,
                  bottom: 2,
                  cursor: "pointer",
                  color: "primary.main",
                  fontSize: "0.875rem",
                  fontWeight: 500,
                  bgcolor: "rgba(255,255,255,0.9)",
                  px: 0.5,
                  borderRadius: 1,
                  "&:hover": { textDecoration: "underline" }
                }}
              >
                Ver más
              </Typography>
            </>
          )}
        </Box>

        <Typography fontWeight={600}>
          {resena.cliente?.nombre ?? "Cliente"}
        </Typography>

        <Chip
          size="small"
          label={resena.servicio?.titulo ?? "Servicio"}
          sx={{ bgcolor: "grey.100", mt: 1 }}
        />

        <Typography
          variant="caption"
          color="text.secondary"
          sx={{ mt: 1, display: "block" }}
        >
          {new Date(resena.fechaReseña).toLocaleDateString("es-UY")}
        </Typography>

        {/* ----COMPARTIR RESEÑA ---- */}
        <Stack direction="row" justifyContent="center" mt={1}>
          <Tooltip title="Compartir">
            <IconButton
              size="small"
              onClick={(e) => setAnchorEl(e.currentTarget)}
            >
              <ShareIcon fontSize="small" />
            </IconButton>
          </Tooltip>

          <Menu
            anchorEl={anchorEl}
            open={openMenu}
            onClose={() => setAnchorEl(null)}
            anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
            transformOrigin={{ vertical: "top", horizontal: "center" }}
          >
            <MenuItem onClick={() => abrirShare(shareUrls.facebook)}>
              <ListItemIcon>
                <FacebookIcon fontSize="small" />
              </ListItemIcon>
              <ListItemText>Facebook</ListItemText>
            </MenuItem>

            <MenuItem onClick={() => abrirShare(shareUrls.whatsapp)}>
              <ListItemIcon>
                <WhatsAppIcon fontSize="small" />
              </ListItemIcon>
              <ListItemText>WhatsApp</ListItemText>
            </MenuItem>

            <MenuItem onClick={() => abrirShare(shareUrls.x)}>
              <ListItemIcon>
                <XIcon fontSize="small" />
              </ListItemIcon>
              <ListItemText>X</ListItemText>
            </MenuItem>
          </Menu>
        </Stack>
      </CardContent>
    </Card>
  );
}
