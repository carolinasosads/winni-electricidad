import * as React from 'react';
import Divider from '@mui/material/Divider';

import SidebarContext from './SideBarContext';
import { getDrawerSxTransitionMixin } from './Mixins';

export default function SidebarDividerItem() {
  const sidebarContext = React.useContext(SidebarContext);
  if (!sidebarContext) {
    throw new Error('Sidebar context was used without a provider.');
  }
  const { fullyExpanded = true, hasDrawerTransitions } = sidebarContext;

  return (
    <li>
      <Divider
        sx={{
          borderBottomWidth: 1,
          my: 1,
          mx: -0.5,
          ...(hasDrawerTransitions
            ? getDrawerSxTransitionMixin(fullyExpanded, 'margin')
            : {}),
        }}
      />
    </li>
  );
}