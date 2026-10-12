INSERT INTO Venda (ClienteId, DataVenda, ValorTotal)
OUTPUT INSERTED.Id
VALUES (@ClienteId, @DataVenda, @ValorTotal);
