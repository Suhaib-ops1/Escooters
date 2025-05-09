using System;
using System.Collections.Generic;

namespace Escooters.Models;

public partial class Location
{
    public int LocationId { get; set; }

    public string LocationName { get; set; } = null!;

    public string? Address { get; set; }

    public virtual ICollection<Booking> BookingDropoffLocations { get; set; } = new List<Booking>();

    public virtual ICollection<Booking> BookingPickupLocations { get; set; } = new List<Booking>();
}
