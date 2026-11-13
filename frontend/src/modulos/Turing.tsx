import { useEffect, useMemo, useRef, useState, type FormEvent } from 'react'
import { exemplosTuring, simularTuring, type MaquinaExemplo, type ResultadoTuring, type TransicaoDto } from '../api'
import { Controles } from '../componentes/Controles'
import { DiagramaEstados, type Aresta } from '../componentes/DiagramaEstados'
import { FitaTuring } from '../componentes/FitaTuring'
import { usePlayer } from '../componentes/usePlayer'
import { Veredito } from '../componentes/Veredito'

const VAZIA: TransicaoDto = { estadoAtual: '', simboloLido: '', novoEstado: '', simboloEscrito: '', direcao: 'R' }

/** Rótulo das setas no formato dos livros: lido/escrito,direção. */
const paraArestas = (ts: TransicaoDto[]): Aresta[] =>
  ts.map(t => ({ de: t.estadoAtual, para: t.novoEstado, rotulo: `${t.simboloLido}/${t.simboloEscrito},${t.direcao}` }))

export function Turing() {
  const [exemplos, setExemplos] = useState<MaquinaExemplo[]>([])
  const [maquinaId, setMaquinaId] = useState('soma-unaria')
  const [entrada, setEntrada] = useState('000100')
  const [estadoInicial, setEstadoInicial] = useState('q0')
  const [estadosFinais, setEstadosFinais] = useState('qf')
  const [limite, setLimite] = useState(1000)
  const [transicoes, setTransicoes] = useState<TransicaoDto[]>([])
  const [mostrarTabela, setMostrarTabela] = useState(false)
  const [res, setRes] = useState<{ r: ResultadoTuring; transicoes: TransicaoDto[]; inicial: string; finais: string[] } | null>(null)
  const [erro, setErro] = useState('')
  const [carregando, setCarregando] = useState(false)

  useEffect(() => {
    exemplosTuring()
      .then(lista => {
        setExemplos(lista)
        carregar(lista[0])
      })
      .catch(e => setErro((e as Error).message))
  }, [])

  function carregar(m: MaquinaExemplo) {
    setMaquinaId(m.id)
    setEstadoInicial(m.estadoInicial)
    setEstadosFinais(m.estadosFinais.join(', '))
    setTransicoes(m.transicoes)
    setEntrada(m.exemplos[0])
    setRes(null)
    setErro('')
  }

  /** Máquina em branco para o usuário montar as próprias regras. */
  function personalizar() {
    setMaquinaId('personalizada')
    setEstadoInicial('q0')
    setEstadosFinais('qf')
    setTransicoes([{ ...VAZIA }])
    setEntrada('')
    setMostrarTabela(true)
    setRes(null)
    setErro('')
  }

  const exemploAtual = exemplos.find(m => m.id === maquinaId)

  async function simular(e?: FormEvent, entradaForcada?: string) {
    e?.preventDefault()
    const cadeia = entradaForcada ?? entrada
    setCarregando(true)
    setErro('')
    try {
      const r = await simularTuring({ entrada: cadeia, estadoInicial, estadosFinais, limitePassos: limite, transicoes })
      setRes({ r, transicoes, inicial: estadoInicial, finais: estadosFinais.split(',').map(s => s.trim()) })
    } catch (err) {
      setRes(null)
      setErro((err as Error).message)
    } finally {
      setCarregando(false)
    }
  }

  /** Apaga todas as regras da tabela. */
  function limparTransicoes() {
    if (!window.confirm(`Apagar as ${transicoes.length} transições da tabela?`)) return
    setMaquinaId('personalizada')
    setTransicoes([])
    setRes(null)
  }

  function alterar(i: number, campo: keyof TransicaoDto, valor: string) {
    setMaquinaId('personalizada')
    setTransicoes(ts => ts.map((t, j) => (j === i ? { ...t, [campo]: valor } : t)))
  }

  return (
    <div className="modulo">
      <form className="painel-definicao" onSubmit={simular}>
        <h2>Máquina de Turing</h2>
        <fieldset className="escolha-maquina">
          <legend>Máquina</legend>
          {exemplos.map(m => (
            <label key={m.id} className="opcao-maquina">
              <input type="radio" name="maquina" checked={maquinaId === m.id} onChange={() => carregar(m)} />
              <span>
                <strong>{m.nome}</strong>
                <small>{m.descricao}</small>
              </span>
            </label>
          ))}
          <label className="opcao-maquina">
            <input type="radio" name="maquina" checked={maquinaId === 'personalizada'} onChange={personalizar} />
            <span>
              <strong>Personalizada</strong>
              <small>Monte sua própria máquina escrevendo as regras de transição. O branco da fita é B.</small>
            </span>
          </label>
        </fieldset>

        <label>
          Entrada
          <input className="mono" value={entrada} onChange={e => setEntrada(e.target.value)} spellCheck={false} placeholder="000100" />
        </label>
        {exemploAtual && (
          <div className="exemplos" aria-label="Entradas de exemplo">
            {exemploAtual.exemplos.map(x => (
              <button type="button" key={x} className="mono" onClick={() => { setEntrada(x); simular(undefined, x) }}>{x}</button>
            ))}
          </div>
        )}

        <div className="tripla">
          <label>Estado inicial<input className="mono" value={estadoInicial} onChange={e => { setEstadoInicial(e.target.value); setMaquinaId('personalizada') }} /></label>
          <label>Estados finais<input className="mono" value={estadosFinais} onChange={e => { setEstadosFinais(e.target.value); setMaquinaId('personalizada') }} /></label>
          <label>Limite de passos<input type="number" min={1} max={100000} value={limite} onChange={e => setLimite(Number(e.target.value))} /></label>
        </div>

        <button type="button" className="link" onClick={() => setMostrarTabela(v => !v)} aria-expanded={mostrarTabela}>
          {mostrarTabela ? 'Ocultar' : 'Ver'} regras de transição δ ({transicoes.length})
        </button>

        {mostrarTabela && (
          <div className="tabela-transicoes">
            <table>
              <thead>
                <tr><th>Estado</th><th>Lê</th><th>Vai para</th><th>Escreve</th><th>Move</th><th /></tr>
              </thead>
              <tbody>
                {transicoes.map((t, i) => (
                  <tr key={i}>
                    <td><input className="mono" value={t.estadoAtual} onChange={e => alterar(i, 'estadoAtual', e.target.value)} aria-label={`Estado atual da transição ${i + 1}`} /></td>
                    <td><input className="mono curto" maxLength={1} value={t.simboloLido} onChange={e => alterar(i, 'simboloLido', e.target.value)} aria-label="Símbolo lido" /></td>
                    <td><input className="mono" value={t.novoEstado} onChange={e => alterar(i, 'novoEstado', e.target.value)} aria-label="Novo estado" /></td>
                    <td><input className="mono curto" maxLength={1} value={t.simboloEscrito} onChange={e => alterar(i, 'simboloEscrito', e.target.value)} aria-label="Símbolo escrito" /></td>
                    <td>
                      <select value={t.direcao} onChange={e => alterar(i, 'direcao', e.target.value)} aria-label="Direção">
                        <option value="R">R</option>
                        <option value="L">L</option>
                        <option value="S">S</option>
                      </select>
                    </td>
                    <td>
                      <button type="button" className="remover" aria-label={`Remover transição ${i + 1}`}
                        onClick={() => { setMaquinaId('personalizada'); setTransicoes(ts => ts.filter((_, j) => j !== i)) }}>✕</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            <div className="acoes-tabela">
              <button type="button" onClick={() => { setMaquinaId('personalizada'); setTransicoes(ts => [...ts, { ...VAZIA }]) }}>
                Adicionar transição
              </button>
              <button type="button" className="perigo" onClick={limparTransicoes} disabled={transicoes.length === 0}>
                Limpar transições
              </button>
            </div>
          </div>
        )}

        <button type="submit" className="primario" disabled={carregando}>
          {carregando ? 'Executando…' : 'Executar máquina'}
        </button>
      </form>

      <section className="area-trabalho" aria-live="polite">
        {erro && <Veredito tipo="erro" titulo="Não foi possível executar">{erro}</Veredito>}
        {!res && !erro && (
          <>
            <div className="vazio">
              <p>Clique em <b>Executar máquina</b> para ver a fita sendo processada.</p>
            </div>
            {transicoes.length > 0 && (
              <div className="bloco">
                <h3>Diagrama de estados</h3>
                <DiagramaEstados arestas={paraArestas(transicoes)} estadoInicial={estadoInicial} estadosFinais={estadosFinais.split(',').map(s => s.trim())} />
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
  fita: string
  cabecote: number
  lido: string
  transicao: string | null
  de?: string
  para?: string
  rotuloAresta?: string
}

function Execucao({ r, transicoes, inicial, finais }: { r: ResultadoTuring; transicoes: TransicaoDto[]; inicial: string; finais: string[] }) {
  const quadros: Quadro[] = useMemo(() => [
    ...r.passos.map(p => ({
      rotulo: `Passo ${p.numero + 1}`,
      estado: p.estado,
      fita: p.fita,
      cabecote: p.cabecote,
      lido: p.simboloLido,
      transicao: `δ(${p.transicao.estadoAtual}, ${p.transicao.simboloLido}) = (${p.transicao.novoEstado}, ${p.transicao.simboloEscrito}, ${p.transicao.movimentoSigla})`,
      de: p.transicao.estadoAtual,
      para: p.transicao.novoEstado,
      rotuloAresta: `${p.transicao.simboloLido}/${p.transicao.simboloEscrito},${p.transicao.movimentoSigla}`,
    })),
    {
      rotulo: 'Configuração final',
      estado: r.estadoFinal,
      fita: r.fitaFinal,
      cabecote: r.cabecoteFinal,
      lido: r.fitaFinal[r.cabecoteFinal] ?? 'B',
      transicao: null,
    },
  ], [r])

  const arestas = useMemo(() => paraArestas(transicoes), [transicoes])
  const player = usePlayer(quadros.length, r)
  const q = quadros[player.atual]
  const tabelaRef = useRef<HTMLTableSectionElement>(null)

  useEffect(() => {
    tabelaRef.current?.children[player.atual]?.scrollIntoView({ block: 'nearest' })
  }, [player.atual])

  return (
    <>
      <Veredito tipo={r.aceita ? 'aceita' : 'rejeita'} titulo={r.aceita ? 'Cadeia aceita' : 'Cadeia rejeitada'}>
        {r.motivo} {r.passos.length} transição(ões) aplicada(s). Conteúdo final da fita: <b className="mono">{r.saida || 'λ'}</b>
      </Veredito>

      <div className="bloco bloco-fita">
        <FitaTuring fita={q.fita} cabecote={q.cabecote} estado={q.estado} />
        <dl className="info-passo">
          <div><dt>{q.rotulo}</dt><dd>de {r.passos.length}</dd></div>
          <div><dt>Estado</dt><dd className="mono">{q.estado}</dd></div>
          <div><dt>Lido</dt><dd className="mono">{q.lido}</dd></div>
          <div><dt>Cabeçote</dt><dd className="mono">{q.cabecote}</dd></div>
          {q.transicao && <div className="largo"><dt>Aplicando</dt><dd className="mono">{q.transicao}</dd></div>}
        </dl>
        <Controles player={player} rotulo={`${player.atual} / ${quadros.length - 1}`} />
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
              <thead><tr><th>#</th><th>Estado</th><th>Lido</th><th>Transição</th></tr></thead>
              <tbody ref={tabelaRef}>
                {quadros.map((f, i) => (
                  <tr key={i} className={i === player.atual ? 'atual' : ''} onClick={() => player.irPara(i)}>
                    <td>{i === quadros.length - 1 ? 'fim' : i + 1}</td>
                    <td className="mono">{f.estado}</td>
                    <td className="mono">{f.lido}</td>
                    <td className="mono">{f.transicao ?? (r.aceita ? 'aceita' : 'parou')}</td>
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
