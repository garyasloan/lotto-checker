import {
  Card,
  Box,
  CardHeader,
  Divider,
  CardContent,
  Typography,
  useTheme,
  useMediaQuery
} from "@mui/material";

type Props = {
  winningPicksForUser: WinningPicksForUser;
};

export default function WinningPicksForUser({ winningPicksForUser }: Props) {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down("sm"));
  return (
    <Card elevation={3} sx={{ borderRadius: 3 }}>
      <CardHeader
        title={
          <Typography
            variant="h5"
            sx={{ fontSize: { xs: "1.4rem", sm: "1.8rem" }, fontWeight: "bold" }}
          >
            Prize:{" "}
            <span style={{ color: "green" }}>
              ${winningPicksForUser.prizeAmount > 999
                ? winningPicksForUser.prizeAmount.toLocaleString()
                : winningPicksForUser.prizeAmount}
            </span>
          </Typography>
        }
      />

      <Divider />

      <CardContent sx={{ p: 2 }}>
        <Box
          display="flex"
          flexDirection={isMobile ? "column" : "row"}
          alignItems={isMobile ? "flex-start" : "center"}
          justifyContent="space-between"
          gap={1}
          mb={2}
        >
          <Typography variant="body2" sx={{ fontSize: "1rem" }}>
            {`Draw Number: ${winningPicksForUser.drawNumber}`}
          </Typography>

          <Typography variant="body2" sx={{ fontSize: "1rem" }}>
            {`Draw Date: ${winningPicksForUser.drawDate}`}
          </Typography>
        </Box>

        <Divider sx={{ mb: 2, mx: -2 }} />

        <Box
          display="flex"
          flexWrap="wrap"
          gap={1.5}
          sx={{ justifyContent: "center" }}
        >
          {[1, 2, 3, 4, 5].map((index) => {
            const keys = ["firstPick", "secondPick", "thirdPick", "fourthPick", "fifthPick"] as const;
            const pickKey = keys[index - 1];
            const pick = winningPicksForUser[pickKey];
            const matched = winningPicksForUser[`matchedNumber${index}` as keyof WinningPicksForUser] === 1;
            return (
              <span
                key={index}
                className={
                  matched
                    ? "grid-ball grid-ball-superlotto-secondary grid-ball--superlotto-secondary--matched grid-ball--superlotto-secondary--checked"
                    : "grid-ball grid-ball-superlotto-primary grid-ball--superlotto-primary--unmatched"
                }
              >
                {pick}
              </span>
            );
          })}

          <span
            className={
              winningPicksForUser.matchedNumberMega === 10
                ? "grid-ball grid-ball-superlotto-primary grid-ball--superlotto-primary--matched grid-ball--superlotto-primary--checked"
                : "grid-ball grid-ball-superlotto-primary grid-ball--superlotto-secondary--unmatched"
            }
          >
            {winningPicksForUser.megaPick}
          </span>
        </Box>
      </CardContent>
    </Card>
  );
}
