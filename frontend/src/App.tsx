import { useState } from 'react'
import { Complexidade } from './modulos/Complexidade'
import { Gramatica } from './modulos/Gramatica'
import { Linguagens } from './modulos/Linguagens'
import { Turing } from './modulos/Turing'

const ABAS = [
  { id: 'linguagens', nome: 'Linguagens formais', simbolo: 'Σ*', Componente: Linguagens },
  { id: 'gramatica', nome: 'Gramática', simbolo: 'S→aSb', Componente: Gramatica },
  { id: 'turing', nome: 'Máquina de Turing', simbolo: 'δ', Componente: Turing },
  { id: 'complexidade', nome: 'Complexidade', simbolo: 'Θ(n)', Componente: Complexidade },
] as const

type IdAba = (typeof ABAS)[number]['id']

function abaInicial(): IdAba {
  const hash = window.location.hash.slice(1)
  return ABAS.some(a => a.id === hash) ? (hash as IdAba) : 'linguagens'
}

export default function App() {
  const [aba, setAba] = useState<IdAba>(abaInicial)

  function trocar(id: IdAba) {
    setAba(id)
    history.replaceState(null, '', `#${id}`)
  }

  return (
    <>
      <header className="topo">
        <div className="marca">
          <span className="logo" aria-hidden>δ</span>
          <div>
            <h1>Simulador de Linguagens Formais</h1>
            <p>Aspectos Teóricos da Computação</p>
          </div>
        </div>
        <nav className="abas" role="tablist" aria-label="Módulos">
          {ABAS.map(a => (
            <button
              key={a.id}
              role="tab"
              aria-selected={aba === a.id}
              className="aba"
              onClick={() => trocar(a.id)}
            >
              <span className="aba-simbolo" aria-hidden>{a.simbolo}</span>
              {a.nome}
            </button>
          ))}
        </nav>
      </header>
      <main>
        {ABAS.map(({ id, Componente }) => (
          <div key={id} role="tabpanel" hidden={aba !== id}>
            <Componente />
          </div>
        ))}
      </main>
    </>
  )
}
