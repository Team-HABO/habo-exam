# SOAP API Requirements

## XSD Complex Types

### Book

| Field               | Type                                                    |
| ------------------- | ------------------------------------------------------- |
| id                  | integer                                                 |
| title               | string                                                  |
| authorId            | integer — must reference an existing author             |
| publishingCompanyId | integer — must reference an existing publishing company |
| publishingYear      | integer — minimum 1900                                  |

### Author

| Field   | Type    |
| ------- | ------- |
| id      | integer |
| name    | string  |
| surname | string  |

### PublishingCompany

| Field | Type    |
| ----- | ------- |
| id    | integer |
| name  | string  |

---

## Faults

| Fault             | Description                                                            |
| ----------------- | ---------------------------------------------------------------------- |
| `NotFoundFault`   | The requested entity ID does not exist                                 |
| `ValidationFault` | Field values are missing, invalid, or referencing nonexistent entities |
| `ConflictFault`   | Attempt to delete an entity referenced in another entity               |

---

## Operations

### Book

| Operation     | Input                                                | Output                  | Faults                         |
| ------------- | ---------------------------------------------------- | ----------------------- | ------------------------------ |
| `CreateBook`  | title, authorId, publishingCompanyId, publishingYear | generated ID            | ValidationFault                |
| `GetBookById` | id                                                   | Book                    | NotFoundFault                  |
| `UpdateBook`  | id + updated book fields                             | success acknowledgement | ValidationFault, NotFoundFault |
| `DeleteBook`  | id                                                   | success acknowledgement | NotFoundFault                  |

### Author

| Operation       | Input                      | Output                  | Faults                         |
| --------------- | -------------------------- | ----------------------- | ------------------------------ |
| `CreateAuthor`  | name, surname              | generated ID            | ValidationFault                |
| `GetAuthorById` | id                         | Author                  | NotFoundFault                  |
| `ListAuthors`   | _(empty)_                  | array of Author         | —                              |
| `UpdateAuthor`  | id + updated author fields | success acknowledgement | ValidationFault, NotFoundFault |
| `DeleteAuthor`  | id                         | success acknowledgement | NotFoundFault, ConflictFault   |

### PublishingCompany

| Operation                  | Input     | Output                     | Faults                         |
| -------------------------- | --------- | -------------------------- | ------------------------------ |
| `CreatePublishingCompany`  | name      | generated ID               | ValidationFault                |
| `GetPublishingCompanyById` | id        | PublishingCompany          | NotFoundFault                  |
| `ListPublishingCompanies`  | _(empty)_ | array of PublishingCompany | —                              |
| `UpdatePublishingCompany`  | id + name | success acknowledgement    | ValidationFault, NotFoundFault |
| `DeletePublishingCompany`  | id        | success acknowledgement    | NotFoundFault, ConflictFault   |
