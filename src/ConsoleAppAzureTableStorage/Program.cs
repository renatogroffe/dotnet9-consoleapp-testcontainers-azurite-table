using Azure.Data.Tables;
using Bogus.DataSets;
using ConsoleAppAzureTableStorage.Data;
using ConsoleAppAzureTableStorage.Utils;
using Serilog;
using System.Text.Json;
using Testcontainers.Azurite;

var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("testcontainers-azuretablestorage.tmp")
    .CreateLogger();
logger.Information("***** Iniciando testes com Testcontainers + Azure Table Storage *****");

CommandLineHelper.Execute("docker container ls",
    "Containers antes da execucao do Testcontainers...");

var azuriteContainer = new AzuriteBuilder()
  .WithImage("mcr.microsoft.com/azure-storage/azurite:3.34.0")
  .Build();
await azuriteContainer.StartAsync();

CommandLineHelper.Execute("docker container ls",
    "Containers apos execucao do Testcontainers...");

var connectionStringTableStorage = azuriteContainer.GetConnectionString();
const string tableName = "Contatos";
logger.Information($"Connection String = {connectionStringTableStorage}");
logger.Information($"Table Endpoint = {azuriteContainer.GetTableEndpoint()}");
logger.Information($"Table a ser utilizada nos testes = {tableName}");

var tableClient = new TableClient(connectionStringTableStorage, tableName);
tableClient.CreateIfNotExists();
logger.Information($"Table {tableName} criada com sucesso!");

const int maxContatos = 10;
var addressDataSet = new Address("pt_BR");
var namesDataSet = new Name("pt_BR");
var companyDataSet = new Company("pt_BR");
for (int i = 1; i <= maxContatos; i++)
{
    var contato = new ContatoEntity
    {
        PartitionKey = addressDataSet.StateAbbr(),
        RowKey = Guid.NewGuid().ToString(),
        Nome = namesDataSet.FullName(),
        Empresa = companyDataSet.CompanyName()
    };
    tableClient.AddEntity(contato);
    
    logger.Information(
        $"Registro {i}/{maxContatos}: {JsonSerializer.Serialize(contato)}");
    logger.Information("Pressione ENTER para continuar...");
    Console.ReadLine();
}

logger.Information("Testes concluidos com sucesso!");
logger.Information("Pressione ENTER para encerrar a aplicacao...");
Console.ReadLine();