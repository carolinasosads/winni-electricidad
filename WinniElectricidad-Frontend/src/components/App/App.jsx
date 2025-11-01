//import { useState } from 'react'
//import reactLogo from './assets/react.svg'
//import viteLogo from '/vite.svg'

/*import './App.css'*/
import React from "react";
import { Routes, Route, Navigate} from "react-router-dom";

import Registro from "../../pages/Public/Registro/Registro.jsx";
import Login from "../../pages/Public/Login/Login.jsx";


/*function App() {
 const [count, setCount] = useState(0)

 return (
    <>

      <div>
        <a href="https://vite.dev" target="_blank">
          <img src={viteLogo} className="logo" alt="Vite logo" />
        </a>
        <a href="https://react.dev" target="_blank">
          <img src={reactLogo} className="logo react" alt="React logo" />
        </a>
      </div>
      <h1>Vite + React</h1>
      <div className="card">
        <button onClick={() => setCount((count) => count + 1)}>
          count is {count}
        </button>
        <p>
          Edit <code>src/App.jsx</code> and save to test HMR
        </p>
      </div>
      <p className="read-the-docs">
        Click on the Vite and React logos to learn more
      </p>
    </>
  )
}
*/
/*
function App() {
  return (
    <Routes>
      <Route path="/" element={<Registro />} />

      <Route path="/login" element={<Login />} />
    </Routes>
  );
}
*/

export default function App() {
  return (
    <Routes>
      {/* si entra a la raíz, lo mando al registro */}
      <Route path="/" element={<Navigate to="/login" replace />} />

      {/* tus páginas */}
      <Route path="/registro" element={<Registro />} />
      <Route path="/login" element={<Login />} />

 
      {/* catch-all por si entra a algo que no existe */}
      <Route path="*" element={<Navigate to="/registro" replace />} />
    </Routes>
  )
}
/*
function App() {
  return <Login />;
}
*/
//export default App;
