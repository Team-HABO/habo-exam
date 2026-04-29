# IArtistService — SOAP Service WSDL

This folder contains the WSDL file that defines a SOAP web service for managing **Artist** records. It describes **what the service does** and **what the data looks like** in a single self-contained file.

---

## Reading the XML — Namespaces and Prefixes

Before diving into the file, it helps to understand the `prefix:something` syntax you'll see everywhere. It can look intimidating at first, but the rule is simple:

**`prefix:tagname` means "this tag belongs to that prefix's namespace".**

A namespace is just a URL used as a unique identifier — it guarantees that a tag called `element` in one vocabulary doesn't get confused with a tag called `element` in another. The URL doesn't have to point to anything real; it's just a name.

The prefixes are declared at the top of the file and act as shorthand aliases:

```xml
xmlns:xs="http://www.w3.org/2001/XMLSchema"
xmlns:wsdl="http://schemas.xmlsoap.org/wsdl/"
xmlns:tns="http://example.com/library/wsdl"
```

After that declaration, `xs:` means "from the XML Schema standard", `wsdl:` means "from the WSDL standard", and `tns:` means "from this service's own namespace" (tns = **t**arget **n**ame**s**pace).

### Prefixes used in this file

| Prefix | Stands for | What it marks |
|---|---|---|
| `xs:` | XML Schema (`http://www.w3.org/2001/XMLSchema`) | Standard schema building blocks — types, elements, sequences |
| `wsdl:` | WSDL vocabulary (`http://schemas.xmlsoap.org/wsdl/`) | Service structure — operations, messages, bindings |
| `tns:` | This service's namespace (`http://example.com/library/wsdl`) | Things defined in this project — `tns:Artist`, `tns:CreateArtist`, etc. |
| `soap:` | SOAP vocabulary | Transport details — how messages are sent over HTTP |
| `wsam:` | WS-Addressing metadata | The `Action` URLs on each operation |
| `ser:` | Microsoft serialization namespace | MS-specific serialization hints (mostly ignored at runtime) |

### Common tag names you'll see

Once you know the prefix, the part after `:` is just the tag's role within that vocabulary:

| Tag | Vocabulary | What it means |
|---|---|---|
| `xs:schema` | XSD | The root container for a schema block |
| `xs:element` | XSD | Declares a named element (like a field or a message wrapper) |
| `xs:complexType` | XSD | Defines a type made up of multiple fields |
| `xs:simpleType` | XSD | Defines a type based on a single primitive (like a restricted string) |
| `xs:sequence` | XSD | The fields inside a complexType, in order |
| `xs:restriction` | XSD | Constrains a base type (e.g. a string matching a pattern) |
| `xs:import` | XSD | Pulls in definitions from another namespace/file |
| `wsdl:message` | WSDL | An envelope definition — wraps one or more parts |
| `wsdl:part` | WSDL | One piece of a message, referencing an XSD element |
| `wsdl:portType` | WSDL | The abstract list of operations (like an interface) |
| `wsdl:operation` | WSDL | A single callable operation |
| `wsdl:binding` | WSDL | Ties operations to a concrete transport (SOAP/HTTP) |
| `wsdl:service` | WSDL | The physical endpoint URL |
| `soap:binding` | SOAP | Declares this binding uses the SOAP HTTP transport |
| `soap:body` | SOAP | Says the message body is sent as-is (literal) |
| `soap:fault` | SOAP | Declares how fault messages are transported |

### Attributes work the same way

Attributes on a tag can also be namespace-prefixed. For example:

```xml
<wsdl:input wsam:Action="http://example.com/library/wsdl/IArtistService/CreateArtist" ... />
```

Here `wsam:Action` is an attribute from the WS-Addressing namespace being added to a `wsdl:input` tag. The prefix tells you which vocabulary that attribute belongs to.

Unprefixed attributes (like `name="CreateArtist"` or `type="xs:int"`) belong to the element's own vocabulary by default.

---

## The File

| File | Role |
|---|---|
| `IArtistService.wsdl` | The full service contract — types, operations, messages, binding, and endpoint |

