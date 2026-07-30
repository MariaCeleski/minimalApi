# Requirements Document: Personal Financial Management Application

## Introduction

Esta aplicação de gestão financeira pessoal permite que usuários gerenciem suas receitas e despesas de forma intuitiva e visual. O sistema oferece funcionalidades completas de CRUD, cálculo automático de saldo, visualização de dados em dashboards dinâmicos, geração de relatórios e exportação de dados. O sistema é responsivo, suporta dark mode, oferece uma interface intuitiva com animações suaves e é acessível em diversos dispositivos.

## Glossary

- **Transaction_Manager**: Sistema responsável por processar e validar transações (receitas e despesas)
- **Finance_Dashboard**: Componente visual que exibe saldo atual, gráficos e resumos financeiros
- **Transaction**: Registro individual de receita ou despesa com data, valor, categoria e descrição
- **Category**: Classificação de transação (alimentação, transporte, lazer, saúde, educação, outros)
- **Balance**: Saldo calculado automaticamente (soma de receitas menos despesas)
- **Report_Generator**: Serviço responsável por gerar relatórios mensais e por categoria
- **Export_Service**: Serviço que exporta dados em formatos CSV e PDF
- **Notification_System**: Sistema de alertas visuais quando limites financeiros são excedidos
- **Authentication_Provider**: Serviço de autenticação simples (login/senha)
- **Financial_Goal**: Meta financeira definida pelo usuário para monitoramento
- **Period_Filter**: Filtro temporal para análise de transações (dia, semana, mês, trimestre, ano)
- **Category_Filter**: Filtro por categoria para análise segmentada
- **Dark_Mode_Engine**: Sistema de tema claro/escuro com persistência de preferência
- **Chart_Renderer**: Motor de renderização de gráficos (pizza, linha, barras)
- **UI_Component**: Elemento visual reutilizável (botão, card, modal, input)

---

## Requirements

### Requirement 1: Cadastro e Validação de Transações

**User Story:** Como usuário, desejo cadastrar receitas e despesas com informações detalhadas, para que eu possa registrar e acompanhar todas as minhas movimentações financeiras.

#### Acceptance Criteria

1. WHEN um usuário submete um novo registro de receita, THE Transaction_Manager SHALL validar se todos os campos obrigatórios (data, valor, categoria, descrição) estão preenchidos e retornar erro se algum estiver ausente
2. THE Transaction_Manager SHALL rejeitar transações com valor igual a zero ou negativo, retornando mensagem de erro específica
3. THE Transaction_Manager SHALL validar se a data fornecida não ultrapassa a data atual e retornar erro caso contrário
4. WHEN uma transação válida é submetida, THE Transaction_Manager SHALL armazenar no banco de dados e retornar sucesso com o ID gerado
5. THE Transaction_Manager SHALL suportar no mínimo 8 categorias predefinidas (Alimentação, Transporte, Lazer, Saúde, Educação, Utilitários, Investimento, Outros)
6. WHERE o usuário seleciona descrição opcional, THE Transaction_Manager SHALL permitir strings de até 255 caracteres sem caracteres especiais prejudiciais

#### Correctness Properties (Property-Based Testing)

- **Invariant**: Para qualquer transação válida armazenada, os campos (data, valor, categoria) devem ser idênticos quando recuperados
- **Round-trip**: Salvar uma transação e recuperá-la deve produzir um objeto equivalente (mesmo valor, data, categoria)
- **Error handling**: Rejeitar sempre e apenas transações com valor ≤ 0 (comportamento consistente)

---

### Requirement 2: Listagem de Transações com Paginação

**User Story:** Como usuário, desejo listar todas as minhas transações de forma organizada e paginada, para que eu possa visualizar meu histórico de movimentações.

#### Acceptance Criteria

1. THE Transaction_Manager SHALL retornar lista paginada de transações com tamanho padrão de 10 itens por página
2. WHEN um usuário solicita a página N, THE Transaction_Manager SHALL retornar itens (N-1)*10 até N*10, mantendo ordem decrescente por data
3. THE Transaction_Manager SHALL incluir metadados de paginação (página atual, total de páginas, total de itens)
4. WHEN nenhuma transação existe, THE Transaction_Manager SHALL retornar lista vazia com metadados apropriados
5. THE Transaction_Manager SHALL retornar cada transação com campos: ID, data, valor, categoria, descrição, tipo (receita/despesa)

