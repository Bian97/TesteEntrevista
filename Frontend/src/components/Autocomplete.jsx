import { useState } from 'react'

export default function Autocomplete({
  ref,
  id,
  texto,
  onTextoChange,
  opcoes = [],
  carregando = false,
  onSelecionar,
  getRotulo,
  getDetalhe,
  placeholder,
  autoFocus = false,
  minimoCaracteres = 1,
  textoVazio = 'Nenhum resultado encontrado.',
}) {
  const [aberto, setAberto] = useState(false)
  const [indiceDestaque, setIndiceDestaque] = useState(0)

  const listaId = `${id}-lista`
  const mostrarLista = aberto && texto.trim().length >= minimoCaracteres
  const indiceAtivo = indiceDestaque < opcoes.length ? indiceDestaque : 0

  function alterarTexto(valor) {
    onTextoChange(valor)
    setAberto(true)
    setIndiceDestaque(0)
  }

  function selecionar(item) {
    setAberto(false)
    onSelecionar(item)
  }

  function tratarTecla(evento) {
    if (evento.key === 'ArrowDown' || evento.key === 'ArrowUp') {
      evento.preventDefault()
      setAberto(true)
      if (opcoes.length === 0) return
      const delta = evento.key === 'ArrowDown' ? 1 : -1
      setIndiceDestaque((indiceAtivo + delta + opcoes.length) % opcoes.length)
    } else if (evento.key === 'Enter') {
      if (mostrarLista && opcoes.length > 0) {
        evento.preventDefault()
        selecionar(opcoes[indiceAtivo])
      }
    } else if (evento.key === 'Escape') {
      setAberto(false)
    }
  }

  return (
    <div className="autocomplete">
      <input
        id={id}
        ref={ref}
        type="text"
        autoComplete="off"
        className="autocomplete-input"
        value={texto}
        placeholder={placeholder}
        autoFocus={autoFocus}
        onChange={(evento) => alterarTexto(evento.target.value)}
        onFocus={() => setAberto(true)}
        onBlur={() => setAberto(false)}
        onKeyDown={tratarTecla}
      />
      {mostrarLista && (
        <ul id={listaId} className="autocomplete-lista">
          {carregando && (
            <li className="autocomplete-info">Buscando...</li>
          )}
          {!carregando && opcoes.length === 0 && (
            <li className="autocomplete-info">{textoVazio}</li>
          )}
          {!carregando &&
            opcoes.map((item, indice) => (
              <li
                key={item.id ?? indice}
                id={`${listaId}-${indice}`}
                className={`autocomplete-opcao${indice === indiceAtivo ? ' ativa' : ''}`}
                onMouseDown={(evento) => {
                  evento.preventDefault()
                  selecionar(item)
                }}
                onMouseEnter={() => setIndiceDestaque(indice)}
              >
                <span className="autocomplete-rotulo">{getRotulo(item)}</span>
                {getDetalhe && <span className="autocomplete-detalhe">{getDetalhe(item)}</span>}
              </li>
            ))}
        </ul>
      )}
    </div>
  )
}
