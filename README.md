# Azure Table Operations
Execute a table operation.

## Usage
```
delete
    -c, --connection-string     A connection string includes the authentication information required for your application to access data in an Azure Table account at runtime.
    -t, --table                 The name of the table with which this client instance will interact.
    -q, --query                 Returns only entities that satisfy the specified OData filter.
    -b, --batch                 Submits as a batch transaction.
    --page-size                 The maximum number of results to return per page.
```