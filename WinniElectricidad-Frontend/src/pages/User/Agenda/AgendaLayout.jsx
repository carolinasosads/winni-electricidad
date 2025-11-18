import * as React from 'react';
import { Box, Toolbar, Stack } from '@mui/material';
import { Outlet, useNavigate } from 'react-router-dom';
import { useTheme } from '@mui/material/styles';
import useMediaQuery from '@mui/material/useMediaQuery';

import AppHeader from '../../Public/Header/SeccionHeader.jsx';
import LogoutButton from '../../../components/UI/Button/Button.jsx';
import DashboardSidebar from '../../../components/MUI/DashboardSidebar.jsx';
import ThemeSwitcher from '../../../components/MUI/ThemeSwitcher.jsx';

export default function AgendaLayout() {
  const theme = useTheme();
  const overMd = useMediaQuery(theme.breakpoints.up('md'));
  const navigate = useNavigate();

  const [expandedDesktop, setExpandedDesktop] = React.useState(true);
  const [expandedMobile, setExpandedMobile] = React.useState(false);
  const expanded = overMd ? expandedDesktop : expandedMobile;

  const setExpanded = (val) => {
    if (overMd) setExpandedDesktop(val);
    else setExpandedMobile(val);
  };

  const layoutRef = React.useRef(null);

  return (
    <Box
      ref={layoutRef}
      sx={{ position: 'relative', display: 'flex', height: '100vh', width: '100%' }}
    >
      {/* Header */}
      <AppHeader
        title="Winni Electricidad"
        showMenuButton
        menuOpen={expanded}
        onToggleMenu={(next) => setExpanded(next)}
        homeHref="/principal"
        rightSlot={
            <>
            <ThemeSwitcher />
            <LogoutButton />
            </>
        }
        />

      {/* Sidebar */}
      <DashboardSidebar
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
    </Box>
  );
}