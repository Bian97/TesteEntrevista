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

CREATE TABLE Produto (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Produto PRIMARY KEY,
    Nome NVARCHAR(150) NOT NULL,
    ValorUnitario DECIMAL(18,2) NOT NULL CONSTRAINT CK_Produto_Valor CHECK (ValorUnitario > 0),
    DataCadastro DATETIME2(0) NOT NULL CONSTRAINT DF_Produto_DataCadastro DEFAULT SYSDATETIME()
);

CREATE UNIQUE INDEX UX_Produto_Nome ON Produto (Nome);

CREATE TABLE Venda (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Venda PRIMARY KEY,
    ClienteId INT NOT NULL CONSTRAINT FK_Venda_Cliente REFERENCES Cliente (Id),
    DataVenda DATE NOT NULL,
    ValorTotal DECIMAL(18,2) NOT NULL,
    DataCadastro DATETIME2(0) NOT NULL CONSTRAINT DF_Venda_DataCadastro DEFAULT SYSDATETIME()
);

CREATE INDEX IX_Venda_ClienteId ON Venda (ClienteId);
CREATE INDEX IX_Venda_DataVenda ON Venda (DataVenda);

CREATE TABLE ItemVenda (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ItemVenda PRIMARY KEY,
    VendaId INT NOT NULL CONSTRAINT FK_ItemVenda_Venda REFERENCES Venda (Id) ON DELETE CASCADE,
    ProdutoId INT NOT NULL CONSTRAINT FK_ItemVenda_Produto REFERENCES Produto (Id),
    Quantidade INT NOT NULL CONSTRAINT CK_ItemVenda_Qtd CHECK (Quantidade > 0),
    ValorUnitario DECIMAL(18,2) NOT NULL CONSTRAINT CK_ItemVenda_Valor CHECK (ValorUnitario > 0)
);

CREATE INDEX IX_ItemVenda_VendaId ON ItemVenda (VendaId);
CREATE INDEX IX_ItemVenda_ProdutoId ON ItemVenda (ProdutoId);
