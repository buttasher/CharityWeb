using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CharityApp.Models;

public partial class Donation
{
    [Key]
    public int DonationId { get; set; }

    [StringLength(100)]
    public string UserName { get; set; } = null!;

    [StringLength(100)]
    public string Email { get; set; } = null!;

    [StringLength(100)]
    public string? IpAddress { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DonationTime { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }

    public bool? IsFirstTimeDonor { get; set; }

    [StringLength(255)]
    public string? Device { get; set; }

    public int? FailedAttempts { get; set; }

    public bool? VpnUsed { get; set; }

    public bool? IsFraud { get; set; }

    [StringLength(500)]
    public string? FraudFlags { get; set; }

    [StringLength(50)]
    public string? FraudMethod { get; set; }

    public double? FraudConfidence { get; set; }
}
