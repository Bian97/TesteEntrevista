using Vendas.Model.Entities;

namespace Vendas.API.ViewModels;

public class ClienteRequest
{
    public string Cpf { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;

    public DateTime? DataNascimento { get; set; }

    public string? Email { get; set; }

    public string? Telefone { get; set; }

    public Cliente ParaEntidade() => new()
    {
        Cpf = Cpf,
        Nome = Nome,
        DataNascimento = DataNascimento,
        Email = Email,
        Telefone = Telefone
    };
}
