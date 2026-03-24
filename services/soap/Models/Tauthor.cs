using System.Runtime.Serialization;

namespace soap.Models;

public partial class Tauthor
{
    [DataMember] public int NAuthorId { get; set; }

    [DataMember] public string CName { get; set; } = null!;

    [DataMember] public string? CSurname { get; set; }
}
