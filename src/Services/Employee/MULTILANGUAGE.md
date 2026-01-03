# Sistema MultiLanguage - Employee Service

## Visão Geral

O Employee Service possui suporte completo para múltiplos idiomas (internacionalização/i18n) usando **Resource Files (.resx)** do .NET.

**Idiomas Suportados:**
- ✅ **Inglês (en)** - Padrão
- ✅ **Português Brasileiro (pt-BR)**
- 🔄 **Pronto para adicionar mais idiomas**

## Estrutura de Arquivos

### 1. Application Layer - Validation Messages

```
Employee.Application/
└── Resources/
    ├── ValidationMessages.resx          (English - Default)
    └── ValidationMessages.pt-BR.resx    (Portuguese)
```

**Uso:**
- Mensagens de validação do FluentValidation
- Erros de validação de Commands
- Validators (CreateEmployeeCommandValidator, UpdateEmployeeCommandValidator)

### 2. Domain Layer - Domain Exceptions

```
Employee.Domain/
└── Resources/
    ├── DomainExceptions.cs              (Dummy class for DI)
    ├── DomainExceptions.resx            (English - Default)
    └── DomainExceptions.pt-BR.resx      (Portuguese)
```

**Uso:**
- Exceções de ArgumentException
- Exceções de InvalidOperationException
- Value Objects (CPF, Email, PhoneNumber, Address)
- Entities (Department)
- Aggregate Roots (EmployeeAggregate)
- Handlers

## Como Usar

### 1. Em Validators (FluentValidation)

```csharp
using Microsoft.Extensions.Localization;

public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator(IStringLocalizer<ValidationMessages> localizer)
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage(localizer["FirstName_Required"])
            .MaximumLength(100).WithMessage(localizer["FirstName_MaxLength"]);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(localizer["Email_Required"])
            .EmailAddress().WithMessage(localizer["Email_InvalidFormat"]);
    }
}
```

### 2. Em Handlers

```csharp
using Microsoft.Extensions.Localization;
using Employee.Domain.Resources;

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, EmployeeDto>
{
    private readonly IStringLocalizer<DomainExceptions> _localizer;

    public CreateEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IMapper mapper,
        IStringLocalizer<DomainExceptions> localizer)
    {
        _employeeRepository = employeeRepository;
        _mapper = mapper;
        _localizer = localizer;
    }

    public async Task<EmployeeDto> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var existingEmployee = await _employeeRepository.GetByCPFAsync(request.CPF, cancellationToken);
        if (existingEmployee != null)
            throw new InvalidOperationException(_localizer["EmployeeWithCPFAlreadyExists", request.CPF]);

        // ...
    }
}
```

### 3. Em Value Objects (Domain)

```csharp
using Microsoft.Extensions.Localization;
using Employee.Domain.Resources;

public class CPF : ValueObject
{
    public static CPF Create(string cpf, IStringLocalizer<DomainExceptions> localizer)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            throw new ArgumentException(localizer["CPFCannotBeEmpty"], nameof(cpf));

        var cleanCpf = Regex.Replace(cpf, @"[^\d]", "");

        if (cleanCpf.Length != 11)
            throw new ArgumentException(localizer["CPFMustHave11Digits"], nameof(cpf));

        if (!IsValid(cleanCpf))
            throw new ArgumentException(localizer["InvalidCPF"], nameof(cpf));

        return new CPF(cleanCpf);
    }
}
```

## Configuração no Startup/Program.cs

```csharp
// Adicionar Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Configurar culturas suportadas
var supportedCultures = new[] { "en", "pt-BR" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("en")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

// FluentValidation com Localization
builder.Services.AddValidatorsFromAssemblyContaining<CreateEmployeeCommandValidator>();
```

## Como Adicionar um Novo Idioma

### Passo 1: Criar Arquivos de Recursos

1. Copie `ValidationMessages.resx`
2. Renomeie para `ValidationMessages.{cultura}.resx` (ex: `ValidationMessages.es.resx` para espanhol)
3. Traduza todos os valores (não modifique as keys/names!)

### Passo 2: Repetir para DomainExceptions

