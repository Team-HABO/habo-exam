using System.ServiceModel;
using soap.Models;

namespace soap.Services;

[ServiceContract(Namespace = "http://example.com/library/wsdl")]
public interface ILibraryService
{
    [OperationContract]
    [FaultContract(typeof(ValidationFault))]
    int CreateBook(CreateBook request);

    [OperationContract]
    [FaultContract(typeof(NotFoundFault))]
    Tbook GetBookById(GetBookById request);

    [OperationContract]
    [FaultContract(typeof(ValidationFault))]
    [FaultContract(typeof(NotFoundFault))]
    Tbook UpdateBook(UpdateBook request);

    [OperationContract]
    [FaultContract(typeof(NotFoundFault))]
    Tbook DeleteBook(DeleteBook request);

    [OperationContract]
    [FaultContract(typeof(ValidationFault))]
    int CreateAuthor(CreateAuthor request);

    [OperationContract]
    [FaultContract(typeof(NotFoundFault))]
    Tauthor GetAuthorById(GetAuthorById request);

    [OperationContract]
    [FaultContract(typeof(NotFoundFault))]
    List<Tauthor> ListAuthors(ListAuthors request);

    [OperationContract]
    [FaultContract(typeof(ValidationFault))]
    [FaultContract(typeof(NotFoundFault))]
    Tauthor UpdateAuthor(UpdateAuthor request);

    [OperationContract]
    [FaultContract(typeof(NotFoundFault))]
    [FaultContract(typeof(ConflictFault))]
    Tauthor DeleteAuthor(DeleteAuthor request);

    [OperationContract]
    [FaultContract(typeof(ValidationFault))]
    int CreatePublishingCompany(CreatePublishingCompany request);

    [OperationContract]
    [FaultContract(typeof(NotFoundFault))]
    Tpublishingcompany GetPublishingCompanyById(GetPublishingCompanyById request);

    [OperationContract]
    List<Tpublishingcompany> ListPublishingCompanies(ListPublishingCompanies request);

    [OperationContract]
    [FaultContract(typeof(NotFoundFault))]
    [FaultContract(typeof(ValidationFault))]
    Tpublishingcompany UpdatePublishingCompany(UpdatePublishingCompany request);

    [OperationContract]
    [FaultContract(typeof(NotFoundFault))]
    [FaultContract(typeof(ConflictFault))]
    Tpublishingcompany DeletePublishingCompany(DeletePublishingCompany request);
}
