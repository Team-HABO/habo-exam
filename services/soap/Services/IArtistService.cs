using System.ServiceModel;
using soap.Models;

namespace soap.Services;

[ServiceContract(Namespace = "http://example.com/library/wsdl")]
public interface IArtistService
{
    [OperationContract]
    [FaultContract(typeof(NotFoundFault))]
    List<Artist> ListArtists(GetArtists request);

    [OperationContract]
    [FaultContract(typeof(NotFoundFault))]
    Artist GetArtistById(GetArtistById request);

    [OperationContract]
    [FaultContract(typeof(ValidationFault))]
    [FaultContract(typeof(ConflictFault))]
    int CreateArtist(CreateArtist request);

    [OperationContract]
    [FaultContract(typeof(ValidationFault))]
    [FaultContract(typeof(NotFoundFault))]
    Artist UpdateArtist(UpdateArtist request);

    [OperationContract]
    [FaultContract(typeof(NotFoundFault))]
    Artist DeleteArtist(DeleteArtist request);
}
