using System;
using System.Collections.Generic;

namespace Escooters.Models;

public partial class Bike
{
    public int BikeId { get; set; }

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string? Description { get; set; }

    public decimal PricePerDay { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
