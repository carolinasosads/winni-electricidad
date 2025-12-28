import PropTypes from "prop-types";
import { styled, useTheme } from "@mui/material/styles";
import {  AppBar as MuiAppBar,  Toolbar,  IconButton,  Tooltip,  Typography,  Stack,  Button} from "@mui/material";
import MenuIcon from "@mui/icons-material/Menu";
import MenuOpenIcon from "@mui/icons-material/MenuOpen";
import RateReviewIcon from "@mui/icons-material/RateReview";
import BuildIcon from '@mui/icons-material/Build';
import LogoutIcon from "@mui/icons-material/Logout";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import { Link, useNavigate } from "react-router-dom";
import { useMediaQuery } from "@mui/material";

const AppBar = styled(MuiAppBar)(({ theme }) => ({
  borderBottom: `1px solid ${(theme.vars ?? theme).palette.divider}`,
  boxShadow: "none",
  zIndex: theme.zIndex.drawer + 1
}));

const LogoContainer = styled("div")({
  height: 40,
  display: "flex",
  alignItems: "center",
  "& img": { maxHeight: 40 }
});

export default function AppHeader({
  logo,
  title = "",
  showMenuButton = false,
  showBackButton = false,
  menuOpen = false,
  onToggleMenu,
  rightSlot,
  homeHref = "/"
}) {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down("sm"));

  const navigate = useNavigate();

  const handleMenuClick = () => {
    onToggleMenu?.(!menuOpen);
  };

  const getPathByRol = (rol, page) => {
    switch (rol) {
      case "Administrador":
        return `/admin/${page}`;
      case "Cliente":
        return `/cliente/${page}`;
      default:
        return `/${page}`;
    }
  };

  const token = localStorage.getItem("token");
  const estaLogueado = Boolean(token);

  const rol = localStorage.getItem("rol");
  const resenasPath = getPathByRol(rol, "resenas");
  const serviciosPath = getPathByRol(rol, "servicios");

  const handleLogout = () => {
  localStorage.removeItem("token");
  localStorage.removeItem("rol");

  window.dispatchEvent(new Event("auth-change"));

  navigate(homeHref, { replace: true });
};

 const authSlotDefault = estaLogueado ? (
  isMobile ? (
    <Tooltip title="Cerrar sesión">
      <IconButton size="small" color="primary" onClick={handleLogout}>
        <LogoutIcon />
      </IconButton>
    </Tooltip>
  ) : (
    <Button
      onClick={handleLogout}
      variant="outlined"
      size="small"
      startIcon={<LogoutIcon />}
      sx={{ textTransform: "none", borderRadius: 2 }}
    >
      Cerrar sesión
    </Button>
  )
) : null;

  return (
    <AppBar color="inherit" position="absolute">
      <Toolbar sx={{ mx: { xs: -0.75, sm: -1 } }}>
        <Stack
          direction="row"
          justifyContent="space-between"
          alignItems="center"
          sx={{ width: "100%" }}
        >
          {/* IZQUIERDA */}
          <Stack direction="row" alignItems="center">
            {showMenuButton && (
              <Tooltip title={menuOpen ? "Cerrar menú" : "Abrir menú"}>
                <IconButton size="small" onClick={handleMenuClick}>
                  {menuOpen ? <MenuOpenIcon /> : <MenuIcon />}
                </IconButton>
              </Tooltip>
            )}

            {showBackButton && (
              <Tooltip title="Volver al inicio">
                <IconButton
                  size="small"
                  onClick={() => navigate(homeHref)}
                  sx={{ mr: 0.5 }}
                >
                  <ArrowBackIcon />
                </IconButton>
              </Tooltip>
            )}

            <Link to={homeHref} style={{ textDecoration: "none" }}>
              <Stack direction="row" alignItems="center" ml={1}>
                {logo && <LogoContainer>{logo}</LogoContainer>}
                {title && (
                  <Typography
                    variant="h6"
                    sx={{
                      color: theme.palette.primary.main,
                      fontWeight: 700,
                      ml: 1,
                      whiteSpace: "nowrap"
                    }}
                  >
                    {title}
                  </Typography>
                )}
              </Stack>
            </Link>
          </Stack>

          {/* DERECHA */}
          <Stack direction="row" spacing={0.5} alignItems="center">
            {isMobile ? (
              <>
                <Tooltip title="Reseñas">
                  <IconButton size="small" color="primary" component={Link} to={resenasPath}>
                    <RateReviewIcon />
                  </IconButton>
                </Tooltip>

                <Tooltip title="Servicios">
                  <IconButton size="small" color="primary" component={Link} to={serviciosPath}>
                    <BuildIcon />
                  </IconButton>
                </Tooltip>
              </>
            ) : (
              <>
                <Link to={resenasPath} style={{ textDecoration: "none" }}>
                  <Button
                    variant="outlined"
                    size="small"
                    startIcon={<RateReviewIcon />}
                    sx={{ textTransform: "none", borderRadius: 2 }}
                  >
                    Reseñas
                  </Button>
                </Link>

                <Link to={serviciosPath} style={{ textDecoration: "none" }}>
                  <Button
                    variant="outlined"
                    size="small"
                    startIcon={<BuildIcon />}
                    sx={{ textTransform: "none", borderRadius: 2 }}
                  >
                    Servicios
                  </Button>
                </Link>
              </>
            )}

            {authSlotDefault}
          </Stack>
        </Stack>
      </Toolbar>
    </AppBar>
  );
}

AppHeader.propTypes = {
  logo: PropTypes.node,
  title: PropTypes.string,
  showMenuButton: PropTypes.bool,
  menuOpen: PropTypes.bool,
  onToggleMenu: PropTypes.func,
  rightSlot: PropTypes.node,
  homeHref: PropTypes.string
};