#### Correctness Properties (Property-Based Testing)

- **Invariant**: O número total de itens retornados não deve exceder o tamanho da página (≤ 10 itens)
- **Metamorphic**: Para uma lista de N transações, `ceil(N/10)` deve ser o total de páginas
- **Round-trip**: Recuperar página 1, depois página 2, depois página 1 novamente deve produzir dados idênticos para página 1

---

### Requirement 3: Filtro por Período

**User Story:** Como usuário, desejo filtrar minhas transações por período específico, para que eu possa analisar gastos em intervalos de tempo específicos.

#### Acceptance Criteria

1. WHEN um usuário aplica filtro de período com data_inicio e data_fim, THE Transaction_Manager SHALL retornar apenas transações onde data >= data_inicio E data <= data_fim
2. THE Transaction_Manager SHALL validar que data_inicio não seja posterior a data_fim, retornando erro se falhar
3. WHEN data_inicio é omitida, THE Transaction_Manager SHALL usar como padrão a data de 30 dias atrás
4. WHEN data_fim é omitida, THE Transaction_Manager SHALL usar como padrão a data atual
5. THE Transaction_Manager SHALL aplicar filtro de período em combinação com filtros de categoria e paginação

#### Correctness Properties (Property-Based Testing)

- **Invariant**: Todas as transações retornadas devem estar dentro do intervalo [data_inicio, data_fim] (nenhuma fora do período)
- **Boundary testing**: Transações na data_inicio e data_fim devem ser incluídas (fronteiras inclusivas)
- **Error handling**: Rejeitar sempre quando data_inicio > data_fim

---

### Requirement 4: Filtro por Categoria

**User Story:** Como usuário, desejo filtrar transações por categoria, para que eu possa visualizar gastos específicos de uma área (ex: transporte, alimentação).

#### Acceptance Criteria

1. WHEN um usuário seleciona uma categoria, THE Transaction_Manager SHALL retornar apenas transações que pertencem à categoria selecionada
2. THE Transaction_Manager SHALL permitir seleção de múltiplas categorias simultaneamente, retornando transações que correspondem a qualquer uma delas
3. WHEN nenhuma categoria é selecionada, THE Transaction_Manager SHALL retornar transações de todas as categorias
4. THE Transaction_Manager SHALL validar que a categoria selecionada existe na lista de categorias predefinidas
5. THE Transaction_Manager SHALL aplicar filtro de categoria combinado com filtro de período e paginação

#### Correctness Properties (Property-Based Testing)

- **Invariant**: Todas as transações retornadas possuem categoria dentro do conjunto selecionado
- **Confluence**: Ordem de aplicação de filtros (período então categoria, ou categoria então período) deve produzir mesmo resultado
- **Metamorphic**: Para categorias [A, B], `count(A) + count(B) ≥ count([A, B])` (com igualdade se A e B são disjuntos)

---

### Requirement 5: Cálculo Automático de Saldo

**User Story:** Como usuário, desejo que o sistema calcule automaticamente meu saldo, para que eu tenha visibilidade instantânea da minha situação financeira.

#### Acceptance Criteria

1. THE Transaction_Manager SHALL calcular saldo como: (soma de todas as receitas) - (soma de todas as despesas)
2. WHEN uma nova transação válida é adicionada, THE Transaction_Manager SHALL atualizar o saldo imediatamente
3. WHEN uma transação é deletada, THE Transaction_Manager SHALL recalcular o saldo removendo seu impacto
4. WHEN uma transação é editada, THE Transaction_Manager SHALL recalcular o saldo com o novo valor
5. THE Transaction_Manager SHALL retornar o saldo total atual com precisão de 2 casas decimais
6. IF o saldo é negativo, THE Transaction_Manager SHALL marcar visualmente como saldo devedor

#### Correctness Properties (Property-Based Testing)

- **Invariant**: Saldo = Σ(receitas) - Σ(despesas) para qualquer conjunto de transações
- **Idempotence**: Calcular saldo múltiplas vezes sem novas transações deve retornar o mesmo valor
- **Round-trip**: Adicionar transação X, calcular saldo, remover transação X deve retornar ao saldo original
- **Metamorphic**: Adicionar X então Y versus adicionar Y então X deve produzir mesmo saldo final

