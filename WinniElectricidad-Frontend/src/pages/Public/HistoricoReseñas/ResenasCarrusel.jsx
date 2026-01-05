import { Box, Typography, IconButton, useMediaQuery } from "@mui/material";
import { useTheme } from "@mui/material/styles";
import ChevronLeftIcon from "@mui/icons-material/ChevronLeft";
import ChevronRightIcon from "@mui/icons-material/ChevronRight";
import { useRef } from "react";

export default function ResenasCarrusel({ resenas, onSelect }) {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down("sm"));
  const scrollRef = useRef(null);

  if (!resenas?.length) return null;

  const scroll = (direction) => {
    if (!scrollRef.current) return;
    scrollRef.current.scrollBy({
      left: direction === "left" ? -360 : 360,
      behavior: "smooth",
    });
  };

  return (
    <Box mt={10}>
      <Typography variant="h5" fontWeight={600} mb={3}>
        Trabajos realizados
      </Typography>

      {/* ================= MOBILE ================= */}
      {isMobile && (
        <Box
          sx={{
            display: "flex",
            flexDirection: "column",
            gap: 3,
          }}
        >
          {resenas.map((r) => (
            <Box
              key={r.id}
              onClick={() => onSelect(r)}
              sx={{
                width: "100%",
                height: 220,
                borderRadius: 3,
                overflow: "hidden",
                boxShadow: "0 8px 24px rgba(0,0,0,0.08)",
                cursor: "pointer",
              }}
            >
              <Box
                component="img"
                src={`${import.meta.env.VITE_API_URL}${r.imagenUrl}`}
                alt="Trabajo realizado"
                sx={{
                  width: "100%",
                  height: "100%",
                  objectFit: "cover",
                }}
                onError={(e) => (e.currentTarget.style.display = "none")}
              />

              <Typography
                sx={{
                  position: "absolute",
                  bottom: 12,
                  left: 12,
                  color: "white",
                  fontWeight: 600,
                  fontSize: 14,
                  textShadow: "0 2px 6px rgba(0,0,0,.6)",
                }}
              >
                {r.servicio?.titulo ?? "Servicio"}
              </Typography>
            </Box>
          ))}
        </Box>
      )}

      {/* ================= DESKTOP ================= */}
      {!isMobile && (
        <Box
          sx={{
            position: "relative",
            overflow: "hidden",
          }}
        >
          <IconButton
            onClick={() => scroll("left")}
            sx={{
              position: "absolute",
              top: "50%",
              left: 8,
              transform: "translateY(-50%)",
              zIndex: 2,
              bgcolor: "background.paper",
              boxShadow: 2,
            }}
          >
            <ChevronLeftIcon />
          </IconButton>

          <IconButton
            onClick={() => scroll("right")}
            sx={{
              position: "absolute",
              top: "50%",
              right: 8,
              transform: "translateY(-50%)",
              zIndex: 2,
              bgcolor: "background.paper",
              boxShadow: 2,
            }}
          >
            <ChevronRightIcon />
          </IconButton>

          <Box
            ref={scrollRef}
            sx={{
              display: "flex",
              gap: 3,
              overflowX: "hidden",
              px: 4,
            }}
          >
            {resenas.map((r) => (
              <Box
                key={r.id}
                onClick={() => onSelect(r)}
                sx={{
                  flexShrink: 0,
                  width: 320,
                  height: 220,
                  borderRadius: 3,
                  overflow: "hidden",
                  boxShadow: "0 8px 24px rgba(0,0,0,0.08)",
                  cursor: "pointer",
                }}
              >
                <Box
                  component="img"
                  src={`${import.meta.env.VITE_API_URL}${r.imagenUrl}`}
                  alt="Trabajo realizado"
                  sx={{
                    width: "100%",
                    height: "100%",
                    objectFit: "cover",
                  }}
                />
              </Box>
            ))}
          </Box>
        </Box>
      )}
    </Box>
  );
}
