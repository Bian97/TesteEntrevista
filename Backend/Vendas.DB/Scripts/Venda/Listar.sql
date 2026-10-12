SELECT TOP 200 v.Id, v.ClienteId, c.Cpf AS ClienteCpf, c.Nome AS ClienteNome, v.DataVenda, v.ValorTotal, v.DataCadastro
FROM Venda v
INNER JOIN Cliente c ON c.Id = v.ClienteId
ORDER BY v.Id DESC;
