# Research: Códigos Personalizados para URLs

## Persistência e concorrência

- **Decision**: Verificar a disponibilidade do código antes de inserir e tratar a violação concorrente do índice único no `SaveChangesAsync` como HTTP 409. Reconhecer somente `Microsoft.Data.Sqlite.SqliteException` com `SqliteExtendedErrorCode == 2067` (`SQLITE_CONSTRAINT_UNIQUE`), encapsulada em `DbUpdateException`; deixar outras exceções propagarem.
- **Rationale**: A consulta prévia produz uma recusa clara no caso comum, mas não elimina a corrida entre requisições. O índice SQLite existente é a autoridade final de unicidade. O código estendido identifica especificamente uma violação de unicidade, enquanto o código primário 19 (`SQLITE_CONSTRAINT`) também cobre outras restrições.
- **Alternatives considered**: Tratar qualquer exceção de persistência como conflito ou comparar a mensagem do provedor. Ambas poderiam mascarar falhas distintas; comparar somente o código primário 19 também classificaria outras restrições incorretamente.

## Validação de código e rotas

- **Decision**: Adicionar uma interface e uma implementação pequena em `Services/` para validar formato, tamanho e nomes reservados. Aceitar apenas ASCII `[A-Za-z0-9_-]`, de 3 a 32 caracteres, e comparar nomes de rota reservados sem distinção de caixa.
- **Rationale**: Centraliza as regras solicitadas sem adicionar dependências e segue a constituição, que exige que serviços sejam pequenos e declarem uma interface. A comparação de rota sem distinção de caixa evita colisões de navegação; a unicidade dos códigos existentes continua seguindo a collation padrão SQLite, conforme especificado.
- **Alternatives considered**: Validar diretamente no endpoint, espalhando as regras no fluxo; adicionar pacote externo de validação, desnecessário para estas regras simples.

## Rotas reservadas

- **Decision**: Reservar os segmentos de primeiro nível usados pela API atual: `urls` e `openapi`.
- **Rationale**: `/urls` contém as rotas de gerenciamento, e `/openapi` é usado pela documentação em Development. A reserva impede que o endpoint de redirecionamento de primeiro nível se sobreponha aos nomes de rota.
- **Alternatives considered**: Permitir esses códigos porque algumas rotas têm segmentos adicionais. Isso deixaria colisões ou comportamento dependente da precedência do roteamento.

## Compatibilidade e armazenamento

- **Decision**: Tornar `Code` opcional na request, mantendo a geração automática como caminho atual quando o valor estiver ausente. Reutilizar o campo `ShortUrl.Code` e seu índice único existentes.
- **Rationale**: Mantém clientes existentes compatíveis e evita mudanças de schema em um projeto que cria o banco com `EnsureCreated`.
- **Alternatives considered**: Nova tabela/coluna ou migrations; desnecessárias porque a entidade já armazena o código e a coluna tem o tamanho requerido.

## Fontes

- Microsoft.Data.Sqlite 10.0 API: `SqliteException` documenta os códigos de erro do provedor e deriva de `DbException`: https://learn.microsoft.com/en-us/dotnet/api/microsoft.data.sqlite.sqliteexception?view=msdata-sqlite-10.0.0
- Código-fonte do EF Core: implementação de `SqliteException` e propriedades `SqliteErrorCode` / `SqliteExtendedErrorCode`: https://github.com/dotnet/efcore/blob/c0f183ba1b7dc5ff392c596350a701f6ad206052/src/Microsoft.Data.Sqlite.Core/SqliteException.cs
- SQLite Result Codes: código primário `SQLITE_CONSTRAINT` (19) e código estendido `SQLITE_CONSTRAINT_UNIQUE` (2067): https://www.sqlite.org/rescode.html#constraint e https://www.sqlite.org/rescode.html#constraint_unique
