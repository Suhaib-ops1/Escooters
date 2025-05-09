using System;
using System.Collections.Generic;

namespace Escooters.Models;

public partial class Testimonial
{
    public int TestimonialId { get; set; }

    public int UserId { get; set; }

    public string Comment { get; set; } = null!;

    public int? Rating { get; set; }

    public DateTime? DatePosted { get; set; }

    public virtual User User { get; set; } = null!;
}