---

### Requirement 6: Dashboard com Visualização de Saldo e Gráficos

**User Story:** Como usuário, desejo visualizar meu saldo e gráficos na tela inicial (dashboard), para que eu tenha uma visão geral da minha situação financeira instantaneamente.

#### Acceptance Criteria

1. THE Finance_Dashboard SHALL exibir saldo total atual em destaque no topo
2. THE Finance_Dashboard SHALL exibir gráfico de pizza mostrando distribuição de despesas por categoria
3. THE Finance_Dashboard SHALL exibir gráfico de linha mostrando evolução do saldo ao longo dos últimos 12 meses
4. WHEN o usuário acessa o dashboard, THE Finance_Dashboard SHALL carregar dados em menos de 2 segundos
5. THE Finance_Dashboard SHALL atualizar em tempo real quando novas transações são adicionadas
6. WHERE o usuário aplica filtro de período no dashboard, THE Finance_Dashboard SHALL atualizar gráficos para mostrar apenas dados no período filtrado
7. IF o saldo é negativo, THE Finance_Dashboard SHALL exibir indicador visual em cor de alerta (vermelho)

#### Correctness Properties (Property-Based Testing)

- **Invariant**: Soma de valores do gráfico de pizza deve ser igual ao saldo total de despesas no período
- **Round-trip**: Adicionar despesa, verificar gráfico, remover despesa deve retornar gráfico ao estado anterior
- **Metamorphic**: Gráfico de despesas por categoria não deve mudar ao aplicar filtro de receitas

---

### Requirement 7: Edição de Transações

**User Story:** Como usuário, desejo editar transações já cadastradas, para que eu possa corrigir informações incorretas.

#### Acceptance Criteria

1. WHEN um usuário seleciona uma transação existente, THE Transaction_Manager SHALL recuperar e exibir todos os campos para edição
2. THE Transaction_Manager SHALL validar as mesmas regras do cadastro durante edição (valor > 0, data não futura, campos obrigatórios)
3. WHEN uma edição válida é submetida, THE Transaction_Manager SHALL atualizar no banco de dados e retornar sucesso
4. THE Transaction_Manager SHALL validar que o ID da transação existe antes de atualizar
5. WHEN uma edição modifica o valor, THE Transaction_Manager SHALL recalcular o saldo automaticamente

#### Correctness Properties (Property-Based Testing)

- **Invariant**: Editar uma transação não deve alterar seu ID ou data de criação
- **Round-trip**: Editar transação para valor X, editar novamente para valor Y, editar para valor X deve resultar em mesmo estado
- **Idempotence**: Editar transação para os mesmos valores deve produzir estado idêntico

---

### Requirement 8: Exclusão de Transações

**User Story:** Como usuário, desejo deletar transações incorretas, para que eu possa manter meu histórico financeiro preciso.

#### Acceptance Criteria

1. WHEN um usuário solicita exclusão de uma transação, THE Transaction_Manager SHALL validar que a transação existe
2. THE Transaction_Manager SHALL remover a transação do banco de dados
3. WHEN uma transação é removida, THE Transaction_Manager SHALL recalcular o saldo imediatamente
4. THE Transaction_Manager SHALL confirmar exclusão com mensagem de sucesso
5. IF o usuário tenta deletar transação inexistente, THE Transaction_Manager SHALL retornar erro apropriado

#### Correctness Properties (Property-Based Testing)

- **Idempotence**: Deletar mesma transação twice deve falhar na segunda tentativa (erro ou silenciar)
- **Round-trip**: Adicionar transação, deletar, verificar se não existe mais, é verificável
- **Invariant**: Após deletar transação com valor V, saldo deve reduzir em exatamente V (se despesa) ou aumentar em V (se receita)

---

### Requirement 9: Relatório Mensal

**User Story:** Como usuário, desejo gerar relatórios de receitas e despesas por mês, para que eu possa analisar meu desempenho financeiro mensal.

#### Acceptance Criteria

