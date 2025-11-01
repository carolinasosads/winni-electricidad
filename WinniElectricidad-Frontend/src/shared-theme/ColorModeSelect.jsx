import * as React from "react";
import IconButton from "@mui/material/IconButton";
import LightModeIcon from "@mui/icons-material/LightMode";
import DarkModeIcon from "@mui/icons-material/DarkMode";
import { useTheme } from "@mui/material/styles";
import { ColorModeContext } from "./AppTheme.jsx";

export default function ColorModeSelect({ sx }) {
  const theme = useTheme();
  const colorMode = React.useContext(ColorModeContext);

  return (
    <IconButton onClick={colorMode.toggleColorMode} sx={sx}>
      {theme.palette.mode === "dark" ? <LightModeIcon /> : <DarkModeIcon />}
    </IconButton>
  );
}
