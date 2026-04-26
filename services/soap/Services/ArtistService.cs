using System.ServiceModel;
using soap.Data;
using soap.Models;

namespace soap.Services;

public class ArtistService(AppDbContext db) : IArtistService
{
    private readonly AppDbContext _db = db;

    public int CreateArtist(CreateArtist request)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(request.FirstName))
            throw new FaultException<ValidationFault>(
                new ValidationFault { ErrorCode = "VALIDATION_ERROR", ErrorMessage = "FirstName is required." },
                "FirstName is required.");

        if (string.IsNullOrWhiteSpace(request.LastName))
            throw new FaultException<ValidationFault>(
                new ValidationFault { ErrorCode = "VALIDATION_ERROR", ErrorMessage = "LastName is required." },
                "LastName is required.");

        if (string.IsNullOrWhiteSpace(request.Gender))
            throw new FaultException<ValidationFault>(
                new ValidationFault { ErrorCode = "VALIDATION_ERROR", ErrorMessage = "Gender is required." },
                "Gender is required.");

        if (request.DateOfBirth == default)
            throw new FaultException<ValidationFault>(
                new ValidationFault { ErrorCode = "VALIDATION_ERROR", ErrorMessage = "DateOfBirth is required." },
                "DateOfBirth is required.");

        // Already exists check
        var exists = _db.Artists.Any(a => a.FirstName == request.FirstName && a.LastName == request.LastName);
        if (exists)
            throw new FaultException<ConflictFault>(
                new ConflictFault { ErrorCode = "CONFLICT", ErrorMessage = $"Artist '{request.FirstName} {request.LastName}' already exists." },
                $"Artist '{request.FirstName} {request.LastName}' already exists.");

        // Create
        var artist = new Artist
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Gender = request.Gender,
            DateOfBirth = request.DateOfBirth
        };

        _db.Artists.Add(artist);
        _db.SaveChanges();

        return artist.Id;
    }

    public Artist DeleteArtist(DeleteArtist request)
    {
        var artist = _db.Artists.Find(request.ArtistId);

        if (artist == null)
            throw new FaultException<NotFoundFault>(
                new NotFoundFault { ErrorCode = "NOT_FOUND", ErrorMessage = $"Artist with Id {request.ArtistId} not found." },
                $"Artist with Id {request.ArtistId} not found.");

        _db.Artists.Remove(artist);
        _db.SaveChanges();

        return artist;
    }

    public Artist GetArtistById(GetArtistById request)
    {
        var artist = _db.Artists.Find(request.ArtistId);

        if (artist == null)
            throw new FaultException<NotFoundFault>(
                new NotFoundFault { ErrorCode = "NOT_FOUND", ErrorMessage = $"Artist with Id {request.ArtistId} not found." },
                $"Artist with Id {request.ArtistId} not found.");

        return artist;
    }

    public List<Artist> ListArtists(GetArtists request)
    {
        return _db.Artists.ToList();
    }

    public Artist UpdateArtist(UpdateArtist request)
    {
        var artist = _db.Artists.Find(request.ArtistId);

        if (artist == null)
            throw new FaultException<NotFoundFault>(
                new NotFoundFault { ErrorCode = "NOT_FOUND", ErrorMessage = $"Artist with Id {request.ArtistId} not found." },
                $"Artist with Id {request.ArtistId} not found.");

        if (string.IsNullOrWhiteSpace(request.FirstName))
            throw new FaultException<ValidationFault>(
                new ValidationFault { ErrorCode = "VALIDATION_ERROR", ErrorMessage = "FirstName is required." },
                "FirstName is required.");

        if (string.IsNullOrWhiteSpace(request.LastName))
            throw new FaultException<ValidationFault>(
                new ValidationFault { ErrorCode = "VALIDATION_ERROR", ErrorMessage = "LastName is required." },
                "LastName is required.");

        if (string.IsNullOrWhiteSpace(request.Gender))
            throw new FaultException<ValidationFault>(
                new ValidationFault { ErrorCode = "VALIDATION_ERROR", ErrorMessage = "Gender is required." },
                "Gender is required.");

        if (request.DateOfBirth == default)
            throw new FaultException<ValidationFault>(
                new ValidationFault { ErrorCode = "VALIDATION_ERROR", ErrorMessage = "DateOfBirth is required." },
                "DateOfBirth is required.");

        artist.FirstName = request.FirstName;
        artist.LastName = request.LastName;
        artist.Gender = request.Gender;
        artist.DateOfBirth = request.DateOfBirth;

        _db.SaveChanges();

        return artist;
    }
}
