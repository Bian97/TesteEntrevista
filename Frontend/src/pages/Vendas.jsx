import { useEffect, useState } from 'react'
import { FiPlus } from 'react-icons/fi'
import { useNavigate } from 'react-router-dom'
import PageHeader from '../components/PageHeader'
import DataTable from '../components/DataTable'
import Modal from '../components/Modal'
import { listarVendas, obterVenda } from '../services/vendas'
import { formatarCpf, formatarData, formatarMoeda } from '../utils/formatadores'

const MODAL_FECHADO = { aberto: false, id: null, venda: null }

export default function Vendas() {
  const navigate = useNavigate()
  const [vendas, setVendas] = useState([])
  const [carregando, setCarregando] = useState(true)
  const [modal, setModal] = useState(MODAL_FECHADO)

  useEffect(() => {
    let ativo = true
    listarVendas()
      .then((dados) => {
        if (ativo) setVendas(dados)
      })
      .catch(() => {
      })
      .finally(() => {
        if (ativo) setCarregando(false)
      })
    return () => {
      ativo = false
    }
  }, [])

  function abrirVenda(venda) {
    setModal({ aberto: true, id: venda.id, venda: null })
    obterVenda(venda.id)
      .then((detalhe) => {
        setModal((atual) => (atual.aberto && atual.id === venda.id ? { ...atual, venda: detalhe } : atual))
      })
      .catch(() => {
        setModal(MODAL_FECHADO)
      })
  }

  function fecharModal() {
    setModal(MODAL_FECHADO)
  }

  const colunas = [
    { titulo: 'Nº', render: (v) => `#${v.id}` },
    { titulo: 'Data', render: (v) => formatarData(v.dataVenda) },
    {
      titulo: 'Cliente',
      render: (v) => (
        <div className="celula-duas-linhas">
          <span>{v.clienteNome}</span>
          <small>{formatarCpf(v.clienteCpf)}</small>
        </div>
      ),
    },
    { titulo: 'Total', className: 'coluna-valor', render: (v) => formatarMoeda(v.valorTotal) },
  ]

  const colunasItens = [
    { titulo: 'Produto', chave: 'produtoNome' },
    { titulo: 'Qtd', className: 'coluna-qtd', chave: 'quantidade' },
    { titulo: 'Unitário', className: 'coluna-valor', render: (i) => formatarMoeda(i.valorUnitario) },
    { titulo: 'Total', className: 'coluna-valor', render: (i) => formatarMoeda(i.valorTotal) },
  ]

  return (
    <section>
      <PageHeader titulo="Vendas">
        <button type="button" className="botao botao-primario" onClick={() => navigate('/vendas/nova')}>
          <FiPlus /> Nova venda
        </button>
      </PageHeader>

      <DataTable
        colunas={colunas}
        linhas={vendas}
        carregando={carregando}
        mensagemVazio="Nenhuma venda registrada."
        onClickLinha={abrirVenda}
      />

      <Modal aberto={modal.aberto} titulo={`Venda #${modal.id ?? ''}`} onFechar={fecharModal} largura="lg">
        {!modal.venda && <div className="celula-centralizada"><span className="spinner" /></div>}
        {modal.venda && (
          <>
            <div className="venda-detalhe-cabecalho">
              <div>
                <span className="rotulo-discreto">Data</span>
                <strong>{formatarData(modal.venda.dataVenda)}</strong>
              </div>
              <div>
                <span className="rotulo-discreto">Cliente</span>
                <strong>{modal.venda.clienteNome}</strong>
              </div>
              <div>
                <span className="rotulo-discreto">CPF</span>
                <strong>{formatarCpf(modal.venda.clienteCpf)}</strong>
              </div>
            </div>
            <DataTable colunas={colunasItens} linhas={modal.venda.itens} mensagemVazio="Nenhum item nesta venda." />
            <div className="total-tabela">
              <span>Total geral</span>
              <strong>{formatarMoeda(modal.venda.valorTotal)}</strong>
            </div>
          </>
        )}
      </Modal>
    </section>
  )
}
