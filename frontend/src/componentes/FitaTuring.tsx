import { useEffect, useRef } from 'react'
import { centralizarDentro } from './rolagem'

export function FitaTuring({ fita, cabecote, estado }: { fita: string; cabecote: number; estado: string }) {
  const ref = useRef<HTMLDivElement>(null)

  useEffect(() => {
    centralizarDentro(ref.current, ref.current?.querySelector('.celula.cabecote'))
  }, [cabecote, fita])

  return (
    <div className="fita-wrap" ref={ref}>
      <div className="fita" role="img" aria-label={`Fita: ${fita}, cabeçote na posição ${cabecote}`}>
        <span className="fita-borda" aria-hidden>…</span>
        {[...fita].map((c, i) => (
          <div key={i} className={`celula ${c === 'B' ? 'branco' : ''} ${i === cabecote ? 'cabecote' : ''}`}>
            {i === cabecote && <span className="marcador-estado">{estado}</span>}
            {c === 'B' ? '␣' : c}
            <small>{i}</small>
          </div>
        ))}
        <span className="fita-borda" aria-hidden>…</span>
      </div>
    </div>
  )
}
