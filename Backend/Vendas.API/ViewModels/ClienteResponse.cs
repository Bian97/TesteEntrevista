using CpfUtil = Vendas.Common.Utilitarios.Cpf;
using Vendas.Model.Entities;

namespace Vendas.API.ViewModels;

public class ClienteResponse
{
    public int Id { get; set; }
    public string Cpf { get; set; } = string.Empty;
    public string CpfFormatado { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public DateTime? DataNascimento { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public DateTime DataCadastro { get; set; }

    public static ClienteResponse De(Cliente cliente) => new()
    {
        Id = cliente.Id,
        Cpf = cliente.Cpf,
        CpfFormatado = CpfUtil.Formatar(cliente.Cpf),
        Nome = cliente.Nome,
        DataNascimento = cliente.DataNascimento,
        Email = cliente.Email,
        Telefone = cliente.Telefone,
        DataCadastro = cliente.DataCadastro
    };
}
