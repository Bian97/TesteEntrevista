namespace Vendas.Common.Utilitarios;

public static class Cpf
{
    public static string Normalizar(string? valor)
    {
        return new string((valor ?? string.Empty).Where(char.IsAsciiDigit).ToArray());
    }

    public static bool EhValido(string? valor)
    {
        var cpf = Normalizar(valor);
        if (cpf.Length != 11 || cpf.Distinct().Count() == 1)
        {
            return false;
        }

        var digitos = cpf.Select(c => c - '0').ToArray();

        var soma = 0;
        for (var i = 0; i < 9; i++)
        {
            soma += digitos[i] * (10 - i);
        }
        var primeiro = soma % 11 < 2 ? 0 : 11 - soma % 11;
        if (digitos[9] != primeiro)
        {
            return false;
        }

        soma = 0;
        for (var i = 0; i < 10; i++)
        {
            soma += digitos[i] * (11 - i);
        }
        var segundo = soma % 11 < 2 ? 0 : 11 - soma % 11;
        return digitos[10] == segundo;
    }

    public static string Formatar(string cpf)
    {
        var numeros = Normalizar(cpf);
        if (numeros.Length != 11)
        {
            return cpf;
        }

        return $"{numeros[..3]}.{numeros[3..6]}.{numeros[6..9]}-{numeros[9..]}";
    }
}
