import * as React from "react";
import SvgIcon from "@mui/material/SvgIcon";

export function SitemarkIcon({ compact = true }) {
  return (
    <SvgIcon
      sx={{
        width: compact ? 160 : 260,
        height: compact ? 32 : 64,
        color: "#0f2a44"
      }}
      viewBox="0 0 260 64"
    >
      {/* RAYO */}
      <defs>
        <linearGradient id="boltGradient" x1="0" y1="0" x2="1" y2="1">
          <stop offset="0%" stopColor="#4fc3f7" />
          <stop offset="100%" stopColor="#0288d1" />
        </linearGradient>
      </defs>

      <path
        d="M36 4 L12 36 H30 L22 60 L52 28 H34 Z"
        fill="url(#boltGradient)"
      />

      {/* TEXTO */}
      <text
        x="64"
        y="32"
        fill="currentColor"
        fontSize="26"
        fontWeight="900"
        letterSpacing="0.18em"
        fontFamily="Poppins, sans-serif"
      >
        WINNI
      </text>

      <text
        x="64"
        y="52"
        fill="currentColor"
        fontSize="12"
        letterSpacing="0.35em"
        fontFamily="Poppins, sans-serif"
        opacity="0.85"
      >
        ELECTRICIDAD
      </text>
    </SvgIcon>
  );
}