# Azure Table Operations
Execute a table operation.

## Usage

### Delete entities
```
ato delete -c <connection-string> -t <table> -q <query> [-b] [--page-size <page-size>]

    -c, --connection-string <connection-string>    The connection string to the storage account.
    -t, --table <table>                            The table name.
    -q, --query <query>                            The OData query to filter the entities.
    -b, --batch                                    Delete entities in batches of <page-size>.
    --page-size <page-size>                        The maximum number of entities that will be returned per page request. Default is 100.
    --help                                         Show help.
```

#### Example

![image](https://user-images.githubusercontent.com/72770006/222926803-c0d5bbcb-2e83-457e-a992-11cbb7832899.png)
