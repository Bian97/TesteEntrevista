# Controle de Vendas

Teste prático: sistema web para cadastro de clientes, produtos e lançamento de vendas.
Backend em .NET 10 (ASP.NET Core + Dapper), frontend em React (Vite) e SQL Server.

## Pré-requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js](https://nodejs.org/) 20.19 ou superior (ou 22.12+), que já inclui o npm
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) em execução
- Visual Studio 2026 (opcional): só para rodar com F5 e executar os testes pela interface

O SQL Server não precisa ser instalado, ele sobe em um container.

## Como rodar

**Backend e banco (Docker)**

1. Copie `Backend/.env.example` para `Backend/.env` e defina a senha do SQL Server.
2. Abra `Backend/Vendas.sln` no Visual Studio e dê F5 com o projeto `docker-compose`.
   Sem o Visual Studio: `docker compose up -d --build` dentro de `Backend/`.

O banco `VendasDb` é criado sozinho na primeira subida, com as tabelas e alguns dados de exemplo.

- API: http://localhost:8080 (abre automaticamente)
- SQL Server: `localhost,1434`, usuário `sa`

**Frontend**

Comandos de Terminal (Feito no Visual Studio Code):
```
cd ../../Frontend (Caminho completo até a pasta do Frontend)
npm install
npm run dev
```

Abre em http://localhost:5173. O Vite repassa o `/api` para a API na porta 8080.

**Testes**

Testes no Visual Studio:
```
cd ../../Backend (Caminho completo até a pasta do Backend)
dotnet test
```
OU
- Botão direito em "Vendas.Tests
- Executar Testes

## Banco de dados

O script completo está em `Database/script-criacao.sql` e pode ser rodado direto no SSMS.
É o mesmo que a API aplica nas Migrations, então não precisa rodar os dois.

## Estrutura

Segui a mesma organização que uso nos meus projetos: cada camada é um projeto separado dentro da solution (`Backend/Vendas.sln`), e no Visual Studio os projetos ficam agrupados em pastas por responsabilidade.

**Domain**
- `Vendas.Model`: entidades e regras de negócio (validações ficam aqui)
- `Vendas.Abstractions`: interfaces de repositórios e services

**Business**
- `Vendas.Business`: services, que orquestram as regras e consultam os repositórios

**Dal**
- `Vendas.DB`: repositórios com Dapper, SQL em arquivos `.sql` embutidos
- `Vendas.DB.Migrations`: criação do banco e das tabelas (DbUp)

**Infra**
- `Vendas.Core`: injeção de dependência
- `Vendas.Common`: exceções e utilitários (CPF)

**View**
- `Vendas.API`: controllers, ViewModels e tratamento de erros

**Tests**
- `Vendas.Tests`: testes das regras de negócio

O projeto `docker-compose` fica na raiz da solution, e é ele que sobe a API e o SQL Server no F5.

## Observações

- **Validação no Model.** Regras que dependem só dos dados (CPF, tamanhos, quantidade, itens da venda) ficam nas entidades.
  Regras que precisam do banco (CPF ou nome duplicado, cliente existente) ficam nos services.
  Assim todos os erros voltam de uma vez só para o usuário.
- **Valor do item gravado na venda.** O preço do produto pode mudar depois sem mexer em vendas antigas.
- **Total recalculado no servidor.** Não confio no total que vem do frontend.
- **Dapper com SQL em arquivo.** Fica fácil de ler e revisar, e tudo parametrizado.
- **Sem AutoMapper.** O mapeamento é pequeno, preferi fazer na mão.