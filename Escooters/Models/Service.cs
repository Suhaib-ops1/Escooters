using System;
using System.Collections.Generic;

namespace Escooters.Models;

public partial class Service
{
    public int ServiceId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? IconUrl { get; set; }
}
