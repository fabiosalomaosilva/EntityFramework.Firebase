# 📘 Documentação Técnica - EF Core Firestore Provider

## 🧭 Visão Geral

Este projeto tem como objetivo criar um **Provider para Entity Framework Core** que permita a integração nativa com o **Firebase Firestore**, possibilitando que aplicações em .NET Core utilizem o **DbContext** para acessar e manipular dados armazenados no Firestore como se fosse um banco relacional.

O comportamento desejado deve **simular ao máximo a experiência de uso do EF Core com bancos como SQL Server**, inclusive suporte a:

* Injeção de dependência via DI
* Configuração fluente (`UseFirestore`)
* Mapeamento via `DbContext` para Collections
* Execução de operações CRUD via LINQ

---

## 🧱 Estrutura do Projeto

```
EntityFramework.Firebase/
│
├── src/
│   ├── EfCore.FirestoreProvider/
│   │   ├── Extensions/
│   │   ├── Infrastructure/
│   │   ├── Query/
│   │   ├── Migrations/
│   │   ├── Model/
│   │   └── EfCore.FirestoreProvider.csproj
│   └── EfCore.FirestoreProvider.Sample/
│       ├── Models/
│       ├── Data/
│       └── Program.cs
│
├── tests/
│   ├── EfCore.FirestoreProvider.Tests/
│   └── test.runsettings
│
├── docs/
│   └── API.md
│
├── EntityFramework.Firebase.sln
├── Directory.Build.props
├── .gitignore
├── .editorconfig
└── README.md
```

---

## 🔧 Configuração no Startup

```csharp
services.AddDbContext<AppDbContext>(options =>
    options.UseFirestore("firestore-project-id", "path/to/serviceAccount.json"));
```

---

## 🔌 Classe de Extensão - `UseFirestore`

```csharp
public static class FirestoreDbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder UseFirestore(
        this DbContextOptionsBuilder optionsBuilder,
        string projectId,
        string serviceAccountPath)
    {
        var extension = new FirestoreDbContextOptionsExtension
        {
            ProjectId = projectId,
            ServiceAccountPath = serviceAccountPath
        };

        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder)
            .AddOrUpdateExtension(extension);

        return optionsBuilder;
    }
}
```

---

## 🧬 Exemplo de `DbContext` com Collections

```csharp
public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Ticket> Tickets { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToCollection("users");
        modelBuilder.Entity<Ticket>().ToCollection("tickets");
    }
}
```

---

## 📄 Exemplo de Entidade

```csharp
public class User
{
    public string Id { get; set; } // Document ID
    public string FullName { get; set; }
    public DateTime DateOfBirth { get; set; }
}
```

---

## ✅ Funcionalidades CRUD Esperadas

| Operação           | Status |
| ------------------ | ------ |
| Create (Add)       | ✅      |
| Read (Find/LINQ)   | ✅      |
| Update             | ✅      |
| Delete             | ✅      |
| Query AsNoTracking | ✅      |
| AnyAsync / Count   | ✅      |

---

## 🔄 Mapeamento do Firestore com EF

* Cada **DbSet** mapeia para uma **Collection**
* Cada **entidade** mapeia para um **documento**
* A chave primária deve mapear para o **ID do documento** (`FirestoreDocumentId`)
* A navegação entre entidades pode ser feita por *referência de ID* (relacionamentos não são nativos do Firestore)

---

## 🔐 Autenticação

A autenticação com o Firestore será realizada via **Service Account Key JSON**, utilizando o SDK oficial do Firebase Admin:

```csharp
var builder = new FirestoreProviderBuilder()
    .UseProject("my-firebase-project")
    .UseCredentials("path/to/serviceAccount.json");
```

---

## 🔎 Consultas LINQ Suportadas

* `Where`, `Select`, `OrderBy`, `Take`, `Skip`
* `FirstOrDefaultAsync`, `AnyAsync`, `CountAsync`
* Suporte parcial a funções como `Contains`, `StartsWith`, `EndsWith` (via tradução para `FirestoreQuery`)

---

## 🧪 Testes Unitários

O projeto deve incluir cobertura de:

* Mapeamento de entidades
* Execução de operações CRUD
* Tradução de queries LINQ para `FirestoreQuery`
* Simulação de transações (emulação apenas, pois Firestore não suporta transações entre coleções diferentes)

---

## ⚙️ Etapas do Desenvolvimento

1. **Inicialização e Conexão**

   * Implementar leitura de credenciais
   * Instanciar `FirestoreDb` e injetar no provider

2. **Configuração do EF Core**

   * Criar extensão `UseFirestore`
   * Criar `DbContextOptionsExtension` personalizada

3. **Mapeamento e ConventionSet**

   * Lidar com annotations personalizadas para collections
   * Gerar identificadores de documentos

4. **Query Translation**

   * Implementar Expression Visitors para tradução de LINQ -> Firestore

5. **Command Execution**

   * Implementar execução de Add, Update, Delete, Query

6. **Testes Automatizados**

   * Utilizar `Firebase Emulator` para testes automatizados

7. **Documentação & Exemplo**

   * Criar README com exemplo de uso
   * Criar projeto de demonstração

---

## 🛠 Tecnologias e Pacotes

* **.NET 8**
* **EntityFrameworkCore v8.0**
* **Google.Cloud.Firestore**
* **Microsoft.Extensions.DependencyInjection**
* **xUnit + Moq (para testes)**

---

## 🔚 Considerações Finais

Esse Provider visa **simplificar o uso do Firebase Firestore** em aplicações modernas .NET, sem necessidade de instanciar o client manualmente ou reescrever lógica de acesso. Ideal para **projetos mobile com MAUI**, **sistemas serverless** ou **web APIs**.