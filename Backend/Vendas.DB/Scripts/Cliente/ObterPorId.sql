SELECT Id, Cpf, Nome, DataNascimento, Email, Telefone, DataCadastro
FROM Cliente
WHERE Id = @Id;
