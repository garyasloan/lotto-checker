import { Box } from "@mui/material";
import { useLottoPicks } from "../../../hooks/useLottoPicks";
import WinningPickCard from "./WinningPickCard";

export default function WinningPicksForUser() {

  const { winningPicksForUserData, isLoadingWinningPicksForUser } = useLottoPicks();

  if (isLoadingWinningPicksForUser) {
    return
  }

  if (!winningPicksForUserData || winningPicksForUserData.length === 0) {
    return
  }

  return (
    <Box
      sx={{
        display: 'flex',
        flexWrap: 'wrap',
        gap: 2,
        justifyContent: 'flex-start',
      }}
    >
      {winningPicksForUserData.map((winningPickForUser) => {
        const key = `${winningPickForUser.drawDate}-${winningPickForUser.firstPick}-${winningPickForUser.secondPick}-${winningPickForUser.thirdPick}-${winningPickForUser.fourthPick}-${winningPickForUser.fifthPick}-${winningPickForUser.megaPick}`;

        return (
          <Box key={key} sx={{ width: 485 }}>
            <WinningPickCard winningPicksForUser={winningPickForUser} />
          </Box>
        );
      })}
    </Box>
  );
}
