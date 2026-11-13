import { useEffect, useMemo, useRef, useState, type ChangeEvent, type FormEvent } from 'react'
import { simularAutomato, type AutomatoJson, type ResultadoAutomato, type TransicaoAf } from '../api'
import { Controles } from '../componentes/Controles'
import { DiagramaEstados, type Aresta } from '../componentes/DiagramaEstados'
import { FitaEntrada } from '../componentes/FitaEntrada'
import { usePlayer } from '../componentes/usePlayer'
import { Veredito } from '../componentes/Veredito'
import { rolarDentro } from '../componentes/rolagem'

type Modo = 'json' | 'personalizada'

interface LinhaTransicao {
  origem: string
  simbolo: string
  destino: string
}

const VAZIAS = ['', 'λ', 'ε', '&']
const LINHA_VAZIA: LinhaTransicao = { origem: '', simbolo: '', destino: '' }

const JSON_INICIAL: AutomatoJson = {
  estados: ['q0', 'q1', 'q2'],
  alfabeto: ['a', 'b'],
  estadoInicial: 'q0',
  estadosFinais: ['q2'],
  transicoes: [
    { origem: 'q0', simbolo: 'a', destino: 'q1' },
    { origem: 'q1', simbolo: 'b', destino: 'q2' },
  ],
}

const texto = (v: unknown) => (typeof v === 'string' ? v.trim() : '')
const rotuloSimbolo = (s: unknown) => (VAZIAS.includes(texto(s)) ? 'λ' : texto(s))
const conjunto = (estados: string[]) => (estados.length === 0 ? '∅' : estados.length === 1 ? estados[0] : `{${estados.join(', ')}}`)
const lista = (v: string) => v.split(',').map(s => s.trim()).filter(Boolean)

function lerJson(fonte: string): { automato?: AutomatoJson; erro?: string } {
  try {
    const a = JSON.parse(fonte)
    if (!a || typeof a !== 'object' || Array.isArray(a)) return { erro: 'O JSON precisa ser um objeto: { "estadoInicial": ..., "transicoes": [...] }.' }
    return { automato: a as AutomatoJson }
  } catch (e) {
    return { erro: `JSON inválido: ${(e as Error).message}` }
  }
}

function montarAutomato(inicial: string, finais: string, alfabeto: string, linhas: LinhaTransicao[]): AutomatoJson {
  const sigma = lista(alfabeto)
  return {
    ...(sigma.length ? { alfabeto: sigma } : {}),
    estadoInicial: inicial.trim(),
    estadosFinais: lista(finais),
    transicoes: linhas
      .filter(t => t.origem.trim() || t.simbolo.trim() || t.destino.trim())
      .map(t => ({ origem: t.origem.trim(), simbolo: t.simbolo.trim() || 'λ', destino: t.destino.trim() })),
  }
}

function desenho(a: AutomatoJson) {
  const transicoes = Array.isArray(a.transicoes) ? a.transicoes : []
  const arestas: Aresta[] = transicoes
    .filter(t => t && texto(t.origem) && texto(t.destino))
    .map(t => ({ de: texto(t.origem), para: texto(t.destino), rotulo: rotuloSimbolo(t.simbolo) }))
  return {
    arestas,
    estados: (Array.isArray(a.estados) ? a.estados : []).map(texto).filter(Boolean),
    inicial: texto(a.estadoInicial),
    finais: (Array.isArray(a.estadosFinais) ? a.estadosFinais : []).map(texto).filter(Boolean),
  }
}

