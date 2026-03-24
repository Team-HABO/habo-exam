# ILibraryService — SOAP Service Files

This folder contains two files that together define a SOAP web service for a library system. One file describes **what the service does**, the other describes **what the data looks like**.

---

## Reading the XML — Namespaces and Prefixes

Before diving into the files, it helps to understand the `prefix:something` syntax you'll see everywhere. It can look intimidating at first, but the rule is simple:

**`prefix:tagname` means "this tag belongs to that prefix's namespace".**

A namespace is just a URL used as a unique identifier — it guarantees that a tag called `element` in one vocabulary doesn't get confused with a tag called `element` in another. The URL doesn't have to point to anything real; it's just a name.

The prefixes are declared at the top of the file and act as shorthand aliases:

```xml
xmlns:xs="http://www.w3.org/2001/XMLSchema"
xmlns:wsdl="http://schemas.xmlsoap.org/wsdl/"
xmlns:tns="http://example.com/library/wsdl"
```

After that declaration, `xs:` means "from the XML Schema standard", `wsdl:` means "from the WSDL standard", and `tns:` means "from this service's own namespace" (tns = **t**arget **n**ame**s**pace).

### Prefixes used in these files

| Prefix | Stands for | What it marks |
|---|---|---|
| `xs:` | XML Schema (`http://www.w3.org/2001/XMLSchema`) | Standard schema building blocks — types, elements, sequences |
| `wsdl:` | WSDL vocabulary (`http://schemas.xmlsoap.org/wsdl/`) | Service structure — operations, messages, bindings |
| `tns:` | This service's namespace (`http://example.com/library/wsdl`) | Things defined in this project — `tns:Tbook`, `tns:CreateBook`, etc. |
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
<wsdl:input wsam:Action="http://example.com/library/wsdl/ILibraryService/CreateBook" ... />
```

Here `wsam:Action` is an attribute from the WS-Addressing namespace being added to a `wsdl:input` tag. The prefix tells you which vocabulary that attribute belongs to.

Unprefixed attributes (like `name="CreateBook"` or `type="xs:int"`) belong to the element's own vocabulary by default.

---

## The Two Files

| File | Role |
|---|---|
| `ILibraryService.wsdl` | The service contract — operations, messages, endpoints |
| `ILibraryService.xsd` | The data shapes — what goes in and comes out of each operation |

The WSDL imports the XSD, so both files must stay in the same folder.

---

## `ILibraryService.wsdl`

The WSDL (Web Service Description Language) file is the **front door** of the service. It answers three questions:

- **What can I call?** — the operations (e.g. `CreateBook`, `DeleteAuthor`)
- **What do I send and receive?** — the messages for each operation
- **Where do I call it?** — the endpoint URL

### Structure

A WSDL is made up of four sections, always in this order:

```
<wsdl:types>       →  "here are the data types used" (delegates to the XSD)
<wsdl:message>     →  "here are the envelopes for each call"
<wsdl:portType>    →  "here are the operations and what they accept/return"
<wsdl:binding>     →  "here is how to physically send the messages (SOAP over HTTP)"
<wsdl:service>     →  "here is the URL to call"
```

### Types (`<wsdl:types>`)

Instead of defining data inline, this WSDL imports the XSD:

```xml
<xs:import namespace="http://example.com/library/wsdl"
           schemaLocation="ILibraryService.xsd" />
```

The two small Microsoft schemas that follow it inline are standard boilerplate — they define serialization primitives like `guid`, `duration`, and `char` that .NET WCF services include automatically. You don't need to touch them.

### Messages (`<wsdl:message>`)

Each operation has two or three messages — one going in, one coming out, and optionally one for each error type:

```
ILibraryService_CreateBook_InputMessage        ← the request
ILibraryService_CreateBook_OutputMessage       ← the response
ILibraryService_CreateBook_ValidationFaultFault_FaultMessage  ← error
```

A message just says "this envelope contains this element from the XSD". It doesn't define the shape itself — that lives in the XSD.

### Port Type (`<wsdl:portType>`)

This is the **abstract definition** of the service — it lists every operation and wires up which messages belong to it:

```xml
<wsdl:operation name="CreateBook">
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
<soap:address location="http://localhost:5292/LibraryService.asmx" />
```

Update this URL if the service is deployed to a real server.

### Operations Overview

The service covers three resources — **Books**, **Authors**, and **Publishing Companies** — each with standard CRUD operations:

| Resource | Operations |
|---|---|
| Book | `CreateBook`, `GetBookById`, `UpdateBook`, `DeleteBook` |
| Author | `CreateAuthor`, `GetAuthorById`, `ListAuthors`, `UpdateAuthor`, `DeleteAuthor` |
| PublishingCompany | `CreatePublishingCompany`, `GetPublishingCompanyById`, `ListPublishingCompanies`, `UpdatePublishingCompany`, `DeletePublishingCompany` |

### Fault Types

Three error types are defined. Any operation can return one of these instead of a normal response:

| Fault | Meaning |
|---|---|
| `ValidationFault` | The input data failed validation (e.g. missing required field) |
| `NotFoundFault` | The requested record does not exist |
| `ConflictFault` | The operation would violate a constraint (e.g. deleting an author who still has books) |

Each fault carries an `ErrorCode` and an `ErrorMessage` string.

---

## `ILibraryService.xsd`

The XSD (XML Schema Definition) file defines the **shape of every piece of data** used by the service. If the WSDL is the contract, the XSD is the **data dictionary**.

### Why a separate file?

The original WSDL had all schema content embedded inline, which made it very long and hard to read. Moving it to a separate XSD keeps the WSDL focused on the service structure, and the XSD focused on data shapes. Tools like SoapUI and Visual Studio handle this split natively.

### Two kinds of definitions

Each entity in the XSD has two kinds of definitions, which serve different purposes:

**1. Message wrapper elements** — these are the outer envelopes that SOAP uses to wrap a call. They match 1:1 with the messages in the WSDL:

```xml
<xs:element name="CreateBook">
    <xs:complexType>
        <xs:sequence>
            <xs:element name="request" type="tns:CreateBook" />
        </xs:sequence>
    </xs:complexType>
