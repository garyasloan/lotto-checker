// src/pages/ErrorPage.tsx
import { isRouteErrorResponse, useRouteError } from 'react-router-dom';
import { Typography, Container } from '@mui/material';

export default function ErrorPage() {
  const error = useRouteError();

  return (
    <Container sx={{ mt: 8 }}>
      <Typography variant="h3" gutterBottom>
        Something went wrong. 
      </Typography>

      <Typography variant="body1" color="text.secondary">
        {isRouteErrorResponse(error)
          ? `Status: ${error.status} — ${error.statusText}`
          : (error as Error)?.message || 'Unknown error'}
      </Typography>
    </Container>
  );
}
