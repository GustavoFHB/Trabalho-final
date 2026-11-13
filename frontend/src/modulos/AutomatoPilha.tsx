import { useEffect, useMemo, useRef, useState, type FormEvent } from 'react'
import { simularPilha, type CriterioAceitacao, type ResultadoPilha, type TransicaoPilhaDto } from '../api'
import { Controles } from '../componentes/Controles'
import { DiagramaEstados, type Aresta } from '../componentes/DiagramaEstados'
import { FitaEntrada } from '../componentes/FitaEntrada'
import { PilhaVisual } from '../componentes/PilhaVisual'
import { usePlayer } from '../componentes/usePlayer'
import { Veredito } from '../componentes/Veredito'
import { rolarDentro } from '../componentes/rolagem'

const VAZIA: TransicaoPilhaDto = { origem: '', simbolo: '', desempilha: '', destino: '', empilha: '' }
const ou = (v: string) => v.trim() || 'λ'
const exibir = (pilha: string) => pilha || 'λ'
const preenchida = (t: TransicaoPilhaDto) => [t.origem, t.simbolo, t.desempilha, t.destino, t.empilha].some(v => v.trim())

const paraArestas = (ts: TransicaoPilhaDto[]): Aresta[] =>
  ts.filter(t => t.origem.trim() && t.destino.trim())
    .map(t => ({ de: t.origem.trim(), para: t.destino.trim(), rotulo: `${ou(t.simbolo)},${ou(t.desempilha)}/${ou(t.empilha)}` }))

