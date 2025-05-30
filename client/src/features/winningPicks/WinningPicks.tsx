import { Box } from "@mui/material";
import UserPickEntryGrid from "./components/UserPickEntryGrid";
import WinningPicksForUser from "./components/WinningPicksForUser";

export default function WinningPicks() {
  return (
    <>
      <UserPickEntryGrid></UserPickEntryGrid>
      <Box sx={{ mt: 3 }}>
        <WinningPicksForUser />
      </Box>
    </>
  )
}
