import CssBaseline from '@mui/material/CssBaseline';
import DashboardLayout from '../../../components/MUI/DashboardLayout';
import NotificationsProvider from '../../../hooks/useNotifications/NotificationsProvider';
import DialogsProvider from '../../../hooks/useDialogs/DialogsProvider';
import AppTheme from '../../../shared-theme/AppTheme';
import { Outlet } from 'react-router-dom';

import {
  dataGridCustomizations, 
  datePickersCustomizations, 
  sidebarCustomizations,
  formInputCustomizations,
} from './theme/customizations';

const themeComponents = {
  ...dataGridCustomizations,
  ...datePickersCustomizations,
  ...sidebarCustomizations,
  ...formInputCustomizations,
};

export default function Dashboard(props) {
  return (
    <AppTheme {...props} themeComponents={themeComponents}>
      <CssBaseline enableColorScheme />
      <NotificationsProvider>
        <DialogsProvider>
          <DashboardLayout>
            <Outlet />
          </DashboardLayout>
        </DialogsProvider>
      </NotificationsProvider>
    </AppTheme>
  );
}
