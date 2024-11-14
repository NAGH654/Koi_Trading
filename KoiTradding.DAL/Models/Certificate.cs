namespace KoiTradding.DAL.Models;

public class Certificate
{
    public int KoiId { get; set; }

    public string? Name { get; set; }

    public byte[]? Image { get; set; }

    public DateTime? CreateDate { get; set; }

    public virtual KoiFish Koi { get; set; } = null!;
}