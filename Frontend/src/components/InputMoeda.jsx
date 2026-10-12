import { formatarMoeda, somenteDigitos } from '../utils/formatadores'

export default function InputMoeda({ id, valor, onChange, onBlur, onFocus, placeholder, disabled = false, className = '', ref, maxDigitos = 12 }) {
  const centavos = Math.round((Number(valor) || 0) * 100)

  function tratarMudanca(evento) {
    const digitos = somenteDigitos(evento.target.value).slice(0, maxDigitos)
    onChange(Number(digitos || '0') / 100)
  }

  return (
    <input
      id={id}
      ref={ref}
      type="text"
      inputMode="numeric"
      className={className}
      value={formatarMoeda(centavos / 100)}
      onChange={tratarMudanca}
      onBlur={onBlur}
      onFocus={onFocus}
      placeholder={placeholder}
      disabled={disabled}
    />
  )
}
