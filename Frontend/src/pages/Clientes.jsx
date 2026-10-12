import { useEffect, useRef, useState } from 'react'
import { FiEdit2, FiPlus, FiSearch, FiTrash2 } from 'react-icons/fi'
import { toast } from 'react-toastify'
import PageHeader from '../components/PageHeader'
import DataTable from '../components/DataTable'
import Modal from '../components/Modal'
import ConfirmDialog from '../components/ConfirmDialog'
import ClienteForm from '../components/ClienteForm'
import { excluirCliente, listarClientes } from '../services/clientes'
import { useDebounce } from '../hooks/useDebounce'
import { formatarCpf, formatarData, formatarTelefone } from '../utils/formatadores'

export default function Clientes() {
  const [busca, setBusca] = useState('')
  const buscaAtrasada = useDebounce(busca, 300)
  const [clientes, setClientes] = useState([])
  const [carregando, setCarregando] = useState(true)
  const [versao, setVersao] = useState(0)

  const [modalAberto, setModalAberto] = useState(false)
  const [clienteEditando, setClienteEditando] = useState(null)
  const [clienteExcluir, setClienteExcluir] = useState(null)
  const [excluindo, setExcluindo] = useState(false)
  const excluindoRef = useRef(false)

  useEffect(() => {
    let ativo = true
    listarClientes(buscaAtrasada.trim())
      .then((dados) => {
        if (ativo) setClientes(dados)
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
    setClienteEditando(null)
    setModalAberto(true)
  }

  function abrirEdicao(cliente) {
    setClienteEditando(cliente)
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
      await excluirCliente(clienteExcluir.id)
      toast.success('Cliente excluído.')
      setVersao((v) => v + 1)
    } catch {
      return
    } finally {
      excluindoRef.current = false
      setExcluindo(false)
      setClienteExcluir(null)
    }
  }

  const colunas = [
    { titulo: 'CPF', render: (c) => c.cpfFormatado || formatarCpf(c.cpf) },
    { titulo: 'Nome', chave: 'nome' },
    { titulo: 'Nascimento', render: (c) => formatarData(c.dataNascimento) },
    { titulo: 'E-mail', render: (c) => c.email || '—' },
    { titulo: 'Telefone', render: (c) => formatarTelefone(c.telefone) },
    {
      titulo: 'Ações',
      className: 'coluna-acoes',
      render: (c) => (
        <div className="acoes-linha">
          <button type="button" className="botao-icone" aria-label={`Editar ${c.nome}`} onClick={() => abrirEdicao(c)}>
            <FiEdit2 />
          </button>
          <button type="button" className="botao-icone botao-icone-perigo" aria-label={`Excluir ${c.nome}`} onClick={() => setClienteExcluir(c)}>
            <FiTrash2 />
          </button>
        </div>
      ),
    },
  ]

  return (
    <section>
      <PageHeader titulo="Clientes">
        <button type="button" className="botao botao-primario" onClick={abrirNovo}>
          <FiPlus /> Novo cliente
        </button>
      </PageHeader>

      <div className="barra-busca">
        <FiSearch />
        <input
          type="search"
          placeholder="Buscar por nome ou CPF"
          value={busca}
          onChange={(e) => setBusca(e.target.value)}
        />
      </div>

      <DataTable
        colunas={colunas}
        linhas={clientes}
        carregando={carregando}
        mensagemVazio="Nenhum cliente encontrado."
      />

      <Modal
        aberto={modalAberto}
        titulo={clienteEditando ? 'Editar cliente' : 'Novo cliente'}
        onFechar={fecharModal}
        largura="lg"
      >
        <ClienteForm
          cliente={clienteEditando}
          onSalvo={aoSalvar}
          onCancelar={fecharModal}
        />
      </Modal>

      <ConfirmDialog
        aberto={Boolean(clienteExcluir)}
        titulo="Excluir cliente"
        mensagem={`Excluir o cliente "${clienteExcluir?.nome ?? ''}"? Esta ação não pode ser desfeita.`}
        textoConfirmar="Excluir"
        perigoso
        carregando={excluindo}
        onConfirmar={confirmarExclusao}
        onCancelar={() => setClienteExcluir(null)}
      />
    </section>
  )
}
