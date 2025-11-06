import * as React from 'react';
import PropTypes from 'prop-types';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import { Box, CircularProgress } from "@mui/material";
import { FormControl, InputLabel, OutlinedInput } from "@mui/material";
import Alert from '@mui/material/Alert';
import { sendPasswordRecoveryEmail } from '../../../services/authService.js';

function ForgotPassword({ open, handleClose }) {
  const [email, setEmail] = React.useState("");
  const [loading, setLoading] = React.useState(false);
  const [success, setSuccess] = React.useState(false);
  const [error, setError] = React.useState("");

  const handleSubmit = async (event) => {
    event.preventDefault();
    setError("");
    setLoading(true);

    try {
      await sendPasswordRecoveryEmail(email);
      setSuccess(true);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleCloseDialog = () => {
    setEmail("");
    setSuccess(false);
    setError("");
    handleClose();
  };

  return (
    <Dialog
      open={open}
      onClose={handleCloseDialog}
      slotProps={{
        paper: {
          component: 'form',
          onSubmit: handleSubmit,
          sx: { backgroundImage: 'none' },
        },
      }}
    >
      <DialogTitle>Recuperar contraseña</DialogTitle>
      <DialogContent
        sx={{ display: 'flex', flexDirection: 'column', gap: 2, width: '100%' }}
      >
        {!success ? (
          <>
            <DialogContentText>
              Ingresa el correo electrónico asociado a tu cuenta y te enviaremos
              un email con instrucciones.
            </DialogContentText>
            <FormControl fullWidth>
              <InputLabel htmlFor="email">Correo electrónico</InputLabel>
              <OutlinedInput id="email" type="email" label="Correo electrónico" value={email}
                            onChange={(e)=>setEmail(e.target.value)} />
            </FormControl> 

            {error && <Alert variant="outlined" severity="error">{error}</Alert>}
          </>
        ) : (
          <Alert variant="outlined" severity="success">
            Si el correo existe, te enviamos un enlace para restablecer tu
            contraseña.
          </Alert>
        )}
      </DialogContent>
      <DialogActions sx={{ pb: 3, px: 3 }}>
        {!success ? (
          <>
            <Button onClick={handleCloseDialog} disabled={loading}>
              Cancelar
            </Button>
            <Button variant="contained" type="submit" disabled={loading}>
              {loading ? (
                <Box sx={{ display: "flex", alignItems: "center" }}>
                  <CircularProgress size={18} sx={{ mr: 1 }} />
                  Enviando...
                </Box>
              ) : (
                "Continuar"
              )}
            </Button>
          </>
        ) : (
          <Button variant="contained" onClick={handleCloseDialog}>
            Cerrar
          </Button>
        )}
      </DialogActions>
    </Dialog>
  );
}

ForgotPassword.propTypes = {
  handleClose: PropTypes.func.isRequired,
  open: PropTypes.bool.isRequired,
};

export default ForgotPassword;
