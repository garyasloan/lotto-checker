import { createBrowserRouter, Navigate } from "react-router";
import App from "../App";
import WinningPicks from "../../features/winningPicks/WinningPicks";
import About from "../../features/about/About";

export const router = createBrowserRouter([
    {
        path: '/',
        element: <App />,
        children: [
            { path: '', element: <Navigate to="super-lotto" replace /> },
             { path: 'super-lotto', element: <WinningPicks /> },
             { path: 'about', element: <About/> },
        ]
    }
])