using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

public class SuperLottoWinningNumber
{
    [Key]
    public long DrawNumber { get; set; }
    public required string DrawDate { get; set; }
    public int FirstPick { get; set; }
    public int MatchedNumber1 { get; set; }
    public int SecondPick { get; set; }
    public int MatchedNumber2 { get; set; }
    public int ThirdPick { get; set; }
    public int MatchedNumber3 { get; set; }
    public int FourthPick { get; set; }
    public int MatchedNumber4 { get; set; }
    public int FifthPick { get; set; }
    public int MatchedNumber5 { get; set; }
    public int MegaPick { get; set; }
    public int MatchedNumberMega { get; set; }
    public long PrizeAmount { get; set; }
}

public class SuperLottoWinningNumberDTO
{
    [Key]
    public long DrawNumber { get; set; }
    public required string DrawDate { get; set; }
    public int FirstPick { get; set; }
    public int MatchedNumber1 { get; set; }
    public int SecondPick { get; set; }
    public int MatchedNumber2 { get; set; }
    public int ThirdPick { get; set; }
    public int MatchedNumber3 { get; set; }
    public int FourthPick { get; set; }
    public int MatchedNumber4 { get; set; }
    public int FifthPick { get; set; }
    public int MatchedNumber5 { get; set; }
    public int MegaPick { get; set; }
    public int MatchedNumberMega { get; set; }
    public long PrizeAmount { get; set; }
}


public class NumberOccurrenceDTO
{
    public int Label { get; set; }
    public int Count { get; set; }
}