1. THE Report_Generator SHALL gerar relatório com total de receitas, total de despesas e saldo líquido para um mês específico
2. WHEN um usuário seleciona um mês, THE Report_Generator SHALL retornar relatório com breakdown por categoria
3. THE Report_Generator SHALL incluir percentual de cada categoria em relação ao total de despesas
4. THE Report_Generator SHALL retornar dados em formato JSON estruturado
5. IF nenhum dado existe para o mês selecionado, THE Report_Generator SHALL retornar relatório com valores zerados

#### Correctness Properties (Property-Based Testing)

- **Invariant**: Total de receitas no relatório deve ser igual a Σ(receitas) para o mês
- **Invariant**: Soma de percentuais por categoria deve ser ≈ 100% (com tolerância de arredondamento)
- **Round-trip**: Gerar relatório mês M, adicionar transação em mês M, gerar relatório novamente deve refletir nova transação

---

### Requirement 10: Relatório por Categoria

**User Story:** Como usuário, desejo visualizar relatórios agregados por categoria, para que eu possa identificar onde meu dinheiro está sendo gasto.

#### Acceptance Criteria

1. THE Report_Generator SHALL retornar total gasto por categoria no período especificado
2. WHEN um período é especificado, THE Report_Generator SHALL filtrar transações e agregar por categoria
3. THE Report_Generator SHALL incluir percentual de cada categoria em relação ao total geral
4. THE Report_Generator SHALL ordenar categorias por valor gasto em ordem decrescente
5. THE Report_Generator SHALL retornar dados tanto para receitas quanto para despesas por categoria

#### Correctness Properties (Property-Based Testing)

- **Invariant**: Soma de todos os valores por categoria deve igualar total do período
- **Metamorphic**: Relatório por categoria + filtro período A deve igualar relatório período A + filtro categoria
- **Confluence**: Gerar relatório por categoria para períodos [M1, M2] versus M1 depois M2 deve produzir mesma agregação

---

### Requirement 11: Exportação em CSV

**User Story:** Como usuário, desejo exportar minhas transações em CSV, para que eu possa analisar dados em ferramentas externas como Excel.

#### Acceptance Criteria

1. WHEN um usuário solicita exportação em CSV, THE Export_Service SHALL gerar arquivo CSV com todas as transações do período filtrado
2. THE Export_Service SHALL incluir headers: ID, Data, Tipo (Receita/Despesa), Valor, Categoria, Descrição
3. THE Export_Service SHALL usar encoding UTF-8
4. THE Export_Service SHALL escapar corretamente valores que contenham vírgulas ou aspas
5. THE Export_Service SHALL retornar arquivo como download com nome: transacoes_YYYY-MM-DD.csv

#### Correctness Properties (Property-Based Testing)

- **Round-trip**: Exportar transações para CSV, parsear CSV, dados recuperados devem ser equivalentes aos originais
- **Invariant**: Número de linhas no CSV (excluindo header) deve igualar número de transações exportadas
- **Data integrity**: Valores especiais (aspas, vírgulas, saltos de linha) devem ser preservados após parse do CSV

---

### Requirement 12: Exportação em PDF

**User Story:** Como usuário, desejo exportar relatórios em PDF, para que eu possa compartilhar ou arquivar informações de forma profissional.

#### Acceptance Criteria

1. WHEN um usuário solicita exportação de relatório em PDF, THE Export_Service SHALL gerar documento PDF formatado
2. THE Export_Service SHALL incluir no PDF: título, período, resumo (receitas, despesas, saldo) e tabela de transações
3. THE Export_Service SHALL usar formatação visual com cores e fontes legíveis
4. THE Export_Service SHALL gerar nome de arquivo: relatorio_YYYY-MM-DD.pdf
5. THE Export_Service SHALL retornar arquivo como download

#### Correctness Properties (Property-Based Testing)

- **Round-trip**: Exportar relatório para PDF, extrair texto, verificar se dados principais estão presentes
- **Invariant**: PDF deve conter todos os valores presentes no relatório JSON original
- **Data preservation**: Caracteres acentuados e especiais devem ser renderizados corretamente

---

### Requirement 13: Alternância entre Dark Mode e Light Mode

**User Story:** Como usuário, desejo alternar entre modo claro e escuro, para que eu possa usar a aplicação confortavelmente em diferentes ambientes.

#### Acceptance Criteria

