using Azure;
using Azure.Data.Tables;
using System.Diagnostics;

switch (args[0])
{
    case "delete":
    {
        string? connectionString = null;
        string? tableName = null;
        string? query = null;
        var batch = false;
        var pageSize = 100;

        for (var i = 1; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "-c":
                case "--connection-string":
                    connectionString = args[++i];
                    break;

                case "-t":
                case "--table":
                    tableName = args[++i];
                    break;

                case "-q":
                case "--query":
                    query = args[++i];
                    break;

                case "-b":
                case "--batch":
                    batch = true;
                    break;
                
                case "--page-size":
                    pageSize = int.Parse(args[++i]);
                    break;
                
                case "--help":
                    Console.WriteLine("""

                        delete -c <connection-string> -t <table> -q <query> [-b] [--page-size <page-size>]

                            -c, --connection-string <connection-string>    The connection string to the storage account.
                            -t, --table <table>                            The table name.
                            -q, --query <query>                            The query to filter the entities.
                            -b, --batch                                    Delete entities in batches of <page-size>. Default is 100.
                            --page-size <page-size>                        The page size.

                    """);
                    break;

                default:
                    Console.WriteLine($"Unknown argument '{args[i]}'.");
                    return;
            }
        }

        if (string.IsNullOrEmpty(connectionString))
        {
            Console.WriteLine("Connection string is not set.");
            return;
        }

        if (string.IsNullOrEmpty(tableName))
        {
            Console.WriteLine("Table name is not set.");
            return;
        }

        if (string.IsNullOrEmpty(query))
        {
            Console.WriteLine("Query is not set.");
            return;
        }

        await DeleteAsync(connectionString, tableName, query, batch, pageSize);

        break;
    }
    default:
        Console.WriteLine("Unknown command.");
        break;
}

async Task DeleteAsync(string connectionString, string tableName, string query, bool batch, int pageSize)
{
    var tableClient = new TableClient(connectionString, tableName);

    AsyncPageable<TableEntity>? entities =
        tableClient.QueryAsync<TableEntity>(query, select: new[] { "PartitionKey", "RowKey" }, maxPerPage: pageSize);

    Debug.WriteLine($"Deleting all entities with filter '{query}' from table '{tableName}'.");

    var count = 0;

    if (batch)
        await foreach (Page<TableEntity> page in entities.AsPages())
        foreach (TableEntity[] chunk in page.Values.Chunk(100))
        {
            await tableClient.SubmitTransactionAsync(chunk.Select(entity =>
                new TableTransactionAction(TableTransactionActionType.Delete, entity)));
            count += chunk.Length;

            foreach (TableEntity entity in chunk)
            {
                Console.WriteLine(
                    $"Deleted entity with partition key '{entity.PartitionKey}' and row key '{entity.RowKey}'.");
            }
        }
    else
        await foreach (TableEntity entity in entities)
        {
            await tableClient.DeleteEntityAsync(entity.PartitionKey, entity.RowKey);
            count++;

            Console.WriteLine(
                $"Deleted entity with partition key '{entity.PartitionKey}' and row key '{entity.RowKey}'.");
        }

    Console.WriteLine($"Deleted {count} entities.");
}