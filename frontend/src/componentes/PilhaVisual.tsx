/** Pilha do AP desenhada na vertical. O conteúdo vem com o topo à esquerda ("AAZ": A no topo). */
export function PilhaVisual({ conteudo }: { conteudo: string }) {
  const simbolos = [...conteudo]
  return (
    <figure className="pilha-wrap" aria-label={`Pilha: ${conteudo || 'vazia'}`}>
      <figcaption>Pilha <small>({simbolos.length})</small></figcaption>
      <div className="pilha">
        {simbolos.length === 0 && <div className="pilha-vazia">vazia</div>}
        {simbolos.map((s, i) => (
          <div key={simbolos.length - i} className={`pilha-celula ${i === 0 ? 'topo' : ''}`}>
            {i === 0 && <span className="pilha-seta">topo →</span>}
            {s}
          </div>
        ))}
        <div className="pilha-fundo" aria-hidden />
      </div>
    </figure>
  )
}
