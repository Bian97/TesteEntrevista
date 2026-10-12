INSERT INTO Cliente (Cpf, Nome, DataNascimento, Email, Telefone)
OUTPUT INSERTED.Id
VALUES (@Cpf, @Nome, @DataNascimento, @Email, @Telefone);
