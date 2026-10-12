IF DB_ID('VendasDb') IS NULL CREATE DATABASE VendasDb;
GO

USE VendasDb;
GO

IF OBJECT_ID('Cliente', 'U') IS NULL
BEGIN
    CREATE TABLE Cliente (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Cliente PRIMARY KEY,
        Cpf CHAR(11) NOT NULL,
        Nome NVARCHAR(150) NOT NULL,
        DataNascimento DATE NULL,
        Email NVARCHAR(200) NULL,
        Telefone NVARCHAR(20) NULL,
        DataCadastro DATETIME2(0) NOT NULL CONSTRAINT DF_Cliente_DataCadastro DEFAULT SYSDATETIME()
    );
    CREATE UNIQUE INDEX UX_Cliente_Cpf ON Cliente (Cpf);
    CREATE INDEX IX_Cliente_Nome ON Cliente (Nome);
END
GO

IF OBJECT_ID('Produto', 'U') IS NULL
BEGIN
    CREATE TABLE Produto (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Produto PRIMARY KEY,
        Nome NVARCHAR(150) NOT NULL,
        ValorUnitario DECIMAL(18,2) NOT NULL CONSTRAINT CK_Produto_Valor CHECK (ValorUnitario > 0),
        DataCadastro DATETIME2(0) NOT NULL CONSTRAINT DF_Produto_DataCadastro DEFAULT SYSDATETIME()
    );
    CREATE UNIQUE INDEX UX_Produto_Nome ON Produto (Nome);
END
GO

IF OBJECT_ID('Venda', 'U') IS NULL
BEGIN
    CREATE TABLE Venda (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Venda PRIMARY KEY,
        ClienteId INT NOT NULL CONSTRAINT FK_Venda_Cliente REFERENCES Cliente (Id),
        DataVenda DATE NOT NULL,
        ValorTotal DECIMAL(18,2) NOT NULL,
        DataCadastro DATETIME2(0) NOT NULL CONSTRAINT DF_Venda_DataCadastro DEFAULT SYSDATETIME()
    );
    CREATE INDEX IX_Venda_ClienteId ON Venda (ClienteId);
    CREATE INDEX IX_Venda_DataVenda ON Venda (DataVenda);
END
GO

IF OBJECT_ID('ItemVenda', 'U') IS NULL
BEGIN
    CREATE TABLE ItemVenda (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ItemVenda PRIMARY KEY,
        VendaId INT NOT NULL CONSTRAINT FK_ItemVenda_Venda REFERENCES Venda (Id) ON DELETE CASCADE,
        ProdutoId INT NOT NULL CONSTRAINT FK_ItemVenda_Produto REFERENCES Produto (Id),
        Quantidade INT NOT NULL CONSTRAINT CK_ItemVenda_Qtd CHECK (Quantidade > 0),
        ValorUnitario DECIMAL(18,2) NOT NULL CONSTRAINT CK_ItemVenda_Valor CHECK (ValorUnitario > 0)
    );
    CREATE INDEX IX_ItemVenda_VendaId ON ItemVenda (VendaId);
    CREATE INDEX IX_ItemVenda_ProdutoId ON ItemVenda (ProdutoId);
END
GO

IF NOT EXISTS (SELECT 1 FROM Cliente WHERE Cpf = '52998224725')
    INSERT INTO Cliente (Cpf, Nome, DataNascimento, Email, Telefone)
    VALUES ('52998224725', N'Maria Silva', NULL, N'maria.silva@exemplo.com', N'11999990001');
GO

IF NOT EXISTS (SELECT 1 FROM Cliente WHERE Cpf = '11144477735')
    INSERT INTO Cliente (Cpf, Nome, DataNascimento, Email, Telefone)
    VALUES ('11144477735', N'João Souza', NULL, NULL, NULL);
GO

IF NOT EXISTS (SELECT 1 FROM Cliente WHERE Cpf = '39053344705')
    INSERT INTO Cliente (Cpf, Nome, DataNascimento, Email, Telefone)
    VALUES ('39053344705', N'Ana Costa', NULL, N'ana.costa@exemplo.com', N'21988880002');
GO

IF NOT EXISTS (SELECT 1 FROM Produto WHERE Nome = N'Teclado Mecânico')
    INSERT INTO Produto (Nome, ValorUnitario) VALUES (N'Teclado Mecânico', 249.90);
GO

IF NOT EXISTS (SELECT 1 FROM Produto WHERE Nome = N'Mouse Sem Fio')
    INSERT INTO Produto (Nome, ValorUnitario) VALUES (N'Mouse Sem Fio', 89.90);
GO

IF NOT EXISTS (SELECT 1 FROM Produto WHERE Nome = N'Monitor 24"')
    INSERT INTO Produto (Nome, ValorUnitario) VALUES (N'Monitor 24"', 899.00);
GO

IF NOT EXISTS (SELECT 1 FROM Produto WHERE Nome = N'Headset USB')
    INSERT INTO Produto (Nome, ValorUnitario) VALUES (N'Headset USB', 159.90);
GO

IF NOT EXISTS (SELECT 1 FROM Produto WHERE Nome = N'Webcam Full HD')
    INSERT INTO Produto (Nome, ValorUnitario) VALUES (N'Webcam Full HD', 199.90);
GO

IF NOT EXISTS (SELECT 1 FROM Produto WHERE Nome = N'Cabo HDMI 2m')
    INSERT INTO Produto (Nome, ValorUnitario) VALUES (N'Cabo HDMI 2m', 29.90);
GO
