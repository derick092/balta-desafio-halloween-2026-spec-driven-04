# Feature Specification: Códigos Personalizados para URLs

**Feature Branch**: `001-custom-url-codes`

**Created**: 2026-10-26

**Status**: Draft

**Input**: User description: Extend the existing URL-shortening flow so a person may optionally choose a code and use it to reach the original address. The code must be unique, follow character and length rules, and not conflict with existing service routes. Omitting it must preserve automatic code generation. Listing, details, and click counting must continue to work for both kinds of code. Editing or removing codes, authentication, and link expiration are out of scope.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Criar e usar uma URL com código personalizado (Priority: P1)

Ao encurtar uma URL, a pessoa informa um código fácil de reconhecer. A URL curta resultante usa esse código, e acessá-la redireciona para o endereço original e registra o clique.

**Why this priority**: É o valor principal da feature: escolher e compartilhar um endereço curto memorável.

**Independent Test**: Criar uma URL com um código válido e disponível, abrir a URL curta e verificar o destino e o aumento da contagem de cliques.

**Acceptance Scenarios**:

1. **Given** um código válido e ainda não utilizado, **When** a pessoa solicita o encurtamento de uma URL com esse código, **Then** recebe uma URL curta que contém exatamente o código informado.
2. **Given** uma URL curta criada com código personalizado, **When** a pessoa navega por essa URL, **Then** é redirecionada ao endereço original e a contagem de cliques aumenta em um.
3. **Given** uma URL curta criada com código personalizado, **When** a pessoa consulta a listagem ou os detalhes, **Then** encontra as mesmas informações e o mesmo formato de resposta oferecidos para uma URL com código gerado.

---

### User Story 2 - Receber retorno claro para códigos inválidos ou ocupados (Priority: P2)

Ao escolher um código que não respeita as regras, já está em uso ou conflita com uma rota do serviço, a pessoa recebe uma recusa clara e pode escolher outro código.

**Why this priority**: Evita ambiguidades na navegação e permite corrigir a escolha sem criar uma URL que não possa ser usada como esperado.

**Independent Test**: Tentar criar URLs com códigos duplicados, fora do formato, fora dos limites de tamanho e reservados; verificar que cada tentativa é recusada com uma explicação correspondente.

**Acceptance Scenarios**:

1. **Given** um código já associado a outra URL, **When** a pessoa tenta criá-lo novamente, **Then** a criação é recusada e a resposta informa que o código está em uso.
2. **Given** um código com caractere não permitido ou tamanho fora do intervalo aceito, **When** a pessoa solicita a criação, **Then** a criação é recusada e a resposta explica que o código é inválido.
3. **Given** um código reservado por uma rota do serviço, **When** a pessoa solicita a criação, **Then** a criação é recusada e a resposta informa que o código não pode ser utilizado.

---

### User Story 3 - Continuar usando códigos gerados e recursos existentes (Priority: P3)

Quem não informar um código continua encurtando URLs como antes. URLs com códigos gerados ou personalizados continuam disponíveis na listagem e nos detalhes.

**Why this priority**: Preserva o comportamento já utilizado e garante que a extensão não exija mudanças no fluxo existente.

**Independent Test**: Criar uma URL sem informar código e confirmar a geração automática; consultar listagem e detalhes de URLs com ambos os tipos de código.

**Acceptance Scenarios**:

1. **Given** uma solicitação sem código personalizado, **When** a pessoa encurta uma URL válida, **Then** o sistema gera um código automaticamente e retorna uma URL curta funcional como antes.
2. **Given** URLs criadas com código gerado e personalizado, **When** a pessoa lista ou consulta os detalhes delas, **Then** ambas aparecem com os mesmos campos e comportamentos atuais.

### Edge Cases

- Um código no tamanho mínimo ou máximo permitido é aceito; um código um caractere menor ou maior é recusado.
- Códigos vazios, contendo espaços, pontos, barras, caracteres acentuados ou qualquer símbolo além de letras, números, hífen e underscore são recusados.
- Códigos reservados são recusados sem alterar ou ocultar as rotas correspondentes do serviço.
- Uma tentativa com código em uso não altera a URL original nem a contagem de cliques da URL existente.
- A ausência do código personalizado mantém integralmente o fluxo de geração automática.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: O fluxo existente de encurtamento MUST aceitar um código personalizado opcional junto à URL original.
- **FR-002**: Quando nenhum código personalizado for informado, o sistema MUST continuar gerando o código automaticamente, sem alterar o comportamento existente.
- **FR-003**: O sistema MUST aceitar códigos personalizados compostos somente por letras sem acento, números, hífen (`-`) e underscore (`_`), com tamanho entre 3 e 32 caracteres, inclusive.
- **FR-004**: O sistema MUST recusar códigos personalizados que não respeitem o formato ou os limites de tamanho e MUST explicar a recusa em português.
- **FR-005**: O sistema MUST garantir que um código personalizado não esteja associado a outra URL antes de criá-la; se estiver, MUST recusar a criação e informar em português que o código está em uso.
- **FR-006**: O sistema MUST recusar códigos que coincidam, sem distinção entre maiúsculas e minúsculas, com nomes de rotas de primeiro nível existentes no serviço e explicar a recusa em português.
- **FR-007**: Para um código personalizado válido e disponível, o sistema MUST usar o código informado na URL curta e MUST redirecionar ao endereço original ao navegar por essa URL, incrementando a contagem de cliques como no fluxo atual.
- **FR-008**: A listagem e os detalhes MUST continuar apresentando URLs com códigos personalizados e gerados com os mesmos campos e comportamentos atuais.
- **FR-009**: O código MUST permanecer inalterável após a criação; edição e remoção de códigos, autenticação e expiração de links não fazem parte desta feature.

### Key Entities *(include if feature involves data)*

- **URL encurtada**: Associação entre um endereço original e um código curto único, gerado automaticamente ou escolhido pela pessoa; inclui a data de criação e a contagem de cliques.
- **Código curto**: Identificador utilizado na URL curta e na navegação para localizar a URL original; pode ser gerado ou personalizado, sujeito às regras de formato, unicidade e reserva de rotas.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Em 100% dos cenários válidos, uma solicitação com código personalizado disponível retorna uma URL curta que contém exatamente o código escolhido.
- **SC-002**: Em 100% dos cenários de navegação testados, a URL curta personalizada redireciona ao endereço original e incrementa a contagem de cliques em exatamente um.
- **SC-003**: Em 100% das tentativas com formato inválido, tamanho fora dos limites, código já utilizado ou nome reservado, a criação é recusada e a resposta identifica o motivo em português.
- **SC-004**: Em 100% dos cenários de regressão, a criação sem código personalizado e a consulta de listagem e detalhes para os dois tipos de código mantêm o comportamento atual.

## Assumptions

- Como os limites exatos não foram especificados, o tamanho permitido será de 3 a 32 caracteres, inclusive.
- Letras são consideradas letras sem acento. A distinção de maiúsculas e minúsculas é preservada para identificar códigos já existentes; nomes de rotas reservados são comparados sem distinção de caixa para evitar colisões de navegação.
- São reservados os nomes de primeiro nível usados pelas rotas existentes do serviço, incluindo os caminhos de gerenciamento de URLs e de documentação. Essa reserva deve acompanhar novas rotas próprias caso sejam adicionadas.
- O código continua imutável depois da criação, e não há alterações ao fluxo de autenticação ou à validade temporal dos links.
