using System.Runtime.Serialization;

namespace soap.Models
{

    // Class-level SOAP attribute in its own partial so re-scaffolding User.cs never removes it.
    [DataContract(Namespace = "http://example.com/library/wsdl")]
    public partial class Tbook { }
}
