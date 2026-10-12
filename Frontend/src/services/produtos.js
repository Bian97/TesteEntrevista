import { api } from './api'

export async function listarProdutos(busca) {
  const { data } = await api.get('/api/produtos', { params: { busca: busca || undefined } })
  return data
}

export async function obterProduto(id) {
  const { data } = await api.get(`/api/produtos/${id}`)
  return data
}

export async function criarProduto(produto) {
  const { data } = await api.post('/api/produtos', produto)
  return data
}

export async function atualizarProduto(id, produto) {
  const { data } = await api.put(`/api/produtos/${id}`, produto)
  return data
}

export async function excluirProduto(id) {
  await api.delete(`/api/produtos/${id}`)
}