1. THE Dark_Mode_Engine SHALL implementar dois temas completos: claro e escuro
2. WHEN um usuário alterna o tema, THE Dark_Mode_Engine SHALL aplicar instantaneamente em toda a interface
3. THE Dark_Mode_Engine SHALL persistir a preferência de tema no localStorage
4. WHEN o usuário retorna à aplicação, THE Dark_Mode_Engine SHALL restaurar o tema previamente selecionado
5. THE Dark_Mode_Engine SHALL usar paleta: azul, verde, cinza com contraste acessível em ambos os temas

#### Correctness Properties (Property-Based Testing)

- **Idempotence**: Alternar tema N vezes deve resultar em estado final após N mod 2 alternâncias
- **Persistence**: Definir tema X, fechar app, abrir app, deve estar em tema X
- **Round-trip**: Definir tema Light, mudar para Dark, mudar para Light deve retornar ao Light original

---

### Requirement 14: Interface Responsiva

**User Story:** Como usuário, desejo que a interface seja responsiva e acessível em dispositivos móveis, tablets e desktops, para que eu possa usar a aplicação em qualquer dispositivo.

#### Acceptance Criteria

1. THE UI_Component SHALL adaptar layout para telas de 320px (mobile), 768px (tablet) e 1920px (desktop)
2. WHEN em dispositivo móvel, THE Finance_Dashboard SHALL exibir gráficos empilhados verticalmente
3. WHEN em dispositivo desktop, THE Finance_Dashboard SHALL exibir gráficos lado a lado
4. THE UI_Component SHALL usar tamanhos de fonte e espaçamento apropriados para cada breakpoint
5. THE UI_Component SHALL funcionar sem scroll horizontal em nenhum breakpoint

#### Correctness Properties (Property-Based Testing)

- **Invariant**: Conteúdo não deve overflow em qualquer viewport (320px-1920px)
- **Metamorphic**: Redimensionar viewport não deve alterar dados ou funcionalidade, apenas layout
- **Round-trip**: Abrir em mobile, redimensionar para desktop, redimensionar para mobile deve manter mesmo estado

---

### Requirement 15: Animações Suaves com Framer Motion

**User Story:** Como usuário, desejo que a interface apresente animações suaves, para que a experiência de uso seja agradável e intuitiva.

#### Acceptance Criteria

1. WHEN uma transação é adicionada, THE UI_Component SHALL animar a entrada do novo item com transição suave
2. WHEN gráficos são renderizados, THE Chart_Renderer SHALL animar a criação das barras/pizza com duração de 500ms
3. WHEN o usuário navega entre páginas, THE UI_Component SHALL aplicar transição suave entre conteúdos
4. THE UI_Component SHALL usar `ease-in-out` como timing function padrão para animações
5. WHEN o usuário alterna tema, THE Dark_Mode_Engine SHALL aplicar transição suave de cores com duração de 300ms

#### Correctness Properties (Property-Based Testing)

- **Idempotence**: Adicionar transação, ver animação, adicionar outra transação deve produzir animações idênticas
- **Metamorphic**: Animar em light mode vs dark mode deve usar mesma função de timing (apenas cores mudam)

---

### Requirement 16: Ícones por Categoria

**User Story:** Como usuário, desejo visualizar ícones para cada categoria, para que eu possa identificar rapidamente o tipo de transação.

#### Acceptance Criteria

1. THE UI_Component SHALL associar ícone único para cada categoria predefinida
2. THE UI_Component SHALL exibir ícone ao lado de cada transação na listagem
3. THE UI_Component SHALL exibir ícone no gráfico de pizza ao lado do label de categoria
4. THE UI_Component SHALL usar ícones consistentes e reconhecíveis para as 8 categorias
5. WHEN em dispositivo móvel, THE UI_Component SHALL redimensionar ícones apropriadamente (24px vs 32px desktop)

#### Correctness Properties (Property-Based Testing)

- **Invariant**: Cada transação deve ter exatamente um ícone (correspondente à sua categoria)
- **Idempotence**: Exibir ícone múltiplas vezes deve produzir mesmo visual

---

### Requirement 17: Autenticação Simples (Opcional)

**User Story:** Como usuário, desejo fazer login com usuário e senha, para que meus dados financeiros sejam protegidos e acessíveis apenas para mim.

#### Acceptance Criteria

