# Simulador de Linguagens Formais e Modelos Computacionais

Projeto prático de **Aspectos Teóricos da Computação** (AV3). Implementa três módulos do enunciado em **C# (.NET 8)**, com interface web em **React** (ponto extra):

| Módulo | O que faz |
|---|---|
| **1. Linguagens formais** | Define Σ e L (conjunto finito ou expressão regular), valida w ∈ Σ*, calcula \|w\|, w·v, wᴿ e enumera Σ* e Σ+ até um tamanho k. |
| **3. Gramática livre de contexto** | Define V, T, P e S; reconhece a cadeia com o algoritmo de **Earley**, mostra a **derivação mais à esquerda passo a passo** e a **árvore de derivação**. |
| **5. Máquina de Turing** | **Opção A** (reconhece w#w) **e opção B** (soma unária), além de máquinas personalizadas. Fita, cabeçote, diagrama de estados e histórico animados. |
| **6. Complexidade** | Aba com a análise O / Ω / Θ (melhor, médio e pior caso) de cada algoritmo — detalhes em [docs/COMPLEXIDADE.md](docs/COMPLEXIDADE.md). |

O módulo 5 é uma evolução de um simulador Java/Spring anterior da dupla: as classes `Fita`, `Transicao`, `Passo`, `ResultadoSimulacao`, `SimulacaoRequest` e `MaquinaTuring` foram portadas para C#, com a função δ trocada de lista (busca linear a cada passo) para dicionário indexado por (estado, símbolo).

## Estrutura de pastas

O backend é dividido **por módulo**: cada módulo é um projeto próprio com seu domínio, serviços, contratos (JSON) e controller. A API só hospeda os módulos. Detalhes em [docs/ARQUITETURA.md](docs/ARQUITETURA.md).

```
backend/
├── SimuladorFormal.sln
├── src/
│   ├── SimuladorFormal.Api/                  # host: Program.cs registra os módulos
│   │   └── Infra/                            # erros de validação → HTTP 400
│   ├── SimuladorFormal.Compartilhado/        # λ e leitura de conjuntos de símbolos
│   └── Modulos/
│       ├── SimuladorFormal.Linguagens/       # Módulo 1
│       │   ├── Dominio/                      # Alfabeto, Linguagem, LinguagemFinita, LinguagemRegular, FabricaLinguagem
│       │   ├── Operacoes/                    # ValidadorCadeia, OperacoesCadeia, GeradorFecho
│       │   ├── Resultados/                   # ValidacaoCadeia, Fecho
│       │   ├── Contratos/                    # requisição e resposta da API
│       │   ├── Servicos/                     # AnaliseLinguagemService
│       │   └── Api/                          # LinguagensController
│       ├── SimuladorFormal.Gramaticas/       # Módulo 3
│       │   ├── Dominio/                      # Producao, GramaticaLivreContexto, ValidadorGramatica
│       │   ├── Leitura/                      # LeitorGramatica ("S -> aSb | λ")
│       │   ├── Analise/                      # VariaveisAnulaveis
│       │   ├── Earley/                       # ItemEarley, ConjuntoEarley, ReconhecedorEarley…
│       │   ├── Derivacao/                    # NoArvore, ConstrutorArvore, GeradorDerivacaoEsquerda…
│       │   ├── Contratos/  Servicos/  Api/
│       └── SimuladorFormal.Turing/           # Módulo 5
│           ├── Dominio/                      # Fita, Transicao, Direcao, FuncaoTransicao, MaquinaTuring
│           ├── Execucao/                     # Passo, ResultadoSimulacao
│           ├── Leitura/                      # LeitorTransicao
│           ├── Exemplos/                     # MaquinaSomaUnaria, MaquinaWCercaW, CatalogoMaquinas
│           ├── Contratos/  Servicos/  Api/
└── tests/SimuladorFormal.Tests/              # xUnit (36 testes), uma pasta por módulo
frontend/                                     # React + Vite + TypeScript
└── src/
    ├── modulos/                       # uma tela por módulo
    └── componentes/                   # fita, diagrama de estados, árvore, controles
docs/                                  # complexidade e arquitetura (UML)
exemplos/                              # requisições JSON prontas
```

## Como executar

Pré-requisitos: **.NET SDK 8+** e **Node.js 20+**.

### Opção 1 — tudo pela API (mais simples)

```bash
cd frontend
npm install
npm run build              # gera o front em backend/src/SimuladorFormal.Api/wwwroot

cd ../backend/src/SimuladorFormal.Api
dotnet run                 # abre em http://localhost:5080
```

### Opção 2 — desenvolvimento (recarregamento automático)

```bash
# terminal 1
cd backend/src/SimuladorFormal.Api
dotnet run                 # API em http://localhost:5080

# terminal 2
cd frontend
npm install
npm run dev                # front em http://localhost:5173 (encaminha /api para a 5080)
```

### Testes

```bash
cd backend
dotnet test
```

## Exemplos de entrada e saída

### Módulo 1 — Linguagens formais (exemplo do enunciado)

Entrada: Σ = `{a, b}`, w = `abba`

```
Cadeia válida para Σ = {a, b}
Tamanho |w| = 4
w·v (v = ab) = abbaab    |w·v| = 6
Σ* até tamanho 2 = { λ, a, b, aa, ab, ba, bb }
Σ+ até tamanho 2 = { a, b, aa, ab, ba, bb }
```

Via API:

```bash
curl -X POST http://localhost:5080/api/linguagens/analisar \
  -H "Content-Type: application/json" -d @exemplos/linguagem.json
```

### Módulo 3 — Gramática (exemplo do enunciado)

Entrada: `S -> aSb | λ`, cadeia `aaabbb`

```
Aceita
S ⇒ aSb ⇒ aaSbb ⇒ aaaSbbb ⇒ aaabbb

        S
      / | \
     a  S  b
      / | \
     a  S  b
      / | \
     a  S  b
        |
        λ
```

Cadeias como `aab` ou `abab` são rejeitadas com o ponto em que a análise falhou.

```bash
curl -X POST http://localhost:5080/api/gramaticas/derivar \
  -H "Content-Type: application/json" -d @exemplos/gramatica-anbn.json
```

### Módulo 5 — Máquina de Turing

**Opção B (soma unária)** — entrada `000100` (3 + 2):

```
Passo 1: q0, lê 0  →  δ(q0, 0) = (q0, 0, R)
...
Passo 4: q0, lê 1  →  δ(q0, 1) = (q1, 0, R)
...
Passo 8: q2, lê 0  →  δ(q2, 0) = (qf, B, S)
ACEITA — fita final: 00000   (8 passos)
```

**Opção A (w#w)** — `ab#ab` e `0110#0110` são aceitas; `ab#ba`, `abb#ab` são rejeitadas.

```bash
curl -X POST http://localhost:5080/api/turing/simular \
  -H "Content-Type: application/json" -d @exemplos/turing-soma-unaria.json
```

## Endpoints

| Método | Rota | Descrição |
|---|---|---|
| POST | `/api/linguagens/analisar` | Validação, tamanho, concatenação, Σ*, Σ+ e pertinência em L |
| POST | `/api/gramaticas/derivar` | Reconhecimento, derivação passo a passo e árvore |
| GET | `/api/turing/exemplos` | Máquinas prontas (soma unária e w#w) |
| POST | `/api/turing/simular` | Executa uma máquina e devolve todos os passos |

Erros de configuração (símbolo fora do alfabeto, gramática mal escrita, transição duplicada…) retornam **HTTP 400** com `{ "erro": "mensagem" }`.

## Convenções

- Símbolos são caracteres únicos. `λ`, `ε` ou `&` representam a cadeia vazia.
- Na Máquina de Turing o branco é `B`; direções `R`, `L` e `S` (parado).
- A MT para quando chega a um estado final (aceita), quando não há transição (rejeita) ou ao atingir o limite de passos.
