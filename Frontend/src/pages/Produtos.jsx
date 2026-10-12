import { useEffect, useRef, useState } from 'react'
import { FiEdit2, FiPlus, FiSearch, FiTrash2 } from 'react-icons/fi'
import { toast } from 'react-toastify'
import PageHeader from '../components/PageHeader'
import DataTable from '../components/DataTable'
import Modal from '../components/Modal'
import ConfirmDialog from '../components/ConfirmDialog'
import ProdutoForm from '../components/ProdutoForm'
import { excluirProduto, listarProdutos } from '../services/produtos'
import { useDebounce } from '../hooks/useDebounce'
import { formatarMoeda } from '../utils/formatadores'

export default function Produtos() {
  const [busca, setBusca] = useState('')
  const buscaAtrasada = useDebounce(busca, 300)
  const [produtos, setProdutos] = useState([])
  const [carregando, setCarregando] = useState(true)
  const [versao, setVersao] = useState(0)

  const [modalAberto, setModalAberto] = useState(false)
  const [produtoEditando, setProdutoEditando] = useState(null)
  const [produtoExcluir, setProdutoExcluir] = useState(null)
  const [excluindo, setExcluindo] = useState(false)
  const excluindoRef = useRef(false)

  useEffect(() => {
    let ativo = true
    listarProdutos(buscaAtrasada.trim())
      .then((dados) => {
        if (ativo) setProdutos(dados)
      })
      .catch(() => {
      })
      .finally(() => {
        if (ativo) setCarregando(false)
      })
    return () => {
      ativo = false
    }
  }, [buscaAtrasada, versao])

  function abrirNovo() {
    setProdutoEditando(null)
    setModalAberto(true)
  }

  function abrirEdicao(produto) {
    setProdutoEditando(produto)
    setModalAberto(true)
  }

  function fecharModal() {
    setModalAberto(false)
  }

  function aoSalvar() {
    setModalAberto(false)
    setVersao((v) => v + 1)
  }

  async function confirmarExclusao() {
    if (excluindoRef.current) return
    excluindoRef.current = true
    setExcluindo(true)
    try {
      await excluirProduto(produtoExcluir.id)
      toast.success('Produto excluído.')
      setVersao((v) => v + 1)
    } catch {
      return
    } finally {
      excluindoRef.current = false
      setExcluindo(false)
      setProdutoExcluir(null)
    }
  }

  const colunas = [
    { titulo: 'Nome', chave: 'nome' },
    { titulo: 'Valor unitário', className: 'coluna-valor', render: (p) => formatarMoeda(p.valorUnitario) },
    {
      titulo: 'Ações',
      className: 'coluna-acoes',
      render: (p) => (
        <div className="acoes-linha">
          <button type="button" className="botao-icone" aria-label={`Editar ${p.nome}`} onClick={() => abrirEdicao(p)}>
            <FiEdit2 />
          </button>
          <button type="button" className="botao-icone botao-icone-perigo" aria-label={`Excluir ${p.nome}`} onClick={() => setProdutoExcluir(p)}>
            <FiTrash2 />
          </button>
        </div>
      ),
    },
  ]

  return (
    <section>
      <PageHeader titulo="Produtos">
        <button type="button" className="botao botao-primario" onClick={abrirNovo}>
          <FiPlus /> Novo produto
        </button>
      </PageHeader>

      <div className="barra-busca">
        <FiSearch />
        <input
          type="search"
          placeholder="Buscar por nome"
          value={busca}
          onChange={(e) => setBusca(e.target.value)}
        />
      </div>

      <DataTable
        colunas={colunas}
        linhas={produtos}
        carregando={carregando}
        mensagemVazio="Nenhum produto encontrado."
      />

      <Modal
        aberto={modalAberto}
        titulo={produtoEditando ? 'Editar produto' : 'Novo produto'}
        onFechar={fecharModal}
      >
        <ProdutoForm
          produto={produtoEditando}
          onSalvo={aoSalvar}
          onCancelar={fecharModal}
        />
      </Modal>

      <ConfirmDialog
        aberto={Boolean(produtoExcluir)}
        titulo="Excluir produto"
        mensagem={`Excluir o produto "${produtoExcluir?.nome ?? ''}"? Esta ação não pode ser desfeita.`}
        textoConfirmar="Excluir"
        perigoso
        carregando={excluindo}
        onConfirmar={confirmarExclusao}
        onCancelar={() => setProdutoExcluir(null)}
      />
    </section>
  )
}
