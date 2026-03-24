using System.Runtime.Serialization;

namespace soap.Models;

public partial class Tpublishingcompany
{
    [DataMember] public int NPublishingCompanyId { get; set; }

    [DataMember] public string CName { get; set; } = null!;
}
