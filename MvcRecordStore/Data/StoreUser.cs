using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MvcRecordStore.Data;

namespace MvcRecordStore.Data;

public class StoreUser : IdentityUser
{
    [PersonalData]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(16, ErrorMessage = "Username can't have more than 30 characters.", MinimumLength = 3)]
    [RegularExpression(@"^[a-zA-Z0-9._-]*$")]
    public string CustomUsername { get; set; } = string.Empty;

    [NotMapped]
    public string Roles { get; set; }
}