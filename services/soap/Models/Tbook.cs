using System.Runtime.Serialization;

namespace soap.Models;

public partial class Tbook
{
    [DataMember] public int NBookId { get; set; }

    [DataMember] public string CTitle { get; set; } = null!;

    [DataMember] public int NAuthorId { get; set; }

    [DataMember] public int? NPublishingYear { get; set; }

    [DataMember] public int NPublishingCompanyId { get; set; }
}
