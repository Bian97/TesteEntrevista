using Vendas.Abstractions.Interfaces.Repositories;
using Vendas.Abstractions.Interfaces.Services;
using Vendas.Common.Utilitarios;
using Vendas.Model.Entities;

namespace Vendas.Business.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clientes;

    public ClienteService(IClienteRepository clientes)
    {
        _clientes = clientes;
    }

    public Task<IEnumerable<Cliente>> ListarAsync(string? busca) => _clientes.ListarAsync(busca);

    public async Task<Cliente> ObterAsync(int id)
    {
        return await _clientes.ObterPorIdAsync(id) ?? throw new NaoEncontradoException("Cliente não encontrado.");
    }

    public async Task<Cliente> CriarAsync(Cliente cliente)
    {
        cliente.Normalizar();
        var erros = cliente.Validar();
        if (erros.Count > 0)
        {
            throw new ValidacaoException(erros);
        }

        if (await _clientes.ExisteCpfAsync(cliente.Cpf, null))
        {
            throw new ConflitoException("Já existe um cliente cadastrado com este CPF.");
        }

        cliente.Id = await _clientes.CriarAsync(cliente);
        return cliente;
    }

    public async Task<Cliente> AtualizarAsync(Cliente cliente)
    {
        if (await _clientes.ObterPorIdAsync(cliente.Id) is null)
        {
            throw new NaoEncontradoException("Cliente não encontrado.");
        }

        cliente.Normalizar();
        var erros = cliente.Validar();
        if (erros.Count > 0)
        {
            throw new ValidacaoException(erros);
        }

        if (await _clientes.ExisteCpfAsync(cliente.Cpf, cliente.Id))
        {
            throw new ConflitoException("Já existe um cliente cadastrado com este CPF.");
        }

        await _clientes.AtualizarAsync(cliente);
        return cliente;
    }

    public async Task ExcluirAsync(int id)
    {
        if (await _clientes.ObterPorIdAsync(id) is null)
        {
            throw new NaoEncontradoException("Cliente não encontrado.");
        }

        if (await _clientes.PossuiVendasAsync(id))
        {
            throw new ConflitoException("Não é possível excluir um cliente que possui vendas.");
        }

        await _clientes.ExcluirAsync(id);
    }
}
