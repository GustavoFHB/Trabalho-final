import { useMemo } from 'react'
import type { TransicaoDto } from '../api'

interface Props {
  transicoes: TransicaoDto[]
  estadoInicial: string
  estadosFinais: string[]
  estadoAtual?: string
  transicaoAtiva?: { de: string; para: string; rotulo: string }
}

const R = 28

/** Diagrama de estados em layout circular; destaca o estado atual e a transição em uso. */
export function DiagramaEstados({ transicoes, estadoInicial, estadosFinais, estadoAtual, transicaoAtiva }: Props) {
  const { estados, arestas, tamanho } = useMemo(() => {
    const nomes = [estadoInicial, ...transicoes.flatMap(t => [t.estadoAtual, t.novoEstado]), ...estadosFinais]
      .map(s => s.trim())
      .filter(Boolean)
    const unicos = [...new Set(nomes)]
    const raio = Math.max(90, unicos.length * 19)
    const tamanho = raio * 2 + R * 2 + 200 // margem para os laços e seus rótulos
    const c = tamanho / 2
    const estados = unicos.map((nome, i) => {
      const ang = -Math.PI / 2 + (2 * Math.PI * i) / unicos.length
      return { nome, x: c + raio * Math.cos(ang), y: c + raio * Math.sin(ang) }
    })

    const grupos = new Map<string, { de: string; para: string; rotulos: string[] }>()
    for (const t of transicoes) {
      const chave = `${t.estadoAtual}→${t.novoEstado}`
      const g = grupos.get(chave) ?? { de: t.estadoAtual, para: t.novoEstado, rotulos: [] }
      g.rotulos.push(`${t.simboloLido}/${t.simboloEscrito},${t.direcao}`)
      grupos.set(chave, g)
    }
    return { estados, arestas: [...grupos.values()], tamanho }
  }, [transicoes, estadoInicial, estadosFinais])

  const pos = new Map(estados.map(e => [e.nome, e]))
  const centro = tamanho / 2

  return (
    <div className="diagrama-wrap">
      <svg viewBox={`0 0 ${tamanho} ${tamanho}`} width={tamanho} height={tamanho} role="img" aria-label="Diagrama de estados">
        <defs>
          <marker id="ponta" viewBox="0 0 10 10" refX="9" refY="5" markerWidth="7" markerHeight="7" orient="auto-start-reverse">
            <path d="M0,0 L10,5 L0,10 z" className="ponta" />
          </marker>
          <marker id="ponta-ativa" viewBox="0 0 10 10" refX="9" refY="5" markerWidth="7" markerHeight="7" orient="auto-start-reverse">
            <path d="M0,0 L10,5 L0,10 z" className="ponta-ativa" />
          </marker>
        </defs>

        {arestas.map(a => {
          const de = pos.get(a.de)
          const para = pos.get(a.para)
          if (!de || !para) return null
          const ativa = transicaoAtiva?.de === a.de && transicaoAtiva?.para === a.para
          const titulo = `${a.de} → ${a.para}: ${a.rotulos.join('  ')}`
          const rotulo = ativa ? transicaoAtiva!.rotulo : a.rotulos.length > 2 ? `${a.rotulos.length} transições` : a.rotulos.join(' ')

          if (a.de === a.para) {
            // laço: aponta para fora do centro do círculo
            const ang = Math.atan2(de.y - centro, de.x - centro)
            const ux = Math.cos(ang), uy = Math.sin(ang)
            const px = -uy, py = ux
            const p1 = { x: de.x + R * (ux * 0.7 + px * 0.7), y: de.y + R * (uy * 0.7 + py * 0.7) }
            const p2 = { x: de.x + R * (ux * 0.7 - px * 0.7), y: de.y + R * (uy * 0.7 - py * 0.7) }
            const ctrl = 2.6 * R
            const d = `M${p1.x},${p1.y} C${p1.x + ux * ctrl + px * R},${p1.y + uy * ctrl + py * R} ${p2.x + ux * ctrl - px * R},${p2.y + uy * ctrl - py * R} ${p2.x},${p2.y}`
            const lx = de.x + ux * (R * 3.1), ly = de.y + uy * (R * 3.1)
            return (
              <g key={`${a.de}-${a.para}`} className={`aresta ${ativa ? 'ativa' : ''}`}>
                <title>{titulo}</title>
                <path d={d} markerEnd={`url(#${ativa ? 'ponta-ativa' : 'ponta'})`} />
                <text x={lx} y={ly} dy="0.35em" textAnchor="middle">{rotulo}</text>
              </g>
            )
          }

          // aresta curva (curvatura evita sobrepor a aresta de volta)
          const dx = para.x - de.x, dy = para.y - de.y
          const dist = Math.hypot(dx, dy)
          const ux = dx / dist, uy = dy / dist
          const nx = -uy, ny = ux
          const curva = 26
          const s = { x: de.x + ux * R + nx * 6, y: de.y + uy * R + ny * 6 }
          const e = { x: para.x - ux * R + nx * 6, y: para.y - uy * R + ny * 6 }
          const m = { x: (de.x + para.x) / 2 + nx * curva, y: (de.y + para.y) / 2 + ny * curva }
          return (
            <g key={`${a.de}-${a.para}`} className={`aresta ${ativa ? 'ativa' : ''}`}>
              <title>{titulo}</title>
              <path d={`M${s.x},${s.y} Q${m.x},${m.y} ${e.x},${e.y}`} markerEnd={`url(#${ativa ? 'ponta-ativa' : 'ponta'})`} />
              <text x={m.x + nx * 8} y={m.y + ny * 8} dy="0.35em" textAnchor="middle">{rotulo}</text>
            </g>
          )
        })}

        {estados.map(e => {
          const final = estadosFinais.includes(e.nome)
          const inicial = e.nome === estadoInicial
          const atual = e.nome === estadoAtual
          return (
            <g key={e.nome} className={`estado ${atual ? 'atual' : ''} ${final ? 'final' : ''}`}>
              <title>{`${e.nome}${inicial ? ' (inicial)' : ''}${final ? ' (final)' : ''}`}</title>
              {inicial && (
                <path d={`M${e.x - R - 22},${e.y} L${e.x - R - 2},${e.y}`} className="seta-inicial" markerEnd="url(#ponta)" />
              )}
              <circle cx={e.x} cy={e.y} r={R} />
              {final && <circle cx={e.x} cy={e.y} r={R - 4} className="anel" />}
              <text x={e.x} y={e.y} dy="0.35em" textAnchor="middle">{e.nome}</text>
            </g>
          )
        })}
      </svg>
    </div>
  )
}