All XSD type definitions are embedded inline within the `<wsdl:types>` section.

---

## `IArtistService.wsdl`

The WSDL (Web Service Description Language) file is the **front door** of the service. It answers three questions:

- **What can I call?** — the operations (e.g. `CreateArtist`, `DeleteArtist`)
- **What do I send and receive?** — the messages for each operation
- **Where do I call it?** — the endpoint URL

### Structure

A WSDL is made up of four sections, always in this order:

```
<wsdl:types>       →  "here are the data types used" (inline XSD schemas)
<wsdl:message>     →  "here are the envelopes for each call"
<wsdl:portType>    →  "here are the operations and what they accept/return"
<wsdl:binding>     →  "here is how to physically send the messages (SOAP over HTTP)"
<wsdl:service>     →  "here is the URL to call"
```

### Types (`<wsdl:types>`)

The type definitions are embedded directly in the WSDL as six inline XSD schemas:

1. **Operation wrapper elements** — request/response envelope elements (e.g. `CreateArtist`, `CreateArtistResponse`)
2. **Serialization primitives** — standard Microsoft DataContract types (`guid`, `duration`, `char`)
3. **Request/response data contracts and fault types** — the typed bodies (`GetArtists`, `CreateArtist`, `NotFoundFault`, etc.)
4. **Artist domain model** — the `Artist` complex type and `ArrayOfArtist` collection
5. **System types** — `DateOnly` and `DayOfWeek` (.NET value types)
6. **Serialization arrays** — empty import namespace (infrastructure)

### Messages (`<wsdl:message>`)

Each operation has two or three messages — one going in, one coming out, and optionally one for each error type:

```
IArtistService_CreateArtist_InputMessage                      ← the request
IArtistService_CreateArtist_OutputMessage                     ← the response
IArtistService_CreateArtist_ValidationFaultFault_FaultMessage ← validation error
IArtistService_CreateArtist_ConflictFaultFault_FaultMessage   ← conflict error
```

A message just says "this envelope contains this element from the schema". It doesn't define the shape itself — that lives in the inline XSD.

### Port Type (`<wsdl:portType>`)

This is the **abstract definition** of the service — it lists every operation and wires up which messages belong to it:

```xml
<wsdl:operation name="CreateArtist">
    <wsdl:input  ... message="...InputMessage" />
    <wsdl:output ... message="...OutputMessage" />
    <wsdl:fault  ... message="...FaultMessage" />
</wsdl:operation>
```

Think of it like an interface in code — it says what exists, but not how it's transported.

### Binding (`<wsdl:binding>`)

This is the **concrete implementation** of the port type. It says: use SOAP, over HTTP, in document-literal style. Each operation is repeated here with transport details (`<soap:body use="literal" />`). This is mostly boilerplate for a standard BasicHttpBinding service.

### Service (`<wsdl:service>`)

The physical address of the service:

```xml
<soap:address location="http://localhost:5292/ArtistService.asmx" />
```

Update this URL if the service is deployed to a real server.

### Operations Overview

The service manages a single resource — **Artist** — with full CRUD operations:

| Operation | Input | Output | Description |
|---|---|---|---|
| `ListArtists` | `GetArtists` (empty) | `ArrayOfArtist` | Retrieve all artists |
| `GetArtistById` | `GetArtistById` (ArtistId) | `Artist` | Retrieve a single artist by ID |
| `CreateArtist` | `CreateArtist` (FirstName, LastName, Gender, DateOfBirth) | `int` (new ID) | Create a new artist |
| `UpdateArtist` | `UpdateArtist` (ArtistId, FirstName, LastName, Gender, DateOfBirth?) | `Artist` | Update an existing artist |
| `DeleteArtist` | `DeleteArtist` (ArtistId) | `Artist` | Delete an artist, returns the deleted record |

### Fault Types

Three error types are defined. Any operation can return one of these instead of a normal response:

