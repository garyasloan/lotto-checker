import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import '../styles/styles.css'
import { RouterProvider } from 'react-router';
import { router } from './router/Routes.tsx';

createRoot(document.getElementById('root')!).render(
  <StrictMode>
        <RouterProvider router={router} />
  </StrictMode>,
)
