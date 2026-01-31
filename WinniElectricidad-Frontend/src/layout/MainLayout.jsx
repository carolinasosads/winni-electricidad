import * as React from 'react';
import { Box, Toolbar } from '@mui/material';
import { Outlet } from 'react-router-dom';
import { useTheme } from '@mui/material/styles';
import useMediaQuery from '@mui/material/useMediaQuery';

import AppHeader from '../components/Header/Header.jsx';
import SeccionSideBar from '../components/SideBar/SideBar.jsx';
import { SitemarkIcon } from "../components/CustomIcons/CustomIcons.jsx";
import WhatsAppIcon from "@mui/icons-material/WhatsApp";
import IconButton from "@mui/material/IconButton";
import Tooltip from "@mui/material/Tooltip";
import Fade from "@mui/material/Fade";

export default function MainLayout() {
  const theme = useTheme();
  const overMd = useMediaQuery(theme.breakpoints.up('md'));

  const [expandedDesktop, setExpandedDesktop] = React.useState(true);
  const [expandedMobile, setExpandedMobile] = React.useState(false);
  const expanded = overMd ? expandedDesktop : expandedMobile;
  const [showHint, setShowHint] = React.useState(false);

  const setExpanded = (val) => {
    if (overMd) setExpandedDesktop(val);
    else setExpandedMobile(val);
  };

  const layoutRef = React.useRef(null);

  React.useEffect(() => {
    const timer = setTimeout(() => {
      setShowHint(true);
    }, 6000);

    const hideTimer = setTimeout(() => {
      setShowHint(false);
    }, 12000);

    return () => {
      clearTimeout(timer);
      clearTimeout(hideTimer);
    };
  }, []);

  return (
    <Box
      ref={layoutRef}
      sx={{ position: 'relative', display: 'flex', height: '100vh', width: '100%' }}
    >
      {/* Header */}
      <AppHeader
        logo={<SitemarkIcon />}
        title=""
        showMenuButton
        menuOpen={expanded}
        onToggleMenu={(next) => setExpanded(next)}
        homeHref="/"
      />
      {/* Sidebar */}
      <SeccionSideBar
        expanded={expanded}
        setExpanded={setExpanded}
        container={layoutRef?.current ?? undefined}
      />
      {/* Contenido */}
      <Box sx={{ display: 'flex', flexDirection: 'column', flex: 1, minWidth: 0 }}>
        {/* separador para el AppBar position="absolute" */}
        <Toolbar sx={{ displayPrint: 'none' }} />
        <Box component="main" sx={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'auto' }}>
          <Outlet />
        </Box>
      </Box>

      <Tooltip
        title="¿Tenés dudas? Contactanos"
        open={showHint}
        placement="left"
        arrow
        TransitionComponent={Fade}
      >
        <IconButton
          component="a"
          href="https://wa.me/59894224578?text=Hola%20Winni%20Electricidad,%20te%20contacto%20desde%20la%20web%20para%20..."
          target="_blank"
          rel="noopener noreferrer"
          sx={{
            position: "fixed",
            bottom: { xs: 16, sm: 24 },
            right: { xs: 16, sm: 24 },
            backgroundColor: "#25D366",
            color: "white",
            width: 56,
            height: 56,
            boxShadow: "0 8px 24px rgba(0,0,0,0.25)",
            zIndex: 3000,
            "&:hover": {
              backgroundColor: "#1ebe5d",
            },
          }}
        >
          <WhatsAppIcon sx={{ fontSize: 30 }} />
        </IconButton>
      </Tooltip>
    </Box>
  );
}