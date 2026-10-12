UPDATE Cliente
SET Cpf = @Cpf,
    Nome = @Nome,
    DataNascimento = @DataNascimento,
    Email = @Email,
    Telefone = @Telefone
WHERE Id = @Id;