export function AutomatoFinito() {
  const [modo, setModo] = useState<Modo>('json')
  const [json, setJson] = useState(() => JSON.stringify(JSON_INICIAL, null, 2))
  const [estadoInicial, setEstadoInicial] = useState('q0')
  const [estadosFinais, setEstadosFinais] = useState('qf')
  const [alfabeto, setAlfabeto] = useState('')
  const [linhas, setLinhas] = useState<LinhaTransicao[]>([{ ...LINHA_VAZIA }])
  const [entrada, setEntrada] = useState('ab')
  const [res, setRes] = useState<{ r: ResultadoAutomato; automato: AutomatoJson } | null>(null)
  const [erro, setErro] = useState('')
  const [carregando, setCarregando] = useState(false)
  const arquivoRef = useRef<HTMLInputElement>(null)

  function trocarModo(novo: Modo) {
    setModo(novo)
    setRes(null)
    setErro('')
  }

  async function abrirArquivo(e: ChangeEvent<HTMLInputElement>) {
    const arquivo = e.target.files?.[0]
    e.target.value = ''
    if (!arquivo) return
    const conteudo = await arquivo.text()
    const r = lerJson(conteudo)
    setRes(null)
    setJson(r.automato ? JSON.stringify(r.automato, null, 2) : conteudo)
    setErro(r.erro ? `${arquivo.name}: ${r.erro}` : '')
  }

  const lido = useMemo(() => lerJson(json), [json])
  const automatoPersonalizado = useMemo(
    () => montarAutomato(estadoInicial, estadosFinais, alfabeto, linhas),
    [estadoInicial, estadosFinais, alfabeto, linhas],
  )
  const automatoAtual = modo === 'json' ? lido.automato : automatoPersonalizado

  async function simular(e?: FormEvent) {
    e?.preventDefault()
    if (!automatoAtual) {
      setRes(null)
      setErro(lido.erro ?? 'JSON inválido.')
      return
    }
    setCarregando(true)
    setErro('')
    try {
      const automato = automatoAtual
      setRes({ r: await simularAutomato({ automato, cadeia: entrada }), automato })
    } catch (err) {
      setRes(null)
      setErro((err as Error).message)
    } finally {
      setCarregando(false)
    }
  }

  function alterar(i: number, campo: keyof LinhaTransicao, valor: string) {
    setLinhas(ls => ls.map((t, j) => (j === i ? { ...t, [campo]: valor } : t)))
  }

  function limparTransicoes() {
    if (!window.confirm(`Apagar as ${linhas.length} transições da tabela?`)) return
    setLinhas([])
    setRes(null)
  }

  return (
    <div className="modulo">
      <form className="painel-definicao" onSubmit={simular}>
        <h2>Autômato finito</h2>
        <fieldset className="escolha-maquina">
          <legend>Autômato</legend>
          <label className="opcao-maquina">
            <input type="radio" name="modo-af" checked={modo === 'json'} onChange={() => trocarModo('json')} />
            <span>
              <strong>JSON</strong>
              <small>Cole o autômato em JSON ou carregue um arquivo .json.</small>
            </span>
          </label>
          <label className="opcao-maquina">
            <input type="radio" name="modo-af" checked={modo === 'personalizada'} onChange={() => trocarModo('personalizada')} />
            <span>
              <strong>Personalizada</strong>
              <small>Monte seu próprio autômato escrevendo as transições. Deixe "Lê" vazio para uma transição λ.</small>
            </span>
          </label>
        </fieldset>

        {modo === 'json' ? (
          <>
            <label>
              Autômato em JSON
              <textarea
                className="mono editor-json"
                rows={12}
                value={json}
                onChange={e => setJson(e.target.value)}
                spellCheck={false}
                aria-invalid={!!lido.erro}
              />
            </label>
            {lido.erro && json && <p className="nota-erro">{lido.erro}</p>}
            <p className="ajuda">
              Para um AFN, repita a mesma origem e símbolo com destinos diferentes. Use <code>"λ"</code> como símbolo para uma transição vazia.
              <code>estados</code> e <code>alfabeto</code> são opcionais.
            </p>
            <div className="acoes-tabela">
              <button type="button" onClick={() => arquivoRef.current?.click()}>Carregar arquivo .json</button>
              <input ref={arquivoRef} type="file" accept=".json,application/json" hidden onChange={abrirArquivo} />
            </div>
          </>
        ) : (
          <>
            <div className="tripla">
              <label>Estado inicial<input className="mono" value={estadoInicial} onChange={e => setEstadoInicial(e.target.value)} /></label>
              <label>Estados finais<input className="mono" value={estadosFinais} onChange={e => setEstadosFinais(e.target.value)} /></label>
              <label>Alfabeto Σ<input className="mono" value={alfabeto} onChange={e => setAlfabeto(e.target.value)} placeholder="auto" /></label>
            </div>
            <p className="ajuda">Separe vários estados ou símbolos por vírgula. Σ em branco é inferido das transições.</p>

            <div className="tabela-transicoes">
              <table>
                <thead>
                  <tr><th>Estado</th><th>Lê</th><th>Vai para</th><th /></tr>
                </thead>
                <tbody>
                  {linhas.map((t, i) => (
                    <tr key={i}>
                      <td><input className="mono" value={t.origem} onChange={e => alterar(i, 'origem', e.target.value)} aria-label={`Estado de origem da transição ${i + 1}`} /></td>
                      <td><input className="mono curto" maxLength={1} value={t.simbolo} placeholder="λ" onChange={e => alterar(i, 'simbolo', e.target.value)} aria-label="Símbolo lido" /></td>
                      <td><input className="mono" value={t.destino} onChange={e => alterar(i, 'destino', e.target.value)} aria-label="Estado de destino" /></td>
                      <td>
                        <button type="button" className="remover" aria-label={`Remover transição ${i + 1}`}
                          onClick={() => setLinhas(ls => ls.filter((_, j) => j !== i))}>✕</button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
              <div className="acoes-tabela">
                <button type="button" onClick={() => setLinhas(ls => [...ls, { ...LINHA_VAZIA }])}>Adicionar transição</button>
                <button type="button" className="perigo" onClick={limparTransicoes} disabled={linhas.length === 0}>Limpar transições</button>
              </div>
            </div>
          </>
        )}

        <label>
          Entrada
          <input className="mono" value={entrada} onChange={e => setEntrada(e.target.value)} placeholder="ab (vazio = λ)" spellCheck={false} />
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
              <p>Escreva o autômato, informe a entrada e clique em <b>Executar autômato</b>.</p>
              <p className="mono exemplo">q0 –a→ q1 –b→ q2 com ab: q0 → q1 → q2, ACEITA</p>
            </div>
            {automatoAtual && <DiagramaPrevia automato={automatoAtual} />}
          </>
        )}
        {res && <ExecucaoAutomato {...res} />}
      </section>
    </div>
  )
}

function DiagramaPrevia({ automato }: { automato: AutomatoJson }) {
  const d = useMemo(() => desenho(automato), [automato])
  if (!d.inicial) return null
  return (
    <div className="bloco">
      <h3>Diagrama de estados</h3>
      <DiagramaEstados arestas={d.arestas} estados={d.estados} estadoInicial={d.inicial} estadosFinais={d.finais} />
    </div>
  )
}

interface Quadro {
  rotulo: string
  posicao: number
  lido: string | null
  antes: string[] | null
  estados: string[]
  transicoes: TransicaoAf[]
}

const descreverTransicao = (t: TransicaoAf) => `${t.origem} –${t.simboloTexto}→ ${t.destino}`

function ExecucaoAutomato({ r, automato }: { r: ResultadoAutomato; automato: AutomatoJson }) {
  const d = useMemo(() => desenho(automato), [automato])
  const quadros: Quadro[] = useMemo(() => [
    { rotulo: 'Início', posicao: 0, lido: null, antes: null, estados: r.estadosIniciais, transicoes: r.transicoesIniciais },
    ...r.passos.map(p => ({
      rotulo: `Passo ${p.numero}`,
      posicao: p.posicao + 1,
      lido: p.simbolo,
      antes: p.estadosAntes,
      estados: p.estadosDepois,
      transicoes: p.transicoes,
    })),
  ], [r])

  const player = usePlayer(quadros.length, r)
  const q = quadros[player.atual]
  const tabelaRef = useRef<HTMLTableSectionElement>(null)

  useEffect(() => {
    rolarDentro(tabelaRef.current?.closest<HTMLElement>('.historico'), tabelaRef.current?.children[player.atual])
  }, [player.atual])

  const temLambda = d.arestas.some(a => a.rotulo === 'λ')
  const tipo = r.deterministico ? 'AFD' : temLambda ? 'AFN-λ' : 'AFN'
  const w = r.cadeia || 'λ'

  return (
    <>
      <Veredito tipo={r.aceita ? 'aceita' : 'rejeita'} titulo={r.aceita ? `ACEITA: ${w} ∈ L(M)` : `REJEITADA: ${w} ∉ L(M)`}>
        {r.mensagem}
      </Veredito>

      {r.caminho && (
        <div className="bloco">
          <h3>{r.aceita ? 'Caminho de aceitação' : 'Caminho percorrido'}</h3>
          <ol className="caminho" aria-label={r.caminhoTexto ?? ''}>
            {r.caminho.map((c, i) => (
              <li key={i}>
                {c.simbolo !== null && <span className="caminho-simbolo">–{c.simbolo}→</span>}
                <span className={`caminho-estado ${d.finais.includes(c.estado) ? 'final' : ''}`}>{c.estado}</span>
              </li>
            ))}
          </ol>
        </div>
      )}

      <dl className="metricas">
        <div><dt>Tipo</dt><dd>{tipo}</dd></div>
        <div><dt>Símbolos lidos</dt><dd>{r.simbolosLidos} de {r.cadeia.length}</dd></div>
        <div><dt>Terminou em</dt><dd className="mono">{conjunto(quadros[quadros.length - 1].estados)}</dd></div>
      </dl>

      <div className="bloco bloco-fita">
        <FitaEntrada cadeia={r.cadeia} posicao={q.posicao} />
        <dl className="info-passo">
          <div><dt>{q.rotulo}</dt><dd>de {r.passos.length}</dd></div>
          {q.lido !== null && <div><dt>Leu</dt><dd className="mono">{q.lido}</dd></div>}
          {q.antes && <div><dt>De</dt><dd className="mono">{conjunto(q.antes)}</dd></div>}
          <div><dt>{r.deterministico ? 'Estado' : 'Estados'}</dt><dd className="mono">{conjunto(q.estados)}</dd></div>
          {q.transicoes.length > 0 && (
            <div className="largo"><dt>Transições</dt><dd className="mono">{q.transicoes.map(descreverTransicao).join('   ')}</dd></div>
          )}
        </dl>
        <Controles player={player} rotulo={`${player.atual} / ${quadros.length - 1}`} />
      </div>

      <div className="turing-grid">
        <div className="bloco">
          <h3>Diagrama de estados</h3>
          <DiagramaEstados
            arestas={d.arestas}
            estados={d.estados}
            estadoInicial={d.inicial}
            estadosFinais={d.finais}
            estadosAtuais={q.estados}
            ativas={q.transicoes.map(t => ({ de: t.origem, para: t.destino, rotulo: t.simboloTexto }))}
          />
        </div>
        <div className="bloco">
          <h3>Passo a passo</h3>
          <div className="historico">
            <table>
              <thead><tr><th>#</th><th>Lido</th><th>De</th><th>Transições</th><th>Para</th></tr></thead>
              <tbody ref={tabelaRef}>
                {quadros.map((f, i) => (
                  <tr key={i} className={i === player.atual ? 'atual' : ''} onClick={() => player.irPara(i)}>
                    <td>{i === 0 ? 'início' : i}</td>
                    <td className="mono">{f.lido ?? '—'}</td>
                    <td className="mono">{f.antes ? conjunto(f.antes) : '—'}</td>
                    <td className="mono">{f.transicoes.length ? f.transicoes.map(descreverTransicao).join(', ') : '—'}</td>
                    <td className="mono">{conjunto(f.estados)}</td>
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
