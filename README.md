# EfCore.FirestoreProvider

[![NuGet Version](https://img.shields.io/nuget/v/EfCore.FirestoreProvider.svg)](https://www.nuget.org/packages/EfCore.FirestoreProvider/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/EfCore.FirestoreProvider.svg)](https://www.nuget.org/packages/EfCore.FirestoreProvider/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

**EfCore.FirestoreProvider** é um provedor do Entity Framework Core para Google Cloud Firestore, permitindo que desenvolvedores utilizem o EF Core com Firestore como backend de banco de dados NoSQL.

## 🚀 Características

- **Integração completa** com Entity Framework Core 8.0+
- **Suporte nativo** ao Google Cloud Firestore
- **Operações CRUD** simplificadas
- **Configuração flexível** com Service Account ou Application Default Credentials
- **Mapping automático** de entidades para collections do Firestore
- **Suporte a relacionamentos** e navegação de propriedades
- **Transações** e operações em lote

## 📦 Instalação

### Via NuGet Package Manager

```bash
dotnet add package EfCore.FirestoreProvider
```

### Via Package Manager Console

```powershell
Install-Package EfCore.FirestoreProvider
```

### Via PackageReference

```xml
<PackageReference Include="EfCore.FirestoreProvider" Version="1.0.0" />
```

## ⚡ Início Rápido

### 1. Configuração do DbContext

```csharp
using Microsoft.EntityFrameworkCore;
using EfCore.FirestoreProvider.Extensions;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Ticket> Tickets { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Mapear entidades para collections do Firestore
        modelBuilder.Entity<User>().HasAnnotation("Firestore:CollectionName", "users");
        modelBuilder.Entity<Ticket>().HasAnnotation("Firestore:CollectionName", "tickets");
    }
}
```

### 2. Definição de Modelos

```csharp
public class User
{
    public string Id { get; set; } // Document ID do Firestore
    public string FullName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; }
}

public class Ticket
{
    public string Id { get; set; } // Document ID do Firestore
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UserId { get; set; } // Referência ao usuário
}
```

### 3. Configuração da Injeção de Dependência

#### Usando Service Account (Recomendado para produção)

```csharp
using Microsoft.Extensions.DependencyInjection;
using EfCore.FirestoreProvider.Extensions;

public void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<AppDbContext>(options =>
        options.UseFirestore("seu-projeto-id", "caminho/para/serviceAccount.json"));
}
```

#### Usando Application Default Credentials

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<AppDbContext>(options =>
        options.UseFirestore("seu-projeto-id"));
}
```

### 4. Utilizando o DbContext

```csharp
public class UserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    // Criar usuário
    public async Task<User> CreateUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    // Buscar usuário por ID
    public async Task<User> GetUserByIdAsync(string id)
    {
        return await _context.Users.FindAsync(id);
    }

    // Listar todos os usuários
    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    // Atualizar usuário
    public async Task<User> UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    // Excluir usuário
    public async Task DeleteUserAsync(string id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
```

## 🔧 Configuração Avançada

### Configuração de Projeto ASP.NET Core

```csharp
// Program.cs (NET 6+)
using EfCore.FirestoreProvider.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configurar DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var projectId = builder.Configuration.GetConnectionString("FirestoreProjectId");
    var serviceAccountPath = builder.Configuration.GetConnectionString("FirestoreServiceAccount");
    
    options.UseFirestore(projectId, serviceAccountPath);
});

// Adicionar serviços
builder.Services.AddScoped<UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline...
```

### Configuração do appsettings.json

```json
{
  "ConnectionStrings": {
    "FirestoreProjectId": "seu-projeto-firestore",
    "FirestoreServiceAccount": "path/to/your/serviceAccount.json"
  }
}
```

### Variáveis de Ambiente (Para produção)

```bash
# Linux/Mac
export GOOGLE_APPLICATION_CREDENTIALS="path/to/your/serviceAccount.json"
export FIRESTORE_PROJECT_ID="seu-projeto-firestore"

# Windows
set GOOGLE_APPLICATION_CREDENTIALS=path\to\your\serviceAccount.json
set FIRESTORE_PROJECT_ID=seu-projeto-firestore
```

## 🏗️ Estrutura do Projeto

```
EfCore.FirestoreProvider/
├── Extensions/                 # Métodos de extensão para configuração
├── Infrastructure/            # Implementações de infraestrutura
├── Model/                     # Annotations e modelo
├── Query/                     # Provedores de consulta
└── Migrations/               # Suporte a migrações (futuro)
```

## 📚 Exemplos de Uso

### Consultas LINQ

```csharp
// Buscar usuários por nome
var users = await _context.Users
    .Where(u => u.FullName.Contains("João"))
    .ToListAsync();

// Buscar tickets de um usuário específico
var userTickets = await _context.Tickets
    .Where(t => t.UserId == "user123")
    .OrderByDescending(t => t.CreatedAt)
    .ToListAsync();

// Contar registros
var totalUsers = await _context.Users.CountAsync();
```

### Transações

```csharp
using (var transaction = await _context.Database.BeginTransactionAsync())
{
    try
    {
        var user = new User { FullName = "João Silva", Email = "joao@example.com" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var ticket = new Ticket 
        { 
            Title = "Novo Ticket", 
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow 
        };
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        await transaction.CommitAsync();
    }
    catch (Exception)
    {
        await transaction.RollbackAsync();
        throw;
    }
}
```

## 🔐 Autenticação e Segurança

### Service Account (Recomendado)

1. Crie um Service Account no Google Cloud Console
2. Baixe o arquivo JSON das credenciais
3. Configure o caminho no seu aplicativo

### Application Default Credentials

1. Instale o Google Cloud SDK
2. Execute `gcloud auth application-default login`
3. O provider utilizará automaticamente as credenciais

## 🧪 Testes

Para executar os testes:

```bash
# Executar todos os testes
dotnet test

# Executar testes específicos
dotnet test --filter "Category=Integration"

# Executar com o emulador do Firestore
dotnet test --settings test.runsettings
```

### Configuração do Emulador Firestore para Testes

```bash
# Instalar o emulador
npm install -g firebase-tools

# Iniciar o emulador
firebase emulators:start --only=firestore
```

## 📋 Requisitos

- **.NET 8.0** ou superior
- **Entity Framework Core 8.0** ou superior
- **Google Cloud Firestore** habilitado no projeto
- **Credenciais válidas** do Google Cloud

## 🤝 Contribuindo

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📝 Licença

Este projeto está licenciado sob a Licença MIT - veja o arquivo [LICENSE](LICENSE) para detalhes.

## 🔗 Links Úteis

- [Documentação do Google Cloud Firestore](https://cloud.google.com/firestore/docs)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Exemplos e Samples](./src/EfCore.FirestoreProvider.Sample/)
- [Documentação da API](./docs/API.md)

## 📞 Suporte

- **Issues**: [GitHub Issues](https://github.com/fabiosalomaosilva/EntityFramework.Firebase/issues)
- **Discussions**: [GitHub Discussions](https://github.com/fabiosalomaosilva/EntityFramework.Firebase/discussions)
- **Email**: fabio@example.com

---

⭐ **Se este projeto foi útil para você, considere dar uma estrela no GitHub!**
