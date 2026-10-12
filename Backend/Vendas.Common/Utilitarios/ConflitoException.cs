namespace Vendas.Common.Utilitarios;

public class ConflitoException : Exception
{
    public ConflitoException(string mensagem) : base(mensagem)
    {
    }
}
