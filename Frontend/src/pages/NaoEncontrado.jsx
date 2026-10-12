import { Link } from 'react-router-dom'

export default function NaoEncontrado() {
  return (
    <section className="estado-centralizado">
      <h1>Página não encontrada</h1>
      <p>O endereço acessado não existe.</p>
      <Link to="/vendas/nova" className="botao botao-primario">Ir para Nova venda</Link>
    </section>
  )
}