| Fault | Meaning | Used by |
|---|---|---|
| `NotFoundFault` | The requested artist does not exist | `ListArtists`, `GetArtistById`, `UpdateArtist`, `DeleteArtist` |
| `ValidationFault` | The input data failed validation (e.g. missing required field) | `CreateArtist`, `UpdateArtist` |
| `ConflictFault` | The operation would create a duplicate record | `CreateArtist` |

Each fault carries an `ErrorCode` and an `ErrorMessage` string.

---

## Inline Schema Details

### Two kinds of definitions

Each operation in the schema has two kinds of definitions, which serve different purposes:

**1. Message wrapper elements** — these are the outer envelopes that SOAP uses to wrap a call. They match 1:1 with the messages in the WSDL:

```xml
<xs:element name="CreateArtist">
    <xs:complexType>
        <xs:sequence>
            <xs:element name="request" type="tns:CreateArtist" />
        </xs:sequence>
    </xs:complexType>
</xs:element>
```

**2. Domain complex types** — these are the actual data structures. They define the fields:

```xml
<xs:complexType name="CreateArtist">
    <xs:sequence>
        <xs:element name="FirstName"    type="xs:string" />
        <xs:element name="LastName"     type="xs:string" />
        <xs:element name="Gender"       type="xs:string" />
        <xs:element name="DateOfBirth"  type="q1:DateOnly" />
    </xs:sequence>
</xs:complexType>
```

The wrapper element references the complex type. This is the standard WCF/CoreWCF pattern.

### Data types used

| XSD type | Meaning |
|---|---|
| `xs:string` | Text |
| `xs:int` | Whole number |
| `DateOnly` | Date without time (custom complex type mapping to .NET `System.DateOnly`) |

### `nillable="true"` and `minOccurs`

Two attributes control whether a field is required:

- **`minOccurs="1"`** — the field is required in the request
- **`minOccurs="0"`** — the field is optional (e.g. `DateOfBirth` on `UpdateArtist`)
- **`nillable="true"`** — the field can be explicitly sent as null (for strings)

### Entity shape

**Artist**

| Field | Type | Notes |
|---|---|---|
| `Id` | int | Primary key, assigned by the server |
| `FirstName` | string | Artist's first name |
| `LastName` | string | Artist's last name |
| `Gender` | string | Artist's gender |
| `DateOfBirth` | DateOnly | Date of birth (Year, Month, Day) |

**Fault types** (`NotFoundFault`, `ValidationFault`, `ConflictFault`)

| Field | Type |
|---|---|
| `ErrorCode` | string |
| `ErrorMessage` | string |

### Array type

`ListArtists` returns a collection defined as a wrapper type containing an unbounded sequence:

```xml
<xs:complexType name="ArrayOfArtist">
    <xs:sequence>
        <xs:element maxOccurs="unbounded" name="Artist" type="tns:Artist" />
    </xs:sequence>
</xs:complexType>
```

---

## File structure overview

```
IArtistService.wsdl
│
├── <wsdl:types>
│     ├── Schema 1: Operation wrapper elements (CreateArtist, ListArtistsResponse, ...)
│     ├── Schema 2: Serialization primitives (guid, duration, char)
│     ├── Schema 3: Data contracts & faults (GetArtists, CreateArtist, NotFoundFault, ...)
│     ├── Schema 4: Domain model (Artist, ArrayOfArtist)
│     ├── Schema 5: System types (DateOnly, DayOfWeek)
│     └── Schema 6: Serialization arrays (empty)
│
├── <wsdl:message>  (references wrapper elements from the inline schemas)
├── <wsdl:portType> (references messages)
├── <wsdl:binding>  (references portType, adds SOAP transport details)
└── <wsdl:service>  (the URL: http://localhost:5292/ArtistService.asmx)
```

---

## Quick reference

### Calling an operation

Every SOAP call follows the same pattern:
1. Wrap your input in the matching request element (defined in the inline XSD)
2. POST it to the service URL as a SOAP envelope
3. The response will contain the matching response element, or a fault

Example — getting an artist by ID:
- Send: `GetArtistById` element containing a `GetArtistById` complex type with `ArtistId`
- Receive: `GetArtistByIdResponse` containing an `Artist`, or a `NotFoundFault`
