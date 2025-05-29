using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

[Table("SuperLottoUserPicks", Schema = "LottoChecker")]
[Index("RowCheckSum", Name = "UC_LottoChecker_SuperLottoUserPicks_RowCheckSum", IsUnique = true)]
public partial class SuperLottoUserPick
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    [Required, StringLength(128)]
    public string UserId { get; set; } = null!;
    [Required, Range(1, 47)]
    public int FirstPick { get; set; }
    [Required, Range(1, 47)]
    public int SecondPick { get; set; }
    [Required, Range(1, 47)]
    public int ThirdPick { get; set; }
    [Required, Range(1, 47)]
    public int FourthPick { get; set; }
    [Required, Range(1, 47)]
    public int FifthPick { get; set; }
    [Required, Range(1, 27)]
    public int MegaPick { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [StringLength(140)]
    public string? RowCheckSum { get; set; }
    public DateTime DateAdded { get; private set; }
}

