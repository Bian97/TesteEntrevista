namespace Vendas.Common.Utilitarios;

public class ValidacaoException : Exception
{
    public IReadOnlyList<string> Erros { get; }

    public ValidacaoException(string mensagem) : base(mensagem)
    {
        Erros = [mensagem];
    }

    public ValidacaoException(IEnumerable<string> erros) : base(string.Join("; ", erros))
    {
        Erros = erros.ToList();
    }
}
