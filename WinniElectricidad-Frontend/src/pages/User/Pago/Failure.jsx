import { Box, Button, Card, Container, Typography } from "@mui/material";
import { useNavigate } from "react-router-dom";

export default function Failure() {
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
          <Box sx={{ p: 3 }}>
            <Typography variant="h5" sx={{ fontWeight: 900 }}>
              Pago rechazado ❌
            </Typography>

            <Typography color="text.secondary" sx={{ mt: 1, lineHeight: 1.6 }}>
              No se pudo completar el pago. Podés intentar nuevamente desde la sección de pagos.
            </Typography>

            <Button
              onClick={handleVolver}
              variant="contained"
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
