export default function PagoExitosoConBoton({ onVolver }) {
  return (
    <div style={{ padding: 16 }}>
      <h2>¡Pago confirmado! ✅</h2>
      <p>Tu pago con Mercado Pago se procesó correctamente.</p>
      <button onClick={onVolver} style={{ marginTop: 12 }}>
        Volver
      </button>
    </div>
  );
}