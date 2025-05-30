type WinningPicksForUser = {
drawNumber: number
  drawDate: string
  firstPick: number
  matchedNumber1: number
  secondPick: number
  matchedNumber2: number
  thirdPick: number
  matchedNumber3: number
  fourthPick: number
  matchedNumber4: number
  fifthPick: number
  matchedNumber5: number
  megaPick: number
  matchedNumberMega: number
  prizeAmount: number
}

type SuperLottoUserPick = {
  id: string,
  userId: string,
  firstPick: number,
  secondPick: number,
  thirdPick: number,
  fourthPick: number,
  fifthPick: number,
  megaPick: number,
}