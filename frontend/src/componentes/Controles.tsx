import type { Player } from './usePlayer'

export function Controles({ player, rotulo }: { player: Player; rotulo: string }) {
  const { atual, total, tocando, irPara, alternar } = player
  return (
    <div className="controles">
      <button type="button" onClick={() => irPara(0)} disabled={atual === 0} aria-label="Primeiro passo">⏮</button>
      <button type="button" onClick={() => irPara(atual - 1)} disabled={atual === 0} aria-label="Passo anterior">◀</button>
      <button type="button" className="tocar" onClick={alternar} disabled={total < 2}>
        {tocando ? 'Pausar' : 'Reproduzir'}
      </button>
      <button type="button" onClick={() => irPara(atual + 1)} disabled={atual >= total - 1} aria-label="Próximo passo">▶</button>
      <button type="button" onClick={() => irPara(total - 1)} disabled={atual >= total - 1} aria-label="Último passo">⏭</button>
      <input
        type="range"
        min={0}
        max={Math.max(0, total - 1)}
        value={atual}
        onChange={e => irPara(Number(e.target.value))}
        aria-label="Linha do tempo"
      />
      <span className="contador">{rotulo}</span>
    </div>
  )
}
