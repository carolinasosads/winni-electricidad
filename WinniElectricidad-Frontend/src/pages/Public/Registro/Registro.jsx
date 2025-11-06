import * as React from 'react';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import CssBaseline from '@mui/material/CssBaseline';
import Divider from '@mui/material/Divider';
import FormLabel from '@mui/material/FormLabel';
import FormControl from '@mui/material/FormControl';
import TextField from '@mui/material/TextField';
import Typography from '@mui/material/Typography';
import Stack from '@mui/material/Stack';
import MuiCard from '@mui/material/Card';
import { styled } from '@mui/material/styles';

import AppTheme from "../../../shared-theme/AppTheme.jsx";
import ColorModeSelect from "../../../shared-theme/ColorModeSelect.jsx";
import { SitemarkIcon } from '../../../components/CustomIcons/CustomIcons.jsx';
import HCaptcha from '@hcaptcha/react-hcaptcha';
import { Link, useNavigate } from 'react-router-dom';
import { registro } from '../../../services/authService.js';

const Card = styled(MuiCard)(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  alignSelf: 'center',
  width: '100%',
  padding: theme.spacing(4),
  gap: theme.spacing(2),
  margin: 'auto',
  maxHeight: 'calc(100dvh - 48px)',
  overflow: 'hidden',
  boxShadow:
    'hsla(220, 30%, 5%, 0.05) 0px 5px 15px 0px, hsla(220, 25%, 10%, 0.05) 0px 15px 35px -5px',
  [theme.breakpoints.up('sm')]: { width: '450px' },
  ...theme.applyStyles('dark', {
    boxShadow:
      'hsla(220, 30%, 5%, 0.5) 0px 5px 15px 0px, hsla(220, 25%, 10%, 0.08) 0px 15px 35px -5px',
  }),
}));

const SignUpContainer = styled(Stack)(({ theme }) => ({
  height: 'calc((1 - var(--template-frame-height, 0)) * 100dvh)',
  minHeight: '100%',
  padding: theme.spacing(2),
  [theme.breakpoints.up('sm')]: { padding: theme.spacing(4) },
  '&::before': {
    content: '""',
    display: 'block',
    position: 'absolute',
    zIndex: -1,
    inset: 0,
    backgroundImage:
      'radial-gradient(ellipse at 50% 50%, hsl(210, 100%, 97%), hsl(0, 0%, 100%))',
    backgroundRepeat: 'no-repeat',
    ...theme.applyStyles('dark', {
      backgroundImage:
        'radial-gradient(at 50% 50%, hsla(210, 100%, 16%, 0.5), hsl(220, 30%, 5%))',
    }),
  },
}));

