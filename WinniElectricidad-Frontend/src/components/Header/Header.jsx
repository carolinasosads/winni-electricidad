import * as React from "react";
import PropTypes from "prop-types";
import { styled, useTheme } from "@mui/material/styles";
import {
  Box,
  AppBar as MuiAppBar,
  Toolbar,
  IconButton,
  Tooltip,
  Typography,
  Stack,
  Button
} from "@mui/material";
import MenuIcon from "@mui/icons-material/Menu";
import MenuOpenIcon from "@mui/icons-material/MenuOpen";
import RateReviewIcon from "@mui/icons-material/RateReview";
import { Link } from "react-router-dom";

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
  menuOpen = false,
  onToggleMenu,
  rightSlot,
  homeHref = "/resenas"
}) {
  const theme = useTheme();

  const handleMenuClick = () => {
    onToggleMenu?.(!menuOpen);
  };

  const getResenasPathByRol = (rol) => {
    switch (rol) {
      case "Administrador":
        return "/admin/resenas";
      case "Cliente":
        return "/cliente/resenas";
      default:
        return "/resenas";
    }
  };

  const rol = localStorage.getItem("rol");
  const resenasPath = getResenasPathByRol(rol);

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
            <Stack direction="row" spacing={2} alignItems="center">
              {/* Link público a reseñas */}
              <Link to={resenasPath} style={{ textDecoration: "none" }}>
                <Button
                  variant="outlined"
                  size="small"
                  startIcon={<RateReviewIcon />}
                  sx={{
                    textTransform: "none",
                    borderRadius: 2
                  }}
                >
                  Reseñas
                </Button>
              </Link>
              {/* Botones externos (logout, login, etc.) */}
              {rightSlot}
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
