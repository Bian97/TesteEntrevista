import { somenteDigitos } from './formatadores'

export function cpfValido(valor) {
  const cpf = somenteDigitos(valor)
  if (cpf.length !== 11) return false
  if (/^(\d)\1{10}$/.test(cpf)) return false

  const calcularDigito = (base, pesoInicial) => {
    let soma = 0
    for (let i = 0; i < base.length; i++) {
      soma += Number(base[i]) * (pesoInicial - i)
    }
    const resto = (soma * 10) % 11
    return resto === 10 ? 0 : resto
  }

  const primeiro = calcularDigito(cpf.slice(0, 9), 10)
  if (primeiro !== Number(cpf[9])) return false
  const segundo = calcularDigito(cpf.slice(0, 10), 11)
  return segundo === Number(cpf[10])
}

export function emailValido(valor) {
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(String(valor ?? '').trim())
}
