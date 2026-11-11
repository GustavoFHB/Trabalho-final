import { useState, type FormEvent } from 'react'
import { analisarLinguagem, type AnaliseLinguagem, type Fecho } from '../api'
import { Veredito } from '../componentes/Veredito'

export function Linguagens() {
  const [alfabeto, setAlfabeto] = useState('a, b')
  const [cadeia, setCadeia] = useState('abba')
  const [cadeia2, setCadeia2] = useState('ab')
  const [k, setK] = useState(3)
  const [tipo, setTipo] = useState<'finita' | 'regular'>('regular')
  const [linguagem, setLinguagem] = useState('')
  const [res, setRes] = useState<AnaliseLinguagem | null>(null)
  const [erro, setErro] = useState('')
  const [carregando, setCarregando] = useState(false)

  async function analisar(e?: FormEvent) {
    e?.preventDefault()
    setCarregando(true)
    setErro('')
    try {
      setRes(await analisarLinguagem({ alfabeto, cadeia, cadeia2, tamanhoMaximo: k, tipoLinguagem: tipo, linguagem }))
    } catch (err) {
      setRes(null)
      setErro((err as Error).message)
    } finally {
      setCarregando(false)
    }
  }

  return (
    <div className="modulo">
      <form className="painel-definicao" onSubmit={analisar}>
        <h2>Linguagens formais</h2>
        <p className="ajuda">Defina um alfabeto Σ e uma cadeia w. O simulador valida w, mede seu tamanho e enumera Σ* e Σ+.</p>

        <label>
          Alfabeto Σ
          <input value={alfabeto} onChange={e => setAlfabeto(e.target.value)} placeholder="a, b" spellCheck={false} />
        </label>
        <div className="dupla">
          <label>
            Cadeia w
            <input className="mono" value={cadeia} onChange={e => setCadeia(e.target.value)} placeholder="abba (vazio = λ)" spellCheck={false} />
          </label>
          <label>
            Cadeia v (para w·v)
            <input className="mono" value={cadeia2} onChange={e => setCadeia2(e.target.value)} placeholder="ab" spellCheck={false} />
          </label>
        </div>
        <label>
          Listar Σ* e Σ+ até tamanho k = {k}
          <input type="range" min={0} max={8} value={k} onChange={e => setK(Number(e.target.value))} />
        </label>

        <fieldset>
          <legend>Linguagem L ⊆ Σ* (opcional)</legend>
          <div className="segmentado" role="radiogroup">
            <button type="button" aria-pressed={tipo === 'regular'} onClick={() => setTipo('regular')}>
              Expressão regular
            </button>
            <button type="button" aria-pressed={tipo === 'finita'} onClick={() => setTipo('finita')}>
              Conjunto finito
            </button>
          </div>
          <input
            className="mono"
            value={linguagem}
            onChange={e => setLinguagem(e.target.value)}
            placeholder={tipo === 'regular' ? 'ex.: (a|b)*a' : 'ex.: ab, ba, λ'}
            spellCheck={false}
            aria-label="Definição de L"
          />
          <p className="ajuda">
            {tipo === 'regular'
              ? 'Uma regra que descreve as palavras de L: | é "ou", * é "repetir 0 ou mais vezes".'
              : 'As palavras de L, separadas por vírgula. Use λ para a cadeia vazia.'}
            {' '}Deixe em branco se não quiser analisar uma linguagem.
          </p>
        </fieldset>

        <button type="submit" className="primario" disabled={carregando}>
          {carregando ? 'Analisando…' : 'Analisar cadeia'}
        </button>
      </form>

      <section className="area-trabalho" aria-live="polite">
        {erro && <Veredito tipo="erro" titulo="Não foi possível analisar">{erro}</Veredito>}
        {!res && !erro && <Vazio />}
        {res && <ResultadoLinguagem r={res} />}
      </section>
    </div>
  )
}

function Vazio() {
  return (
    <div className="vazio">
      <p>Preencha Σ e w e clique em <b>Analisar cadeia</b>.</p>
      <p className="mono exemplo">Σ = {'{a, b}'} · w = abba → válida, |w| = 4</p>
    </div>
  )
}

