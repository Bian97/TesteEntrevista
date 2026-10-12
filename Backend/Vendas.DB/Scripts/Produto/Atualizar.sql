UPDATE Produto
SET Nome = @Nome,
    ValorUnitario = @ValorUnitario
WHERE Id = @Id;
