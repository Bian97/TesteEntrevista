SELECT i.Id, i.VendaId, i.ProdutoId, p.Nome AS ProdutoNome, i.Quantidade, i.ValorUnitario
FROM ItemVenda i
INNER JOIN Produto p ON p.Id = i.ProdutoId
WHERE i.VendaId = @VendaId
ORDER BY i.Id;
