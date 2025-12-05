import * as React from 'react';
import PropTypes from 'prop-types';
import ListSubheader from '@mui/material/ListSubheader';

import SidebarContext from './SideBarContext';

import { DRAWER_WIDTH } from './Constants';
import { getDrawerSxTransitionMixin } from './Mixins';

function SidebarHeaderItem({ children }) {
  const sidebarContext = React.useContext(SidebarContext);
  if (!sidebarContext) {
    throw new Error('Sidebar context was used without a provider.');
  }
  const {
    mini = false,
    fullyExpanded = true,
    hasDrawerTransitions,
  } = sidebarContext;

  return (
    <ListSubheader
      sx={{
        fontSize: 12,
        fontWeight: '600',
        height: mini ? 0 : 36,
        ...(hasDrawerTransitions
          ? getDrawerSxTransitionMixin(fullyExpanded, 'height')
          : {}),
        px: 1.5,
        py: 0,
        minWidth: DRAWER_WIDTH,
        overflow: 'hidden',
        textOverflow: 'ellipsis',
        whiteSpace: 'nowrap',
        zIndex: 2,
      }}
    >
      {children}
    </ListSubheader>
  );
}

SidebarHeaderItem.propTypes = {
  children: PropTypes.node,
};

export default SidebarHeaderItem;