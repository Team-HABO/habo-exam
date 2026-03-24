using System.ServiceModel;
using soap.Data;
using soap.Models;

namespace soap.Services;

public class LibraryService(AppDbContext db) : ILibraryService
{
    private readonly AppDbContext _db = db;

    public int CreateBook(CreateBook request)
    {
        if (string.IsNullOrWhiteSpace(request.CTitle))
        {
            throw new FaultException<ValidationFault>(
                new ValidationFault { ErrorCode = "INVALID_TITLE", ErrorMessage = "Title cannot be empty." },
                new FaultReason("Validation failed")
            );
        }

        if (request.NAuthorId <= 0)
        {
            throw new FaultException<ValidationFault>(
                new ValidationFault
                    { ErrorCode = "INVALID_AUTHOR_ID", ErrorMessage = "Author ID must be a positive number." },
                new FaultReason("Validation failed")
            );
        }

        if (request.NPublishingCompanyId <= 0)
        {
            throw new FaultException<ValidationFault>(
                new ValidationFault
                {
                    ErrorCode = "INVALID_PUBLISHING_COMPANY_ID",
                    ErrorMessage = "Publishing company ID must be a positive number."
                },
                new FaultReason("Validation failed")
            );
        }

        if (request.NPublishingYear < 1000 || request.NPublishingYear > DateTime.UtcNow.Year)
        {
            throw new FaultException<ValidationFault>(
                new ValidationFault
                {
                    ErrorCode = "INVALID_PUBLISHING_YEAR",
                    ErrorMessage = $"Publishing year must be between 1000 and {DateTime.UtcNow.Year}."
                },
                new FaultReason("Validation failed")
            );
        }

        var book = new Tbook
        {
            CTitle = request.CTitle,
            NAuthorId = request.NAuthorId,
            NPublishingCompanyId = request.NPublishingCompanyId,
            NPublishingYear = request.NPublishingYear
        };

        _db.Tbooks.Add(book);
        _db.SaveChanges();
        return book.NBookId;
    }


    public Tbook DeleteBook(DeleteBook request)
    {
        var book = _db.Tbooks.FirstOrDefault(b => b.NBookId == request.NBookId);

        if (book is null)
        {
            throw new FaultException<NotFoundFault>(
                new NotFoundFault
                {
                    ErrorCode = "BOOK_NOT_FOUND",
                    ErrorMessage = $"No book with ID {request.NBookId}."
                },
                new FaultReason("Book not found")
            );
        }

        _db.Tbooks.Remove(book);
        _db.SaveChanges();
        return book;
    }

    public Tbook GetBookById(GetBookById request)
    {
        var book = _db.Tbooks.FirstOrDefault(b => b.NBookId == request.NBookId);

        if (book is null)
        {
            throw new FaultException<NotFoundFault>(
                new NotFoundFault
                {
                    ErrorCode = "BOOK_NOT_FOUND",
                    ErrorMessage = $"No book with ID {request.NBookId}."
                },
                new FaultReason("Book not found")
            );
        }

        return book;
    }

    public Tbook UpdateBook(UpdateBook request)
    {
        var book = _db.Tbooks.FirstOrDefault(b => b.NBookId == request.NBookId);

        if (book is null)
        {
            throw new FaultException<NotFoundFault>(
                new NotFoundFault
                {
                    ErrorCode = "BOOK_NOT_FOUND",
                    ErrorMessage = $"No book with ID {request.NBookId}."
                },
                new FaultReason("Book not found")
            );
        }

        if (request.CTitle is not null)
        {
            if (string.IsNullOrWhiteSpace(request.CTitle))
            {
                throw new FaultException<ValidationFault>(
                    new ValidationFault { ErrorCode = "INVALID_TITLE", ErrorMessage = "Title cannot be empty." },
                    new FaultReason("Validation failed")
                );
            }

            book.CTitle = request.CTitle;
        }

        if (request.NAuthorId is not null)
        {
            if (request.NAuthorId <= 0)
            {
                throw new FaultException<ValidationFault>(
                    new ValidationFault
                        { ErrorCode = "INVALID_AUTHOR_ID", ErrorMessage = "Author ID must be a positive number." },
                    new FaultReason("Validation failed")
                );
            }

            book.NAuthorId = request.NAuthorId.Value;
        }

        if (request.NPublishingYear is not null)
        {
            if (request.NPublishingYear < 1000 || request.NPublishingYear > DateTime.UtcNow.Year)
            {
                throw new FaultException<ValidationFault>(
                    new ValidationFault
                    {
                        ErrorCode = "INVALID_PUBLISHING_YEAR",
                        ErrorMessage = $"Publishing year must be between 1000 and {DateTime.UtcNow.Year}."
                    },
                    new FaultReason("Validation failed")
                );
            }

            book.NPublishingYear = request.NPublishingYear.Value;
        }

        if (request.NPublishingCompanyId is not null)
        {
            if (request.NPublishingCompanyId <= 0)
            {
                throw new FaultException<ValidationFault>(
                    new ValidationFault
                    {
                        ErrorCode = "INVALID_PUBLISHING_COMPANY_ID",
                        ErrorMessage = "Publishing company ID must be a positive number."
                    },
                    new FaultReason("Validation failed")
                );
            }

            book.NPublishingCompanyId = request.NPublishingCompanyId.Value;
        }

        _db.SaveChanges();
        return book;
    }

