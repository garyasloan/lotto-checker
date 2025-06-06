// src/router/Routes.tsx
import { createBrowserRouter, Navigate } from "react-router-dom"; // ← make sure to use react-router-dom here
import App from "../App";
import WinningPicks from "../../features/winningPicks/WinningPicks";
import About from "../../features/about/About";
import ErrorPage from "../../pages/ErrorPage"; 

export const router = createBrowserRouter([
  {
    path: '/',
    element: <App />,
    errorElement: <ErrorPage />, 
    children: [
      { index: true, element: <Navigate to="super-lotto" replace /> },
      {
        path: 'super-lotto',
        element: <WinningPicks />,
        errorElement: <ErrorPage /> 
      },
      {
        path: 'about',
        element: <About />,
        errorElement: <ErrorPage />
      }
    ]
  }
]);
