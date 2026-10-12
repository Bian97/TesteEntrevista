import { useEffect, useRef } from 'react'
import { FiX } from 'react-icons/fi'

export default function Modal({ aberto, titulo, onFechar, largura = 'md', children }) {
  const dialogRef = useRef(null)
  const onFecharRef = useRef(onFechar)

  useEffect(() => {
    onFecharRef.current = onFechar
  }, [onFechar])

  useEffect(() => {
    if (!aberto) return undefined
    dialogRef.current?.focus()

    function tratarEsc(evento) {
      if (evento.key === 'Escape') onFecharRef.current()
    }

    window.addEventListener('keydown', tratarEsc)
    return () => window.removeEventListener('keydown', tratarEsc)
  }, [aberto])

  if (!aberto) return null

  function tratarCliqueOverlay(evento) {
    if (evento.target === evento.currentTarget) onFechar()
  }

  return (
    <div className="modal-overlay" onMouseDown={tratarCliqueOverlay}>
      <div
        ref={dialogRef}
        className={`modal modal-${largura}`}
        role="dialog"
        tabIndex={-1}
      >
        <div className="modal-cabecalho">
          <h2>{titulo}</h2>
          <button type="button" className="botao-icone" aria-label="Fechar" onClick={onFechar}>
            <FiX />
          </button>
        </div>
        <div className="modal-corpo">{children}</div>
      </div>
    </div>
  )
}