    public int CreateAuthor(CreateAuthor request)
    {
        if (string.IsNullOrWhiteSpace(request.CName))
        {
            throw new FaultException<ValidationFault>(
                new ValidationFault { ErrorCode = "INVALID_NAME", ErrorMessage = "Name cannot be empty." },
                new FaultReason("Validation failed")
            );
        }


        var author = new Tauthor
        {
            CName = request.CName,
            CSurname = request.CSurname ?? ""
        };

        _db.Tauthors.Add(author);
        _db.SaveChanges();
        return author.NAuthorId;
    }

    public Tauthor GetAuthorById(GetAuthorById request)
    {
        var author = _db.Tauthors.FirstOrDefault(a => a.NAuthorId == request.NAuthorId);

        if (author is null)
        {
            throw new FaultException<NotFoundFault>(
                new NotFoundFault
                {
                    ErrorCode = "AUTHOR_NOT_FOUND",
                    ErrorMessage = $"No author with ID {request.NAuthorId}."
                },
                new FaultReason("Author not found")
            );
        }

        return author;
    }

    public List<Tauthor> ListAuthors(ListAuthors request)
    {
        var authors = _db.Tauthors.ToList();

        return authors;
    }

    public Tauthor UpdateAuthor(UpdateAuthor request)
    {
        var author = _db.Tauthors.FirstOrDefault(a => a.NAuthorId == request.NAuthorId);

        if (author is null)
        {
            throw new FaultException<NotFoundFault>(
                new NotFoundFault
                {
                    ErrorCode = "AUTHOR_NOT_FOUND",
                    ErrorMessage = $"No author with ID {request.NAuthorId}."
                },
                new FaultReason("Author not found")
            );
        }

        if (request.CName is not null)
        {
            if (string.IsNullOrWhiteSpace(request.CName))
            {
                throw new FaultException<ValidationFault>(
                    new ValidationFault { ErrorCode = "INVALID_NAME", ErrorMessage = "Name cannot be empty." },
                    new FaultReason("Validation failed")
                );
            }

            author.CName = request.CName;
        }

        if (request.CSurname is not null)
        {
            if (string.IsNullOrWhiteSpace(request.CSurname))
            {
                throw new FaultException<ValidationFault>(
                    new ValidationFault { ErrorCode = "INVALID_SURNAME", ErrorMessage = "Surname cannot be empty." },
                    new FaultReason("Validation failed")
                );
            }

            author.CSurname = request.CSurname;
        }

        _db.SaveChanges();
        return author;
    }

    public Tauthor DeleteAuthor(DeleteAuthor request)
    {
        var author = _db.Tauthors.FirstOrDefault(a => a.NAuthorId == request.NAuthorId);

        if (author is null)
        {
            throw new FaultException<NotFoundFault>(
                new NotFoundFault
                {
                    ErrorCode = "AUTHOR_NOT_FOUND",
                    ErrorMessage = $"No author with ID {request.NAuthorId}."
                },
                new FaultReason("Author not found")
            );
        }

        if (_db.Tbooks.Any(b => b.NAuthorId == author.NAuthorId))
        {
            throw new FaultException<ConflictFault>(
                new ConflictFault
                {
                    ErrorCode = "AUTHOR_HAS_BOOKS",
                    ErrorMessage =
                        $"Author with ID {request.NAuthorId} cannot be deleted because they are referenced by one or more books."
                },
                new FaultReason("Conflict")
            );
        }

        _db.Tauthors.Remove(author);
        _db.SaveChanges();
        return author;
    }