1. Copie `DomainExceptions.resx`
2. Renomeie para `DomainExceptions.{cultura}.resx`
3. Traduza todos os valores

### Passo 3: Atualizar Startup

```csharp
var supportedCultures = new[] { "en", "pt-BR", "es" }; // Adicionar novo idioma
```

## Estrutura dos Resource Files

### ValidationMessages Keys (Exemplos)

```
FirstName_Required
FirstName_MaxLength
LastName_Required
CPF_Required
CPF_InvalidFormat
Email_Required
Email_InvalidFormat
PhoneNumber_Required
PhoneNumber_InvalidFormat
BirthDate_Required
BirthDate_MustBeInPast
Salary_GreaterThanZero
```

### DomainExceptions Keys (Exemplos)

```
FirstNameCannotBeEmpty
LastNameCannotBeEmpty
SalaryMustBeGreaterThanZero
CPFCannotBeEmpty
CPFMustHave11Digits
InvalidCPF
EmailCannotBeEmpty
InvalidEmailFormat
EmployeeWithCPFAlreadyExists
EmployeeNotFound
```

## Formato de Mensagens com Parâmetros

Para mensagens com placeholders:

**Resource File:**
```xml
<data name="EmployeeWithCPFAlreadyExists" xml:space="preserve">
  <value>Employee with CPF {0} already exists</value>
</data>
```

**Uso no Código:**
```csharp
_localizer["EmployeeWithCPFAlreadyExists", cpfValue]
```

## Testando Diferentes Idiomas

### Via Header HTTP

```http
GET /api/employees
Accept-Language: pt-BR
```

```http
GET /api/employees
Accept-Language: en
```

### Via Query String

```http
GET /api/employees?culture=pt-BR
GET /api/employees?culture=en
```

### Via Cookie

```csharp
Response.Cookies.Append(
    CookieRequestCultureProvider.DefaultCookieName,
    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture("pt-BR"))
);
```

## Benefícios

✅ **Centralização** - Todas as mensagens em um único lugar
✅ **Manutenibilidade** - Fácil adicionar novos idiomas
✅ **Type-Safe** - Intellisense para keys
✅ **Padrão .NET** - Usa infraestrutura nativa do framework
✅ **Performance** - Resources compilados no assembly
✅ **Fallback Automático** - Se tradução não existir, usa idioma padrão (en)

## Notas Importantes

1. **Nunca modifique as keys** - Apenas os valores (value)
2. **Mantenha consistência** - Mesmas keys em todos os idiomas
3. **Teste ambos idiomas** - Verifique que todas as mensagens foram traduzidas
4. **Use placeholders** - `{0}`, `{1}` para valores dinâmicos
5. **Compile após modificar .resx** - Os arquivos são compilados em resources

## Estrutura de Pastas Completa

```
Employee.Application/
├── Resources/
│   ├── ValidationMessages.resx        (en - default)
│   └── ValidationMessages.pt-BR.resx  (pt-BR)
├── Validators/
│   ├── CreateEmployeeCommandValidator.cs   ← Usa IStringLocalizer<ValidationMessages>
│   └── UpdateEmployeeCommandValidator.cs   ← Usa IStringLocalizer<ValidationMessages>
└── Handlers/
    ├── CreateEmployeeCommandHandler.cs     ← Usa IStringLocalizer<DomainExceptions>
    └── UpdateEmployeeCommandHandler.cs     ← Usa IStringLocalizer<DomainExceptions>

Employee.Domain/
├── Resources/
│   ├── DomainExceptions.cs            (Dummy class)
│   ├── DomainExceptions.resx          (en - default)
│   └── DomainExceptions.pt-BR.resx    (pt-BR)
├── ValueObjects/                      ← Podem usar IStringLocalizer<DomainExceptions>
├── Entities/                          ← Podem usar IStringLocalizer<DomainExceptions>
└── Aggregates/                        ← Podem usar IStringLocalizer<DomainExceptions>
```

## Próximos Passos (Opcional)

- [ ] Adicionar mais idiomas (Espanhol, Francês, etc)
- [ ] Criar helper methods para formatação de mensagens
- [ ] Adicionar validação de que todas as keys existem em todos os idiomas
- [ ] Criar testes unitários para validar traduções
