namespace Vendas.Model.Configs;

public class BancoConfig
{
    public string ConnectionString { get; set; } = string.Empty;
    public int TimeOut { get; set; } = 30;
}
