import type { NoArvore } from '../api'

interface NoPosicionado {
  id: number
  simbolo: string
  x: number
  y: number
  folha: boolean
  ordem: number // ordem de expansão (pré-ordem) — usada para revelar a árvore passo a passo
  pai?: NoPosicionado
}

const DX = 46
const DY = 64
const R = 17

/**
 * Layout simples: folhas recebem colunas da esquerda para a direita;
 * cada nó interno fica centralizado sobre seus filhos.
 */
function posicionar(raiz: NoArvore) {
  const nos: NoPosicionado[] = []
  let coluna = 0
  let id = 0
  let ordemExpansao = 0

  function visitar(n: NoArvore, prof: number, pai?: NoPosicionado, ordemPai = 0): NoPosicionado {
    const no: NoPosicionado = { id: id++, simbolo: n.simbolo, x: 0, y: prof * DY + R + 8, folha: n.filhos.length === 0, ordem: ordemPai, pai }
    nos.push(no)
    if (no.folha) {
      no.x = coluna++ * DX + R + 8
    } else {
      const minhaExpansao = ++ordemExpansao // o passo em que este nó é expandido
      const filhos = n.filhos.map(f => visitar(f, prof + 1, no, minhaExpansao))
      no.x = (filhos[0].x + filhos[filhos.length - 1].x) / 2
    }
    return no
  }

  visitar(raiz, 0)
  const largura = Math.max(...nos.map(n => n.x)) + R + 8
  const altura = Math.max(...nos.map(n => n.y)) + R + 8
  return { nos, largura, altura }
}

/** Árvore de derivação; mostra apenas os nós já produzidos até o passo `passo`. */
export function ArvoreDerivacao({ raiz, passo }: { raiz: NoArvore; passo: number }) {
  const { nos, largura, altura } = posicionar(raiz)
  const visiveis = nos.filter(n => n.ordem <= passo)

  return (
    <div className="arvore-wrap">
      <svg width={largura} height={altura} viewBox={`0 0 ${largura} ${altura}`} role="img" aria-label="Árvore de derivação">
        {visiveis.filter(n => n.pai).map(n => (
          <line key={`l${n.id}`} x1={n.pai!.x} y1={n.pai!.y + R} x2={n.x} y2={n.y - R} className="aresta-arvore" />
        ))}
        {visiveis.map(n => {
          const classe = n.simbolo === 'λ' ? 'lambda' : n.folha ? 'terminal' : 'variavel'
          const recente = n.ordem === passo && passo > 0
          return (
            <g key={n.id} className={`no-arvore ${classe} ${recente ? 'recente' : ''}`}>
              {n.folha ? (
                <rect x={n.x - R + 2} y={n.y - R + 2} width={2 * R - 4} height={2 * R - 4} rx={3} />
              ) : (
                <circle cx={n.x} cy={n.y} r={R} />
              )}
              <text x={n.x} y={n.y} dy="0.35em" textAnchor="middle">{n.simbolo}</text>
            </g>
          )
        })}
      </svg>
    </div>
  )
}
