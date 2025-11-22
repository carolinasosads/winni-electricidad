import * as React from 'react';
import { useTheme } from '@mui/material/styles';
import useMediaQuery from '@mui/material/useMediaQuery';
import Box from '@mui/material/Box';
import Toolbar from '@mui/material/Toolbar';
import { Outlet } from 'react-router-dom';
import AppHeader from '../../pages/Public/Header/SeccionHeader';
import DashboardSidebar from './DashboardSidebar';
import { SitemarkIcon } from '../CustomIcons/CustomIcons';
import LogoutButton from '../UI/Button/Button';
import ThemeSwitcher from './ThemeSwitcher';

export default function DashboardLayout() {
  const theme = useTheme();

  const [isDesktopNavigationExpanded, setIsDesktopNavigationExpanded] = React.useState(true);
  const [isMobileNavigationExpanded, setIsMobileNavigationExpanded] = React.useState(false);

  const isOverMdViewport = useMediaQuery(theme.breakpoints.up('md'));
  const isNavigationExpanded = isOverMdViewport ? isDesktopNavigationExpanded : isMobileNavigationExpanded;

  const setIsNavigationExpanded = React.useCallback((newExpanded) => {
    if (isOverMdViewport) setIsDesktopNavigationExpanded(newExpanded);
    else setIsMobileNavigationExpanded(newExpanded);
  }, [isOverMdViewport]);

  const handleToggleHeaderMenu = React.useCallback((isExpanded) => {
    setIsNavigationExpanded(isExpanded);
  }, [setIsNavigationExpanded]);

  const layoutRef = React.useRef(null);

  return (
    <Box ref={layoutRef} sx={{ position: 'relative', display: 'flex', overflow: 'hidden', height: '100%', width: '100%' }}>
      <AppHeader
        logo={<SitemarkIcon />}
        title=""
        showMenuButton
        menuOpen={isNavigationExpanded}
        onToggleMenu={handleToggleHeaderMenu}
        rightSlot={
            <>
            <ThemeSwitcher />
            <LogoutButton />
            </>
        }
        />

      <DashboardSidebar
        expanded={isNavigationExpanded}
        setExpanded={setIsNavigationExpanded}
        container={layoutRef?.current ?? undefined}
      />

      <Box sx={{ display: 'flex', flexDirection: 'column', flex: 1, minWidth: 0 }}>
        <Toolbar sx={{ displayPrint: 'none' }} />
        <Box component="main" sx={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'auto' }}>
          <Outlet />
        </Box>
      </Box>
    </Box>
  );
}