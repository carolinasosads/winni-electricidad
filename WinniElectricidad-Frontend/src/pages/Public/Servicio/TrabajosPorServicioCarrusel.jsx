import {
  Box,
  Typography,
  IconButton,
  useMediaQuery
} from "@mui/material";
import ChevronLeftIcon from "@mui/icons-material/ChevronLeft";
import ChevronRightIcon from "@mui/icons-material/ChevronRight";
import { useRef } from "react";

export default function TrabajosPorServicioCarrusel({ trabajos, onSelect }) {
  const scrollRef = useRef(null);
  const isMobile = useMediaQuery("(max-width:768px)");

  if (!trabajos?.length) return null;

  const scroll = (dir) => {
    scrollRef.current?.scrollBy({
      left: dir === "left" ? -320 : 320,
      behavior: "smooth"
    });
  };

  return (
    <Box mt={4}>
      <Typography variant="subtitle1" fontWeight={600} mb={2}>
        Trabajos realizados
      </Typography>

      <Box sx={{ position: "relative" }}>
        {/* Flechas solo desktop */}
        {!isMobile && (
          <>
            <IconButton
              onClick={() => scroll("left")}
              sx={{
                position: "absolute",
                left: -16,
                top: "40%",
                zIndex: 2,
                bgcolor: "white",
                boxShadow: 2,
                "&:hover": { bgcolor: "white" }
              }}
            >
              <ChevronLeftIcon />
            </IconButton>

            <IconButton
              onClick={() => scroll("right")}
              sx={{
                position: "absolute",
                right: -16,
                top: "40%",
                zIndex: 2,
                bgcolor: "white",
                boxShadow: 2,
                "&:hover": { bgcolor: "white" }
              }}
            >
              <ChevronRightIcon />
            </IconButton>
          </>
        )}

        {/* Carrusel */}
        <Box
          ref={scrollRef}
          sx={{
            display: "flex",
            gap: 2,
            overflowX: "auto",
            scrollSnapType: "x mandatory",
            WebkitOverflowScrolling: "touch",
            px: isMobile ? 0 : 3,
            "&::-webkit-scrollbar": { display: "none" }
          }}
        >
          {trabajos.map(t => (
            <Box
              key={t.id}
              onClick={() => onSelect(t)}
              sx={{
                flex: "0 0 auto",
                width: isMobile ? 260 : 300,
                height: 180,
                borderRadius: 3,
                overflow: "hidden",
                cursor: "pointer",
                scrollSnapAlign: "start",
                boxShadow: "0 8px 24px rgba(0,0,0,0.08)",
                transition: "transform .2s ease",
                "&:hover": { transform: "translateY(-4px)" }
              }}
            >
              <Box
                component="img"
                src={`${import.meta.env.VITE_API_URL}${t.imagenUrl}`}
                alt="Trabajo realizado"
                sx={{
                  width: "100%",
                  height: "100%",
                  objectFit: "cover"
                }}
              />
            </Box>
          ))}
        </Box>
      </Box>
    </Box>
  );
}