export default function SignUp(props) {
  const [emailError, setEmailError] = React.useState(false);
  const [emailErrorMessage, setEmailErrorMessage] = React.useState('');
  const [passwordError, setPasswordError] = React.useState(false);
  const [passwordErrorMessage, setPasswordErrorMessage] = React.useState('');
  const [nameError, setNameError] = React.useState(false);
  const [nameErrorMessage, setNameErrorMessage] = React.useState('');
  const [telefonoError, setTelefonoError] = React.useState(false);
  const [telefonoErrorMessage, setTelefonoErrorMessage] = React.useState('');

  // Dirección principal (con 4 campos)
  const [dirPrincipal, setDirPrincipal] = React.useState({
    calle: '',
    esquina: '',
    numero: '',
    apto: '',
  });
  const [dirPrincipalError, setDirPrincipalError] = React.useState({
    calle: false,
    esquina: false,
  });
  const [dirPrincipalMsg, setDirPrincipalMsg] = React.useState({
    calle: '',
    esquina: '',
  });

  // Direcciones opcionales: array de objetos {calle, esquina, numero, apto}
  const [direccionesExtra, setDireccionesExtra] = React.useState([]);

  // hCaptcha
  const [captchaToken, setCaptchaToken] = React.useState(null);
  const [captchaError, setCaptchaError] = React.useState('');
  const [isSubmitting, setIsSubmitting] = React.useState(false);

  const navigate = useNavigate();
  const captchaRef = React.useRef(null);

  // Validaciones
  const validateInputs = () => {
    const email = document.getElementById('email');
    const password = document.getElementById('password');
    const name = document.getElementById('name');
    const telefono = document.getElementById('telefono');


    let isValid = true;

    if (!email.value || !/\S+@\S+\.\S+/.test(email.value)) {
      setEmailError(true);
      setEmailErrorMessage('Por favor ingrese un correo valido.');
      isValid = false;
    } else {
      setEmailError(false);
      setEmailErrorMessage('');
    }

    if (!password.value || password.value.length < 6) {
      setPasswordError(true);
      setPasswordErrorMessage('La contraseña debe tener un mínimo de 6 dígitos.');
      isValid = false;
    } else {
      setPasswordError(false);
      setPasswordErrorMessage('');
    }

    if (!name.value || name.value.trim().length < 1) {
      setNameError(true);
      setNameErrorMessage('Nombre completo es obligatorio.');
      isValid = false;
    } else {
      setNameError(false);
      setNameErrorMessage('');
    }

    if (!telefono.value || !/^\d{7,15}$/.test(telefono.value)) {
      setTelefonoError(true);
      setTelefonoErrorMessage('Por favor entra un numero de telefono valido (7–15 dígitos).');
      isValid = false;
    } else {
      setTelefonoError(false);
      setTelefonoErrorMessage('');
    }

    // Validar dirección principal: Calle y Esquina obligatorias
    const errs = { calle: false, esquina: false };
    const msgs = { calle: '', esquina: '' };

    if (!dirPrincipal.calle.trim()) {
      errs.calle = true;
      msgs.calle = 'La calle es obligatoria.';
      isValid = false;
    }
    if (!dirPrincipal.esquina.trim()) {
      errs.esquina = true;
      msgs.esquina = 'La esquina es obligatoria.';
      isValid = false;
    }

    setDirPrincipalError(errs);
    setDirPrincipalMsg(msgs);

    return isValid;
  };

  // Handlers direcciones extra
  const handleAddDireccionExtra = () => {
    setDireccionesExtra((prev) => [...prev, { calle: '', esquina: '', numero: '', apto: '' }]);
  };

  const handleCambioDireccionExtra = (index, field, value) => {
    setDireccionesExtra((prev) => {
      const copy = [...prev];
      copy[index] = { ...copy[index], [field]: value };
      return copy;
    });
  };

  const handleBorrarDireccionExtra = (index) => {
    setDireccionesExtra((prev) => prev.filter((_, i) => i !== index));
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    const isValid = validateInputs();
    if (!isValid) return;

    if (!captchaToken) {
      setCaptchaError('Por favor completá el captcha.');
      return;
    }

    setIsSubmitting(true);
    try {
      const data = new FormData(event.currentTarget);

      const p = {
        name: data.get('name'),
        email: data.get('email'),
        telefono: data.get('telefono'),
        password: data.get('password'),
        hcaptchaToken: captchaToken,
        // Un solo array de direcciones 
        direcciones: [
          {
            calle: dirPrincipal.calle.trim(),
            esquina: dirPrincipal.esquina.trim(),
            numero: (dirPrincipal.numero || '').trim() || null,
            apto: (dirPrincipal.apto || '').trim() || null,
          },
          ...direccionesExtra
            .map(d => ({
              calle: (d.calle || '').trim(),
              esquina: (d.esquina || '').trim(),
              numero: (d.numero || '').trim() || null,
              apto: (d.apto || '').trim() || null,
            }))
            .filter(d => d.calle || d.esquina || d.numero || d.apto),
        ].filter(d => d.calle || d.esquina || d.numero || d.apto),
      };

      await registro(p);
      navigate("/principal");
    } catch (e) {
      setCaptchaError(e.message || "No pudimos completar el registro.");
      captchaRef.current?.resetCaptcha();
      setCaptchaToken(null);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <AppTheme {...props}>
      <CssBaseline enableColorScheme />
      <ColorModeSelect sx={{ position: 'fixed', top: '1rem', right: '1rem' }} />
      <SignUpContainer direction="column" justifyContent="space-between">
        <Card variant="outlined">
          <SitemarkIcon />
          <Typography component="h1" variant="h4" sx={{ width: '100%', fontSize: 'clamp(2rem, 10vw, 2.15rem)' }}>
            Regístrate
          </Typography>

          <Box
            component="form"
            onSubmit={handleSubmit}
            sx={{
              display: 'flex',
              flexDirection: 'column',
              gap: 2,
              overflowY: 'auto',
              // deja espacio para el título y el footer (botón + links)
              maxHeight: { xs: 'calc(100dvh - 220px)', sm: 'calc(100dvh - 260px)' },
              pr: 1,
            }}
          >
            {/* Nombre */}
            <FormControl>
              <FormLabel htmlFor="name">Nombre completo</FormLabel>
              <TextField
                autoComplete="name"
                name="name"
                required
                fullWidth
                id="name"
                placeholder="Jon Snow"
                error={nameError}
                helperText={nameErrorMessage}
                color={nameError ? 'error' : 'primary'}
              />
            </FormControl>
            <FormControl>
              <FormLabel htmlFor="email">Email</FormLabel>
              <TextField
                required
                fullWidth
                id="email"
                placeholder="your@email.com"
                name="email"
                autoComplete="email"
                variant="outlined"
                error={emailError}
                helperText={emailErrorMessage}
                color={emailError ? 'error' : 'primary'}
              />
            </FormControl>

            {/* Teléfono */}
            <FormControl>
              <FormLabel htmlFor="telefono">Teléfono</FormLabel>
              <TextField
                required
                fullWidth
                id="telefono"
                name="telefono"
                placeholder="099123456"
                autoComplete="tel"
                inputProps={{ inputMode: 'numeric', pattern: '[0-9]*' }}
                error={telefonoError}
                helperText={telefonoErrorMessage}
                color={telefonoError ? 'error' : 'primary'}
              />
            </FormControl>

            {/* Password */}
            <FormControl>
              <FormLabel htmlFor="password">Contraseña</FormLabel>
              <TextField
                required
                fullWidth
                name="password"
                placeholder="••••••"
                type="password"
                id="password"
                autoComplete="new-password"
                variant="outlined"
                error={passwordError}
                helperText={passwordErrorMessage}
                color={passwordError ? 'error' : 'primary'}
              />
            </FormControl>

            {/* Dirección principal */}
            <Stack gap={1}>
              <Typography variant="body2" sx={{ fontWeight: 600 }}>
                Dirección principal
              </Typography>
              <Stack direction={{ xs: 'column', sm: 'row' }} gap={1}>
                <TextField
                  required
                  fullWidth
                  id="dirPrincipalCalle"
                  label="Calle"
                  value={dirPrincipal.calle}
                  onChange={(e) => setDirPrincipal({ ...dirPrincipal, calle: e.target.value })}
                  error={dirPrincipalError.calle}
                  helperText={dirPrincipalMsg.calle}
                />
                <TextField
                  required
                  fullWidth
                  id="dirPrincipalEsquina"
                  label="Esquina"
                  value={dirPrincipal.esquina}
                  onChange={(e) => setDirPrincipal({ ...dirPrincipal, esquina: e.target.value })}
                  error={dirPrincipalError.esquina}
                  helperText={dirPrincipalMsg.esquina}
                />
              </Stack>
              <Stack direction={{ xs: 'column', sm: 'row' }} gap={1}>
                <TextField
                  fullWidth
                  id="dirPrincipalNumero"
                  label="Número (opcional)"
                  value={dirPrincipal.numero}
                  onChange={(e) => setDirPrincipal({ ...dirPrincipal, numero: e.target.value })}
                />
                <TextField
                  fullWidth
                  id="dirPrincipalApto"
                  label="Apto (opcional)"
                  value={dirPrincipal.apto}
                  onChange={(e) => setDirPrincipal({ ...dirPrincipal, apto: e.target.value })}
                />
              </Stack>
            </Stack>

            {/* Direcciones opcionales */}
            <Stack gap={1}>
              <Typography variant="body2" sx={{ fontWeight: 600 }}>
                Otras direcciones (opcional)
              </Typography>
              {direccionesExtra.map((d, index) => (
                <Stack key={index} gap={1}>
                  <Stack direction={{ xs: 'column', sm: 'row' }} gap={1}>
                    <TextField
                      fullWidth
                      label="Calle"
                      value={d.calle}
                      onChange={(e) => handleCambioDireccionExtra(index, 'calle', e.target.value)}
                    />
                    <TextField
                      fullWidth
                      label="Esquina"
                      value={d.esquina}
                      onChange={(e) => handleCambioDireccionExtra(index, 'esquina', e.target.value)}
                    />
                  </Stack>
                  <Stack direction={{ xs: 'column', sm: 'row' }} gap={1}>
                    <TextField
                      fullWidth
                      label="Número (opcional)"
                      value={d.numero}
                      onChange={(e) => handleCambioDireccionExtra(index, 'numero', e.target.value)}
                    />
                    <TextField
                      fullWidth
                      label="Apto (opcional)"
                      value={d.apto}
                      onChange={(e) => handleCambioDireccionExtra(index, 'apto', e.target.value)}
                    />
                    <Button type="button" onClick={() => handleBorrarDireccionExtra(index)}>
                      X
                    </Button>
                  </Stack>
                  <Divider />
                </Stack>
              ))}
              <Button type="button" variant="outlined" onClick={handleAddDireccionExtra}>
                Agregar otra dirección
              </Button>
            </Stack>

            {/* hCaptcha */}
            <HCaptcha
              ref={captchaRef}
              sitekey={import.meta.env.VITE_HCAPTCHA_SITEKEY || "10000000-ffff-ffff-ffff-000000000001"} 
              theme="light"
              size="normal"
              languageOverride="es"
              onVerify={(token) => { setCaptchaToken(token); setCaptchaError(''); }}
              onExpire={() => { setCaptchaToken(null); setCaptchaError('El captcha expiró, por favor completalo de nuevo.'); }}
              onError={() => { setCaptchaToken(null); setCaptchaError('Hubo un problema con el captcha, reintenta.'); }}
            />
            {captchaError && (
              <Typography color="error" variant="body2" sx={{ mt: 1 }}>
                {captchaError}
              </Typography>
            )}

            <Button
              type="submit"
              fullWidth
              variant="contained"
              disabled={!captchaToken || isSubmitting}
            >
              {isSubmitting ? 'Enviando…' : 'Regístrate'}
            </Button>
          </Box>
          <Divider>
            <Typography sx={{ color: 'text.secondary' }}>or</Typography>
          </Divider>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
            <Typography sx={{ textAlign: 'center' }}>
              ¿Ya tienes una cuenta? <Link to="/login">Iniciar sesión</Link>
            </Typography>
          </Box>
        </Card>
      </SignUpContainer>
    </AppTheme>
  );
}
