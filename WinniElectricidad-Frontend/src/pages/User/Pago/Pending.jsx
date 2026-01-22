export default function PagoPendienteConBoton({ onVolver }) {
  return (
    <div style={{ padding: 16 }}>
      <h2>Pago pendiente ⏳</h2>
      <p>Tu pago está siendo procesado por Mercado Pago.</p>
      <p>Esto puede demorar unos minutos.</p>
      <button onClick={onVolver} style={{ marginTop: 12 }}>
        Volver
      </button>
    </div>
  );
}
