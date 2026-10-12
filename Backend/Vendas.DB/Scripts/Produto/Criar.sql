INSERT INTO Produto (Nome, ValorUnitario)
OUTPUT INSERTED.Id
VALUES (@Nome, @ValorUnitario);
