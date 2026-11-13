interface Linha {
  algoritmo: string
  melhor: string
  medio: string
  pior: string
  nota: string
}

interface Grupo {
  titulo: string
  variaveis: string
  linhas: Linha[]
}

const GRUPOS: Grupo[] = [
  {
    titulo: 'Linguagens formais',
    variaveis: 'n = |w|, m = |v|, s = |Σ|, k = tamanho máximo listado',
    linhas: [
      { algoritmo: 'Validar w ∈ Σ*', melhor: 'Θ(n)', medio: 'Θ(n)', pior: 'Θ(n)', nota: 'Percorre a cadeia inteira para listar todas as posições inválidas; cada consulta ao HashSet de Σ é O(1).' },
      { algoritmo: 'Tamanho |w|', melhor: 'Θ(1)', medio: 'Θ(1)', pior: 'Θ(1)', nota: 'A string .NET guarda o próprio comprimento.' },
      { algoritmo: 'Concatenação w·v', melhor: 'Θ(n + m)', medio: 'Θ(n + m)', pior: 'Θ(n + m)', nota: 'Copia os dois operandos para uma nova string.' },
      { algoritmo: 'Enumerar Σ* e Σ+ até k', melhor: 'Θ(k+W)', medio: 'Θ(k+W)', pior: 'Θ(k+W)', nota: 'W é o tamanho total das cadeias geradas. Sem truncamento, Θ(k·sᵏ) para s ≥ 2; a API limita cada listagem a 5000 cadeias.' },
      { algoritmo: 'w ∈ L (L finita)', melhor: 'Θ(n)', medio: 'Θ(n)', pior: 'Θ(n)', nota: 'Validação em Σ* mais uma busca em HashSet, cujo hash custa O(n).' },
      { algoritmo: 'w ∈ L (L regular)', melhor: 'Ω(n)', medio: 'Depende do padrão', pior: 'Pode ser exponencial', nota: 'O motor de regex usa backtracking. Há timeout de 1 s no casamento.' },
    ],
  },
  {
    titulo: 'Autômatos finitos', variaveis: 'n = comprimento da cadeia; q = estados; t = regras',
    linhas: [
      { algoritmo: 'AFD (após indexar)', melhor: 'Θ(1)', medio: 'Θ(n)*', pior: 'Θ(n)', nota: 'Uma regra por símbolo; pode rejeitar no primeiro símbolo. *Modelo com proporção fixa de cadeias aceitas.' },
      { algoritmo: 'AFN com fecho λ (após indexar)', melhor: 'Ω(1)', medio: 'O((n+1)(q+t))', pior: 'O((n+1)(q+t))', nota: 'Propaga o conjunto de estados e percorre as transições vazias sem revisitar estados. Limite superior válido para qualquer distribuição.' },
    ],
  },
  {
    titulo: 'Autômato finito (AFD / AFN)',
    variaveis: 'n = |w|, |Q| = número de estados, |δ| = número de transições',
    linhas: [
      { algoritmo: 'Montar δ a partir do JSON', melhor: 'Θ(|Q| + |δ|)', medio: 'Θ(|Q| + |δ|)', pior: 'Θ(|Q| + |δ|)', nota: 'Cada transição entra num dicionário indexado por (estado, símbolo); Q e Σ ficam em HashSet.' },
      { algoritmo: 'Executar um AFD', melhor: 'Θ(1)', medio: 'Θ(n)', pior: 'Θ(n)', nota: 'Uma consulta O(1) a δ por símbolo. Melhor caso: não há transição para o 1º símbolo e o autômato trava.' },
      { algoritmo: 'Executar um AFN (subconjuntos)', melhor: 'Θ(1)', medio: 'O(n·(|Q| + |δ|))', pior: 'O(n·(|Q| + |δ|))', nota: 'Mantém o conjunto de estados ativos: a cada símbolo, cada estado do conjunto segue suas transições e depois calcula o fecho-λ (BFS). Evita a explosão 2^|Q| de converter para AFD.' },
      { algoritmo: 'Fecho-λ', melhor: 'Θ(|S|)', medio: 'O(|Q| + |δλ|)', pior: 'O(|Q| + |δλ|)', nota: 'Busca em largura só pelas transições λ; cada estado entra no conjunto uma vez.' },
      { algoritmo: 'Reconstruir o caminho', melhor: 'Θ(n)', medio: 'Θ(n + λ)', pior: 'Θ(n + λ)', nota: 'Cada estado guarda de onde veio; basta seguir os ponteiros de trás para a frente.' },
    ],
  },
  {
    titulo: 'Gramática livre de contexto (Earley)',
    variaveis: 'n = |w|, |G| = soma dos tamanhos das produções',
    linhas: [
      { algoritmo: 'Calcular anuláveis', melhor: 'Θ(|G|)', medio: 'O(|V|·|G|)', pior: 'O(|V|·|G|)', nota: 'Ponto fixo: cada rodada passa por todas as produções e marca ao menos uma variável nova.' },
      { algoritmo: 'Reconhecimento de Earley', melhor: 'Θ(1)', medio: 'O(n³)', pior: 'O(n³)', nota: 'Com gramática fixa. A validação pode rejeitar no primeiro símbolo; uma conclusão pode visitar O(n) itens.' },
      { algoritmo: 'Montar a árvore', melhor: 'Θ(t)', medio: 'Θ(t)', pior: 'Θ(t)', nota: 't = número de nós. Cada item guarda ponteiros para o item anterior e o filho que o completou.' },
      { algoritmo: 'Derivação mais à esquerda', melhor: 'O(p·f)', medio: 'O(p·f)', pior: 'O(p·f)', nota: 'p passos; em cada um busca a variável mais à esquerda numa forma sentencial de tamanho máximo f.' },
    ],
  },
  {
    titulo: 'Autômato com pilha', variaveis: 'n = comprimento da cadeia',
    linhas: [
      { algoritmo: 'Decidir aⁿbⁿ, sem histórico', melhor: 'Θ(1)', medio: 'Θ(n)*', pior: 'Θ(n)', nota: 'Uma operação de pilha por símbolo, com rejeição antecipada. *Modelo com proporção fixa de entradas aceitas.' },
      { algoritmo: 'Simulação com histórico', melhor: 'Θ(1)', medio: 'Θ(n²)*', pior: 'Θ(n²)', nota: 'O custo é a soma das alturas da pilha copiadas a cada passo. *Mesmo modelo do caso médio.' },
    ],
  },
  {
    titulo: 'Autômato com pilha',
    variaveis: 'n = |w|, t = movimentos, h = altura da pilha, c = configurações exploradas',
    linhas: [
      { algoritmo: 'Um movimento', melhor: 'Θ(1)', medio: 'O(h)', pior: 'O(h)', nota: 'Escolher a transição é O(1) por candidata; a pilha imutável é copiada (O(h)) para que cada ramo tenha a sua.' },
      { algoritmo: 'aⁿbⁿ (determinístico)', melhor: 'Θ(1)', medio: 'Θ(n)', pior: 'Θ(n)', nota: 'Exatamente n + 1 movimentos para cadeias aceitas (um por símbolo e um λ no fim). Melhor caso: a entrada começa com b e não há transição.' },
      { algoritmo: 'Simular AP não determinístico (BFS)', melhor: 'Θ(n)', medio: 'O(c·h)', pior: 'exponencial', nota: 'A busca em largura explora todas as configurações (estado, posição, pilha) alcançáveis. Em palíndromos wwᴿ c = O(n²). Em geral c pode crescer exponencialmente, por isso há limite de configurações e de altura da pilha.' },
      { algoritmo: 'Registrar o histórico', melhor: 'Θ(t·h)', medio: 'Θ(t·h)', pior: 'Θ(t·h)', nota: 'Cada passo guarda uma cópia da pilha para a visualização; custo do simulador, não do autômato.' },
    ],
  },
  {
    titulo: 'Máquina de Turing',
    variaveis: 'n = |entrada|, t = passos executados',
    linhas: [
      { algoritmo: 'Um passo de δ', melhor: 'Θ(1)', medio: 'Θ(1)', pior: 'O(|δ|)', nota: 'Dicionário indexado por (estado, símbolo). A versão Java original fazia busca linear O(|δ|) em todo passo.' },
      { algoritmo: 'Soma unária (opção B)', melhor: 'Θ(n)', medio: 'Θ(n)', pior: 'Θ(n)', nota: 'Uma única varredura da esquerda para a direita: exatamente n + 2 passos.' },
      { algoritmo: 'Reconhecer w#w (opção A)', melhor: 'Θ(1)', medio: 'Θ(n²)*', pior: 'Θ(n²)', nota: '*Modelo com proporção fixa de entradas aceitas. Para cada um dos |w| símbolos a cabeça vai e volta pela fita (≈ 2n movimentos). Melhor caso: o primeiro símbolo não tem transição.' },
      { algoritmo: 'Registrar o histórico', melhor: 'Θ(n) se t=0', medio: 'O((t+1)(n+t))', pior: 'O((t+1)(n+t))', nota: 'Cada passo guarda uma cópia da fita para a visualização; custo do simulador, não da máquina.' },
    ],
  },
]

export function Complexidade() {
  return (
    <div className="modulo modulo-unico">
      <section className="area-trabalho">
        <h2>Complexidade computacional</h2>
        <p className="ajuda largura-texto">
          O(f) é um limite superior, Ω(f) um limite inferior e Θ(f) um limite justo (vale O e Ω ao mesmo tempo).
          O caso médio depende de uma distribuição de entradas. As hipóteses e os custos de preparação estão no documento de complexidade do projeto.
        </p>
        {GRUPOS.map(g => (
          <div className="bloco" key={g.titulo}>
            <h3>{g.titulo}</h3>
            <p className="ajuda">{g.variaveis}</p>
            <div className="tabela-complexidade">
              <table>
                <thead>
                  <tr><th>Algoritmo</th><th>Melhor</th><th>Médio</th><th>Pior</th><th>Por quê</th></tr>
                </thead>
                <tbody>
                  {g.linhas.map(l => (
                    <tr key={l.algoritmo}>
                      <th scope="row">{l.algoritmo}</th>
                      <td className="mono">{l.melhor}</td>
                      <td className="mono">{l.medio}</td>
                      <td className="mono">{l.pior}</td>
                      <td>{l.nota}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        ))}
      </section>
    </div>
  )
}
