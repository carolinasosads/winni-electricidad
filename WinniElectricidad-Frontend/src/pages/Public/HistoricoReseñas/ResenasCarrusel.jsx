import { Box, Typography, IconButton } from "@mui/material";
import ChevronLeftIcon from "@mui/icons-material/ChevronLeft";
import ChevronRightIcon from "@mui/icons-material/ChevronRight";
import { useRef } from "react";

export default function ResenasCarrusel({ resenas, onSelect }) {
  const scrollRef = useRef(null);

  if (!resenas?.length) return null;

  const scroll = (direction) => {
    const { current } = scrollRef;
    if (!current) return;

    const scrollAmount = 360;
    current.scrollBy({
      left: direction === "left" ? -scrollAmount : scrollAmount,
      behavior: "smooth"
    });
  };

  return (
    <Box mt={10}>
      <Typography variant="h5" fontWeight={600} mb={3}>
        Trabajos realizados
      </Typography>

      <Box
        sx={{
            position: "relative",
            overflow: "hidden",
            width: "100%"
        }}
        >

        {/* Flecha izquierda */}
        <IconButton
            onClick={() => scroll("left")}
            sx={{
                position: "absolute",
                top: "50%",
                left: 8,
                transform: "translateY(-50%)",
                bgcolor: "rgba(255,255,255,0.9)",
                boxShadow: 2,
                zIndex: 2,
                "&:hover": { bgcolor: "white" }
            }}
            >
            <ChevronLeftIcon />
        </IconButton>


        {/* Flecha derecha */}
        <IconButton
            onClick={() => scroll("right")}
            sx={{
                position: "absolute",
                top: "50%",
                right: 8,
                transform: "translateY(-50%)",
                bgcolor: "rgba(255,255,255,0.9)",
                boxShadow: 2,
                zIndex: 2,
                "&:hover": { bgcolor: "white" }
            }}
            >
            <ChevronRightIcon />
        </IconButton>


        {/* Carrusel */}
        <Box
          ref={scrollRef}
          sx={{
            display: "flex",
            gap: 3,
            overflowX: "hidden",
            scrollBehavior: "smooth",
            px: 4
          }}
        >
          {resenas.map((r) => (
            <Box
              key={r.id}
              onClick={() => onSelect(r)}
              sx={{
                position: "relative",
                cursor: "pointer",
                flexShrink: 0,
                width: 320,
                height: 220,
                borderRadius: 3,
                overflow: "hidden",
                boxShadow: "0 8px 24px rgba(0,0,0,0.08)",
                transition: "transform .2s ease",
                "&:hover": { transform: "translateY(-6px)" }
              }}
            >
              <Box
                component="img"
                src={`${import.meta.env.VITE_API_URL}${r.imagenUrl}`}
                alt="Trabajo realizado"
                sx={{
                  width: "100%",
                  height: "100%",
                  objectFit: "cover"
                }}
                onError={(e) => (e.currentTarget.style.display = "none")}
              />

              {/* Overlay */}
              <Box
                sx={{
                  position: "absolute",
                  inset: 0,
                  background:
                    "linear-gradient(to top, rgba(0,0,0,0.6), transparent)"
                }}
              />

              {/* Servicio */}
              <Typography
                sx={{
                  position: "absolute",
                  bottom: 12,
                  left: 12,
                  color: "white",
                  fontWeight: 600,
                  fontSize: 14
                }}
              >
                {r.servicio?.titulo ?? "Servicio"}
              </Typography>
            </Box>
          ))}
        </Box>
      </Box>
    </Box>
  );
}
