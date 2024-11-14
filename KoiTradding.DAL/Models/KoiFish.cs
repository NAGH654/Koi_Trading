namespace KoiTradding.DAL.Models;

public class KoiFish
{
    public int KoiId { get; set; }

    public string? Origin { get; set; }

    public string? Gender { get; set; }

    public int? Age { get; set; }

    public decimal? Size { get; set; }

    public string? Status { get; set; }

    public decimal? Price { get; set; }

    public int? CategoryId { get; set; }

    public string? Health { get; set; }

    public byte[]? KoiImage { get; set; }


    public virtual Category? Category { get; set; }

    public virtual Certificate? Certificate { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}