import { useCallback, useEffect, useState } from 'react'

/** Navegação passo a passo com reprodução automática. */
export function usePlayer(total: number, chave: unknown, intervaloMs = 550) {
  const [atual, setAtual] = useState(0)
  const [tocando, setTocando] = useState(false)

  useEffect(() => {
    setAtual(0)
    setTocando(false)
  }, [total, chave])

  useEffect(() => {
    if (!tocando) return
    if (atual >= total - 1) {
      setTocando(false)
      return
    }
    const t = setTimeout(() => setAtual(a => a + 1), intervaloMs)
    return () => clearTimeout(t)
  }, [tocando, atual, total, intervaloMs])

  const irPara = useCallback(
    (i: number) => {
      setTocando(false)
      setAtual(Math.max(0, Math.min(i, total - 1)))
    },
    [total],
  )

  const alternar = useCallback(() => {
    setTocando(t => {
      if (!t && atual >= total - 1) setAtual(0)
      return !t
    })
  }, [atual, total])

  return { atual, tocando, irPara, alternar, total }
}

export type Player = ReturnType<typeof usePlayer>
