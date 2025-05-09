using System;
using System.Collections.Generic;

namespace Escooters.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int BookingId { get; set; }

    public int UserId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public DateTime PaymentDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual Booking Booking { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
