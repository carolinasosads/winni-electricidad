import PropTypes from "prop-types";
import { styled, useTheme } from "@mui/material/styles";
import {  AppBar as MuiAppBar,  Toolbar,  IconButton,  Tooltip,  Typography,  Stack,  Button, Box} from "@mui/material";
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

const mobileIconButtonSx = {
  width: 36,
  height: 36,
  borderRadius: "50%",
  border: "1.5px solid #0288d1",
  color: "#0288d1",
  backgroundColor: "transparent",
  transition: "all .2s ease",
  "&:hover": {
    backgroundColor: "#0288d1",
    color: "white",
  },
  "&:active": {
    transform: "scale(0.95)",
  },
};

 const authSlotDefault = estaLogueado ? (
  isMobile ? (
    <Tooltip title="Cerrar sesión">
      <IconButton
        onClick={handleLogout}
        sx={mobileIconButtonSx}
      >
        <LogoutIcon fontSize="small" />
      </IconButton>
    </Tooltip>
  ) : (
    <Button
      onClick={handleLogout}
      variant="outlined"
      startIcon={<LogoutIcon />}
      sx={{
        textTransform: "none",
        borderRadius: 999,
        px: 2.5,
        py: 0.8,
        fontWeight: 500,
        color: "#0288d1",
        borderColor: "#0288d1",
        backgroundColor: "rgba(15, 42, 68, 0.04)",
        transition: "all .2s ease",
        "&:hover": {
          backgroundColor: "#0288d1",
          color: "white",
          borderColor: "#0288d1",
        },
        "&:active": {
          transform: "scale(0.97)",
        },
      }}
    >
      Cerrar sesión
    </Button>
  )
) : null;

const actionButtonSx = {
  textTransform: "none",
  borderRadius: 999,
  px: 2.5,
  py: 0.8,
  fontWeight: 500,
  color: "#0288d1",
  borderColor: "#0288d1",
  backgroundColor: "rgba(15, 42, 68, 0.04)",
  transition: "all .2s ease",
  "&:hover": {
    backgroundColor: "#0288d1",
    color: "white",
    borderColor: "#0288d1",
  },
  "&:active": {
    transform: "scale(0.97)",
  },
};

  return (
    <AppBar color="inherit" position="absolute">
      <Toolbar sx={{ px: { xs: 1, sm: 2 } }}>
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
                  <Box sx={{ ml: 1 }}>
                    {typeof title === "string" ? (
                      <Typography
                        variant="h6"
                        sx={{
                          color: theme.palette.primary.main,
                          fontWeight: 700,
                          whiteSpace: "nowrap"
                        }}
                      >
                        {title}
                      </Typography>
                    ) : (
                      title
                    )}
                  </Box>
                )}
              </Stack>
            </Link>
          </Stack>

          {/* DERECHA */}
          <Stack direction="row" spacing={0.5} alignItems="center">
            {isMobile ? (
            <>
              <Tooltip title="Servicios">
              <IconButton
                component={Link}
                to={serviciosPath}
                sx={mobileIconButtonSx}
              >
                <BuildIcon fontSize="small" />
              </IconButton>
            </Tooltip>

            <Tooltip title="Reseñas">
              <IconButton
                component={Link}
                to={resenasPath}
                sx={mobileIconButtonSx}
              >
                <RateReviewIcon fontSize="small" />
              </IconButton>
            </Tooltip>
            </>
          ) : (
              <>
                <Link to={serviciosPath} style={{ textDecoration: "none" }}>
                  <Button
                    variant="outlined"
                    startIcon={<BuildIcon />}
                    sx={actionButtonSx}
                  >
                    Servicios
                  </Button>
                </Link>

              <Link to={resenasPath} style={{ textDecoration: "none" }}>
                <Button
                  variant="outlined"
                  startIcon={<RateReviewIcon />}
                  sx={actionButtonSx}
                >
                  Reseñas
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
  homeHref: PropTypes.string
};