</xs:element>
```

**2. Domain complex types** — these are the actual data structures. They define the fields:

```xml
<xs:complexType name="CreateBook">
    <xs:sequence>
        <xs:element name="CTitle"              type="xs:string" />
        <xs:element name="NAuthorId"           type="xs:int" />
        <xs:element name="NPublishingYear"     type="xs:int" />
        <xs:element name="NPublishingCompanyId" type="xs:int" />
    </xs:sequence>
</xs:complexType>
```

The wrapper element references the complex type. This is the standard WCF pattern.

### Data types used

All fields use basic XSD primitives:

| XSD type | Meaning |
|---|---|
| `xs:string` | Text |
| `xs:int` | Whole number |

### Field naming conventions

Fields follow a Hungarian notation prefix convention inherited from the database layer:

| Prefix | Meaning |
|---|---|
| `C` | Character/string (e.g. `CTitle`, `CName`) |
| `N` | Numeric/integer (e.g. `NBookId`, `NAuthorId`) |

### `nillable="true"` and `minOccurs`

Two attributes control whether a field is required:

- **`minOccurs="1"`** — the field is required in the request
- **`minOccurs="0"`** — the field is optional (typically used on update operations where you only send what you want to change)
- **`nillable="true"`** — the field can be explicitly sent as null (for strings, which can't be omitted the same way integers can)

### Entity shapes

**Book**

| Field | Type | Notes |
|---|---|---|
| `NBookId` | int | ID, assigned by the server |
| `CTitle` | string | Title of the book |
| `NAuthorId` | int | Foreign key to Author |
| `NPublishingYear` | int | Year of publication |
| `NPublishingCompanyId` | int | Foreign key to PublishingCompany |

**Author**

| Field | Type | Notes |
|---|---|---|
| `NAuthorId` | int | ID, assigned by the server |
| `CName` | string | First name |
| `CSurname` | string | Last name (optional) |

**PublishingCompany**

| Field | Type | Notes |
|---|---|---|
| `NPublishingCompanyId` | int | ID, assigned by the server |
| `CName` | string | Company name |

**Fault types** (`ValidationFault`, `NotFoundFault`, `ConflictFault`)

| Field | Type |
|---|---|
| `ErrorCode` | string |
| `ErrorMessage` | string |

### Array types

`ListAuthors` and `ListPublishingCompanies` return collections. These are defined as wrapper types containing an unbounded sequence:

```xml
<xs:complexType name="ArrayOfTauthor">
    <xs:sequence>
        <xs:element maxOccurs="unbounded" name="Tauthor" type="tns:Tauthor" />
    </xs:sequence>
</xs:complexType>
```

---

## How the two files relate

```
ILibraryService.wsdl
│
├── <wsdl:types>
│     └── xs:import ──────────────────────► ILibraryService.xsd
│                                                 ├── Message wrapper elements
│                                                 ├── Domain complex types (Tbook, Tauthor, ...)
│                                                 ├── Array types
│                                                 └── Fault types
│
├── <wsdl:message>  (references elements from XSD by name)
├── <wsdl:portType> (references messages)
├── <wsdl:binding>  (references portType, adds SOAP transport details)
└── <wsdl:service>  (the URL)
```

---

## Quick reference

### Calling an operation

Every SOAP call follows the same pattern:
1. Wrap your input in the matching request element (defined in the XSD)
2. POST it to the service URL as a SOAP envelope
3. The response will contain the matching response element, or a fault

Example — getting a book by ID:
- Send: `GetBookById` containing a `GetBookById` complex type with `NBookId`
- Receive: `GetBookByIdResponse` containing a `Tbook`, or a `NotFoundFault`

### Adding a new operation

1. Add the input/output complex types and wrapper elements to the XSD
2. Add the `<wsdl:message>` entries to the WSDL
3. Add the `<wsdl:operation>` to the `<wsdl:portType>`
4. Add the matching `<wsdl:operation>` to the `<wsdl:binding>`
