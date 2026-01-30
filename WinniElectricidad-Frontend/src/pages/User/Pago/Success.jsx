import { Box, Button, Card, CardContent, Container, Typography } from "@mui/material";
import { useNavigate } from "react-router-dom";

export default function Success() {
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
            overflow: "hidden",
          }}
        >
          <Box
            sx={{
              px: 3,
              py: 3,
              color: "white",
              backgroundImage: "linear-gradient(135deg, #0f2027, #203a43, #2c5364)",
            }}
          >
            <Typography variant="h5" sx={{ fontWeight: 900 }}>
              ¡Pago confirmado! ✅
            </Typography>
            <Typography sx={{ opacity: 0.92, mt: 0.5 }}>
              Tu pago con Mercado Pago se procesó correctamente.
            </Typography>
          </Box>

          <CardContent sx={{ p: 3 }}>
            <Typography color="text.secondary" sx={{ lineHeight: 1.6 }}>
              Ya podés volver a la sección de pagos.
            </Typography>

            <Button
              onClick={handleVolver}
              variant="contained"
              fullWidth
              sx={{ mt: 2.5, py: 1.2, fontWeight: 800 }}
            >
              Volver a pagos
            </Button>
          </CardContent>
        </Card>
      </Container>
    </Box>
  );
}
