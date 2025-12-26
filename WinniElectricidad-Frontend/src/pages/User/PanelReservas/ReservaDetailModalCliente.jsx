import { Box, Modal, Paper, Typography, Divider, Button, IconButton, Alert } from "@mui/material";

import CloseIcon from "@mui/icons-material/Close";
import CheckCircleOutline from "@mui/icons-material/CheckCircleOutline";
import HighlightOff from "@mui/icons-material/HighlightOff";
import EditCalendar from "@mui/icons-material/EditCalendar";

const fechaNormalizada = (fechaIso) => {
    if (!fechaIso) return "";
    const d = new Date(fechaIso);
    return d.toLocaleDateString("es-UY", { day: "2-digit", month: "2-digit", year: "numeric" });
};

const fechaHoraNormalizada = (fechaIso) => {
    if (!fechaIso) return "";
    const d = new Date(fechaIso);
    return d.toLocaleString("es-UY", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
        hour: "2-digit",
        minute: "2-digit",
    });
};

const serviciosComoTexto = (reserva) => {
    if (!reserva) return "";

    if (Array.isArray(reserva.servicios) && reserva.servicios.length > 0) {
        if (typeof reserva.servicios[0] === "string") {
            return reserva.servicios.filter(Boolean).join(", ");
        }
        if (typeof reserva.servicios[0] === "object") {
            return reserva.servicios.map((s) => s?.titulo).filter(Boolean).join(", ");
        }
    }

    const ns = (reserva.nombreServicio || "").trim();
    return ns || "";
};

const direccionComoTexto = (reserva) => {
    if (typeof reserva?.direccion === "string") return reserva.direccion;

    if (reserva?.direccion && typeof reserva.direccion === "object") {
        const d = reserva.direccion;

        const partes = [
            d.calle,
            d.numero,
            d.apartamento ? `Apto ${d.apartamento}` : null,
            d.esquina ? `Esq. ${d.esquina}` : null,
            d.barrio,
            d.ciudad,
            d.departamento,
            d.codigoPostal,
        ].filter(Boolean);

        return partes.join(", ");
    }

    if (typeof reserva?.direccionTexto === "string") return reserva.direccionTexto;

    return "";
};

const getTituloReserva = (reserva) => {
    const fecha = fechaNormalizada(reserva.fechaReserva);
    const servicios = serviciosComoTexto(reserva);
    return servicios ? `${servicios} · ${fecha}` : `Reserva del ${fecha}`;
};

export default function ReservaDetailModalCliente({
                                                      open,  onClose,  reserva,  onAceptar,  onCancelar,  onModificar,  feedback,  onClearFeedback,  isWorking,
                                                  }) {
    if (!reserva) return null;

    const estado = (reserva.estado || "").toLowerCase();
    const esPendiente = estado === "pendiente";
    const esConfirmada = estado === "confirmada";

    const requiereConfirmacionCliente = !!reserva.requiereConfirmacionCliente;

    const serviciosTxt = serviciosComoTexto(reserva);
    const direccionTxt = direccionComoTexto(reserva);
    const comentarioTxt = (reserva.comentario || "").trim();
    const fechaTxt = fechaHoraNormalizada(reserva.fechaReserva);
    const tipoServicioTxt =
        (reserva.tipoServicio || reserva.tipoServicioReserva || reserva.tipo || "").toString?.().trim?.() || "";

    return (
        <Modal open={open} onClose={onClose}>
            <Box sx={{ width: "95%", maxWidth: 520, mx: "auto", mt: "8vh", outline: "none" }}>
                <Paper sx={{ p: 3, borderRadius: 3 }}>
                    <Box sx={{ display: "flex", justifyContent: "space-between", mb: 2 }}>
                        <Typography variant="h6" fontWeight={600}>
                            {getTituloReserva(reserva)}
                        </Typography>
                        <IconButton onClick={onClose} disabled={!!isWorking}>
                            <CloseIcon />
                        </IconButton>
                    </Box>

                    {!!feedback?.text && (
                        <Alert severity={feedback.severity || "info"} sx={{ mb: 2 }} onClose={onClearFeedback}>
                            {feedback.text}
                        </Alert>
                    )}

                    <Divider sx={{ mb: 2 }} />

                    {esPendiente && requiereConfirmacionCliente && (
                        <Alert severity="warning" sx={{ mb: 2 }}>
                            <strong>El representante sugiere un cambio de fecha.</strong>
                            <br />
                            Podés aprobar la nueva fecha, cancelarla o proponer otra modificación.
                        </Alert>
                    )}

                    <Box sx={{ display: "flex", flexDirection: "column", gap: 0.75 }}>
                        <Typography>
                            <strong>Estado:</strong> {reserva.estado}
                        </Typography>

                        {!!fechaTxt && (
                            <Typography>
                                <strong>Fecha:</strong> {fechaTxt}
                            </Typography>
                        )}

                        {!!serviciosTxt && (
                            <Typography>
                                <strong>Servicios:</strong> {serviciosTxt}
                            </Typography>
                        )}

                        {!!tipoServicioTxt && (
                            <Typography>
                                <strong>Tipo:</strong> {tipoServicioTxt}
                            </Typography>
                        )}

                        {!!direccionTxt && (
                            <Typography>
                                <strong>Dirección:</strong> {direccionTxt}
                            </Typography>
                        )}

                        <Typography sx={{ whiteSpace: "pre-wrap" }}>
                            <strong>Comentario:</strong> {comentarioTxt || "Sin comentarios."}
                        </Typography>
                    </Box>

                    <Box sx={{ mt: 3, display: "flex", flexDirection: "column", gap: 1.5 }}>
                        {esPendiente && requiereConfirmacionCliente && (
                            <Button
                                variant="contained"
                                color="success"
                                startIcon={<CheckCircleOutline />}
                                onClick={() => onAceptar(reserva)}
                                disabled={!!isWorking}
                            >
                                Aprobar nueva fecha
                            </Button>
                        )}

                        {esPendiente && !requiereConfirmacionCliente && (
                            <Alert severity="info">
                                Esta reserva está pendiente de revisión del administrador. Cuando haya un cambio que necesite tu
                                confirmación, vas a poder aceptarla desde acá.
                            </Alert>
                        )}

                        {(esPendiente || esConfirmada) && (
                            <>
                                <Button
                                    variant="outlined"
                                    startIcon={<EditCalendar />}
                                    onClick={() => onModificar(reserva)}
                                    disabled={!!isWorking}
                                >
                                    {esConfirmada ? "Sugerir modificación" : "Modificar"}
                                </Button>

                                <Button
                                    variant="outlined"
                                    color="error"
                                    startIcon={<HighlightOff />}
                                    onClick={() => onCancelar(reserva)}
                                    disabled={!!isWorking}
                                >
                                    Cancelar
                                </Button>
                            </>
                        )}
                    </Box>
                </Paper>
            </Box>
        </Modal>
    );
}