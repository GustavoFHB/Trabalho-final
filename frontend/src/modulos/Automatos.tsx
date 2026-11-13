import { useState, type FormEvent } from 'react'
import { simularFinito, simularPilha, type DefinicaoAF } from '../api'
import { Controles } from '../componentes/Controles'
import { DiagramaEstados } from '../componentes/DiagramaEstados'
import { usePlayer } from '../componentes/usePlayer'
import { Veredito } from '../componentes/Veredito'
import './automatos.css'

const exemplos: Record<'AFD' | 'AFN', DefinicaoAF> = {
  AFD: {
    tipo: 'AFD', estados: ['q0', 'q1', 'q2'], alfabeto: ['a', 'b'],
    estadoInicial: 'q0', estadosFinais: ['q2'],
    transicoes: [{ origem: 'q0', simbolo: 'a', destino: 'q1' }, { origem: 'q1', simbolo: 'b', destino: 'q2' }],
  },
  AFN: {
    tipo: 'AFN', estados: ['q0', 'q1', 'q2'], alfabeto: ['a', 'b'],
    estadoInicial: 'q0', estadosFinais: ['q2'],
    transicoes: [
      { origem: 'q0', simbolo: 'λ', destino: 'q1' },
      { origem: 'q1', simbolo: 'a', destino: 'q1' },
      { origem: 'q1', simbolo: 'a', destino: 'q2' },
      { origem: 'q1', simbolo: 'b', destino: 'q1' },
    ],
  },
}

export function AutomatoFinito() {
  const [json, setJson] = useState(JSON.stringify(exemplos.AFD, null, 2))
  const [cadeia, setCadeia] = useState('ab')
  const [res, setRes] = useState<{ modelo: DefinicaoAF; execucao: Awaited<ReturnType<typeof simularFinito>> } | null>(null)
  const [erro, setErro] = useState('')
  const [ocupado, setOcupado] = useState(false)
  const player = usePlayer(res?.execucao.passos.length ?? 0, res)
  const passo = res?.execucao.passos[player.atual]

  async function simular(e: FormEvent) {
    e.preventDefault()
    setErro('')
    setRes(null)
    setOcupado(true)
    try {
      let modelo: DefinicaoAF
      try { modelo = JSON.parse(json) } catch { throw new Error('O JSON está mal formatado. Confira aspas, vírgulas e chaves.') }
      const execucao = await simularFinito(modelo, cadeia)
      setRes({ modelo, execucao })
    } catch (e) {
      setErro((e as Error).message)
    } finally {
      setOcupado(false)
    }
  }

  return (
    <div className="modulo">
      <form className="painel-definicao" onSubmit={simular}>
        <h2>Autômato finito</h2>
        <p className="ajuda">Carregue ou edite o JSON do AFD ou AFN. No AFN, λ muda de estado sem consumir símbolo.</p>
        <fieldset disabled={ocupado}>
          <legend>Definição do autômato</legend>
          <div className="exemplos">
            {(['AFD', 'AFN'] as const).map(tipo => <button key={tipo} type="button" onClick={() => {
              setJson(JSON.stringify(exemplos[tipo], null, 2)); setCadeia(tipo === 'AFD' ? 'ab' : 'baa'); setRes(null); setErro('')
            }}>Exemplo {tipo}</button>)}
          </div>
          <label>Importar arquivo JSON<input type="file" accept=".json,application/json" onChange={async e => {
            const arquivo = e.currentTarget.files?.[0]
            e.currentTarget.value = ''
            if (!arquivo) return
            setRes(null)
            if (arquivo.size > 100000) { setErro('Use um arquivo de até 100 KB.'); return }
            try { setJson(await arquivo.text()); setErro('') } catch { setErro('Não foi possível ler o arquivo.') }
          }} /></label>
          <label>Autômato (JSON)<textarea className="mono" rows={15} spellCheck={false} value={json} onChange={e => { setJson(e.target.value); setRes(null) }} /></label>
          <label>Cadeia de entrada<input className="mono" value={cadeia} maxLength={1000} onChange={e => { setCadeia(e.target.value); setRes(null) }} /></label>
          <button type="submit" className="primario">{ocupado ? 'Simulando…' : 'Simular cadeia'}</button>
        </fieldset>
      </form>
      <section className="area-trabalho">
        <h2>Estados ativos</h2>
        {erro && <Veredito tipo="erro" titulo="Não foi possível simular">{erro}</Veredito>}
        {res && passo && <>
          <Veredito tipo={res.execucao.aceita ? 'aceita' : 'rejeita'} titulo={res.execucao.aceita ? 'ACEITA' : 'REJEITA'}>{res.execucao.mensagem}</Veredito>
          <Controles player={player} rotulo={`Passo ${player.atual + 1} de ${player.total}`} />
          <p>Posição: <strong>{passo.posicao}</strong> · Símbolo lido: <code>{passo.simbolo ?? '—'}</code></p>
          <p>Estados ativos: <strong>{passo.estadosAtivos.join(', ') || '∅'}</strong></p>
          <p className="ajuda">{passo.transicoes.map(t => `${t.origem} —${t.simbolo}→ ${t.destino}`).join(' · ') || 'Sem transições neste passo.'}</p>
          <DiagramaEstados transicoes={res.modelo.transicoes.map(t => ({
            estadoAtual: t.origem, novoEstado: t.destino, simboloLido: t.simbolo, simboloEscrito: '', direcao: '',
          }))} estados={res.modelo.estados} estadoInicial={res.modelo.estadoInicial}
            estadosFinais={res.modelo.estadosFinais} estadosAtivos={passo.estadosAtivos} somenteLeitura />
          <details><summary>Histórico completo</summary>
            <ol start={0}>{res.execucao.passos.map(p => <li key={p.posicao}>{p.simbolo ?? 'Início'} → {p.estadosAtivos.join(', ') || '∅'}</li>)}</ol>
          </details>
        </>}
        {!res && !erro && <p className="ajuda">Execute uma cadeia para acompanhar os estados e transições.</p>}
      </section>
    </div>
  )
}

