IF NOT EXISTS (SELECT 1 FROM Cliente WHERE Cpf = '52998224725')
    INSERT INTO Cliente (Cpf, Nome, DataNascimento, Email, Telefone)
    VALUES ('52998224725', N'Maria Silva', NULL, N'maria.silva@exemplo.com', N'11999990001');

IF NOT EXISTS (SELECT 1 FROM Cliente WHERE Cpf = '11144477735')
    INSERT INTO Cliente (Cpf, Nome, DataNascimento, Email, Telefone)
    VALUES ('11144477735', N'João Souza', NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM Cliente WHERE Cpf = '39053344705')
    INSERT INTO Cliente (Cpf, Nome, DataNascimento, Email, Telefone)
    VALUES ('39053344705', N'Ana Costa', NULL, N'ana.costa@exemplo.com', N'21988880002');

IF NOT EXISTS (SELECT 1 FROM Produto WHERE Nome = N'Teclado Mecânico')
    INSERT INTO Produto (Nome, ValorUnitario) VALUES (N'Teclado Mecânico', 249.90);

IF NOT EXISTS (SELECT 1 FROM Produto WHERE Nome = N'Mouse Sem Fio')
    INSERT INTO Produto (Nome, ValorUnitario) VALUES (N'Mouse Sem Fio', 89.90);

IF NOT EXISTS (SELECT 1 FROM Produto WHERE Nome = N'Monitor 24"')
    INSERT INTO Produto (Nome, ValorUnitario) VALUES (N'Monitor 24"', 899.00);

IF NOT EXISTS (SELECT 1 FROM Produto WHERE Nome = N'Headset USB')
    INSERT INTO Produto (Nome, ValorUnitario) VALUES (N'Headset USB', 159.90);

IF NOT EXISTS (SELECT 1 FROM Produto WHERE Nome = N'Webcam Full HD')
    INSERT INTO Produto (Nome, ValorUnitario) VALUES (N'Webcam Full HD', 199.90);

IF NOT EXISTS (SELECT 1 FROM Produto WHERE Nome = N'Cabo HDMI 2m')
    INSERT INTO Produto (Nome, ValorUnitario) VALUES (N'Cabo HDMI 2m', 29.90);
