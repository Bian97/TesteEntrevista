SELECT TOP 100 Id, Nome, ValorUnitario, DataCadastro
FROM Produto
WHERE (@Busca IS NULL OR Nome LIKE '%' + @Busca + '%')
ORDER BY Nome;
