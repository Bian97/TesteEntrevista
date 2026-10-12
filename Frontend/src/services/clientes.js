import { api } from './api'

export async function listarClientes(busca) {
  const { data } = await api.get('/api/clientes', { params: { busca: busca || undefined } })
  return data
}

export async function obterCliente(id) {
  const { data } = await api.get(`/api/clientes/${id}`)
  return data
}

export async function criarCliente(cliente) {
  const { data } = await api.post('/api/clientes', cliente)
  return data
}

export async function atualizarCliente(id, cliente) {
  const { data } = await api.put(`/api/clientes/${id}`, cliente)
  return data
}

export async function excluirCliente(id) {
  await api.delete(`/api/clientes/${id}`)
}
