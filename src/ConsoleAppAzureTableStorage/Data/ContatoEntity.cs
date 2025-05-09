using Azure;
using Azure.Data.Tables;

namespace ConsoleAppAzureTableStorage.Data;

public class ContatoEntity : ITableEntity
{
    public string? PartitionKey { get; set; }
    public string? RowKey { get; set; }
    public string? Nome { get; set; }
    public string? Empresa { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}