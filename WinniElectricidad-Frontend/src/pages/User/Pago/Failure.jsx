export default function PagoFallidoConBoton({ onReintentar }) {
  return (
    <div style={{ padding: 16 }}>
      <h2>Pago no realizado ❌</h2>
      <p>No se pudo completar el pago con Mercado Pago.</p>
      <p>Por favor, intentá nuevamente.</p>
      <button onClick={onReintentar} style={{ marginTop: 12 }}>
        Reintentar pago
      </button>
    </div>
  );
}
