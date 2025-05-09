using System;
using System.Collections.Generic;

namespace Escooters.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int UserId { get; set; }

    public int BikeId { get; set; }

    public int PickupLocationId { get; set; }

    public int DropoffLocationId { get; set; }

    public DateTime PickupDate { get; set; }

    public DateTime ReturnDate { get; set; }

    public decimal TotalCost { get; set; }

    public string Status { get; set; } = null!;

    public virtual Bike Bike { get; set; } = null!;

    public virtual Location DropoffLocation { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Location PickupLocation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
