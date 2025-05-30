import { Box, Container, CssBaseline } from "@mui/material";
import NavBar from "../components/NavBar";
import { Outlet } from "react-router";
import { initializeUserId } from "../lib/user";
import { Toaster } from 'react-hot-toast';

function App() {

  initializeUserId();

  return (

    <Box sx={{ bgcolor: '#eeeeee', minHeight: '100vh' }}>
      <CssBaseline />
      <>
        <Toaster
          position="bottom-right"
          toastOptions={{
            duration: 4000,
            style: {
              fontSize: '1rem',
            },
            success: {
              duration: 4000,
            },
            error: {
              duration: 8000,
            },
          }}
        />
        <NavBar />
        <Container maxWidth='xl' sx={{ mt: 3 }}>
          <Outlet />
        </Container>
      </>
    </Box>
  )
}

export default App
