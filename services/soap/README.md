# SOAP Library Service

A SOAP web service for managing a library — books, authors, and publishing companies. Built with ASP.NET Core (.NET 9) and [SoapCore](https://github.com/DigDes/SoapCore), using SQLite as the database.

---

## What is SOAP?

**SOAP** (Simple Object Access Protocol) is a messaging protocol for exchanging structured data between systems over a network. Unlike REST (which uses plain JSON over HTTP), SOAP:

- Uses **XML** for all messages — both requests and responses
- Defines its API contract in a **WSDL** (Web Services Description Language) file, which acts like a formal specification of every available operation
- Sends all requests via **HTTP POST** to a single endpoint URL
- Returns structured **fault messages** (typed errors) instead of HTTP status codes

### Key concepts

| Term | What it means |
|------|--------------|
| **WSDL** | An XML file that describes every operation the service offers, including input/output types. Clients use it to know how to call the service. |
| **SOAP Envelope** | The XML wrapper around every message. Contains a `Header` (optional metadata) and a `Body` (the actual request or response). |
| **Operation** | A single callable action on the service (e.g. `CreateBook`, `GetBookById`). |
| **Fault** | A typed error returned by the service when something goes wrong (e.g. `ValidationFault`, `NotFoundFault`). |

A minimal SOAP request looks like this:

```xml
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/"
                  xmlns:lib="http://example.com/library/wsdl">
  <soapenv:Header/>
  <soapenv:Body>
    <lib:GetBookById>
      <lib:NBookId>1</lib:NBookId>
    </lib:GetBookById>
  </soapenv:Body>
</soapenv:Envelope>
```

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Runtime | .NET 9 (ASP.NET Core) |
| SOAP framework | [SoapCore](https://github.com/DigDes/SoapCore) |
| Database | SQLite via Entity Framework Core |

---

## Project Structure

```
soap/
├── Services/
│   ├── ILibraryService.cs   # SOAP contract — defines all operations
│   └── LibraryService.cs    # Implementation of the operations
├── Models/
│   ├── Contracts.cs         # Request/response data shapes
│   ├── Tbook.cs             # Book database model
│   ├── Tauthor.cs           # Author database model
│   └── Tpublishingcompany.cs# Publishing company database model
├── Data/
│   └── AppDbContext.cs      # Entity Framework database context
├── Docs/
│   ├── ILibraryService.wsdl # WSDL contract file
│   ├── ILibraryService.xsd  # XML schema for data types
│   └── SOAP - Library Service.postman_collection.json
├── Program.cs               # App entry point and configuration
└── soap.csproj
```

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### Run the service

```bash
cd services/soap
dotnet run
```

The service starts on `http://localhost:5000` by default (see [Properties/launchSettings.json](Properties/launchSettings.json) for the exact port).

### Verify it is running

```
GET http://localhost:5000/health
```

Should return `healthy`.

---

## SOAP Endpoint

All SOAP calls go to a **single URL** using HTTP POST:

```
POST http://localhost:5000/LibraryService.asmx
Content-Type: text/xml
```

The WSDL (service description) is available at:

```
http://localhost:5000/LibraryService.asmx?wsdl
```

You can paste this URL into tools like SoapUI or Postman to automatically generate request templates for every operation.

---

## Available Operations

### Books

| Operation | Description |
|-----------|-------------|
| `CreateBook` | Add a new book. Returns the new book's ID. |
| `GetBookById` | Retrieve a book by its ID. |
| `UpdateBook` | Update one or more fields of an existing book. |
| `DeleteBook` | Remove a book by its ID. Returns the deleted book. |

#### CreateBook — example request

```xml
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/"
                  xmlns:lib="http://example.com/library/wsdl">
  <soapenv:Header/>
  <soapenv:Body>
    <lib:CreateBook>
      <lib:CTitle>The Pragmatic Programmer</lib:CTitle>
      <lib:NAuthorId>1</lib:NAuthorId>
      <lib:NPublishingYear>1999</lib:NPublishingYear>
      <lib:NPublishingCompanyId>1</lib:NPublishingCompanyId>
    </lib:CreateBook>
  </soapenv:Body>
</soapenv:Envelope>
```

#### GetBookById — example request

```xml
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/"
                  xmlns:lib="http://example.com/library/wsdl">
  <soapenv:Header/>
  <soapenv:Body>
    <lib:GetBookById>
      <lib:NBookId>1</lib:NBookId>
    </lib:GetBookById>
  </soapenv:Body>
</soapenv:Envelope>
```

---

### Authors

| Operation | Description |
|-----------|-------------|
| `CreateAuthor` | Add a new author. Returns the new author's ID. |
| `GetAuthorById` | Retrieve an author by their ID. |
| `ListAuthors` | Retrieve all authors. |
| `UpdateAuthor` | Update an existing author's name or surname. |
| `DeleteAuthor` | Remove an author (fails if they have books). |

---

### Publishing Companies

| Operation | Description |
|-----------|-------------|
| `CreatePublishingCompany` | Add a new publishing company. Returns its ID. |
| `GetPublishingCompanyById` | Retrieve a company by its ID. |
| `ListPublishingCompanies` | Retrieve all publishing companies. |
| `UpdatePublishingCompany` | Rename a publishing company. |
| `DeletePublishingCompany` | Remove a company (fails if it has books). |

---

## Error Handling (SOAP Faults)

SOAP uses **Fault messages** instead of HTTP error status codes. When an operation fails, the service returns a SOAP Fault inside the response body.

This service uses three fault types:

| Fault | When it is returned |
|-------|-------------------|
| `ValidationFault` | Input is invalid (e.g. empty title, invalid year) |
| `NotFoundFault` | The requested resource does not exist |
| `ConflictFault` | The operation would violate a constraint (e.g. deleting an author who still has books) |

All faults include an `ErrorCode` and an `ErrorMessage`. Example fault response:

```xml
<soapenv:Envelope>
  <soapenv:Body>
    <soapenv:Fault>
      <faultcode>soapenv:Client</faultcode>
      <faultstring>Book not found</faultstring>
      <detail>
        <NotFoundFault>
          <ErrorCode>BOOK_NOT_FOUND</ErrorCode>
          <ErrorMessage>No book with ID 99.</ErrorMessage>
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
Docs/SOAP - Library Service.postman_collection.json
```

Import it into Postman to get pre-built requests for every operation.

> **Tip:** In Postman, set the request method to `POST`, the URL to `http://localhost:5000/LibraryService.asmx`, and add the header `Content-Type: text/xml` before sending.

---

## Data Model

```
Tbook
 ├── NBookId (int, PK)
 ├── CTitle (string)
 ├── NAuthorId (int, FK → Tauthor)
 ├── NPublishingYear (int)
 └── NPublishingCompanyId (int, FK → Tpublishingcompany)

Tauthor
 ├── NAuthorId (int, PK)
 ├── CName (string)
 └── CSurname (string)

Tpublishingcompany
 ├── NPublishingCompanyId (int, PK)
 └── CName (string)
```

The database is stored as a SQLite file at `data/library.db` (created automatically on first run).

---

## Further Reading

- [W3C SOAP Specification](https://www.w3.org/TR/soap/)
- [SoapCore GitHub](https://github.com/DigDes/SoapCore)
- [WSDL explained](https://www.w3.org/TR/wsdl20/)
