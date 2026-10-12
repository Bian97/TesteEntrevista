
export function somenteDigitos(valor) {
  return String(valor ?? '').replace(/\D/g, '')
}

export function formatarMoeda(valor) {
  return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(Number(valor) || 0)
}

export function formatarData(valor) {
  if (!valor) return '—'
  const [dataParte] = String(valor).split('T')
  const [ano, mes, dia] = dataParte.split('-')
  if (!ano || !mes || !dia) return String(valor)
  return `${dia}/${mes}/${ano}`
}

export function formatarCpf(cpf) {
  const digitos = somenteDigitos(cpf)
  if (digitos.length !== 11) return cpf ?? ''
  return mascaraCpf(digitos)
}

export function formatarTelefone(telefone) {
  const digitos = somenteDigitos(telefone)
  if (!digitos) return '—'
  if (digitos.length !== 10 && digitos.length !== 11) return telefone
  return mascaraTelefone(digitos)
}

export function hojeISO() {
  const agora = new Date()
  const ano = agora.getFullYear()
  const mes = String(agora.getMonth() + 1).padStart(2, '0')
  const dia = String(agora.getDate()).padStart(2, '0')
  return `${ano}-${mes}-${dia}`
}

export const DATA_MINIMA_NASCIMENTO = '1900-01-01'

export function dataMaximaNascimento() {
  const ontem = new Date()
  ontem.setDate(ontem.getDate() - 1)
  const ano = ontem.getFullYear()
  const mes = String(ontem.getMonth() + 1).padStart(2, '0')
  const dia = String(ontem.getDate()).padStart(2, '0')
  return `${ano}-${mes}-${dia}`
}

export function mascaraCpf(valor) {
  const d = somenteDigitos(valor).slice(0, 11)
  let r = d.slice(0, 3)
  if (d.length > 3) r += `.${d.slice(3, 6)}`
  if (d.length > 6) r += `.${d.slice(6, 9)}`
  if (d.length > 9) r += `-${d.slice(9, 11)}`
  return r
}

export function mascaraTelefone(valor) {
  const d = somenteDigitos(valor).slice(0, 11)
  if (!d) return ''
  if (d.length <= 2) return `(${d}`
  const tamanhoPrimeiraParte = d.length > 10 ? 5 : 4
  let r = `(${d.slice(0, 2)}) ${d.slice(2, 2 + tamanhoPrimeiraParte)}`
  if (d.length > 2 + tamanhoPrimeiraParte) r += `-${d.slice(2 + tamanhoPrimeiraParte)}`
  return r
}
