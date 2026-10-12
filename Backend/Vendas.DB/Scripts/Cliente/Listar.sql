SELECT TOP 100 Id, Cpf, Nome, DataNascimento, Email, Telefone, DataCadastro
FROM Cliente
WHERE (@Busca IS NULL OR Nome LIKE '%' + @Busca + '%' OR (@BuscaCpf <> '' AND Cpf LIKE '%' + @BuscaCpf + '%'))
ORDER BY Nome;
