using System;
using System.Collections.Generic;

namespace Escooters.Models;

public partial class Maintenance
{
    public int MaintenanceId { get; set; }

    public int? UserId { get; set; }

    public string? BikeName { get; set; }

    public string? BikeType { get; set; }

    public string? IssueDescription { get; set; }

    public DateTime? RequestDate { get; set; }

    public virtual User? User { get; set; }
}
