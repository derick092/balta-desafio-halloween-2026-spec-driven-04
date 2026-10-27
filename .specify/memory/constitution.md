<!--
Sync Impact Report
Version change: unratified scaffold -> 1.0.0
Modified principles: none; all eight principles are established for the first time.
Added sections: eight Core Principles; Restrições Técnicas; Fluxo de Desenvolvimento e Qualidade.
Removed sections: none.
Follow-up TODO: confirm the original ratification date.
-->
# EncurtaUrl API Constitution

## Core Principles

### I. Simplicidade Arquitetural
A aplicação MUST permanecer como uma única API em .NET 10 com Minimal APIs. Endpoints
MUST acessar o `AppDbContext` diretamente. Não devem ser introduzidos camadas extras,
MediatR ou repositórios: a arquitetura deve permanecer simples e coerente com o projeto
existente.

### II. Organização Consistente
Endpoints MUST ficar em `Endpoints/` como métodos de extensão sobre `IEndpointRouteBuilder`,
com rotas relacionadas agrupadas por `MapGroup`. Contratos de request e response MUST ficar
em `Contracts/` como records; entidades MUST ficar em `Models/`. Serviços em `Services/`
MUST permanecer pequenos e declarar uma interface.

### III. Respostas HTTP Tipadas e Corretas
Endpoints MUST declarar seus resultados com `TypedResults` e assinaturas `Results<...>` ou
outro tipo de resultado concreto apropriado. Erros de validação MUST retornar
`ValidationProblem` com status 400. Outros erros MUST produzir `ProblemDetails` com o status
HTTP correspondente, diretamente ou pela infraestrutura padrão configurada para a API.

### IV. Persistência Simples e Justificada
O acesso a dados MUST usar EF Core com SQLite e o contexto existente. A criação do banco
MUST usar `EnsureCreated`; migrations não são utilizadas. Qualquer alteração de schema MUST
ser justificada no plano, incluindo como bancos já existentes serão tratados.

### V. Compatibilidade de Contratos
Contratos existentes MUST permanecer compatíveis. Novos campos em requests MUST ser
opcionais. Alterações que possam quebrar contratos MUST incluir uma estratégia explícita de
compatibilidade e não podem ser aplicadas sem aprovação documentada.

### VI. Dependências Justificadas
Não deve ser adicionado pacote NuGet sem justificativa explícita no plano. A justificativa
MUST explicar por que a dependência é necessária e por que as capacidades existentes não
são suficientes.

### VII. Validação Reproduzível
Como o projeto não possui um projeto de testes, cada feature MUST adicionar cenários de
sucesso e de erro ao arquivo `EncurtaUrl.Api.http`. O projeto MUST compilar com `dotnet
build` sem warnings novos.

### VIII. Idioma Adequado ao Público
Mensagens de erro apresentadas ao usuário MUST estar em português. Código, identificadores
e comentários técnicos MUST seguir o padrão já adotado no projeto, que usa nomes técnicos
em inglês quando apropriado.

## Restrições Técnicas

- O alvo da API MUST permanecer em .NET 10 e usar EF Core com SQLite, salvo alteração
  aprovada por meio de emenda a esta constituição.
- O banco é criado com `EnsureCreated` e não usa migrations; por isso, alterações de schema
  MUST explicitar no plano os efeitos sobre bancos existentes e a estratégia de evolução.
- Dependências NuGet existentes podem ser reutilizadas. Uma dependência nova exige a
  justificativa definida no princípio VI.

## Fluxo de Desenvolvimento e Qualidade

- Cada feature MUST registrar no plano alterações de schema, dependências novas e qualquer
  impacto potencial sobre contratos existentes.
- O arquivo `EncurtaUrl.Api.http` MUST conter exemplos executáveis para ao menos um cenário
  de sucesso e um de erro relevantes à feature.
- Antes de concluir uma feature, `dotnet build` MUST ser executado; warnings novos devem
  ser corrigidos ou explicitamente avaliados e aprovados antes da conclusão.
- Revisões de especificações, planos e alterações MUST verificar conformidade com esta
  constituição. Desvios pretendidos exigem justificativa e aprovação documentadas e, quando
  alterarem uma regra permanente, uma emenda constitucional.

## Governance

Esta constituição governa decisões de arquitetura e desenvolvimento da API. Em caso de
conflito com outros documentos de trabalho, suas regras prevalecem até que uma emenda
aprovada as altere.

Uma emenda MUST descrever a motivação, o impacto sobre princípios e features existentes e
as ações necessárias para manter compatibilidade. A proposta deve ser revisada e aprovada
pelos responsáveis pelo projeto antes de atualizar esta constituição. Especificações,
planos, implementação e revisões subsequentes MUST ser avaliados contra a versão vigente.

O versionamento segue SemVer (`MAJOR.MINOR.PATCH`): MAJOR para alterações incompatíveis
nas regras de governança ou princípios; MINOR para princípios, seções ou requisitos
materialmente novos; PATCH para esclarecimentos e correções sem mudança de significado.
Toda emenda aprovada MUST atualizar a versão e a data de última alteração.

**Version**: 1.0.0 | **Ratified**: TODO(RATIFICATION_DATE): confirmar a data original de adoção | **Last Amended**: 2026-10-26
