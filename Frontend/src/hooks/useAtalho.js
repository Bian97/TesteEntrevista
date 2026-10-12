import { useEffect, useRef } from 'react'

export function useAtalho(tecla, acao, ativo = true) {
  const acaoRef = useRef(acao)

  useEffect(() => {
    acaoRef.current = acao
  }, [acao])

  useEffect(() => {
    if (!ativo) return undefined

    function tratarTecla(evento) {
      if (evento.key === tecla) {
        evento.preventDefault()
        acaoRef.current()
      }
    }

    window.addEventListener('keydown', tratarTecla)
    return () => window.removeEventListener('keydown', tratarTecla)
  }, [tecla, ativo])
}
