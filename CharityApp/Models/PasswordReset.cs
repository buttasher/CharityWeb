using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CharityApp.Models;

public partial class PasswordReset
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    public string Token { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime ExpiryDate { get; set; }
}
