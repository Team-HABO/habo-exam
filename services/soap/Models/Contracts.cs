using System.Runtime.Serialization;

namespace soap.Models;

[DataContract(Namespace = "http://example.com/library/wsdl")]
public class CreateArtist
{
    [DataMember(Order = 1, IsRequired = true)] public required string FirstName { get; set; }
    [DataMember(Order = 2, IsRequired = true)] public required string LastName { get; set; }
    [DataMember(Order = 3, IsRequired = true)] public required string Gender { get; set; }
    [DataMember(Order = 4, IsRequired = true)] public required DateOnly DateOfBirth { get; set; }
}

[DataContract(Namespace = "http://example.com/library/wsdl")]
public class GetArtistById
{
    [DataMember(Order = 1, IsRequired = true)] public required int ArtistId { get; set; }
}

[DataContract(Namespace = "http://example.com/library/wsdl")]
public class DeleteArtist
{
    [DataMember(Order = 1, IsRequired = true)] public required int ArtistId { get; set; }
}

[DataContract(Namespace = "http://example.com/library/wsdl")]
public class UpdateArtist
{
    [DataMember(Order = 1, IsRequired = true)] public required int ArtistId { get; set; }
    [DataMember(Order = 2, IsRequired = true)] public required string FirstName { get; set; }
    [DataMember(Order = 3, IsRequired = true)] public required string LastName { get; set; }
    [DataMember(Order = 4, IsRequired = true)] public required string Gender { get; set; }
    [DataMember(Order = 5)] public DateOnly DateOfBirth { get; set; }
}

[DataContract(Namespace = "http://example.com/library/wsdl")]
public class GetArtists
{
}

// ---------- Faults ---------- //
[DataContract(Namespace = "http://example.com/library/wsdl")]
public class NotFoundFault
{
    [DataMember(Order = 1)] public string ErrorCode { get; set; } = null!;
    [DataMember(Order = 2)] public string ErrorMessage { get; set; } = null!;
}

[DataContract(Namespace = "http://example.com/library/wsdl")]
public class ValidationFault
{
    [DataMember(Order = 1)] public string ErrorCode { get; set; } = null!;
    [DataMember(Order = 2)] public string ErrorMessage { get; set; } = null!;
}

[DataContract(Namespace = "http://example.com/library/wsdl")]
public class ConflictFault
{
    [DataMember(Order = 1)] public string ErrorCode { get; set; } = null!;
    [DataMember(Order = 2)] public string ErrorMessage { get; set; } = null!;
}
