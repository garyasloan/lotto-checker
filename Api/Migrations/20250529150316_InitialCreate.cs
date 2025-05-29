using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "LottoChecker");

            migrationBuilder.CreateTable(
                name: "SuperLottoPrizes",
                schema: "LottoChecker",
                columns: table => new
                {
                    DrawNumber = table.Column<int>(type: "int", nullable: false),
                    //.Annotation("SqlServer:Identity", "1, 1"),
                    FiveMatchesPlusMegaPrizeAmount = table.Column<long>(type: "bigint", nullable: false),
                    FiveMatchesPrizeAmount = table.Column<long>(type: "bigint", nullable: false),
                    FourMatchesPlusMegaPrizeAmount = table.Column<long>(type: "bigint", nullable: false),
                    FourMatchesPrizeAmount = table.Column<long>(type: "bigint", nullable: false),
                    ThreeMatchesPlusMegaPrizeAmount = table.Column<long>(type: "bigint", nullable: false),
                    ThreeMatchesPrizeAmount = table.Column<long>(type: "bigint", nullable: false),
                    TwoMatchesPlusMegaPrizeAmount = table.Column<long>(type: "bigint", nullable: false),
                    OneMatchPlusMegaPrizeAmount = table.Column<long>(type: "bigint", nullable: false),
                    MegaMatchOnlyPrizeAmount = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuperLottoPrizes", x => x.DrawNumber);
                });

            migrationBuilder.CreateTable(
                name: "SuperLottoUserPicks",
                schema: "LottoChecker",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    FirstPick = table.Column<int>(type: "int", nullable: false),
                    SecondPick = table.Column<int>(type: "int", nullable: false),
                    ThirdPick = table.Column<int>(type: "int", nullable: false),
                    FourthPick = table.Column<int>(type: "int", nullable: false),
                    FifthPick = table.Column<int>(type: "int", nullable: false),
                    MegaPick = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuperLottoUserPicks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SuperLottoWinningNumbers",
                schema: "LottoChecker",
                columns: table => new
                {
                    DrawNumber = table.Column<int>(type: "int", nullable: false),
                    // .Annotation("SqlServer:Identity", "1, 1"),
                    DrawDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Number1 = table.Column<int>(type: "int", nullable: false),
                    Number2 = table.Column<int>(type: "int", nullable: false),
                    Number3 = table.Column<int>(type: "int", nullable: false),
                    Number4 = table.Column<int>(type: "int", nullable: false),
                    Number5 = table.Column<int>(type: "int", nullable: false),
                    MegaNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuperLottoWinningNumbers", x => x.DrawNumber);
                });

            // Create SP
            migrationBuilder.Sql(@"
                    Create Procedure [LottoChecker].[GetWinningSuperLottoDrawsForUser]
                        (
                          @UserId UniqueIdentifier
                        )
                    As
                        Begin 

                            Declare @AnalyzeNumbers Table
                                (
                                  PickId UniqueIdentifier,
                                  DrawNumber BigInt,
                                  DrawDate Varchar(10),
                                  FirstPick Int,
                                  MatchedNumber1 Int,
                                  SecondPick Int,
                                  MatchedNumber2 Int,
                                  ThirdPick Int,
                                  MatchedNumber3 Int,
                                  FourthPick Int,
                                  MatchedNumber4 Int,
                                  FifthPick Int,
                                  MatchedNumber5 Int,
                                  MegaPick Int,
                                  MatchedNumberMega Int,
                                  MatchScore Int,
                                  PrizeAmount BigInt
                                );

                            Insert  @AnalyzeNumbers
                                    ( PickId,
                                      DrawNumber,
                                      DrawDate,
                                      FirstPick,
                                      MatchedNumber1,
                                      SecondPick,
                                      MatchedNumber2,
                                      ThirdPick,
                                      MatchedNumber3,
                                      FourthPick,
                                      MatchedNumber4,
                                      FifthPick,
                                      MatchedNumber5,
                                      MegaPick,
                                      MatchedNumberMega
                                    )
                                    Select  mmp.Id,
                                            slwn.DrawNumber,
                                            DrawDate,
                                            FirstPick,
                                            Case When ( FirstPick = Number1
                                                        Or FirstPick = Number2
                                                        Or FirstPick = Number3
                                                        Or FirstPick = Number4
                                                        Or FirstPick = Number5
                                                      ) Then 1
                                                 Else 0
                                            End,
                                            SecondPick,
                                            Case When ( SecondPick = Number1
                                                        Or SecondPick = Number2
                                                        Or SecondPick = Number3
                                                        Or SecondPick = Number4
                                                        Or SecondPick = Number5
                                                      ) Then 1
                                                 Else 0
                                            End,
                                            ThirdPick,
                                            Case When ( ThirdPick = Number1
                                                        Or ThirdPick = Number2
                                                        Or ThirdPick = Number3
                                                        Or ThirdPick = Number4
                                                        Or ThirdPick = Number5
                                                      ) Then 1
                                                 Else 0
                                            End,
                                            FourthPick,
                                            Case When ( FourthPick = Number1
                                                        Or FourthPick = Number2
                                                        Or FourthPick = Number3
                                                        Or FourthPick = Number4
                                                        Or FourthPick = Number5
                                                      ) Then 1
                                                 Else 0
                                            End,
                                            FifthPick,
                                            Case When ( FifthPick = Number1
                                                        Or FifthPick = Number2
                                                        Or FifthPick = Number3
                                                        Or FifthPick = Number4
                                                        Or FifthPick = Number5
                                                      ) Then 1
                                                 Else 0
                                            End,
                                            MegaPick,
                                            Case When MegaPick = MegaNumber Then 10
                                                 Else 0
                                            End
                                    From    LottoChecker.SuperLottoWinningNumbers slwn
											join LottoChecker.SuperLottoPrizes slp
												on slwn.DrawNumber = slp.DrawNumber
                                            Cross Join LottoChecker.SuperLottoUserPicks mmp
                                    Where   UserId = @UserId;

                            Update  @AnalyzeNumbers
                            Set     MatchScore = MatchedNumber1 + MatchedNumber2 + MatchedNumber3 + MatchedNumber4 + MatchedNumber5 + MatchedNumberMega;

                            Delete  @AnalyzeNumbers
                            Where   MatchScore < 3;

                            Update  @AnalyzeNumbers
                            Set     PrizeAmount = Case MatchScore
                                                    When 10 Then MegaMatchOnlyPrizeAmount
                                                    When 11 Then OneMatchPlusMegaPrizeAmount
                                                    When 12 Then TwoMatchesPlusMegaPrizeAmount
                                                    When 3 Then ThreeMatchesPrizeAmount
                                                    When 13 Then ThreeMatchesPlusMegaPrizeAmount
                                                    When 4 Then FourMatchesPrizeAmount
                                                    When 14 Then FourMatchesPlusMegaPrizeAmount
                                                    When 5 Then FiveMatchesPrizeAmount
                                                    When 15 Then FiveMatchesPlusMegaPrizeAmount
                                                  End
                            From    @AnalyzeNumbers an
                                    Join LottoChecker.SuperLottoWinningNumbers slwn
                                        On an.DrawNumber = slwn.DrawNumber
									Join LottoChecker.SuperLottoPrizes slp
										On slwn.DrawNumber = slp.DrawNumber;

	
                            Select  DrawNumber,
                                    Convert(Varchar(10),Cast(DrawDate As Date),101) DrawDate,
                                    FirstPick,
                                    MatchedNumber1,
                                    SecondPick,
                                    MatchedNumber2,
                                    ThirdPick,
                                    MatchedNumber3,
                                    FourthPick,
                                    MatchedNumber4,
                                    FifthPick,
                                    MatchedNumber5,
                                    MegaPick,
                                    MatchedNumberMega,
                                    PrizeAmount
                            From    @AnalyzeNumbers
                            Order By DrawNumber Desc;
                        End;

            ");

            // Create UDF
            migrationBuilder.Sql(@"
	Create Function [LottoChecker].[SuperLottoUserPicksRowCheckSum]
                    (
                      @UserId UniqueIdentifier,
                      @FirstPick Int,
                      @SecondPick Int,
                      @ThirdPick Int,
                      @FourthPick Int,
                      @FifthPick Int,
                      @MegaPick Int

                    )
                Returns NVarchar(140)
                    With SchemaBinding
                As
                    Begin
                        Declare @SortTable Table ( Pick Varchar(2) );
                        Insert  @SortTable
                        Values  ( Right('0' + Convert(Varchar(2),@FirstPick),2) );
                        Insert  @SortTable
                        Values  ( Right('0' + Convert(Varchar(2),@SecondPick),2) );
                        Insert  @SortTable
                        Values  ( Right('0' + Convert(Varchar(2),@ThirdPick),2) );
                        Insert  @SortTable
                        Values  ( Right('0' + Convert(Varchar(2),@FourthPick),2) );
                        Insert  @SortTable
                        Values  ( Right('0' + Convert(Varchar(2),@FifthPick),2) );
		

                        Declare @CheckSum Varchar(12);
                        Set @CheckSum = '';
                        Select  @CheckSum = @CheckSum + Pick
                        From    @SortTable
                        Order By Pick;
                        Set @CheckSum = @CheckSum + Right('0' + Convert(Varchar(2),@MegaPick),2);
                        Return @CheckSum + Convert(NVarchar(128), @UserId); 
                    End;    ");


            // Add user picks computed column after creating UDF
            migrationBuilder.Sql(@"
    ALTER TABLE LottoChecker.SuperLottoUserPicks
    ADD RowCheckSum AS LottoChecker.SuperLottoUserPicksRowCheckSum(
        UserId, FirstPick, SecondPick, ThirdPick, FourthPick, FifthPick, MegaPick
    );
    ");

            // Add constraints
            migrationBuilder.Sql(@"


ALTER TABLE [LottoChecker].[SuperLottoUserPicks] ADD DateAdded DATETIME NOT NULL CONSTRAINT DF_LottoChecker_SuperLottoUserPicks_DateAdded DEFAULT GETDATE();

ALTER TABLE [LottoChecker].[SuperLottoUserPicks]  WITH CHECK ADD  CONSTRAINT [CK_LottoChecker_SuperLottoUserPicks_FifthPick_Range] CHECK  (([FifthPick]>=(1) AND [FifthPick]<=(47)))

ALTER TABLE [LottoChecker].[SuperLottoUserPicks] CHECK CONSTRAINT [CK_LottoChecker_SuperLottoUserPicks_FifthPick_Range]

ALTER TABLE [LottoChecker].[SuperLottoUserPicks]  WITH CHECK ADD  CONSTRAINT [CK_LottoChecker_SuperLottoUserPicks_FirstPick_Range] CHECK  (([FirstPick]>=(1) AND [FirstPick]<=(47)))

ALTER TABLE [LottoChecker].[SuperLottoUserPicks] CHECK CONSTRAINT [CK_LottoChecker_SuperLottoUserPicks_FirstPick_Range]

ALTER TABLE [LottoChecker].[SuperLottoUserPicks]  WITH CHECK ADD  CONSTRAINT [CK_LottoChecker_SuperLottoUserPicks_FirstPick_Unique] CHECK  (([FirstPick]<>[SecondPick] AND [FirstPick]<>[ThirdPick] AND [FirstPick]<>[FourthPick] AND [FirstPick]<>[FifthPick]))

ALTER TABLE [LottoChecker].[SuperLottoUserPicks] CHECK CONSTRAINT [CK_LottoChecker_SuperLottoUserPicks_FirstPick_Unique]

ALTER TABLE [LottoChecker].[SuperLottoUserPicks]  WITH CHECK ADD  CONSTRAINT [CK_LottoChecker_SuperLottoUserPicks_FourthdPick_Unique] CHECK  (([FourthPick]<>[FifthPick]))

ALTER TABLE [LottoChecker].[SuperLottoUserPicks] CHECK CONSTRAINT [CK_LottoChecker_SuperLottoUserPicks_FourthdPick_Unique]

ALTER TABLE [LottoChecker].[SuperLottoUserPicks]  WITH CHECK ADD  CONSTRAINT [CK_LottoChecker_SuperLottoUserPicks_FourthPick_Range] CHECK  (([FourthPick]>=(1) AND [FourthPick]<=(47)))

ALTER TABLE [LottoChecker].[SuperLottoUserPicks] CHECK CONSTRAINT [CK_LottoChecker_SuperLottoUserPicks_FourthPick_Range]

ALTER TABLE [LottoChecker].[SuperLottoUserPicks]  WITH CHECK ADD  CONSTRAINT [CK_LottoChecker_SuperLottoUserPicks_MegaPick_Range] CHECK  (([FifthPick]>=(1) AND [MegaPick]<=(27)))

ALTER TABLE [LottoChecker].[SuperLottoUserPicks] CHECK CONSTRAINT [CK_LottoChecker_SuperLottoUserPicks_MegaPick_Range]

ALTER TABLE [LottoChecker].[SuperLottoUserPicks]  WITH CHECK ADD  CONSTRAINT [CK_LottoChecker_SuperLottoUserPicks_SecondPick_Range] CHECK  (([SecondPick]>=(1) AND [SecondPick]<=(47)))

ALTER TABLE [LottoChecker].[SuperLottoUserPicks] CHECK CONSTRAINT [CK_LottoChecker_SuperLottoUserPicks_SecondPick_Range]

ALTER TABLE [LottoChecker].[SuperLottoUserPicks]  WITH CHECK ADD  CONSTRAINT [CK_LottoChecker_SuperLottoUserPicks_SecondPick_Unique] CHECK  (([SecondPick]<>[ThirdPick] AND [SecondPick]<>[FourthPick] AND [SecondPick]<>[FifthPick]))

ALTER TABLE [LottoChecker].[SuperLottoUserPicks] CHECK CONSTRAINT [CK_LottoChecker_SuperLottoUserPicks_SecondPick_Unique]

ALTER TABLE [LottoChecker].[SuperLottoUserPicks]  WITH CHECK ADD  CONSTRAINT [CK_LottoChecker_SuperLottoUserPicks_ThirdPick_Range] CHECK  (([ThirdPick]>=(1) AND [ThirdPick]<=(47)))

ALTER TABLE [LottoChecker].[SuperLottoUserPicks] CHECK CONSTRAINT [CK_LottoChecker_SuperLottoUserPicks_ThirdPick_Range]

ALTER TABLE [LottoChecker].[SuperLottoUserPicks]  WITH CHECK ADD  CONSTRAINT [CK_LottoChecker_SuperLottoUserPicks_ThirdPick_Unique] CHECK  (([ThirdPick]<>[FourthPick] AND [ThirdPick]<>[FifthPick]))

ALTER TABLE [LottoChecker].[SuperLottoUserPicks] CHECK CONSTRAINT [CK_LottoChecker_SuperLottoUserPicks_ThirdPick_Unique]


-----

ALTER TABLE [LottoChecker].[SuperLottoUserPicks] ADD  CONSTRAINT [UC_LottoChecker_SuperLottoUserPicks_RowCheckSum] UNIQUE NONCLUSTERED 
(
	[RowCheckSum] ASC
)


-------

ALTER TABLE [LottoChecker].[SuperLottoUserPicks] ADD  CONSTRAINT [DF_LottoChecker_SuperLottoUserPicks_Id]  DEFAULT (newsequentialid()) FOR [Id]


----
ALTER TABLE [LottoChecker].[SuperLottoPrizes] ADD CONSTRAINT FK_LottoChecker_SuperLottoWinningNumbers_SuperLottoPrizes_SuperLottoWinningNumbers FOREIGN KEY ([DrawNumber]) REFERENCES [LottoChecker].[SuperLottoWinningNumbers]([DrawNumber]);

    ");


            migrationBuilder.InsertData(
                schema: "LottoChecker",
                table: "SuperLottoWinningNumbers",
                columns: new[] { "DrawNumber", "DrawDate", "Number1", "Number2", "Number3", "Number4", "Number5", "MegaNumber" },
                values: new object[,]
                {
                    { 3873, new DateTime(2024, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 19, 30, 39, 41, 42, 2 },
                    { 3874, new DateTime(2024, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 25, 27, 32, 35, 37, 22 },
                    { 3875, new DateTime(2024, 5, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 6, 13, 23, 32, 10 },
                    { 3876, new DateTime(2024, 5, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 8, 18, 19, 33, 3 },
                    { 3877, new DateTime(2024, 5, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 19, 21, 25, 37, 26 },
                    { 3878, new DateTime(2024, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 10, 12, 17, 21, 16 },
                    { 3879, new DateTime(2024, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 18, 21, 27, 34, 4 },
                    { 3880, new DateTime(2024, 6, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 7, 27, 30, 40, 6 },
                    { 3881, new DateTime(2024, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 18, 25, 30, 40, 44, 23 },
                    { 3882, new DateTime(2024, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 8, 12, 24, 31, 13 },
                    { 3883, new DateTime(2024, 6, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), 22, 23, 32, 38, 41, 25 },
                    { 3884, new DateTime(2024, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 36, 37, 38, 43, 8 },
                    { 3885, new DateTime(2024, 6, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 16, 23, 35, 46, 26 },
                    { 3886, new DateTime(2024, 6, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 10, 12, 21, 25, 16 },
                    { 3887, new DateTime(2024, 7, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 16, 22, 25, 32, 39, 26 },
                    { 3888, new DateTime(2024, 7, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 11, 17, 27, 31, 21 },
                    { 3889, new DateTime(2024, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 9, 13, 17, 30, 25 },
                    { 3890, new DateTime(2024, 7, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 18, 25, 27, 31, 5 },
                    { 3891, new DateTime(2024, 7, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 14, 23, 24, 29, 4 },
                    { 3892, new DateTime(2024, 7, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 11, 13, 14, 15, 46, 5 },
                    { 3893, new DateTime(2024, 7, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 11, 25, 28, 47, 11 },
                    { 3894, new DateTime(2024, 7, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, 22, 24, 26, 27, 22 },
                    { 3895, new DateTime(2024, 7, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 12, 13, 43, 47, 15 },
                    { 3896, new DateTime(2024, 8, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 4, 28, 40, 44, 19 },
                    { 3897, new DateTime(2024, 8, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 19, 20, 29, 34, 10 },
                    { 3898, new DateTime(2024, 8, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 9, 21, 32, 34, 42, 15 },
                    { 3899, new DateTime(2024, 8, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 6, 17, 42, 45, 20 },
                    { 3900, new DateTime(2024, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 12, 15, 21, 32, 27 },
                    { 3901, new DateTime(2024, 8, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, 22, 27, 42, 43, 8 },
                    { 3902, new DateTime(2024, 8, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), 15, 20, 23, 24, 46, 9 },
                    { 3903, new DateTime(2024, 8, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 15, 27, 28, 31, 47, 20 },
                    { 3904, new DateTime(2024, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 32, 37, 40, 42, 43, 5 },
                    { 3905, new DateTime(2024, 9, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 10, 30, 32, 39, 42, 15 },
                    { 3906, new DateTime(2024, 9, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 29, 30, 35, 37, 18 },
                    { 3907, new DateTime(2024, 9, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 7, 9, 25, 37, 6 },
                    { 3908, new DateTime(2024, 9, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 13, 28, 30, 47, 2 },
                    { 3909, new DateTime(2024, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 14, 19, 29, 30, 47, 24 },
                    { 3910, new DateTime(2024, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 8, 26, 27, 35, 10 },
                    { 3911, new DateTime(2024, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 9, 16, 25, 46, 11 },
                    { 3912, new DateTime(2024, 9, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 7, 8, 16, 32, 15 },
                    { 3913, new DateTime(2024, 10, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 6, 10, 13, 16, 17 },
                    { 3914, new DateTime(2024, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 8, 17, 33, 47, 3 },
                    { 3915, new DateTime(2024, 10, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 28, 39, 41, 42, 4 },
                    { 3916, new DateTime(2024, 10, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 15, 17, 18, 19, 8 },
                    { 3917, new DateTime(2024, 10, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), 18, 25, 31, 38, 39, 14 },
                    { 3918, new DateTime(2024, 10, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), 12, 29, 31, 37, 46, 24 },
                    { 3919, new DateTime(2024, 10, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 13, 19, 38, 46, 17 },
                    { 3920, new DateTime(2024, 10, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 8, 10, 36, 46, 3 },
                    { 3921, new DateTime(2024, 10, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 13, 23, 36, 41, 13 },
                    { 3922, new DateTime(2024, 11, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 11, 13, 24, 39, 19 },
                    { 3923, new DateTime(2024, 11, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 19, 30, 32, 41, 44, 2 },
                    { 3924, new DateTime(2024, 11, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 14, 15, 33, 38, 39, 11 },
                    { 3925, new DateTime(2024, 11, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 18, 36, 40, 44, 20 },
                    { 3926, new DateTime(2024, 11, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 11, 15, 25, 43, 16 },
                    { 3927, new DateTime(2024, 11, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 24, 26, 30, 41, 10 },
                    { 3928, new DateTime(2024, 11, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 7, 16, 39, 40, 1 },
                    { 3929, new DateTime(2024, 11, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 5, 15, 16, 42, 24 },
                    { 3930, new DateTime(2024, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 10, 13, 23, 29, 37, 17 },
                    { 3931, new DateTime(2024, 12, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 21, 26, 33, 42, 7 },
                    { 3932, new DateTime(2024, 12, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 7, 17, 19, 44, 4 },
                    { 3933, new DateTime(2024, 12, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 11, 19, 24, 38, 41, 26 },
                    { 3934, new DateTime(2024, 12, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 9, 10, 17, 33, 45, 8 },
                    { 3935, new DateTime(2024, 12, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 5, 17, 19, 47, 6 },
                    { 3936, new DateTime(2024, 12, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 6, 27, 33, 37, 2 },
                    { 3937, new DateTime(2024, 12, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 20, 23, 24, 36, 9 },
                    { 3938, new DateTime(2024, 12, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 14, 22, 28, 35, 24 },
                    { 3939, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 11, 12, 16, 25, 35, 20 },
                    { 3940, new DateTime(2025, 1, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 18, 37, 42, 43, 15 },
                    { 3941, new DateTime(2025, 1, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 13, 33, 35, 36, 38, 12 },
                    { 3942, new DateTime(2025, 1, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 26, 31, 40, 41, 11 },
                    { 3943, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 14, 16, 21, 28, 10 },
                    { 3944, new DateTime(2025, 1, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 7, 8, 28, 33, 7 },
                    { 3945, new DateTime(2025, 1, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 3, 12, 16, 30, 26 },
                    { 3946, new DateTime(2025, 1, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 13, 20, 25, 32, 35, 21 },
                    { 3947, new DateTime(2025, 1, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 21, 24, 33, 37, 7 },
                    { 3948, new DateTime(2025, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 12, 16, 22, 43, 2 },
                    { 3949, new DateTime(2025, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 15, 20, 30, 36, 46, 3 },
                    { 3950, new DateTime(2025, 2, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 6, 27, 41, 47, 25 },
                    { 3951, new DateTime(2025, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 13, 20, 22, 27, 35, 17 },
                    { 3952, new DateTime(2025, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 9, 26, 29, 47, 20 },
                    { 3953, new DateTime(2025, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 4, 12, 15, 28, 15 },
                    { 3954, new DateTime(2025, 2, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 22, 33, 42, 44, 5 },
                    { 3955, new DateTime(2025, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 8, 13, 28, 33, 20 },
                    { 3956, new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 18, 26, 32, 44, 9 },
                    { 3957, new DateTime(2025, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 13, 17, 23, 35, 24 },
                    { 3958, new DateTime(2025, 3, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 9, 33, 45, 46, 22 },
                    { 3959, new DateTime(2025, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 15, 21, 29, 37, 45, 3 },
                    { 3960, new DateTime(2025, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 11, 13, 18, 21, 44, 27 },
                    { 3961, new DateTime(2025, 3, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), 15, 22, 24, 35, 38, 22 },
                    { 3962, new DateTime(2025, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 9, 24, 27, 33, 40, 19 },
                    { 3963, new DateTime(2025, 3, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 13, 26, 30, 33, 26 },
                    { 3964, new DateTime(2025, 3, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 3, 8, 14, 19, 9 },
                    { 3965, new DateTime(2025, 4, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 11, 14, 27, 37, 22 },
                    { 3966, new DateTime(2025, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 4, 14, 21, 46, 10 },
                    { 3967, new DateTime(2025, 4, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 21, 26, 37, 44, 13 },
                    { 3968, new DateTime(2025, 4, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 8, 10, 32, 34, 5 },
                    { 3969, new DateTime(2025, 4, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), 12, 19, 23, 33, 41, 4 },
                    { 3970, new DateTime(2025, 4, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 24, 41, 45, 46, 27 },
                    { 3971, new DateTime(2025, 4, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), 19, 27, 29, 40, 45, 9 },
                    { 3972, new DateTime(2025, 4, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 31, 33, 35, 42, 6 },
                    { 3973, new DateTime(2025, 4, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 7, 11, 37, 16 },
                    { 3974, new DateTime(2025, 5, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 19, 30, 39, 46, 22 },
                    { 3975, new DateTime(2025, 5, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 23, 38, 40, 41, 1 },
                    { 3976, new DateTime(2025, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 10, 11, 21, 32, 5 },
                    { 3977, new DateTime(2025, 5, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 19, 39, 42, 46, 8 },
                    { 3978, new DateTime(2025, 5, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 11, 17, 18, 40, 44, 24 },
                    { 3979, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), 20, 22, 28, 29, 45, 18 },
                    { 3980, new DateTime(2025, 5, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 4, 5, 7, 35, 17 }
                });

            migrationBuilder.InsertData(
            schema: "LottoChecker",
            table: "SuperLottoPrizes",
            columns: new[] { "DrawNumber", "FiveMatchesPlusMegaPrizeAmount", "FiveMatchesPrizeAmount", "FourMatchesPlusMegaPrizeAmount", "FourMatchesPrizeAmount", "ThreeMatchesPlusMegaPrizeAmount", "ThreeMatchesPrizeAmount", "TwoMatchesPlusMegaPrizeAmount", "OneMatchPlusMegaPrizeAmount", "MegaMatchOnlyPrizeAmount" },
            values: new object[,]
            {
                    { 3873, 35000000L, 38758L, 2153L, 121L, 66L, 11L, 12L, 2L, 1L },
                    { 3874, 36000000L, 13020L, 3255L, 101L, 57L, 11L, 11L, 2L, 1L },
                    { 3875, 37000000L, 38544L, 1070L, 92L, 53L, 9L, 9L, 2L, 1L },
                    { 3876, 38000000L, 39013L, 1083L, 95L, 47L, 9L, 9L, 2L, 1L },
                    { 3877, 39000000L, 18667L, 2074L, 97L, 59L, 10L, 11L, 2L, 1L },
                    { 3878, 40000000L, 20127L, 1183L, 71L, 54L, 8L, 11L, 2L, 1L },
                    { 3879, 41000000L, 9868L, 986L, 84L, 50L, 9L, 10L, 2L, 1L },
                    { 3880, 42000000L, 13584L, 1455L, 96L, 59L, 9L, 11L, 2L, 1L },
                    { 3881, 43000000L, 21387L, 2138L, 105L, 60L, 11L, 11L, 2L, 1L },
                    { 3882, 44000000L, 21979L, 845L, 72L, 42L, 8L, 9L, 1L, 1L },
                    { 3883, 45000000L, 43253L, 1201L, 112L, 59L, 10L, 11L, 2L, 1L },
                    { 3884, 46000000L, 14128L, 1766L, 96L, 55L, 11L, 11L, 2L, 1L },
                    { 3885, 47000000L, 13909L, 1227L, 91L, 61L, 10L, 11L, 2L, 1L },
                    { 3886, 48000000L, 10600L, 1766L, 68L, 48L, 8L, 11L, 2L, 1L },
                    { 3887, 49000000L, 14588L, 2188L, 116L, 59L, 10L, 12L, 2L, 1L },
                    { 3888, 50000000L, 41887L, 1102L, 81L, 52L, 8L, 10L, 2L, 1L },
                    { 3889, 51000000L, 43294L, 1202L, 77L, 47L, 9L, 10L, 2L, 1L },
                    { 3890, 52000000L, 21750L, 1087L, 91L, 52L, 10L, 10L, 2L, 1L },
                    { 3891, 53000000L, 43379L, 1445L, 92L, 53L, 9L, 10L, 2L, 1L },
                    { 3892, 54000000L, 43311L, 2406L, 91L, 58L, 10L, 10L, 2L, 1L },
                    { 3893, 7000000L, 27187L, 1359L, 80L, 43L, 9L, 9L, 1L, 1L },
                    { 3894, 8000000L, 13956L, 1744L, 90L, 56L, 9L, 10L, 2L, 1L },
                    { 3895, 9000000L, 28324L, 2832L, 107L, 60L, 10L, 11L, 2L, 1L },
                    { 3896, 10000000L, 30469L, 1171L, 90L, 51L, 10L, 11L, 2L, 1L },
                    { 3897, 11000000L, 30180L, 1257L, 101L, 50L, 10L, 10L, 2L, 1L },
                    { 3898, 12000000L, 31557L, 1577L, 88L, 59L, 10L, 11L, 2L, 1L },
                    { 3899, 13000000L, 30711L, 1395L, 90L, 59L, 10L, 11L, 2L, 1L },
                    { 3900, 14000000L, 31224L, 1301L, 81L, 57L, 9L, 10L, 2L, 1L },
                    { 3901, 15000000L, 7806L, 1419L, 102L, 52L, 10L, 10L, 2L, 1L },
                    { 3902, 16000000L, 32047L, 1456L, 101L, 54L, 10L, 10L, 2L, 1L },
                    { 3903, 17000000L, 32028L, 1779L, 99L, 53L, 10L, 11L, 2L, 1L },
                    { 3904, 18000000L, 33826L, 2114L, 114L, 54L, 11L, 11L, 2L, 1L },
                    { 3905, 19000000L, 33077L, 3307L, 106L, 57L, 11L, 11L, 2L, 1L },
                    { 3906, 20000000L, 8664L, 1332L, 119L, 62L, 11L, 11L, 2L, 1L },
                    { 3907, 21000000L, 35308L, 1176L, 86L, 55L, 9L, 10L, 2L, 1L },
                    { 3908, 22000000L, 17747L, 887L, 86L, 54L, 9L, 11L, 2L, 1L },
                    { 3909, 23000000L, 11640L, 1247L, 114L, 60L, 10L, 11L, 2L, 1L },
                    { 3910, 24000000L, 35338L, 1104L, 90L, 54L, 9L, 10L, 2L, 1L },
                    { 3911, 25000000L, 35336L, 1766L, 94L, 51L, 9L, 10L, 2L, 1L },
                    { 3912, 26000000L, 35206L, 1466L, 77L, 51L, 8L, 10L, 2L, 1L },
                    { 3913, 27000000L, 17585L, 1953L, 83L, 49L, 8L, 10L, 2L, 1L },
                    { 3914, 28000000L, 17853L, 1623L, 91L, 53L, 9L, 9L, 2L, 1L },
                    { 3915, 29000000L, 35476L, 1773L, 91L, 58L, 11L, 11L, 2L, 1L },
                    { 3916, 30000000L, 9008L, 1201L, 83L, 48L, 9L, 9L, 1L, 1L },
                    { 3917, 31000000L, 35512L, 2536L, 125L, 53L, 11L, 11L, 2L, 1L },
                    { 3918, 32000000L, 35689L, 4461L, 103L, 58L, 11L, 11L, 2L, 1L },
                    { 3919, 33000000L, 36444L, 2024L, 96L, 58L, 10L, 11L, 2L, 1L },
                    { 3920, 34000000L, 36089L, 2255L, 93L, 51L, 9L, 10L, 2L, 1L },
                    { 3921, 35000000L, 17854L, 1487L, 86L, 39L, 10L, 9L, 1L, 1L },
                    { 3922, 36000000L, 37190L, 1239L, 93L, 50L, 9L, 11L, 2L, 1L },
                    { 3923, 37000000L, 35846L, 2240L, 114L, 61L, 11L, 11L, 2L, 1L },
                    { 3924, 38000000L, 12211L, 2289L, 102L, 59L, 10L, 11L, 2L, 1L },
                    { 3925, 39000000L, 36263L, 1208L, 107L, 56L, 10L, 11L, 2L, 1L },
                    { 3926, 40000000L, 18534L, 2059L, 106L, 54L, 10L, 11L, 2L, 1L },
                    { 3927, 41000000L, 36193L, 2585L, 108L, 60L, 10L, 10L, 2L, 1L },
                    { 3928, 42000000L, 36675L, 2292L, 88L, 52L, 10L, 11L, 2L, 1L },
                    { 3929, 43000000L, 19309L, 2413L, 96L, 52L, 9L, 10L, 2L, 1L },
                    { 3930, 44000000L, 35287L, 1260L, 101L, 48L, 10L, 10L, 2L, 1L },
                    { 3931, 45000000L, 38967L, 1498L, 103L, 49L, 10L, 9L, 1L, 1L },
                    { 3932, 46000000L, 38778L, 1615L, 89L, 49L, 9L, 10L, 2L, 1L },
                    { 3933, 47000000L, 39175L, 1305L, 87L, 60L, 10L, 12L, 2L, 1L },
                    { 3934, 7000000L, 9053L, 1508L, 84L, 53L, 9L, 9L, 1L, 1L },
                    { 3935, 8000000L, 27876L, 1991L, 85L, 57L, 9L, 10L, 2L, 1L },
                    { 3936, 9000000L, 30123L, 3765L, 91L, 58L, 9L, 11L, 2L, 1L },
                    { 3937, 10000000L, 18138L, 1066L, 107L, 51L, 10L, 10L, 1L, 1L },
                    { 3938, 11000000L, 18548L, 1854L, 96L, 50L, 9L, 11L, 2L, 1L },
                    { 3939, 12000000L, 33098L, 2068L, 91L, 56L, 10L, 10L, 2L, 1L },
                    { 3940, 13000000L, 31297L, 1956L, 108L, 62L, 11L, 11L, 2L, 1L },
                    { 3941, 14000000L, 15352L, 1535L, 88L, 54L, 10L, 10L, 2L, 1L },
                    { 3942, 15000000L, 31004L, 3100L, 113L, 56L, 11L, 11L, 2L, 1L },
                    { 3943, 16000000L, 15592L, 1559L, 83L, 43L, 8L, 9L, 1L, 1L },
                    { 3944, 17000000L, 15806L, 1756L, 74L, 38L, 8L, 8L, 1L, 1L },
                    { 3945, 18000000L, 15885L, 1323L, 78L, 53L, 8L, 11L, 2L, 1L },
                    { 3946, 19000000L, 10933L, 1171L, 105L, 60L, 10L, 11L, 2L, 1L },
                    { 3947, 20000000L, 17196L, 1228L, 96L, 52L, 10L, 9L, 1L, 1L },
                    { 3948, 21000000L, 34012L, 1214L, 106L, 55L, 9L, 11L, 2L, 1L },
                    { 3949, 22000000L, 16693L, 1284L, 92L, 55L, 10L, 10L, 2L, 1L },
                    { 3950, 23000000L, 33675L, 1870L, 98L, 56L, 10L, 11L, 2L, 1L },
                    { 3951, 24000000L, 32699L, 1362L, 108L, 52L, 10L, 10L, 2L, 1L },
                    { 3952, 25000000L, 16620L, 1278L, 91L, 51L, 9L, 11L, 2L, 1L },
                    { 3953, 26000000L, 33186L, 1508L, 69L, 47L, 8L, 10L, 2L, 1L },
                    { 3954, 27000000L, 34467L, 1566L, 74L, 53L, 9L, 11L, 2L, 1L },
                    { 3955, 28000000L, 11474L, 1564L, 72L, 47L, 8L, 10L, 2L, 1L },
                    { 3956, 29000000L, 35601L, 1483L, 109L, 53L, 11L, 10L, 2L, 1L },
                    { 3957, 30000000L, 11609L, 1741L, 77L, 54L, 8L, 10L, 2L, 1L },
                    { 3958, 7000000L, 27005L, 3375L, 100L, 56L, 10L, 11L, 2L, 1L },
                    { 3959, 8000000L, 26830L, 2683L, 110L, 59L, 11L, 10L, 2L, 1L },
                    { 3960, 9000000L, 27650L, 6912L, 100L, 51L, 9L, 10L, 2L, 1L },
                    { 3961, 10000000L, 29425L, 1226L, 96L, 61L, 10L, 11L, 2L, 1L },
                    { 3962, 11000000L, 15014L, 1668L, 99L, 55L, 10L, 11L, 2L, 1L },
                    { 3963, 12000000L, 29471L, 1227L, 102L, 52L, 10L, 11L, 2L, 1L },
                    { 3964, 13000000L, 30011L, 1250L, 77L, 42L, 8L, 9L, 1L, 1L },
                    { 3965, 14000000L, 15048L, 2508L, 83L, 53L, 9L, 10L, 2L, 1L },
                    { 3966, 15000000L, 10535L, 3950L, 100L, 48L, 9L, 10L, 2L, 1L },
                    { 3967, 16000000L, 31083L, 1036L, 100L, 49L, 10L, 9L, 1L, 1L },
                    { 3968, 17000000L, 31535L, 1313L, 89L, 51L, 9L, 10L, 2L, 1L },
                    { 3969, 18000000L, 10372L, 1555L, 79L, 54L, 9L, 10L, 2L, 1L },
                    { 3970, 19000000L, 31963L, 2283L, 112L, 68L, 11L, 12L, 2L, 1L },
                    { 3971, 20000000L, 31909L, 1595L, 105L, 47L, 10L, 10L, 1L, 1L },
                    { 3972, 21000000L, 15885L, 1985L, 99L, 58L, 10L, 11L, 2L, 1L },
                    { 3973, 22000000L, 33096L, 1103L, 77L, 49L, 8L, 10L, 2L, 1L },
                    { 3974, 23000000L, 34004L, 1545L, 97L, 57L, 10L, 11L, 2L, 1L },
                    { 3975, 24000000L, 16777L, 1398L, 103L, 56L, 10L, 12L, 2L, 1L },
                    { 3976, 25000000L, 33650L, 2103L, 67L, 39L, 7L, 9L, 1L, 1L },
                    { 3977, 7000000L, 25729L, 1608L, 105L, 58L, 11L, 10L, 2L, 1L },
                    { 3978, 8000000L, 26783L, 3347L, 113L, 57L, 10L, 11L, 2L, 1L },
                    { 3979, 9000000L, 26941L, 2245L, 105L, 55L, 10L, 10L, 2L, 1L },
                    { 3980, 10000000L, 14278L, 1298L, 52L, 45L, 7L, 9L, 2L, 1L }
            });
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SuperLottoPrizes",
                schema: "LottoChecker");

            migrationBuilder.DropTable(
                name: "SuperLottoUserPicks",
                schema: "LottoChecker");

            migrationBuilder.DropTable(
                name: "SuperLottoWinningNumbers",
                schema: "LottoChecker");
        }
    }
}
