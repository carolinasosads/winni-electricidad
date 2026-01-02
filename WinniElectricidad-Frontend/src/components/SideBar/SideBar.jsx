import * as React from 'react';
import PropTypes from 'prop-types';
import { useTheme } from '@mui/material/styles';
import useMediaQuery from '@mui/material/useMediaQuery';
import Box from '@mui/material/Box';
import Drawer from '@mui/material/Drawer';
import List from '@mui/material/List';
import Toolbar from '@mui/material/Toolbar';

import BarChartIcon from '@mui/icons-material/BarChart';
import AccountBoxIcon from '@mui/icons-material/AccountBox';
import DateRangeIcon from '@mui/icons-material/DateRange';
import GroupIcon from '@mui/icons-material/Group';
import ViewAgendaIcon from '@mui/icons-material/ViewAgenda';
import RateReviewIcon from '@mui/icons-material/RateReview';
import { matchPath, useLocation } from 'react-router-dom';

import SidebarContext from './Context/SideBarContext';
import { DRAWER_WIDTH, MINI_DRAWER_WIDTH } from './Context/Constants'
import SidebarPageItem from './Context/SideBarPageItem';
import SidebarHeaderItem from './Context/SideBarHeaderItem';

import {
  getDrawerSxTransitionMixin,
  getDrawerWidthTransitionMixin,
} from '../SideBar/Context/Mixins';