export function AutomatoPilha() {
  const [entrada, setEntrada] = useState('')
  const [estadoInicial, setEstadoInicial] = useState('q0')
  const [estadosFinais, setEstadosFinais] = useState('qf')
  const [simboloInicial, setSimboloInicial] = useState('Z')
  const [criterio, setCriterio] = useState<CriterioAceitacao>('EstadoFinal')
  const [limite, setLimite] = useState(10000)
  const [transicoes, setTransicoes] = useState<TransicaoPilhaDto[]>([{ ...VAZIA }])
  const [res, setRes] = useState<{ r: ResultadoPilha; transicoes: TransicaoPilhaDto[]; inicial: string; finais: string[] } | null>(null)
  const [erro, setErro] = useState('')
  const [carregando, setCarregando] = useState(false)

  const finaisLista = estadosFinais.split(',').map(s => s.trim()).filter(Boolean)
  const arestasPrevia = useMemo(() => paraArestas(transicoes), [transicoes])

  async function simular(e?: FormEvent) {
    e?.preventDefault()
    setCarregando(true)
    setErro('')
    try {
      const usadas = transicoes.filter(preenchida)
      const r = await simularPilha({
        entrada,
        estadoInicial,
        estadosFinais,
        simboloInicialPilha: simboloInicial,
        criterio,
        limiteConfiguracoes: limite,
        transicoes: usadas,
      })
      setRes({ r, transicoes: usadas, inicial: estadoInicial, finais: finaisLista })
    } catch (err) {
      setRes(null)
      setErro((err as Error).message)
    } finally {
      setCarregando(false)
    }
  }

  function limparTransicoes() {
    if (!window.confirm(`Apagar as ${transicoes.length} transições da tabela?`)) return
    setTransicoes([])
    setRes(null)
  }

  function alterar(i: number, campo: keyof TransicaoPilhaDto, valor: string) {
    setTransicoes(ts => ts.map((t, j) => (j === i ? { ...t, [campo]: valor } : t)))
  }

  return (
    <div className="modulo">
      <form className="painel-definicao" onSubmit={simular}>
        <h2>Autômato com pilha</h2>
        <p className="ajuda">
          Monte seu próprio AP escrevendo as transições. Deixe um campo vazio para não ler (λ), não desempilhar ou não empilhar.
        </p>

        <div className="tripla">
          <label>Estado inicial<input className="mono" value={estadoInicial} onChange={e => setEstadoInicial(e.target.value)} /></label>
          <label>Estados finais<input className="mono" value={estadosFinais} onChange={e => setEstadosFinais(e.target.value)} /></label>
          <label>Início da pilha<input className="mono" value={simboloInicial} maxLength={1} onChange={e => setSimboloInicial(e.target.value)} placeholder="Z" /></label>
        </div>

        <div className="dupla">
          <div className="campo">
            <span className="rotulo-campo" id="criterio-ap">Aceita por</span>
            <div className="segmentado" role="group" aria-labelledby="criterio-ap">
              <button type="button" aria-pressed={criterio === 'EstadoFinal'} onClick={() => setCriterio('EstadoFinal')}>Estado final</button>
              <button type="button" aria-pressed={criterio === 'PilhaVazia'} onClick={() => setCriterio('PilhaVazia')}>Pilha vazia</button>
            </div>
          </div>
          <label>Limite de configurações<input type="number" min={1} max={200000} value={limite} onChange={e => setLimite(Number(e.target.value))} /></label>
        </div>

        <div className="tabela-transicoes">
          <table>
            <thead>
              <tr><th>Estado</th><th>Lê</th><th>Desempilha</th><th>Vai para</th><th>Empilha</th><th /></tr>
            </thead>
            <tbody>
              {transicoes.map((t, i) => (
                <tr key={i}>
                  <td><input className="mono" value={t.origem} onChange={e => alterar(i, 'origem', e.target.value)} aria-label={`Estado de origem da transição ${i + 1}`} /></td>
                  <td><input className="mono curto" maxLength={1} value={t.simbolo} placeholder="λ" onChange={e => alterar(i, 'simbolo', e.target.value)} aria-label="Símbolo lido" /></td>
                  <td><input className="mono curto" maxLength={1} value={t.desempilha} placeholder="λ" onChange={e => alterar(i, 'desempilha', e.target.value)} aria-label="Símbolo desempilhado" /></td>
                  <td><input className="mono" value={t.destino} onChange={e => alterar(i, 'destino', e.target.value)} aria-label="Estado de destino" /></td>
                  <td><input className="mono medio" value={t.empilha} placeholder="λ" onChange={e => alterar(i, 'empilha', e.target.value)} aria-label="Cadeia empilhada" /></td>
                  <td>
                    <button type="button" className="remover" aria-label={`Remover transição ${i + 1}`}
                      onClick={() => setTransicoes(ts => ts.filter((_, j) => j !== i))}>✕</button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          <p className="ajuda">Em <b>Empilha</b>, o primeiro símbolo fica no topo: <code>AZ</code> deixa A sobre Z.</p>
          <div className="acoes-tabela">
            <button type="button" onClick={() => setTransicoes(ts => [...ts, { ...VAZIA }])}>Adicionar transição</button>
            <button type="button" className="perigo" onClick={limparTransicoes} disabled={transicoes.length === 0}>Limpar transições</button>
          </div>
        </div>

        <label>
          Entrada
          <input className="mono" value={entrada} onChange={e => setEntrada(e.target.value)} spellCheck={false} placeholder="aaabbb (vazio = λ)" />
        </label>

        <button type="submit" className="primario" disabled={carregando}>
          {carregando ? 'Executando…' : 'Executar autômato'}
        </button>
      </form>

      <section className="area-trabalho" aria-live="polite">
        {erro && <Veredito tipo="erro" titulo="Não foi possível executar">{erro}</Veredito>}
        {!res && !erro && (
          <>
            <div className="vazio">
              <p>Escreva as transições, informe a entrada e clique em <b>Executar autômato</b> para ver a leitura, o estado e a pilha a cada passo.</p>
            </div>
            {arestasPrevia.length > 0 && (
              <div className="bloco">
                <h3>Diagrama de estados</h3>
                <DiagramaEstados arestas={arestasPrevia} estadoInicial={estadoInicial} estadosFinais={finaisLista} />
              </div>
            )}
          </>
        )}
        {res && <Execucao {...res} />}
      </section>
    </div>
  )
}

interface Quadro {
  rotulo: string
  estado: string
  posicao: number
  pilha: string
  leitura: string | null
  transicao: string | null
  operacao: string | null
  de?: string
  para?: string
  rotuloAresta?: string
}

function Execucao({ r, transicoes, inicial, finais }: { r: ResultadoPilha; transicoes: TransicaoPilhaDto[]; inicial: string; finais: string[] }) {
  const quadros: Quadro[] = useMemo(() => [
    ...r.passos.map(p => ({
      rotulo: `Passo ${p.numero}`,
      estado: p.estado,
      posicao: p.posicao,
      pilha: p.pilha,
      leitura: p.leitura,
      transicao: `δ(${p.transicao.origem}, ${p.transicao.simboloTexto}, ${p.transicao.desempilhaTexto}) = (${p.transicao.destino}, ${p.transicao.empilhaTexto})`,
      operacao: p.operacao,
      de: p.transicao.origem,
      para: p.transicao.destino,
      rotuloAresta: `${p.transicao.simboloTexto},${p.transicao.desempilhaTexto}/${p.transicao.empilhaTexto}`,
    })),
    {
      rotulo: 'Configuração final',
      estado: r.estadoFinal,
      posicao: r.posicaoFinal,
      pilha: r.pilhaFinal,
      leitura: null,
      transicao: null,
      operacao: null,
    },
  ], [r])

  const arestas = useMemo(() => paraArestas(transicoes), [transicoes])
  const player = usePlayer(quadros.length, r)
  const q = quadros[player.atual]
  const tabelaRef = useRef<HTMLTableSectionElement>(null)

  useEffect(() => {
    rolarDentro(tabelaRef.current?.closest<HTMLElement>('.historico'), tabelaRef.current?.children[player.atual])
  }, [player.atual])

  const w = r.cadeia || 'λ'

  return (
    <>
      <Veredito tipo={r.aceita ? 'aceita' : 'rejeita'} titulo={r.aceita ? `ACEITA: ${w} ∈ L(M)` : `REJEITADA: ${w} ∉ L(M)`}>
        {r.mensagem}
      </Veredito>

      <dl className="metricas">
        <div><dt>Tipo</dt><dd>{r.deterministico ? 'Determinístico (APD)' : 'Não determinístico'}</dd></div>
        <div><dt>Aceitação</dt><dd>{r.criterio === 'EstadoFinal' ? 'Por estado final' : 'Por pilha vazia'}</dd></div>
        <div><dt>Movimentos</dt><dd>{r.passos.length}</dd></div>
        <div><dt>Configurações exploradas</dt><dd>{r.configuracoesExploradas}</dd></div>
      </dl>

      <div className="bloco bloco-ap">
        <div className="bloco-fita">
          <FitaEntrada cadeia={r.cadeia} posicao={q.posicao} />
          <dl className="info-passo">
            <div><dt>{q.rotulo}</dt><dd>de {r.passos.length}</dd></div>
            <div><dt>Estado</dt><dd className="mono">{q.estado}</dd></div>
            {q.leitura !== null && <div><dt>Leitura</dt><dd className="mono">{q.leitura}</dd></div>}
            <div><dt>Pilha</dt><dd className="mono">{exibir(q.pilha)}</dd></div>
            {q.operacao && <div><dt>Operação</dt><dd>{q.operacao}</dd></div>}
            {q.transicao && <div className="largo"><dt>Aplicando</dt><dd className="mono">{q.transicao}</dd></div>}
          </dl>
          <Controles player={player} rotulo={`${player.atual} / ${quadros.length - 1}`} />
        </div>
        <PilhaVisual conteudo={q.pilha} />
      </div>

      <div className="turing-grid">
        <div className="bloco">
          <h3>Diagrama de estados</h3>
          <DiagramaEstados
            arestas={arestas}
            estadoInicial={inicial}
            estadosFinais={finais}
            estadosAtuais={[q.estado]}
            ativas={q.de ? [{ de: q.de, para: q.para!, rotulo: q.rotuloAresta }] : undefined}
          />
        </div>
        <div className="bloco">
          <h3>Histórico</h3>
          <div className="historico">
            <table>
              <thead><tr><th>#</th><th>Estado</th><th>Leitura</th><th>Resta ler</th><th>Pilha</th><th>Transição</th><th>Operação</th></tr></thead>
              <tbody ref={tabelaRef}>
                {quadros.map((f, i) => (
                  <tr key={i} className={i === player.atual ? 'atual' : ''} onClick={() => player.irPara(i)}>
                    <td>{i === quadros.length - 1 ? 'fim' : i + 1}</td>
                    <td className="mono">{f.estado}</td>
                    <td className="mono">{f.leitura ?? '—'}</td>
                    <td className="mono">{r.cadeia.slice(f.posicao) || 'λ'}</td>
                    <td className="mono">{exibir(f.pilha)}</td>
                    <td className="mono">{f.transicao ?? (r.aceita ? 'aceita' : 'parou')}</td>
                    <td>{f.operacao ?? '—'}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </>
  )
}
