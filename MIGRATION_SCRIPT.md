# Script de Migração - Estrutura Profissional

Este documento contém as instruções para completar a reorganização do projeto.

## Status: ✅ Estrutura de Pastas Criada

A estrutura de pastas foi criada com sucesso em:
```
c:\Aluracord\minimalApi\financial-management-app\
├── backend/
│   ├── src/
│   │   ├── Financial.Api/
│   │   ├── Financial.Domain/
│   │   ├── Financial.Application/
│   │   └── Financial.Infrastructure/
│   └── tests/
├── frontend/
├── database/
├── docs/
└── .github/
```

## Próximas Etapas

### 1. Criar Arquivos de Projeto C# (.csproj)

Os arquivos precisam ser criados em cada pasta do backend:

**Financial.Domain.csproj** - `backend/src/Financial.Domain/`
**Financial.Application.csproj** - `backend/src/Financial.Application/`
**Financial.Infrastructure.csproj** - `backend/src/Financial.Infrastructure/`
**Financial.Api.csproj** - `backend/src/Financial.Api/` (já copiado como Financial.Api.csproj)

### 2. Criar Solution File

Criar `Financial.sln` na raiz do backend

### 3. Atualizar Namespaces

Todos os arquivos C# precisam ter seus namespaces atualizados:

- `Dominio` → `Financial.Domain`
- `Aplicacao` → `Financial.Application`
- `Infraestrutura` → `Financial.Infrastructure`

### 4. Atualizar Referências de Projeto

Adicionar referências entre os projetos:

- Api → Application, Domain, Infrastructure
- Application → Domain, Infrastructure
- Infrastructure → Domain

### 5. Frontend (TypeScript)

O frontend já está em boa estrutura. Apenas reorganizar componentes por features.

### 6. Banco de Dados

Mover migrações EF Core para `database/migrations/`

### 7. Documentação

Criar arquivos de documentação em `docs/`

## Estrutura Recomendada Após Conclusão

```
financial-management-app/
├── backend/
│   ├── src/
│   │   ├── Financial.Api/
│   │   │   ├── Endpoints/
│   │   │   ├── Middleware/
│   │   │   ├── Program.cs
│   │   │   └── Financial.Api.csproj
│   │   ├── Financial.Domain/
│   │   │   ├── Entities/
│   │   │   ├── Exceptions/
│   │   │   ├── Interfaces/
│   │   │   └── Financial.Domain.csproj
│   │   ├── Financial.Application/
│   │   │   ├── DTOs/
│   │   │   ├── Services/
│   │   │   ├── Mappers/
│   │   │   └── Financial.Application.csproj
│   │   └── Financial.Infrastructure/
│   │       ├── Persistence/
│   │       ├── Repositories/
│   │       └── Financial.Infrastructure.csproj
│   ├── tests/
│   ├── Financial.sln
│   └── README.md
├── frontend/
│   ├── src/
│   │   ├── components/
│   │   ├── pages/
│   │   ├── services/
│   │   ├── hooks/
│   │   ├── context/
│   │   ├── types/
│   │   └── utils/
│   └── package.json
├── database/
│   ├── scripts/
│   ├── migrations/
│   ├── schemas/
│   └── backups/
├── docs/
│   ├── guides/
│   ├── api/
│   └── specs/
├── .github/workflows/
├── README.md
└── .gitignore
```

## Próximas Ações Necessárias

- [ ] Abrir a solução e criar os projetos .csproj
- [ ] Atualizar namespaces dos arquivos C#
- [ ] Adicionar referências entre projetos
- [ ] Testar compilação
- [ ] Atualizar imports do frontend se necessário
- [ ] Criar arquivo de documentação ARCHITECTURE.md
- [ ] Criar arquivo SETUP.md com instruções de desenvolvimento

