using System;
using System.Collections.Generic;

namespace KoiTradding.DAL.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public decimal? Amount { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? Status { get; set; }

    public string? TransactionCode { get; set; }

    public int? ConsignmentId { get; set; }

    public int? OrderId { get; set; }

    public virtual Order? Order { get; set; }
}
