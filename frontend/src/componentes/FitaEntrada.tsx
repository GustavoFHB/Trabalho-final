/**
 * Cadeia de entrada de um autômato, lida da esquerda para a direita.
 * Símbolos já consumidos ficam apagados; o próximo a ser lido fica marcado.
 */
export function FitaEntrada({ cadeia, posicao }: { cadeia: string; posicao: number }) {
  if (cadeia.length === 0) {
    return (
      <div className="cadeia-simbolos" aria-label="Entrada vazia">
        <div className="simbolo fim atual">λ<small>vazia</small></div>
      </div>
    )
  }

  return (
    <div className="cadeia-simbolos fita-entrada" role="img" aria-label={`Entrada ${cadeia}: ${posicao} de ${cadeia.length} símbolos lidos`}>
      {[...cadeia].map((c, i) => (
        <div key={i} className={`simbolo ${i < posicao ? 'lido' : ''} ${i === posicao ? 'atual' : ''}`}>
          {c}
          <small>{i}</small>
        </div>
      ))}
      <div className={`simbolo fim ${posicao >= cadeia.length ? 'atual' : ''}`} title="Fim da entrada">⊣<small>fim</small></div>
    </div>
  )
}