function nomeLinguagem(l: NonNullable<AnaliseLinguagem['linguagem']>) {
  return l.tipo === 'Regular' ? `L(${l.definicao})` : `{${l.definicao.replace(/^\s*\{|\}\s*$/g, '')}}`
}

function ResultadoLinguagem({ r }: { r: AnaliseLinguagem }) {
  const w = r.cadeia
  return (
    <>
      <Veredito tipo={w.valida ? 'aceita' : 'rejeita'} titulo={w.valida ? `Cadeia válida para Σ = ${r.alfabetoTexto}` : 'Cadeia inválida para Σ'}>
        Tamanho |w| = {w.tamanho}
      </Veredito>

      {r.linguagem && (
        <Veredito
          tipo={r.linguagem.cadeiaPertence ? 'aceita' : 'rejeita'}
          titulo={`${w.cadeia || 'λ'} ${r.linguagem.cadeiaPertence ? '∈' : '∉'} L — a cadeia ${r.linguagem.cadeiaPertence ? 'pertence' : 'não pertence'} à linguagem`}
        >
          L = {nomeLinguagem(r.linguagem)}
          {!w.valida && '. Uma cadeia fora de Σ* nunca pertence a L.'}
        </Veredito>
      )}

      <div className="cadeia-simbolos" aria-label="Símbolos da cadeia">
        {w.cadeia.length === 0 ? (
          <span className="simbolo">λ</span>
        ) : (
          [...w.cadeia].map((c, i) => (
            <span key={i} className={`simbolo ${w.posicoesInvalidas.includes(i) ? 'invalido' : ''}`} title={`posição ${i + 1}`}>
              {c}
              <small>{i + 1}</small>
            </span>
          ))
        )}
      </div>
      {!w.valida && <p className="nota-erro">{w.mensagem}</p>}

      <dl className="metricas">
        <div><dt>Concatenação w·v</dt><dd className="mono">{r.concatenacao.resultado}</dd><dd className="sub">|w·v| = {r.concatenacao.tamanho}</dd></div>
        <div><dt>Reverso wᴿ</dt><dd className="mono">{r.reverso}</dd></div>
        <div><dt>|Σ|</dt><dd className="mono">{r.alfabeto.length}</dd></div>
      </dl>

      {r.linguagem && (
        <div className="bloco">
          <h3>Linguagem L = {nomeLinguagem(r.linguagem)}</h3>
          <p>
            w·v = {r.concatenacao.resultado} {r.linguagem.concatenacaoPertence ? '∈' : '∉'} L
          </p>
          <ListaCadeias titulo={`Cadeias de L com tamanho ≤ ${r.fechoKleene.comprimentoMaximo}`} cadeias={r.linguagem.cadeias} destaque={w.cadeia} />
        </div>
      )}

      <FechoBloco f={r.fechoKleene} destaque={w.cadeia} />
      <FechoBloco f={r.fechoPositivo} destaque={w.cadeia} />
    </>
  )
}

function FechoBloco({ f, destaque }: { f: Fecho; destaque: string }) {
  const descricao = f.notacao === 'Σ*' ? 'Fecho de Kleene: Σ⁰ ∪ Σ¹ ∪ Σ² ∪ …' : 'Fecho positivo: Σ¹ ∪ Σ² ∪ … (sem λ)'
  return (
    <div className="bloco">
      <h3>{f.notacao} até tamanho {f.comprimentoMaximo}</h3>
      <p className="ajuda">{descricao} — {f.totalTeorico.toLocaleString('pt-BR')} cadeias{f.truncado ? ` (mostrando as ${f.cadeias.length} primeiras)` : ''}.</p>
      <ListaCadeias cadeias={f.cadeias} destaque={destaque} />
    </div>
  )
}

function ListaCadeias({ titulo, cadeias, destaque }: { titulo?: string; cadeias: string[]; destaque: string }) {
  const alvo = destaque === '' ? 'λ' : destaque
  return (
    <>
      {titulo && <p className="ajuda">{titulo}</p>}
      {cadeias.length === 0 ? (
        <p className="ajuda">∅ — nenhuma cadeia nesse intervalo.</p>
      ) : (
        <ul className="lista-cadeias">
          {cadeias.map(c => (
            <li key={c} className={c === alvo ? 'atual' : ''}>{c}</li>
          ))}
        </ul>
      )}
    </>
  )
}
