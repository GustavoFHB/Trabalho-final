export function rolarDentro(container: HTMLElement | null | undefined, alvo: Element | null | undefined) {
  if (!container || !alvo) return
  const caixa = container.getBoundingClientRect()
  const item = alvo.getBoundingClientRect()
  const cabecalho = container.querySelector('thead')?.getBoundingClientRect().height ?? 0
  const topo = caixa.top + cabecalho
  if (item.top < topo) container.scrollTop -= topo - item.top
  else if (item.bottom > caixa.bottom) container.scrollTop += item.bottom - caixa.bottom
}

export function centralizarDentro(container: HTMLElement | null | undefined, alvo: Element | null | undefined) {
  if (!container || !alvo) return
  const caixa = container.getBoundingClientRect()
  const item = alvo.getBoundingClientRect()
  const deslocamento = item.left + item.width / 2 - (caixa.left + caixa.width / 2)
  container.scrollTo({ left: container.scrollLeft + deslocamento, behavior: 'smooth' })
}
