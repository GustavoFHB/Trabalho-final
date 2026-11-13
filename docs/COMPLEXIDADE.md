# Análise de complexidade

**O(f)** indica limite superior; **Ω(f)**, inferior; **Θ(f)**, os dois ao mesmo tempo.
Melhor e pior caso comparam entradas de mesmo tamanho. Caso médio depende de
uma distribuição: onde não há modelo probabilístico, a tabela dá um limite
válido para qualquer distribuição ou indica que não há média única. Custos de
tabelas hash são esperados; nomes de estados são limitados pela implementação.
A análise separa o núcleo da simulação do histórico retornado para a interface.

| Algoritmo | Melhor | Médio | Pior | Justificativa |
|---|---|---|---|---|
| M1 — validação em Σ* | Θ(n) | Θ(n) | Θ(n) | Visita toda a cadeia para listar posições inválidas. n é seu comprimento. |
| M1 — comprimento, concatenação e reverso | Θ(1), Θ(n+m), Θ(n) | Mesmos custos | Mesmos custos | Comprimento armazenado; concatenação copia os dois operandos; reverso percorre n símbolos. |
| M1 — gerar Σ* e Σ+ até k | Θ(k+W) | Θ(k+W) | Θ(k+W) | W é o tamanho total das cadeias efetivamente geradas, contando um custo constante por cadeia. O total teórico é calculado em Θ(k). Sem truncamento, W = Θ(k·sᵏ) para s = \|Σ\| ≥ 2 e k ≥ 1; para s = 1, W = Θ(k²). A listagem é limitada a 5000 resultados por fecho. |
| M1 — pertinência em L finita | Θ(n) | Θ(n) esperado | O(n·d) | Após validar a cadeia, consulta ao conjunto; seu hash custa Θ(n). d é o número de palavras na linguagem; colisões podem exigir até d comparações. Exclui montagem do conjunto. |
| M1 — pertinência por regex | Ω(n) | Depende do padrão e das entradas | Pode ser exponencial sem timeout | A validação lê a cadeia inteira. O motor usa backtracking; a implementação aplica timeout de 1 s ao casamento. Não há uma média única para regex fornecidas pelo usuário. |
| M2 — AFD, após indexar regras | Θ(1) | Θ(n) no modelo descrito abaixo | Θ(n) | Um estado e no máximo uma regra por símbolo; histórico de tamanho linear. Uma falha imediata permite rejeição no primeiro símbolo. |
| M2 — AFN, após indexar regras | Ω(1) | O((n+1)(q+t)) | O((n+1)(q+t)) | Conjunto de estados ativos e busca em largura no fecho λ. Por símbolo, visita até q estados e t regras. Fecho inicial também conta. |
| M3 — variáveis anuláveis | Θ(g) | O(v·g) | O(v·g) | Em cada rodada, percorre produções de tamanho total g (incluindo lados esquerdos). Há no máximo v+1 rodadas, para v variáveis. |
| M3 — reconhecimento de Earley | Θ(1) | O(n³), com gramática fixa | O(n³), com gramática fixa | Um terminal inválido na primeira posição é detectado antes de criar os conjuntos. Para entradas válidas, há n+1 conjuntos com O(n) itens cada; completar um item pode percorrer O(n) candidatos. |
| M3 — reconstruir árvore e derivação | Θ(T) e O(p·f) | Mesmos limites em função da saída | Mesmos limites em função da saída | T é o número de nós reconstruídos. A derivação usa p expansões e percorre ou copia formas sentenciais de tamanho máximo f. |
| M4 — AP, sem histórico | Θ(1) | Θ(n) no modelo descrito abaixo | Θ(n) | Operações de pilha em O(1) amortizado por símbolo; rejeição antecipada se a ordem ou a pilha não permitem uma transição. |
| M4 — AP, com histórico | Θ(1) | Θ(n²) no modelo descrito abaixo | Θ(n²) | Cada passo copia a pilha. Para aⁿᐟ²bⁿᐟ², a soma das alturas cresce quadraticamente. |
| M5 — MT de soma unária, passos da máquina | Θ(n) | Θ(n) | Θ(n) | Considerando somente entradas bem formadas: uma varredura, exatamente n+2 transições. |
| M5 — MT de w#w, passos da máquina | Θ(1) | Θ(n²) no modelo descrito abaixo | Θ(n²) | Falha imediata se a primeira leitura não tiver regra. Para duas metades iguais, há Θ(n) idas e voltas de Θ(n) passos. |
| M5 — fita e histórico | Θ(n) quando t=0 | O((t+1)(n+t)) | O((t+1)(n+t)) | O simulador copia a fita em cada transição e ao final. Após t passos, a fita tem até n+t+2 células; inserir à esquerda em lista também pode deslocar toda a fita. |

Para ilustrar **caso médio** com hipótese explícita, suponha em cada tamanho
admissível n que metade das entradas chegue ao fim com sucesso e metade falhe
logo no primeiro símbolo. No AFD, considere um autômato que aceite cadeias de
comprimento arbitrário, por exemplo a*. No AP, considere tamanhos pares; na MT
de w#w, ímpares. A média é proporcional ao custo dos casos aceitos: Θ(n) para
o AFD, Θ(n) para o AP sem histórico, Θ(n²) para o AP com histórico e Θ(n²)
passos para w#w. Outra distribuição pode mudar esses resultados.

No **AF**, q é o número de estados e t o número de transições cadastradas. A
validação e a indexação usam Θ(q+s+t) tempo e espaço esperados, onde s = |Σ|.
O histórico do AFD ocupa O(n); o do AFN, O((n+1)(q+t)), incluindo regras
percorridas. No **AP**, a pilha ocupa O(n), mas o histórico pode ocupar Θ(n²).
As cotas do núcleo contam a cadeia já normalizada; ler a requisição JSON e
materializar a entrada do usuário tem custo adicional linear em seu tamanho.

Na **MT**, t denota transições executadas, não regras cadastradas. O histórico
custa Θ(n²) para soma unária e Θ(n³) no pior caso de w#w, pois nessas máquinas
a fita tem Θ(n) células durante a execução. O limite de passos do simulador
não demonstra que a máquina entraria em loop.
