using System;
using System.Collections.Generic;

namespace Escooters.Models;

public partial class ContactMessage
{
    public int MessageId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string Subject { get; set; } = null!;

    public string Message { get; set; } = null!;

    public DateTime? SentDate { get; set; }
}