function Sidebar({
  expanded = true,
  setExpanded,
  disableCollapsibleSidebar = false,
  container,
}) {
  const theme = useTheme();

  const { pathname } = useLocation();

  const [expandedItemIds, setExpandedItemIds] = React.useState([]);

  const isOverSmViewport = useMediaQuery(theme.breakpoints.up('sm'));
  const isOverMdViewport = useMediaQuery(theme.breakpoints.up('md'));

  const [isFullyExpanded, setIsFullyExpanded] = React.useState(expanded);
  const [isFullyCollapsed, setIsFullyCollapsed] = React.useState(!expanded);

  React.useEffect(() => {
    if (expanded) {
      const drawerWidthTransitionTimeout = setTimeout(() => {
        setIsFullyExpanded(true);
      }, theme.transitions.duration.enteringScreen);

      return () => clearTimeout(drawerWidthTransitionTimeout);
    }

    setIsFullyExpanded(false);

    return () => {};
  }, [expanded, theme.transitions.duration.enteringScreen]);

  React.useEffect(() => {
    if (!expanded) {
      const drawerWidthTransitionTimeout = setTimeout(() => {
        setIsFullyCollapsed(true);
      }, theme.transitions.duration.leavingScreen);

      return () => clearTimeout(drawerWidthTransitionTimeout);
    }

    setIsFullyCollapsed(false);

    return () => {};
  }, [expanded, theme.transitions.duration.leavingScreen]);

  const mini = !disableCollapsibleSidebar && !expanded;

  const handleSetSidebarExpanded = React.useCallback(
    (newExpanded) => () => {
      setExpanded(newExpanded);
    },
    [setExpanded],
  );

  const handlePageItemClick = React.useCallback(
    (itemId, hasNestedNavigation) => {
      if (hasNestedNavigation && !mini) {
        setExpandedItemIds((previousValue) =>
          previousValue.includes(itemId)
            ? previousValue.filter(
              (previousValueItemId) => previousValueItemId !== itemId,
            )
            : [...previousValue, itemId],
        );
      } else if (!isOverSmViewport && !hasNestedNavigation) {
        setExpanded(false);
      }
    },
    [mini, setExpanded, isOverSmViewport],
  );

  const hasDrawerTransitions =
    isOverSmViewport && (!disableCollapsibleSidebar || isOverMdViewport);

  const getDrawerContent = React.useCallback(
    (viewport) => {
      const rol = localStorage.getItem("rol");

      const isAdmin = rol === "Administrador";
      const isCliente = rol === "Cliente";

      return (
        <React.Fragment>
          <Toolbar />
          <Box
            component="nav"
            aria-label={`${viewport.charAt(0).toUpperCase()}${viewport.slice(1)}`}
            sx={{
              height: '100%',
              display: 'flex',
              flexDirection: 'column',
              justifyContent: 'space-between',
              overflow: 'auto',
              scrollbarGutter: mini ? 'stable' : 'auto',
              overflowX: 'hidden',
              pt: !mini ? 0 : 2,
              ...(hasDrawerTransitions
                ? getDrawerSxTransitionMixin(isFullyExpanded, 'padding')
                : {}),
            }}
          >
            <List
              dense
              sx={{
                padding: mini ? 0 : 0.5,
                mb: 4,
                width: mini ? MINI_DRAWER_WIDTH : 'auto',
              }}
            >
              <SidebarHeaderItem>Menú</SidebarHeaderItem>

              {/* CLIENTE */}
              {isCliente && (
                <>
                  <SidebarPageItem
                    id="dashboard-cliente"
                    title="Dashboard"
                    icon={<AccountBoxIcon />}
                    href="/cliente"
                    selected={pathname === "/cliente"}
                  />

                  <SidebarPageItem
                    id="agenda-cliente"
                    title="Agenda"
                    icon={<DateRangeIcon />}
                    href="/cliente/agenda"
                    selected={pathname.startsWith("/cliente/agenda")}
                  />

                  <SidebarPageItem
                    id="resena-cliente"
                    title="Nueva reseña"
                    icon={<RateReviewIcon />}
                    href="/cliente/resenas/crear"
                    selected={pathname.startsWith("/cliente/resenas/crear")}
                  />
                </>
              )}

              {/* ADMINISTRADOR */}
              {isAdmin && (
                <>
                  <SidebarPageItem
                    id="admin-dashboard"
                    title="Dashboard Admin"
                    icon={<BarChartIcon />}
                    href="/admin"
                    selected={pathname === "/admin" || pathname === "/admin/"}
                  />

                  <SidebarPageItem
                    id="admin-historico-clientes"
                    title="Histórico de Clientes"
                    icon={<GroupIcon />}
                    href="/admin/historico"
                    selected={pathname.startsWith("/admin/historico")}
                  />

                  <SidebarPageItem
                    id="admin-panel-reservas"
                    title="Panel de Reservas"
                    icon={<ViewAgendaIcon />}
                    href="/admin/panel-reservas"
                    selected={pathname.startsWith("/admin/panel-reservas")}
                  />

                  <SidebarPageItem
                    id="admin-crear-cliente-reserva"
                    title="Gestionar Presupuesto"
                    icon={<DateRangeIcon />}
                    href="/admin/crear-reserva"
                    selected={pathname.startsWith("/admin/crear-reserva")}
                  />
                  <SidebarPageItem
                    id="admin-ficha-clientes"
                    title="Ficha clientes"
                    icon={<AccountBoxIcon />}
                    href="/admin/ficha-clientes"
                    selected={pathname.startsWith("/admin/ficha-clientes")}
                  />
                </>
              )}
            </List>
          </Box>
        </React.Fragment>
      );
    },
    [
      mini,
      hasDrawerTransitions,
      isFullyExpanded,
      expandedItemIds,
      pathname,
    ]
  );

  const getDrawerSharedSx = React.useCallback(
    (isTemporary) => {
      const drawerWidth = mini ? MINI_DRAWER_WIDTH : DRAWER_WIDTH;

      return {
        displayPrint: 'none',
        width: drawerWidth,
        flexShrink: 0,
        ...getDrawerWidthTransitionMixin(expanded),
        ...(isTemporary ? { position: 'absolute' } : {}),
        [`& .MuiDrawer-paper`]: {
          position: 'absolute',
          width: drawerWidth,
          boxSizing: 'border-box',
          backgroundImage: 'none',
          ...getDrawerWidthTransitionMixin(expanded),
        },
      };
    },
    [expanded, mini],
  );

  const sidebarContextValue = React.useMemo(() => {
    return {
      onPageItemClick: handlePageItemClick,
      mini,
      fullyExpanded: isFullyExpanded,
      fullyCollapsed: isFullyCollapsed,
      hasDrawerTransitions,
    };
  }, [
    handlePageItemClick,
    mini,
    isFullyExpanded,
    isFullyCollapsed,
    hasDrawerTransitions,
  ]);

  return (
    <SidebarContext.Provider value={sidebarContextValue}>
      <Drawer
        container={container}
        variant="temporary"
        open={expanded}
        onClose={handleSetSidebarExpanded(false)}
        ModalProps={{
          keepMounted: true, // Better open performance on mobile.
        }}
        sx={{
          display: {
            xs: 'block',
            sm: disableCollapsibleSidebar ? 'block' : 'none',
            md: 'none',
          },
          ...getDrawerSharedSx(true),
        }}
      >
        {getDrawerContent('phone')}
      </Drawer>
      <Drawer
        variant="permanent"
        sx={{
          display: {
            xs: 'none',
            sm: disableCollapsibleSidebar ? 'none' : 'block',
            md: 'none',
          },
          ...getDrawerSharedSx(false),
        }}
      >
        {getDrawerContent('tablet')}
      </Drawer>
      <Drawer
        variant="permanent"
        sx={{
          display: { xs: 'none', md: 'block' },
          ...getDrawerSharedSx(false),
        }}
      >
        {getDrawerContent('desktop')}
      </Drawer>
    </SidebarContext.Provider>
  );
}

Sidebar.propTypes = {
  container: (props, propName) => {
    if (props[propName] == null) {
      return null;
    }
    if (typeof props[propName] !== 'object' || props[propName].nodeType !== 1) {
      return new Error(`Expected prop '${propName}' to be of type Element`);
    }
    return null;
  },
  disableCollapsibleSidebar: PropTypes.bool,
  expanded: PropTypes.bool,
  setExpanded: PropTypes.func.isRequired,
};

export default Sidebar;