1. THE Authentication_Provider SHALL permitir registro de novo usuário com email e senha
2. THE Authentication_Provider SHALL validar email em formato válido (RFC 5322 simplificado)
3. THE Authentication_Provider SHALL exigir senha com mínimo 6 caracteres
4. WHEN um usuário faz login com credenciais corretas, THE Authentication_Provider SHALL retornar token de sessão
5. THE Authentication_Provider SHALL validar token em cada requisição protegida
6. IF credenciais são inválidas, THE Authentication_Provider SHALL retornar erro sem revelar qual campo está errado
7. WHEN usuário faz logout, THE Authentication_Provider SHALL invalidar token e redirecionar para login

#### Correctness Properties (Property-Based Testing)

- **Idempotence**: Login com mesmas credenciais múltiplas vezes deve sempre retornar token válido
- **Error handling**: Rejeitar sempre email inválido, mesmo se persistido previamente
- **Round-trip**: Registrar usuário, fazer login, fazer logout, fazer login novamente deve funcionar

---

### Requirement 18: Metas Financeiras (Opcional)

**User Story:** Como usuário, desejo definir metas financeiras de poupança, para que eu possa acompanhar progresso em direção aos meus objetivos.

#### Acceptance Criteria

1. THE Notification_System SHALL permitir que o usuário crie meta com nome, valor alvo e data limite
2. THE Finance_Dashboard SHALL exibir progresso de cada meta em forma de barra de progresso
3. WHEN meta é atingida, THE Notification_System SHALL exibir notificação de sucesso
4. WHEN saldo excede meta, THE Notification_System SHALL indicar meta como ultrapassada com cor verde
5. THE Report_Generator SHALL incluir análise de metas no relatório mensal

#### Correctness Properties (Property-Based Testing)

- **Invariant**: Progresso_meta = min((saldo_poupança / valor_alvo) * 100, 100%)
- **Idempotence**: Adicionar mesma meta múltiplas vezes deve falhar ou consolidar
- **Round-trip**: Criar meta X, adicionar transações, verificar progresso, remover transações deve retornar progresso ao original

---

### Requirement 19: Notificações de Limite Excedido (Opcional)

**User Story:** Como usuário, desejo receber notificações quando ultrapasso limites de gastos, para que eu possa controlar melhor meu orçamento.

#### Acceptance Criteria

1. THE Notification_System SHALL permitir que o usuário defina limite de gastos por categoria
2. WHEN gastos em uma categoria excedem 80% do limite, THE Notification_System SHALL exibir notificação de aviso (amarelo)
3. WHEN gastos em uma categoria excedem 100% do limite, THE Notification_System SHALL exibir notificação de alerta (vermelho)
4. THE Notification_System SHALL verificar limites após cada nova transação adicionada
5. WHEN o usuário visualiza o dashboard, THE Notification_System SHALL exibir fila de notificações recentes

#### Correctness Properties (Property-Based Testing)

- **Invariant**: Notificação é disparada se e somente se (gastos_categoria / limite) >= threshold
- **Metamorphic**: Adicionar transação X à categoria A, depois Y, deve disparar mesma sequência de notificações que adicionar Y então X (se ordem importa para timing)
- **Threshold testing**: Limite de 80% deve disparar aviso em [80%, 99.99%], mas não em [0%, 79.99%]

---

### Requirement 20: Validação de Integridade de Dados

**User Story:** Como sistema, desejo garantir que dados financeiros sejam armazenados com integridade, para que não haja inconsistências ou corrupção de informações.

#### Acceptance Criteria

1. THE Transaction_Manager SHALL validar que saldo calculado = soma transações em cada acesso
2. THE Transaction_Manager SHALL validar que não existem transações órfãs (sem categoria válida)
3. WHEN um erro de integridade é detectado, THE Transaction_Manager SHALL registrar em log e retornar erro ao usuário
4. THE Transaction_Manager SHALL implementar transações de banco de dados para operações críticas
5. IF operação falha mid-way, THE Transaction_Manager SHALL reverter para estado anterior

#### Correctness Properties (Property-Based Testing)

- **Invariant**: Sempre que system é consultado, saldo_calculado == saldo_armazenado
- **Idempotence**: Validar integridade múltiplas vezes consecutivas deve sempre retornar mesmo resultado
- **Round-trip**: Se saldo_antes == X, após operação falha, saldo_depois deve retornar a X

