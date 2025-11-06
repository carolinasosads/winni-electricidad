import * as React from 'react';
import SvgIcon from '@mui/material/SvgIcon';

export function SitemarkIcon() { 
  return (
    <SvgIcon sx={{ height: 30, width: 230 }}>
      <svg
        width={220}
        height={30}
        viewBox="0 0 220 30"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >

        <text
          x="0"
          y="20"
          fill="#4876EE"
          fontSize="20"
          fontWeight="bold"
          fontFamily="Poppins, sans-serif"
        >
          Winni Electricidad
        </text>
      </svg>
    </SvgIcon>
  );
}

