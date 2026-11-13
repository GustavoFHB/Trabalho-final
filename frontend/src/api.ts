export class ErroApi extends Error {}

async function post<T>(url: string, corpo: unknown): Promise<T> {
  let resp: Response
  try {
    resp = await fetch(url, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(corpo),
    })
  } catch {
    throw new ErroApi('Não foi possível falar com a API. Confira se "dotnet run" está rodando na porta 5080.')
  }
  const dados = await resp.json().catch(() => ({}))
  if (!resp.ok) throw new ErroApi(dados.erro ?? `A API respondeu com erro ${resp.status}.`)
  return dados as T
}

async function get<T>(url: string): Promise<T> {
  const resp = await fetch(url).catch(() => {
    throw new ErroApi('Não foi possível falar com a API. Confira se "dotnet run" está rodando na porta 5080.')
  })
  if (!resp.ok) throw new ErroApi(`A API respondeu com erro ${resp.status}.`)
  return resp.json()
}

export interface ValidacaoCadeia {
  cadeia: string
  valida: boolean
  tamanho: number
  posicoesInvalidas: number[]
  mensagem: string
}

export interface Fecho {
  notacao: string
  comprimentoMaximo: number
  totalTeorico: number
  truncado: boolean
  cadeias: string[]
}

export interface AnaliseLinguagem {
  alfabeto: string[]
  alfabetoTexto: string
  cadeia: ValidacaoCadeia
  cadeia2: ValidacaoCadeia | null
  concatenacao: { resultado: string; tamanho: number; valida: boolean }
  reverso: string
  fechoKleene: Fecho
  fechoPositivo: Fecho
  linguagem: null | {
    tipo: 'Finita' | 'Regular'
    definicao: string
    cadeiaPertence: boolean
    concatenacaoPertence: boolean
    cadeias: string[]
  }
}

export const analisarLinguagem = (req: {
  alfabeto: string
  cadeia: string
  cadeia2: string
  tamanhoMaximo: number
  tipoLinguagem: string
  linguagem: string
}) => post<AnaliseLinguagem>('/api/linguagens/analisar', req)

export interface AutomatoJson {
  estados?: string[]
  alfabeto?: string[]
  estadoInicial: string
  estadosFinais?: string[]
  transicoes?: { origem: string; simbolo: string | null; destino: string }[]
}

export interface AutomatoExemplo {
  id: string
  nome: string
  descricao: string
  exemplos: string[]
  automato: AutomatoJson
}

export interface TransicaoAf {
  origem: string
  simbolo: string | null
  destino: string
  simboloTexto: string
}

export interface PassoAutomato {
  numero: number
  posicao: number
  simbolo: string
  estadosAntes: string[]
  transicoes: TransicaoAf[]
  estadosDepois: string[]
}

export interface ResultadoAutomato {
  aceita: boolean
  deterministico: boolean
  mensagem: string
  cadeia: string
  simbolosLidos: number
  estadosIniciais: string[]
  transicoesIniciais: TransicaoAf[]
  passos: PassoAutomato[]
  estadosFinaisAlcancados: string[]
  caminho: { estado: string; simbolo: string | null }[] | null
  caminhoTexto: string | null
}

export const exemplosAutomato = () => get<AutomatoExemplo[]>('/api/automatos/exemplos')

export const simularAutomato = (req: { automato: AutomatoJson; cadeia: string }) =>
  post<ResultadoAutomato>('/api/automatos/simular', req)

export interface NoArvore {
  simbolo: string
  filhos: NoArvore[]
}

export interface PassoDerivacao {
  numero: number
  formaSentencial: string
  producao: string | null
  posicao: number | null
}

export interface RespostaDerivacao {
  gramatica: {
    variaveis: string[]
    terminais: string[]
    simboloInicial: string
    producoes: string[]
    anulaveis: string[]
  }
  resultado: {
    aceita: boolean
    cadeia: string
    mensagem: string
    passos: PassoDerivacao[]
    arvore: NoArvore | null
    itensEarley: number
  }
}

export const derivar = (req: {
  producoes: string
  variaveis: string
  terminais: string
  simboloInicial: string
  cadeia: string
}) => post<RespostaDerivacao>('/api/gramaticas/derivar', req)

export type CriterioAceitacao = 'EstadoFinal' | 'PilhaVazia'

export interface TransicaoPilhaDto {
  origem: string
  simbolo: string
  desempilha: string
  destino: string
  empilha: string
}

export interface AutomatoPilhaExemplo {
  id: string
  nome: string
  descricao: string
  estadoInicial: string
  estadosFinais: string[]
  simboloInicialPilha: string
  criterio: CriterioAceitacao
  exemplos: string[]
  transicoes: TransicaoPilhaDto[]
}

export interface PassoPilha {
  numero: number
  estado: string
  posicao: number
  restante: string
  leitura: string
  pilha: string
  transicao: { origem: string; simboloTexto: string; desempilhaTexto: string; destino: string; empilhaTexto: string }
  operacao: string
  novoEstado: string
  pilhaDepois: string
}

export interface ResultadoPilha {
  aceita: boolean
  deterministico: boolean
  criterio: CriterioAceitacao
  mensagem: string
  cadeia: string
  configuracoesExploradas: number
  estadoFinal: string
  posicaoFinal: number
  pilhaFinal: string
  passos: PassoPilha[]
}

export const exemplosPilha = () => get<AutomatoPilhaExemplo[]>('/api/pilha/exemplos')

export const simularPilha = (req: {
  entrada: string
  estadoInicial: string
  estadosFinais: string
  simboloInicialPilha: string
  criterio: CriterioAceitacao
  limiteConfiguracoes: number
  transicoes: TransicaoPilhaDto[]
}) => post<ResultadoPilha>('/api/pilha/simular', req)

export interface TransicaoDto {
  estadoAtual: string
  simboloLido: string
  novoEstado: string
  simboloEscrito: string
  direcao: string
}

export interface MaquinaExemplo {
  id: string
  nome: string
  descricao: string
  estadoInicial: string
  estadosFinais: string[]
  exemplos: string[]
  transicoes: TransicaoDto[]
}

export interface TransicaoResposta {
  estadoAtual: string
  simboloLido: string
  novoEstado: string
  simboloEscrito: string
  movimento: 'Esquerda' | 'Direita' | 'Parado'
  movimentoSigla: string
}

export interface PassoTuring {
  numero: number
  estado: string
  simboloLido: string
  fita: string
  cabecote: number
  transicao: TransicaoResposta
}

export interface ResultadoTuring {
  aceita: boolean
  motivo: string
  estadoFinal: string
  fitaFinal: string
  cabecoteFinal: number
  saida: string
  passos: PassoTuring[]
}

export const exemplosTuring = () => get<MaquinaExemplo[]>('/api/turing/exemplos')

export const simularTuring = (req: {
  entrada: string
  estadoInicial: string
  estadosFinais: string
  limitePassos: number
  transicoes: TransicaoDto[]
}) => post<ResultadoTuring>('/api/turing/simular', req)
