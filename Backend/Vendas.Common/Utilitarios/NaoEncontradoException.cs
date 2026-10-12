namespace Vendas.Common.Utilitarios;

public class NaoEncontradoException : Exception
{
    public NaoEncontradoException(string mensagem) : base(mensagem)
    {
    }
}
