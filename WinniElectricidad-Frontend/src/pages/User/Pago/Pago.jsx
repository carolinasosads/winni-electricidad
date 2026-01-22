import { useEffect, useState } from "react";
import {
  Box,
  Button,
  TextField,
  Typography,
  Alert,
  Divider,
  CircularProgress,
} from "@mui/material";

import { initMercadoPago, Wallet } from "@mercadopago/sdk-react";
import PresupuestoCarousel from "./PresupuestoCarrusel";
import { obtenerPresupuestoSegunIdUsuario } from "../../../services/presupuestoService";
import { crearPago } from "../../../services/pagoService";

initMercadoPago(import.meta.env.VITE_MP_PUBLIC_KEY);

export default function Pago() {
  const [monto, setMonto] = useState("");
  const [preferenceId, setPreferenceId] = useState(null);
  const [loading, setLoading] = useState(false);
  const [errorMsg, setErrorMsg] = useState("");
  const [presupuestos, setPresupuestos] = useState([]);
  const [presupuestoSeleccionado, setPresupuestoSeleccionado] =
    useState(null);

  const saldoPendiente = presupuestoSeleccionado
    ? presupuestoSeleccionado.montoTotal -
    presupuestoSeleccionado.montoPagado
    : 0;


  const loadPresupuestos = async (signal) => {
    try {
      const data = await obtenerPresupuestoSegunIdUsuario(signal)
      setPresupuestos(data);
    } catch (err) {
      if (err.name === "AbortError") return;
      setErrorMsg(err?.message || "Error al cargar los servicios.");
    }
  };

  useEffect(() => {
    setErrorMsg("");
    const ac = new AbortController();
    loadPresupuestos(ac.signal);
    return () => ac.abort();
  }, []);

  const handleConfirmarPago = async () => {
    if (!presupuestoSeleccionado) {
      setErrorMsg("Debes seleccionar un presupuesto.");
      return;
    }

    if (!monto || Number(monto) <= 0) {
      setErrorMsg("El monto debe ser mayor a 0.");
      return;
    }

    if (Number(monto) > saldoPendiente) {
      setErrorMsg("El monto supera el saldo pendiente.");
      return;
    }

    setLoading(true);
    setErrorMsg("");

    try {
      const pago = { idPresupuesto: presupuestoSeleccionado.id, monto: Number(monto) };
      const resp = await crearPago(pago);

      const prefId = resp?.IdPreference ?? resp?.idPreference ?? resp?.preferenceId ?? resp?.id;
      if (!prefId) throw new Error("No se recibió preferenceId del backend.");

      setPreferenceId(prefId);
    } catch (e) {
      setErrorMsg(e?.message || "Error al generar el pago.");
    } finally {
      setLoading(false);
    }

  };

  return (
    <Box
      sx={{
        width: "100%",
        px: { xs: 2, sm: 0 },
        py: { xs: 3, sm: 5 },
        display: "flex",
        justifyContent: "center",
      }}
    >
      <Box
        sx={{
          width: "100%",
          maxWidth: 520,
          display: "flex",
          flexDirection: "column",
          gap: 2.5,
          px: { xs: 2, sm: 3 },
          py: { xs: 3, sm: 4 },
        }}
      >
        <Box
          sx={{
            width: 48,
            height: 4,
            backgroundColor: "primary.main",
            borderRadius: 2,
          }}
        />

        <Typography variant="h5" fontWeight={600}>
          Realizar Pago
        </Typography>

        <Divider />

        {errorMsg && <Alert severity="error">{errorMsg}</Alert>}

        {presupuestos.length === 0 && (
          <Alert severity="info">
            Aún no tenés presupuestos disponibles para realizar pagos.
            Cuando un técnico genere un presupuesto, lo vas a poder abonar desde acá.
          </Alert>
        )}

        {presupuestos.length > 0 && (
          <PresupuestoCarousel
            presupuestos={presupuestos}
            seleccionadoId={presupuestoSeleccionado?.id}
            onSeleccionar={setPresupuestoSeleccionado}
          />
        )}

        <TextField
          label="Monto a pagar"
          type="number"
          fullWidth
          value={monto}
          onChange={(e) => setMonto(e.target.value)}
          inputProps={{ min: 1, max: saldoPendiente }}
        />

        {!preferenceId && (
          <Button
            variant="contained"
            fullWidth
            onClick={handleConfirmarPago}
            disabled={loading}
            sx={{
              py: 1.6,
              fontSize: "1rem",
              position: { xs: "sticky", sm: "static" },
              bottom: 0,
            }}
          >
            {loading ? (
              <CircularProgress size={24} />
            ) : (
              "Confirmar"
            )}
          </Button>
        )}

        {preferenceId && (
          <>
            <Alert severity="success">
              Pago listo para ser realizado
            </Alert>

            <Wallet initialization={{ preferenceId }} />
          </>
        )}
      </Box>
    </Box>
  );
}
