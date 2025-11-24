import * as React from 'react';
import PropTypes from 'prop-types';
import { styled, useTheme } from '@mui/material/styles';
import Box from '@mui/material/Box';
import MuiAppBar from '@mui/material/AppBar';
import IconButton from '@mui/material/IconButton';
import Toolbar from '@mui/material/Toolbar';
import Tooltip from '@mui/material/Tooltip';
import Typography from '@mui/material/Typography';
import MenuIcon from '@mui/icons-material/Menu';
import MenuOpenIcon from '@mui/icons-material/MenuOpen';
import Stack from '@mui/material/Stack';
import { Link } from 'react-router-dom';

const AppBar = styled(MuiAppBar)(({ theme }) => ({
  borderWidth: 0,
  borderBottomWidth: 1,
  borderStyle: 'solid',
  borderColor: (theme.vars ?? theme).palette.divider,
  boxShadow: 'none',
  zIndex: theme.zIndex.drawer + 1,
}));

const LogoContainer = styled('div')({
  position: 'relative',
  height: 40,
  display: 'flex',
  alignItems: 'center',
  '& img': { maxHeight: 40 },
});


export default function AppHeader({
  logo,
  title = '',
  showMenuButton = false,
  menuOpen = false,
  onToggleMenu,
  rightSlot,
  homeHref = '/',
}) {
  const theme = useTheme();

  const handleMenuClick = React.useCallback(() => {
    onToggleMenu?.(!menuOpen);
  }, [menuOpen, onToggleMenu]);

  const MenuToggle = (
    <Tooltip
      title={`${menuOpen ? 'Collapse' : 'Expand'} menu`}
      enterDelay={1000}
    >
      <span>
        <IconButton
          size="small"
          aria-label={`${menuOpen ? 'Collapse' : 'Expand'} navigation menu`}
          onClick={handleMenuClick}
          disabled={!showMenuButton || !onToggleMenu}
        >
          {menuOpen ? <MenuOpenIcon /> : <MenuIcon />}
        </IconButton>
      </span>
    </Tooltip>
  );

  return (
    <AppBar color="inherit" position="absolute" sx={{ displayPrint: 'none' }}>
      <Toolbar sx={{ backgroundColor: 'inherit', mx: { xs: -0.75, sm: -1 } }}>
        <Stack direction="row" justifyContent="space-between" alignItems="center" sx={{ flexWrap: 'wrap', width: '100%' }}>
          <Stack direction="row" alignItems="center">
            {showMenuButton ? <Box sx={{ mr: 1 }}>{MenuToggle}</Box> : null}
            <Link to={homeHref} style={{ textDecoration: 'none' }}>
              <Stack direction="row" alignItems="center">
                {logo ? <LogoContainer>{logo}</LogoContainer> : null}
                {title ? (
                  <Typography
                    variant="h6"
                    sx={{
                      color: (theme.vars ?? theme).palette.primary.main,
                      fontWeight: 700,
                      ml: 1,
                      whiteSpace: 'nowrap',
                      lineHeight: 1,
                    }}
                  >
                    {title}
                  </Typography>
                ) : null}
              </Stack>
            </Link>
          </Stack>

          {/* Botonera derecha inyectable */}
          {rightSlot ? (
            <Stack direction="row" alignItems="center" spacing={2} sx={{ marginLeft: 'auto' }}>
              {rightSlot}
            </Stack>
          ) : null}
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
  homeHref: PropTypes.string,
};