export function AutomatoPilha() {
  const [cadeia, setCadeia] = useState('aaabbb')
  const [res, setRes] = useState<Awaited<ReturnType<typeof simularPilha>> | null>(null)
  const [erro, setErro] = useState('')
  const [ocupado, setOcupado] = useState(false)
  const player = usePlayer(res?.passos.length ?? 0, res)
  const passo = res?.passos[player.atual]

  async function simular(e: FormEvent) {
    e.preventDefault()
    setOcupado(true)
    setErro('')
    setRes(null)
    try { setRes(await simularPilha(cadeia)) } catch (e) { setErro((e as Error).message) }
    finally { setOcupado(false) }
  }

  return (
    <div className="modulo">
      <form className="painel-definicao" onSubmit={simular}>
        <h2>Autômato com pilha</h2>
        <p>L = {'{aⁿbⁿ | n ≥ 1}'}</p>
        <p className="ajuda">Empilha A para cada a em q0. O primeiro b muda para q1; cada b tira um A. Aceita se a cadeia terminar em q1 com a pilha vazia.</p>
        <label>Cadeia de entrada<input className="mono" value={cadeia} maxLength={1000} disabled={ocupado} onChange={e => { setCadeia(e.target.value); setRes(null); setErro('') }} /></label>
        <div className="exemplos">
          {['ab', 'aaabbb', 'aab', 'abab', 'λ'].map(c => <button type="button" key={c} disabled={ocupado} onClick={() => {
            setCadeia(c); setRes(null); setErro('')
          }}>{c}</button>)}
        </div>
        <button type="submit" className="primario" disabled={ocupado}>{ocupado ? 'Simulando…' : 'Simular cadeia'}</button>
      </form>
      <section className="area-trabalho">
        <h2>Pilha da execução</h2>
        {erro && <Veredito tipo="erro" titulo="Não foi possível simular">{erro}</Veredito>}
        {res && passo && <>
          <Veredito tipo={res.aceita ? 'aceita' : 'rejeita'} titulo={res.aceita ? 'ACEITA' : 'REJEITA'}>{res.mensagem}</Veredito>
          <Controles player={player} rotulo={`Passo ${player.atual + 1} de ${player.total}`} />
          <p>Posição: <strong>{passo.posicao}</strong> · Leitura: <code>{passo.simbolo ?? '—'}</code> · Estado: <strong>{passo.estado}</strong></p>
          <p>{passo.acao}</p>
          <div className="pilha-visual" aria-label={`Pilha do topo ao fundo: ${passo.pilha || 'vazia'}`}>
            <small>Topo</small>
            <div>{Array.from(passo.pilha, (s, i) => <span key={i}>{s}</span>)}<span>⊥</span></div>
            <small>⊥ = fundo lógico</small>
          </div>
          <details><summary>Histórico completo</summary>
            <ol start={0}>{res.passos.map((p, i) => <li key={i}>{p.simbolo ?? '—'} · {p.estado} · {p.acao} · {p.pilha}⊥</li>)}</ol>
          </details>
        </>}
        {!res && !erro && <p className="ajuda">Simule uma cadeia para ver cada operação de pilha.</p>}
      </section>
    </div>
  )
}
