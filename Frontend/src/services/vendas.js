import { api } from './api'

export async function listarVendas() {
  const { data } = await api.get('/api/vendas')
  return data
}

export async function obterVenda(id) {
  const { data } = await api.get(`/api/vendas/${id}`)
  return data
}

export async function criarVenda(venda) {
  const { data } = await api.post('/api/vendas', venda)
  return data
}
