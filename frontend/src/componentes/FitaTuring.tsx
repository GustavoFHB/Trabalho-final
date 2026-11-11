import { useEffect, useRef } from 'react'

/** Fita da Máquina de Turing com o cabeçote destacado. */
export function FitaTuring({ fita, cabecote, estado }: { fita: string; cabecote: number; estado: string }) {
  const ref = useRef<HTMLDivElement>(null)

  useEffect(() => {
    ref.current?.querySelector('.celula.cabecote')?.scrollIntoView({ block: 'nearest', inline: 'center', behavior: 'smooth' })
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
