import type { ReactNode } from 'react'

export function Veredito({ tipo, titulo, children }: { tipo: 'aceita' | 'rejeita' | 'erro'; titulo: string; children?: ReactNode }) {
  return (
    <div className={`veredito ${tipo}`} role="status">
      <strong>{titulo}</strong>
      {children && <div className="veredito-detalhe">{children}</div>}
    </div>
  )
}
