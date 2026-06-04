# SOAP Artist Service

A SOAP web service for managing artists. Built with ASP.NET Core (.NET 10) and [SoapCore](https://github.com/DigDes/SoapCore), using PostgreSQL as the database.

---

## What is SOAP?

**SOAP** (Simple Object Access Protocol) is a messaging protocol for exchanging structured data between systems over a network. Unlike REST (which uses plain JSON over HTTP), SOAP:

- Uses **XML** for all messages — both requests and responses
- Defines its API contract in a **WSDL** (Web Services Description Language) file, which acts like a formal specification of every available operation
- Sends all requests via **HTTP POST** to a single endpoint URL
- Returns structured **fault messages** (typed errors) instead of HTTP status codes

### Key concepts

| Term              | What it means                                                                                                                                |
| ----------------- | -------------------------------------------------------------------------------------------------------------------------------------------- |
| **WSDL**          | An XML file that describes every operation the service offers, including input/output types. Clients use it to know how to call the service. |
| **SOAP Envelope** | The XML wrapper around every message. Contains a `Header` (optional metadata) and a `Body` (the actual request or response).                 |
| **Operation**     | A single callable action on the service (e.g. `CreateArtist`, `GetArtistById`).                                                              |
| **Fault**         | A typed error returned by the service when something goes wrong (e.g. `ValidationFault`, `NotFoundFault`).                                   |

A minimal SOAP request looks like this:

```xml
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/"
                  xmlns:lib="http://example.com/library/wsdl">
  <soapenv:Header/>
  <soapenv:Body>
    <lib:GetArtistById>
      <lib:ArtistId>1</lib:ArtistId>
    </lib:GetArtistById>
  </soapenv:Body>
</soapenv:Envelope>
```

---

## Tech Stack

| Layer              | Technology                                               |
| ------------------ | -------------------------------------------------------- |
| Runtime            | .NET 10 (ASP.NET Core)                                   |
| SOAP framework     | [SoapCore](https://github.com/DigDes/SoapCore)           |
| Database           | PostgreSQL via Entity Framework Core (Npgsql)            |
| Input sanitization | [HtmlSanitizer](https://github.com/mganss/HtmlSanitizer) |

---

## Project Structure

```
soap/
├── Services/
│   ├── IArtistService.cs    # SOAP contract — defines all operations
│   └── ArtistService.cs     # Implementation of the operations
├── Models/
│   ├── Artist.cs            # Artist database model
│   └── Contracts.cs         # Request/response data shapes and fault types
├── data/
│   ├── AppDbContext.cs      # Entity Framework database context
│   └── DbSeeder.cs         # Seeds initial artist data
├── docs/
│   ├── IArtistService.wsdl  # WSDL contract file
│   └── soap.postman_collection.json
├── Migrations/              # EF Core database migrations
├── Program.cs               # App entry point and configuration
├── Dockerfile               # Docker build definition
└── soap.csproj
```

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- A PostgreSQL instance (set the connection string via the `CONNECTION_STRING` environment variable)

### Run with Docker

Make sure to fill out the .env file. Run in Dev Container. Once running, type:

```bash
dotnet start
```

### Verify it is running

```
GET http://localhost:5292/health
```

Should return `healthy`.

---

## SOAP Endpoint

All SOAP calls go to a **single URL** using HTTP POST:

```
POST http://localhost:5292/ArtistService.asmx
Content-Type: text/xml
```

The WSDL (service description) is available at:

```
http://localhost:5292/ArtistService.asmx?wsdl
```

You can paste this URL into tools like SoapUI or Postman to automatically generate request templates for every operation.

---

## Available Operations

### Artists

| Operation       | Description                                               |
| --------------- | --------------------------------------------------------- |
| `CreateArtist`  | Add a new artist. Returns the new artist's ID.            |
| `GetArtistById` | Retrieve an artist by their ID.                           |
| `ListArtists`   | Retrieve all artists.                                     |
| `UpdateArtist`  | Update an existing artist's details.                      |
| `DeleteArtist`  | Remove an artist by their ID. Returns the deleted artist. |

#### CreateArtist — example request

```xml
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/"
                  xmlns:lib="http://example.com/library/wsdl">
  <soapenv:Header/>
  <soapenv:Body>
    <lib:CreateArtist>
      <lib:FirstName>Leonardo</lib:FirstName>
      <lib:LastName>da Vinci</lib:LastName>
      <lib:Gender>Male</lib:Gender>
      <lib:DateOfBirth>1452-04-15</lib:DateOfBirth>
    </lib:CreateArtist>
  </soapenv:Body>
</soapenv:Envelope>
```

#### GetArtistById — example request

```xml
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/"
                  xmlns:lib="http://example.com/library/wsdl">
  <soapenv:Header/>
  <soapenv:Body>
    <lib:GetArtistById>
      <lib:ArtistId>1</lib:ArtistId>
    </lib:GetArtistById>
  </soapenv:Body>
</soapenv:Envelope>
```

---

## Error Handling (SOAP Faults)

SOAP uses **Fault messages** instead of HTTP error status codes. When an operation fails, the service returns a SOAP Fault inside the response body.

This service uses three fault types:

| Fault             | When it is returned                                                         |
| ----------------- | --------------------------------------------------------------------------- |
| `ValidationFault` | Input is invalid (e.g. empty first name, missing date of birth)             |
| `NotFoundFault`   | The requested artist does not exist                                         |
| `ConflictFault`   | The operation would violate a constraint (e.g. creating a duplicate artist) |

All faults include an `ErrorCode` and an `ErrorMessage`. Example fault response:

```xml
<soapenv:Envelope>
  <soapenv:Body>
    <soapenv:Fault>
      <faultcode>soapenv:Client</faultcode>
      <faultstring>Artist not found</faultstring>
      <detail>
        <NotFoundFault>
          <ErrorCode>NOT_FOUND</ErrorCode>
          <ErrorMessage>Artist with Id 99 not found.</ErrorMessage>
        </NotFoundFault>
      </detail>
    </soapenv:Fault>
  </soapenv:Body>
</soapenv:Envelope>
```

---

## Testing with Postman

A ready-made Postman collection is included:

```
docs/soap.postman_collection.json
```

Import it into Postman to get pre-built requests for every operation.

> **Tip:** In Postman, set the request method to `POST`, the URL to `http://localhost:5292/ArtistService.asmx`, and add the header `Content-Type: text/xml` before sending.

---

## Data Model

```
Artist
 ├── Id (int, PK)
 ├── FirstName (string)
 ├── LastName (string)
 ├── Gender (string)
 └── DateOfBirth (DateOnly)
```

The database is PostgreSQL, configured via the `CONNECTION_STRING` environment variable. On first run the service applies migrations and seeds sample artist data (Leonardo da Vinci, Frida Kahlo, Pablo Picasso, etc.).
