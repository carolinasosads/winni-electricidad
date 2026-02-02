import { Box, Button, Card, Container, LinearProgress, Typography } from "@mui/material";
import { useNavigate } from "react-router-dom";

export default function Pending() {
  const navigate = useNavigate();

  const handleVolver = () => {
    navigate("/cliente/pago/crear");
  };

  return (
    <Box sx={{ minHeight: "calc(100vh - 64px)", backgroundColor: "#f7f8fb", py: { xs: 4, sm: 6 } }}>
      <Container maxWidth="sm">
        <Card
          sx={{
            borderRadius: 4,
            border: "1px solid #eef0f4",
            boxShadow: "0 10px 30px rgba(0,0,0,0.08)",
          }}
        >
          <Box sx={{ p: 3 }}>
            <Typography variant="h5" sx={{ fontWeight: 900 }}>
              Pago pendiente ⏳
            </Typography>

            <Typography color="text.secondary" sx={{ mt: 1, lineHeight: 1.6 }}>
              Mercado Pago está procesando tu pago. Esto puede demorar unos minutos.
            </Typography>

            <LinearProgress sx={{ mt: 2.5, borderRadius: 2 }} />

            <Button
              onClick={handleVolver}
              variant="outlined"
              fullWidth
              sx={{ mt: 2.5, py: 1.2, fontWeight: 800 }}
            >
              Volver a pagos
            </Button>
          </Box>
        </Card>
      </Container>
    </Box>
  );
}
