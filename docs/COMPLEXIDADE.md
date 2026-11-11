# Análise de complexidade

Notação: **O(f)** limite superior, **Ω(f)** limite inferior, **Θ(f)** limite justo (O e Ω ao mesmo tempo).

## Módulo 1 — Linguagens formais

n = |w|, m = |v|, s = |Σ|, k = tamanho máximo listado.

| Algoritmo | Melhor | Médio | Pior | Justificativa |
|---|---|---|---|---|
| Validar w ∈ Σ* (`ValidadorCadeia.Validar`) | Θ(n) | Θ(n) | Θ(n) | Percorre toda a cadeia para listar todas as posições inválidas; cada consulta ao `HashSet<char>` de Σ é O(1) esperado. |
| Tamanho \|w\| | Θ(1) | Θ(1) | Θ(1) | A string guarda o comprimento. |
| Concatenação w·v | Θ(n+m) | Θ(n+m) | Θ(n+m) | Cópia dos dois operandos. |
| Reverso wᴿ | Θ(n) | Θ(n) | Θ(n) | Uma passagem. |
| Enumerar Σ* / Σ+ até k | Θ(k·sᵏ) | Θ(k·sᵏ) | Θ(k·sᵏ) | Existem Σᵢ₌₀..ₖ sⁱ = (sᵏ⁺¹−1)/(s−1) cadeias e cada uma custa O(k) para ser montada. Exponencial em k, por isso a API limita k ≤ 12 e no máximo 5000 cadeias. |
| w ∈ L, L finita | Θ(n) | Θ(n) | Θ(n) | Validação + `HashSet<string>` (o hash da cadeia custa Θ(n)). |
| w ∈ L, L regular | Ω(n) | O(n) | O(2ⁿ) | O motor `Regex` do .NET usa backtracking: linear nos casos comuns, exponencial em padrões patológicos (ex.: `(a*)*b`). Há timeout de 1 s. Um AFD equivalente seria Θ(n). |

## Módulo 3 — Gramática livre de contexto

n = |w|, |G| = soma dos tamanhos dos lados direitos, |V| = número de variáveis.

| Algoritmo | Melhor | Médio | Pior | Justificativa |
|---|---|---|---|---|
| Variáveis anuláveis (ponto fixo) | Θ(\|G\|) | O(\|V\|·\|G\|) | O(\|V\|·\|G\|) | Cada rodada percorre todas as produções e marca pelo menos uma variável nova; no máximo \|V\| rodadas. |
| Reconhecedor de Earley | Θ(n) | O(n²) | O(n³) | Há n+1 conjuntos; cada Sₖ tem O(k·\|G\|) itens. Uma conclusão percorre os itens de Sᵢ que esperam a variável (O(n)). Pior caso O(n³) em gramáticas ambíguas (`E → E+E`); O(n²) em gramáticas não ambíguas; Θ(n) em gramáticas LR(k), como `S → aSb \| λ`. |
| Reconstruir a árvore | Θ(t) | Θ(t) | Θ(t) | t = número de nós; cada item guarda ponteiros para o item anterior e para o filho que o completou, então cada nó é visitado uma vez. |
| Derivação mais à esquerda | Θ(p·f) | Θ(p·f) | Θ(p·f) | p passos (= número de nós internos); em cada um procura a variável mais à esquerda numa forma sentencial de tamanho f = O(n + p). |

**Por que Earley e não busca de derivações?** Gerar derivações por força bruta é exponencial e não termina em gramáticas recursivas à esquerda. CYK exigiria converter a gramática para a Forma Normal de Chomsky, o que mudaria a árvore mostrada ao aluno. Earley trabalha com a gramática original, aceita qualquer GLC e tem pior caso polinomial.

## Módulo 5 — Máquina de Turing

n = |entrada|, t = passos executados.

| Algoritmo | Melhor | Médio | Pior | Justificativa |
|---|---|---|---|---|
| Um passo de δ | Θ(1) | Θ(1) | O(\|δ\|) | `Dictionary<(estado, símbolo), Transicao>`. O original em Java fazia busca linear O(\|δ\|) a cada passo. |
| Mover o cabeçote | Θ(1) | Θ(1) amortizado | O(\|fita\|) | Crescer à direita é `List.Add` (amortizado O(1)); crescer à esquerda é `Insert(0)` (O(\|fita\|)), raro. |
| Soma unária (opção B) | Θ(n) | Θ(n) | Θ(n) | Uma varredura: exatamente n + 2 passos. |
| Reconhecer w#w (opção A) | Θ(1) | Θ(n²) | Θ(n²) | Para cada um dos \|w\| símbolos o cabeçote vai até o outro lado do # e volta (≈ 2n movimentos): Σ 2n·\|w\| = Θ(n²). Melhor caso: o primeiro símbolo não tem transição e a máquina para imediatamente. |
| Guardar o histórico | Θ(t·n) | Θ(t·n) | Θ(t·n) | Cada passo salva uma cópia da fita para a visualização. É custo do simulador, não da máquina: no w#w o simulador inteiro custa Θ(n³). |

A execução tem um limite de passos configurável (padrão 1000, máximo 100 000), pois o problema da parada é indecidível: não há como saber, em geral, se uma MT vai parar.
