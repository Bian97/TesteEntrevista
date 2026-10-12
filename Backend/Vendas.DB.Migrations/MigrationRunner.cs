using System.Reflection;
using System.Text;
using DbUp;
using Microsoft.Data.SqlClient;

namespace Vendas.DB.Migrations;

public static class MigrationRunner
{
    private const int TentativasConexao = 10;
    private static readonly TimeSpan EsperaEntreTentativas = TimeSpan.FromSeconds(3);

    public static void RunMigrations(string connectionString)
    {
        AguardarBanco(connectionString);

        EnsureDatabase.For.SqlDatabase(connectionString);

        var upgrader = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), Encoding.UTF8)
            .WithTransaction()
            .LogToConsole()
            .Build();

        var resultado = upgrader.PerformUpgrade();
        if (!resultado.Successful)
        {
            throw resultado.Error;
        }
    }

    private static void AguardarBanco(string connectionString)
    {
        var builder = new SqlConnectionStringBuilder(connectionString) { InitialCatalog = "master" };
        connectionString = builder.ConnectionString;

        for (var tentativa = 1; ; tentativa++)
        {
            try
            {
                using var conexao = new SqlConnection(connectionString);
                conexao.Open();
                return;
            }
            catch (SqlException) when (tentativa < TentativasConexao)
            {
                Thread.Sleep(EsperaEntreTentativas);
            }
        }
    }
}
