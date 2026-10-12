import axios from 'axios'
import { toast } from 'react-toastify'

export const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || '',
  headers: { 'Content-Type': 'application/json' },
})

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (!error.response) {
      toast.error('Não foi possível conectar à API.')
    } else {
      const dados = error.response.data
      if (dados?.erros?.length) {
        dados.erros.forEach((mensagem) => toast.error(mensagem))
      } else {
        toast.error(dados?.mensagem || 'Ocorreu um erro inesperado.')
      }
    }
    return Promise.reject(error)
  },
)
