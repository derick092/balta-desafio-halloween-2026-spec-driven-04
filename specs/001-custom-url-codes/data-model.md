# Data Model: Códigos Personalizados para URLs

## ShortUrl

Representa a associação persistida entre o código de navegação e o endereço original. A entidade e a tabela existentes não mudam.

| Field | Type | Constraints / behavior |
|---|---|---|
| `Id` | `int` | Chave primária existente. |
| `Code` | `string` | Obrigatório, máximo de 32 caracteres, índice único já existente. Pode ser gerado ou escolhido na criação. Códigos personalizados aceitam somente ASCII letters, digits, `-` e `_`, com 3-32 caracteres; códigos de rota reservados não são aceitos. Imutável após a criação. |
| `OriginalUrl` | `string` | Obrigatório, máximo de 2048 caracteres; deve ser URL absoluta HTTP ou HTTPS conforme a validação já existente. |
| `CreatedAt` | `DateTime` | Definido na criação, como no comportamento atual. |
| `Clicks` | `int` | Incrementado ao navegar pelo código, como no comportamento atual. |

## CreateShortUrlRequest

Contrato de entrada existente estendido com `Code`, uma string opcional e anulável. Quando ausente ou nulo, a API mantém a geração automática atual. Quando informado, o código deve satisfazer as regras de validação e disponibilidade antes de persistir.

## Relacionamentos e transições

- Cada `ShortUrl` tem um único `Code`, e o índice único impede códigos duplicados.
- Criação válida: request -> validação -> verificação de disponibilidade -> persistência -> resposta `201 Created`.
- Formato, tamanho ou nome reservado inválido: request -> resposta `400 ValidationProblem`; nenhuma entidade é criada.
- Código já utilizado, inclusive corrida entre inserções concorrentes: resposta `409 ProblemDetails`; nenhuma entidade duplicada é criada.
- A leitura e o redirecionamento consultam a mesma entidade independentemente de o código ser automático ou personalizado.
- Não há transição de edição ou remoção nesta feature.

## Schema

Nenhuma alteração. A coluna `Code` com tamanho máximo 32 e o índice único já existem; bancos atuais permanecem compatíveis.
