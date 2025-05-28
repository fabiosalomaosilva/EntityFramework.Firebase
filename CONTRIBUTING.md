# Contributing to EfCore.FirestoreProvider

Obrigado por considerar contribuir para o EfCore.FirestoreProvider! Este documento fornece diretrizes para contribuir com o projeto.

## Códigos de Conduta

Este projeto segue o [Código de Conduta da Microsoft Open Source](https://opensource.microsoft.com/codeofconduct/). Para mais informações, consulte as [Perguntas Frequentes do Código de Conduta](https://opensource.microsoft.com/codeofconduct/faq/) ou entre em contato conosco em opencode@microsoft.com com qualquer pergunta ou comentário adicional.

## Como Contribuir

### Reportando Bugs

1. Certifique-se de que o bug não foi reportado anteriormente verificando os [issues existentes](https://github.com/fabiosalomaosilva/EntityFramework.Firebase/issues).
2. Se você não encontrar um issue existente, [crie um novo issue](https://github.com/fabiosalomaosilva/EntityFramework.Firebase/issues/new/choose) usando o template de bug report.
3. Forneça o máximo de detalhes possível, incluindo:
   - Versão do .NET
   - Versão do pacote
   - Sistema operacional
   - Código de exemplo que reproduz o problema
   - Stack trace completo

### Sugerindo Melhorias

1. Verifique se a melhoria não foi sugerida anteriormente nos [issues existentes](https://github.com/fabiosalomaosilva/EntityFramework.Firebase/issues).
2. [Crie um novo issue](https://github.com/fabiosalomaosilva/EntityFramework.Firebase/issues/new/choose) usando o template de feature request.
3. Explique claramente a melhoria e como ela beneficiaria os usuários.

### Pull Requests

1. Fork o repositório
2. Crie uma branch para sua funcionalidade: `git checkout -b feature/amazing-feature`
3. Faça commit das suas alterações: `git commit -m 'Add amazing feature'`
4. Push para a branch: `git push origin feature/amazing-feature`
5. Abra um Pull Request

#### Diretrizes para Pull Requests

- Use o template de Pull Request fornecido
- Certifique-se de que todos os testes passam
- Adicione testes para novas funcionalidades
- Mantenha o código consistente com o estilo existente
- Atualize a documentação se necessário
- Use mensagens de commit descritivas

## Configuração do Ambiente de Desenvolvimento

### Pré-requisitos

- .NET 8.0 SDK ou superior
- Visual Studio 2022 ou Visual Studio Code
- Git

### Configuração

1. Clone o repositório:
   ```bash
   git clone https://github.com/fabiosalomaosilva/EntityFramework.Firebase.git
   cd EntityFramework.Firebase
   ```

2. Restaure as dependências:
   ```bash
   dotnet restore
   ```

3. Compile o projeto:
   ```bash
   dotnet build
   ```

4. Execute os testes:
   ```bash
   dotnet test
   ```

### Estrutura do Projeto

```
src/
├── EfCore.FirestoreProvider/          # Projeto principal do provider
│   ├── Extensions/                    # Métodos de extensão
│   ├── Infrastructure/               # Infraestrutura do EF Core
│   ├── Model/                        # Modelos e anotações
│   └── Query/                        # Tradução de queries
├── EfCore.FirestoreProvider.Sample/   # Projeto de exemplo
tests/
└── EfCore.FirestoreProvider.Tests/   # Testes unitários e de integração
```

## Padrões de Código

### Estilo de Código

- Use C# naming conventions
- Configure o EditorConfig se disponível
- Use async/await para operações assíncronas
- Documente APIs públicas com XML comments

### Testes

- Escreva testes unitários para nova funcionalidade
- Use nomes descritivos para métodos de teste
- Organize testes em classes apropriadas (Unit, Integration)
- Use mocks quando apropriado

### Commits

- Use mensagens de commit descritivas
- Considere usar conventional commits (feat, fix, docs, etc.)
- Mantenha commits atomicos (uma mudança por commit)

## Versionamento

Este projeto usa [Semantic Versioning](https://semver.org/). As versões são automaticamente geradas usando GitVersion baseado em:

- **Patch**: Bug fixes e pequenas melhorias
- **Minor**: Novas funcionalidades compatíveis
- **Major**: Breaking changes

## Licença

Ao contribuir, você concorda que suas contribuições serão licenciadas sob a licença MIT do projeto.

## Perguntas?

Se você tiver alguma pergunta, sinta-se à vontade para:

- Abrir um issue no GitHub
- Entrar em contato através do email fornecido no perfil

Obrigado por contribuir! 🎉
