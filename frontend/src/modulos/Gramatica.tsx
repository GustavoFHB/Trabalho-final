import { useEffect, useRef, useState, type FormEvent } from 'react'
import { derivar, type RespostaDerivacao } from '../api'
import { ArvoreDerivacao } from '../componentes/ArvoreDerivacao'
import { Controles } from '../componentes/Controles'
import { usePlayer } from '../componentes/usePlayer'
import { Veredito } from '../componentes/Veredito'
import { rolarDentro } from '../componentes/rolagem'

const EXEMPLOS = [
  { nome: 'aⁿbⁿ', producoes: 'S -> aSb | λ', cadeia: 'aaabbb' },
  { nome: 'Palíndromos', producoes: 'S -> aSa | bSb | a | b | λ', cadeia: 'abbba' },
  { nome: 'Expressões', producoes: 'E -> E+T | T\nT -> T*F | F\nF -> (E) | x', cadeia: 'x+x*(x+x)' },
  { nome: 'Parênteses', producoes: 'S -> (S)S | λ', cadeia: '(()())' },
]

export function Gramatica() {
  const [producoes, setProducoes] = useState(EXEMPLOS[0].producoes)
  const [variaveis, setVariaveis] = useState('')
  const [terminais, setTerminais] = useState('')
  const [inicial, setInicial] = useState('')
  const [cadeia, setCadeia] = useState(EXEMPLOS[0].cadeia)
  const [res, setRes] = useState<RespostaDerivacao | null>(null)
  const [erro, setErro] = useState('')
  const [carregando, setCarregando] = useState(false)

  async function executar(e?: FormEvent) {
    e?.preventDefault()
    setCarregando(true)
    setErro('')
    try {
      setRes(await derivar({ producoes, variaveis, terminais, simboloInicial: inicial, cadeia }))
    } catch (err) {
      setRes(null)
      setErro((err as Error).message)
    } finally {
      setCarregando(false)
    }
  }

  function usarExemplo(i: number) {
    setProducoes(EXEMPLOS[i].producoes)
    setCadeia(EXEMPLOS[i].cadeia)
    setVariaveis('')
    setTerminais('')
    setInicial('')
    setRes(null)
  }

  return (
    <div className="modulo">
      <form className="painel-definicao" onSubmit={executar}>
        <h2>Gramática livre de contexto</h2>
        <p className="ajuda">Uma produção por linha, alternativas separadas por <code>|</code>. Use <code>λ</code> para a cadeia vazia.</p>

        <div className="exemplos" aria-label="Gramáticas de exemplo">
          {EXEMPLOS.map((ex, i) => (
            <button type="button" key={ex.nome} onClick={() => usarExemplo(i)}>{ex.nome}</button>
          ))}
        </div>

        <label>
          Produções P
          <textarea className="mono" rows={5} value={producoes} onChange={e => setProducoes(e.target.value)} spellCheck={false} />
        </label>
        <div className="tripla">
          <label>Variáveis V<input className="mono" value={variaveis} onChange={e => setVariaveis(e.target.value)} placeholder="auto" /></label>
          <label>Terminais T<input className="mono" value={terminais} onChange={e => setTerminais(e.target.value)} placeholder="auto" /></label>
          <label>Inicial<input className="mono" value={inicial} onChange={e => setInicial(e.target.value)} placeholder="auto" maxLength={1} /></label>
        </div>
        <p className="ajuda">Deixe em branco para inferir: V = lados esquerdos, T = demais símbolos, inicial = 1ª produção.</p>
        <label>
          Cadeia de entrada
          <input className="mono" value={cadeia} onChange={e => setCadeia(e.target.value)} placeholder="aaabbb (vazio = λ)" spellCheck={false} />
        </label>
        <button type="submit" className="primario" disabled={carregando}>
          {carregando ? 'Derivando…' : 'Derivar cadeia'}
        </button>
      </form>

      <section className="area-trabalho" aria-live="polite">
        {erro && <Veredito tipo="erro" titulo="Gramática inválida">{erro}</Veredito>}
        {!res && !erro && (
          <div className="vazio">
            <p>Escolha um exemplo ou escreva suas produções e clique em <b>Derivar cadeia</b>.</p>
            <p className="mono exemplo">S → aSb | λ com aaabbb: aceita</p>
          </div>
        )}
        {res && <ResultadoGramatica r={res} />}
      </section>
    </div>
  )
}

function ResultadoGramatica({ r }: { r: RespostaDerivacao }) {
  const { resultado, gramatica } = r
  const player = usePlayer(resultado.passos.length, r)
  const listaRef = useRef<HTMLOListElement>(null)

  useEffect(() => {
    rolarDentro(listaRef.current, listaRef.current?.children[player.atual])
  }, [player.atual])

  const cadeiaExibida = resultado.cadeia === '' ? 'λ' : resultado.cadeia

  return (
    <>
      <Veredito tipo={resultado.aceita ? 'aceita' : 'rejeita'} titulo={resultado.aceita ? `Aceita: ${cadeiaExibida} ∈ L(G)` : `Rejeitada: ${cadeiaExibida} ∉ L(G)`}>
        {resultado.mensagem} O reconhecedor de Earley criou {resultado.itensEarley} itens.
      </Veredito>

      <dl className="metricas">
        <div><dt>V</dt><dd className="mono">{`{${gramatica.variaveis.join(', ')}}`}</dd></div>
        <div><dt>T</dt><dd className="mono">{`{${gramatica.terminais.join(', ')}}`}</dd></div>
        <div><dt>Símbolo inicial</dt><dd className="mono">{gramatica.simboloInicial}</dd></div>
        <div><dt>Anuláveis</dt><dd className="mono">{gramatica.anulaveis.length ? `{${gramatica.anulaveis.join(', ')}}` : '∅'}</dd></div>
      </dl>

      {resultado.aceita && resultado.arvore && (
        <>
          <Controles player={player} rotulo={`passo ${player.atual} de ${resultado.passos.length - 1}`} />
          <div className="derivacao-grid">
            <div className="bloco">
              <h3>Derivação mais à esquerda</h3>
              <ol className="passos-derivacao" ref={listaRef}>
                {resultado.passos.map((p, i) => (
                  <li key={i} className={i === player.atual ? 'atual' : i > player.atual ? 'futuro' : ''} onClick={() => player.irPara(i)}>
                    <span className="seta">{i === 0 ? '' : '⇒'}</span>
                    <span className="mono forma">{p.formaSentencial}</span>
                    {p.producao && <span className="producao">{p.producao}</span>}
                  </li>
                ))}
              </ol>
            </div>
            <div className="bloco">
              <h3>Árvore de derivação</h3>
              <ArvoreDerivacao raiz={resultado.arvore} passo={player.atual} />
            </div>
          </div>
        </>
      )}
    </>
  )
}
