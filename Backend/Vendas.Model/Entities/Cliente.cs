using System.Net.Mail;
using CpfUtil = Vendas.Common.Utilitarios.Cpf;

namespace Vendas.Model.Entities;

public class Cliente
{
    public const int NomeTamanhoMaximo = 150;
    public const int EmailTamanhoMaximo = 200;

    public int Id { get; set; }
    public string Cpf { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public DateTime? DataNascimento { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public DateTime DataCadastro { get; set; }

    public void Normalizar()
    {
        Nome = (Nome ?? string.Empty).Trim();
        Cpf = CpfUtil.Normalizar(Cpf);
        Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim().ToLowerInvariant();
        Telefone = string.IsNullOrWhiteSpace(Telefone)
            ? null
            : new string(Telefone.Where(char.IsAsciiDigit).ToArray());
    }

    public IReadOnlyList<string> Validar()
    {
        var erros = new List<string>();

        if (string.IsNullOrEmpty(Nome))
        {
            erros.Add("Nome é obrigatório.");
        }
        else if (Nome.Length > NomeTamanhoMaximo)
        {
            erros.Add($"Nome deve ter no máximo {NomeTamanhoMaximo} caracteres.");
        }

        if (string.IsNullOrEmpty(Cpf))
        {
            erros.Add("CPF é obrigatório.");
        }
        else if (!CpfUtil.EhValido(Cpf))
        {
            erros.Add("CPF inválido.");
        }

        if (Email is not null)
        {
            if (Email.Length > EmailTamanhoMaximo)
            {
                erros.Add($"E-mail deve ter no máximo {EmailTamanhoMaximo} caracteres.");
            }
            else if (!EmailValido(Email))
            {
                erros.Add("E-mail inválido.");
            }
        }

        if (Telefone is not null && Telefone.Length is not (10 or 11))
        {
            erros.Add("Telefone deve ter 10 ou 11 dígitos.");
        }

        if (DataNascimento is { } nascimento)
        {
            if (nascimento.Date >= DateTime.Today)
            {
                erros.Add("A data de nascimento deve ser anterior à data de hoje.");
            }

            if (nascimento < new DateTime(1900, 1, 1))
            {
                erros.Add("Data de nascimento deve ser maior ou igual a 01/01/1900.");
            }
        }

        return erros;
    }

    private static bool EmailValido(string email)
    {
        try
        {
            return new MailAddress(email).Address == email;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