    public int CreatePublishingCompany(CreatePublishingCompany request)
    {
        if (string.IsNullOrWhiteSpace(request.CName))
        {
            throw new FaultException<ValidationFault>(
                new ValidationFault { ErrorCode = "INVALID_NAME", ErrorMessage = "Name cannot be empty." },
                new FaultReason("Validation failed")
            );
        }

        var publishingCompany = new Tpublishingcompany
        {
            CName = request.CName
        };

        _db.Tpublishingcompanies.Add(publishingCompany);
        _db.SaveChanges();

        return publishingCompany.NPublishingCompanyId;
    }

    public Tpublishingcompany GetPublishingCompanyById(GetPublishingCompanyById request)
    {
        var publishingCompany =
            _db.Tpublishingcompanies.FirstOrDefault(pc => pc.NPublishingCompanyId == request.NPublishingCompanyId);

        if (publishingCompany is null)
        {
            throw new FaultException<NotFoundFault>(
                new NotFoundFault
                {
                    ErrorCode = "PUBLISHING_COMPANY_NOT_FOUND",
                    ErrorMessage = $"No publishing company with ID {request.NPublishingCompanyId}."
                },
                new FaultReason("Publishing company not found")
            );
        }

        return publishingCompany;
    }

    public List<Tpublishingcompany> ListPublishingCompanies(ListPublishingCompanies request)
    {
        var publishingCompanies = _db.Tpublishingcompanies.ToList();

        return publishingCompanies;
    }

    public Tpublishingcompany UpdatePublishingCompany(UpdatePublishingCompany request)
    {
        var publishingCompany =
            _db.Tpublishingcompanies.FirstOrDefault(pc => pc.NPublishingCompanyId == request.NPublishingCompanyId);

        if (publishingCompany is null)
        {
            throw new FaultException<NotFoundFault>(
                new NotFoundFault
                {
                    ErrorCode = "PUBLISHING_COMPANY_NOT_FOUND",
                    ErrorMessage = $"No publishing company with ID {request.NPublishingCompanyId}."
                },
                new FaultReason("Publishing company not found")
            );
        }


        if (string.IsNullOrWhiteSpace(request.CName))
        {
            throw new FaultException<ValidationFault>(
                new ValidationFault { ErrorCode = "INVALID_NAME", ErrorMessage = "Name cannot be empty." },
                new FaultReason("Validation failed")
            );
        }

        publishingCompany.CName = request.CName;


        _db.SaveChanges();
        return publishingCompany;
    }

    public Tpublishingcompany DeletePublishingCompany(DeletePublishingCompany request)
    {
        var publishingCompany =
            _db.Tpublishingcompanies.FirstOrDefault(pc => pc.NPublishingCompanyId == request.NPublishingCompanyId);

        if (publishingCompany is null)
        {
            throw new FaultException<NotFoundFault>(
                new NotFoundFault
                {
                    ErrorCode = "PUBLISHING_COMPANY_NOT_FOUND",
                    ErrorMessage = $"No publishing company with ID {request.NPublishingCompanyId}."
                },
                new FaultReason("Publishing company not found")
            );
        }

        if (_db.Tbooks.Any(b => b.NPublishingCompanyId == publishingCompany.NPublishingCompanyId))
        {
            throw new FaultException<ConflictFault>(
                new ConflictFault
                {
                    ErrorCode = "PUBLISHING_COMPANY_HAS_BOOKS",
                    ErrorMessage =
                        $"Publishing company with ID {request.NPublishingCompanyId} cannot be deleted because it is referenced by one or more books."
                },
                new FaultReason("Conflict")
            );
        }

        _db.Tpublishingcompanies.Remove(publishingCompany);
        _db.SaveChanges();
        return publishingCompany;
    }
}