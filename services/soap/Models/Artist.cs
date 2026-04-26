using System.Runtime.Serialization;

namespace soap.Models;

public partial class Artist
{
    [DataMember] public int Id { get; set; }

    [DataMember] public string? FirstName { get; set; }

    [DataMember] public string? LastName { get; set; }

    [DataMember] public string? Gender { get; set; }

    [DataMember] public DateOnly DateOfBirth { get; set; }
}
