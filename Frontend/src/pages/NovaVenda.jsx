import { useEffect, useRef, useState } from 'react'
import { FiCheckCircle, FiMinus, FiPlus, FiTrash2, FiUserPlus } from 'react-icons/fi'
import { useNavigate } from 'react-router-dom'
import { toast } from 'react-toastify'
import Autocomplete from '../components/Autocomplete'
import Campo from '../components/Campo'
import ClienteForm from '../components/ClienteForm'
import ConfirmDialog from '../components/ConfirmDialog'
import DataTable from '../components/DataTable'
import InputMoeda from '../components/InputMoeda'
import Modal from '../components/Modal'
import PageHeader from '../components/PageHeader'
import { useAtalho } from '../hooks/useAtalho'
import { useDebounce } from '../hooks/useDebounce'
import { listarClientes } from '../services/clientes'
import { listarProdutos } from '../services/produtos'
import { criarVenda } from '../services/vendas'
import {
  formatarCpf,
  formatarData,
  formatarMoeda,
  formatarTelefone,
  hojeISO,
} from '../utils/formatadores'

const LINHA_INICIAL = { produto: null, texto: '', valor: 0, quantidade: '1' }

function paraCentavos(valor) {
  return Math.round(Number(valor) * 100)
}

function subtotalCentavos(item) {
  return paraCentavos(item.valorUnitario) * item.quantidade
}

export default function NovaVenda() {
  const navigate = useNavigate()

  const [cliente, setCliente] = useState(null)
  const [textoCliente, setTextoCliente] = useState('')
  const buscaClienteAtrasada = useDebounce(textoCliente, 250)
  const [resultadoClientes, setResultadoClientes] = useState({ termo: '', dados: [] })
  const [modalCliente, setModalCliente] = useState(false)

  const [dataVenda, setDataVenda] = useState(hojeISO)

  const [produtos, setProdutos] = useState([])
  const [carregandoProdutos, setCarregandoProdutos] = useState(true)

  const [linha, setLinha] = useState(LINHA_INICIAL)
  const [errosLinha, setErrosLinha] = useState({})
  const produtoInputRef = useRef(null)
  const quantidadeRef = useRef(null)

  const [itens, setItens] = useState([])
  const [confirmarLimpeza, setConfirmarLimpeza] = useState(false)
  const [vendaRegistrada, setVendaRegistrada] = useState(null)
  const [salvando, setSalvando] = useState(false)
  const finalizandoRef = useRef(false)

  useEffect(() => {
    const termo = buscaClienteAtrasada.trim()
    if (!termo) return undefined
    let ativo = true
    listarClientes(termo)
      .then((dados) => {
        if (ativo) setResultadoClientes({ termo, dados })
      })
      .catch(() => {
        if (ativo) setResultadoClientes({ termo, dados: [] })
      })
    return () => {
      ativo = false
    }
  }, [buscaClienteAtrasada])

  useEffect(() => {
    let ativo = true
    listarProdutos()
      .then((dados) => {
        if (ativo) setProdutos(dados)
      })
      .catch(() => {
      })
      .finally(() => {
        if (ativo) setCarregandoProdutos(false)
      })
    return () => {
      ativo = false
    }
  }, [])

  const termoClienteAtual = buscaClienteAtrasada.trim()
  const carregandoClientes = termoClienteAtual !== resultadoClientes.termo
  const opcoesClientes = resultadoClientes.dados

  const termoProduto = linha.texto.trim()
  const opcoesProdutos = termoProduto
    ? produtos.filter((p) => p.nome.toLowerCase().includes(termoProduto.toLowerCase()))
    : []

  const quantidadeLinha = Number(linha.quantidade)
  const subtotalLinha =
    Number.isInteger(quantidadeLinha) && quantidadeLinha > 0
      ? (paraCentavos(linha.valor) * quantidadeLinha) / 100
      : 0
  const totalCentavos = itens.reduce((soma, item) => soma + subtotalCentavos(item), 0)
  const total = totalCentavos / 100
  const totalUnidades = itens.reduce((soma, item) => soma + item.quantidade, 0)

  let motivoBloqueio = ''
  if (!cliente) motivoBloqueio = 'Selecione um cliente para finalizar.'
  else if (!dataVenda) motivoBloqueio = 'Informe a data da venda.'
  else if (dataVenda > hojeISO()) motivoBloqueio = 'A data da venda não pode ser futura.'
  else if (itens.length === 0) motivoBloqueio = 'Lance ao menos um item.'
  else if (itens.some((item) => !(item.valorUnitario > 0))) motivoBloqueio = 'Há itens com valor zero.'
  const podeFinalizar = !motivoBloqueio && !salvando

  function selecionarCliente(clienteEscolhido) {
    setCliente(clienteEscolhido)
    setTextoCliente('')
  }

  function trocarCliente() {
    setCliente(null)
    setTextoCliente('')
  }

  function aoSalvarNovoCliente(clienteCriado) {
    setModalCliente(false)
    selecionarCliente(clienteCriado)
  }

  function selecionarProduto(produto) {
    setLinha((atual) => ({ ...atual, produto, texto: produto.nome, valor: produto.valorUnitario }))
    setErrosLinha({})
    quantidadeRef.current?.focus()
  }

  function alterarTextoProduto(texto) {
    setLinha((atual) => ({ ...atual, texto, produto: null }))
  }

  function adicionarItem() {
    const novosErros = {}
    if (!linha.produto) novosErros.produto = 'Selecione um produto.'
    if (!(linha.valor > 0)) novosErros.valor = 'Informe um valor maior que zero.'
    const quantidade = Number(linha.quantidade)
    if (!Number.isInteger(quantidade) || quantidade < 1) {
      novosErros.quantidade = 'Informe uma quantidade inteira maior ou igual a 1.'
    }
    setErrosLinha(novosErros)
    if (Object.keys(novosErros).length > 0) return

    const produto = linha.produto
    const valorUnitario = Math.round(linha.valor * 100) / 100
    const existente = itens.find((item) => item.produtoId === produto.id)

    if (existente) {
      setItens(
        itens.map((item) =>
          item.produtoId === produto.id ? { ...item, quantidade: item.quantidade + quantidade } : item,
        ),
      )
      toast.info(`${produto.nome} já está na venda: a quantidade foi somada à linha existente.`)
    } else {
      setItens([
        ...itens,
        { produtoId: produto.id, produtoNome: produto.nome, quantidade, valorUnitario },
      ])
    }

    setLinha(LINHA_INICIAL)
    setErrosLinha({})
    produtoInputRef.current?.focus()
  }

  function alterarQuantidade(produtoId, novaQuantidade) {
    if (!Number.isInteger(novaQuantidade) || novaQuantidade < 1) return
    setItens((atuais) =>
      atuais.map((item) => (item.produtoId === produtoId ? { ...item, quantidade: novaQuantidade } : item)),
    )
  }

  function alterarValorItem(produtoId, valorUnitario) {
    setItens((atuais) =>
      atuais.map((item) => (item.produtoId === produtoId ? { ...item, valorUnitario } : item)),
    )
  }

  function removerItem(produtoId) {
    setItens((atuais) => atuais.filter((item) => item.produtoId !== produtoId))
  }

  function limparTudo() {
    setCliente(null)
    setTextoCliente('')
    setDataVenda(hojeISO())
    setLinha(LINHA_INICIAL)
    setErrosLinha({})
    setItens([])
    setConfirmarLimpeza(false)
  }

  function pedirLimpeza() {
    if (itens.length > 0) setConfirmarLimpeza(true)
    else limparTudo()
  }

  function novaVenda() {
    setVendaRegistrada(null)
    limparTudo()
  }

  async function finalizarVenda() {
    if (!podeFinalizar || finalizandoRef.current) return
    finalizandoRef.current = true
    setSalvando(true)
    try {
      const venda = await criarVenda({
        clienteId: cliente.id,
        dataVenda,
        itens: itens.map((item) => ({
          produtoId: item.produtoId,
          quantidade: item.quantidade,
          valorUnitario: item.valorUnitario,
        })),
      })
      toast.success(`Venda #${venda.id} registrada com sucesso`)
      setVendaRegistrada(venda)
    } catch {
      return
    } finally {
      finalizandoRef.current = false
      setSalvando(false)
    }
  }

  useAtalho('F2', finalizarVenda, !vendaRegistrada && !modalCliente && !confirmarLimpeza)

  const colunasItens = [
    { titulo: 'Produto', chave: 'produtoNome' },
    {
      titulo: 'Qtd',
      className: 'coluna-qtd',
      render: (item) => (
        <div className="qtd-controle">
          <button
            type="button"
            className="botao-icone"
            aria-label={`Diminuir quantidade de ${item.produtoNome}`}
            disabled={item.quantidade <= 1}
            onClick={() => alterarQuantidade(item.produtoId, item.quantidade - 1)}
          >
            <FiMinus />
          </button>
          <input
            type="number"
            min="1"
            step="1"
            value={item.quantidade}
            onChange={(evento) => {
              const numero = Number(evento.target.value)
              if (Number.isInteger(numero) && numero >= 1) alterarQuantidade(item.produtoId, numero)
            }}
          />
          <button
            type="button"
            className="botao-icone"
            aria-label={`Aumentar quantidade de ${item.produtoNome}`}
            onClick={() => alterarQuantidade(item.produtoId, item.quantidade + 1)}
          >
            <FiPlus />
          </button>
        </div>
      ),
    },
    {
      titulo: 'Valor unitário',
      className: 'coluna-valor',
      render: (item) => (
        <InputMoeda
          className="input-inline"
          valor={item.valorUnitario}
          onChange={(valor) => alterarValorItem(item.produtoId, valor)}
        />
      ),
    },
    {
      titulo: 'Subtotal',
      className: 'coluna-valor',
      render: (item) => formatarMoeda(subtotalCentavos(item) / 100),
    },
    {
      titulo: '',
      className: 'coluna-acoes',
      render: (item) => (
        <button
          type="button"
          className="botao-icone botao-icone-perigo"
          aria-label={`Remover ${item.produtoNome}`}
          onClick={() => removerItem(item.produtoId)}
        >
          <FiTrash2 />
        </button>
      ),
    },
  ]

  return (
    <section>
      <PageHeader titulo="Nova venda">
        <span className="badge">Nº será gerado ao salvar</span>
      </PageHeader>

      <div className="venda-layout">
        <div className="venda-principal">
          <div className="cartao">
            <h2 className="cartao-titulo">Cliente</h2>
            {cliente ? (
              <div className="cliente-selecionado">
                <div className="cliente-selecionado-dados">
                  <strong>{cliente.nome}</strong>
                  <span>{cliente.cpfFormatado || formatarCpf(cliente.cpf)}</span>
                  {cliente.email && <span>{cliente.email}</span>}
                  {cliente.telefone && <span>{formatarTelefone(cliente.telefone)}</span>}
                </div>
                <button type="button" className="botao botao-secundario" onClick={trocarCliente}>
                  Trocar
                </button>
              </div>
            ) : (
              <>
                <Autocomplete
                  id="venda-cliente"
                  texto={textoCliente}
                  onTextoChange={setTextoCliente}
                  opcoes={opcoesClientes}
                  carregando={carregandoClientes}
                  onSelecionar={selecionarCliente}
                  getRotulo={(c) => c.nome}
                  getDetalhe={(c) => c.cpfFormatado || formatarCpf(c.cpf)}
                  placeholder="Buscar cliente por CPF ou nome"
                  autoFocus
                  minimoCaracteres={1}
                  textoVazio="Nenhum cliente encontrado."
                />
                <button type="button" className="botao-link" onClick={() => setModalCliente(true)}>
                  <FiUserPlus /> Cadastrar novo cliente
                </button>
              </>
            )}
          </div>

          <div className="cartao">
            <Campo id="venda-data" rotulo="Data da venda">
              <input
                id="venda-data"
                type="date"
                max={hojeISO()}
                value={dataVenda}
                onChange={(evento) => setDataVenda(evento.target.value)}
              />
            </Campo>
          </div>

          <div className="cartao">
            <h2 className="cartao-titulo">Lançar item</h2>
            <div className="linha-item">
              <Campo id="item-produto" rotulo="Produto" erro={errosLinha.produto} className="linha-item-produto">
                <Autocomplete
                  ref={produtoInputRef}
                  id="item-produto"
                  texto={linha.texto}
                  onTextoChange={alterarTextoProduto}
                  opcoes={opcoesProdutos}
                  carregando={carregandoProdutos}
                  onSelecionar={selecionarProduto}
                  getRotulo={(p) => p.nome}
                  getDetalhe={(p) => formatarMoeda(p.valorUnitario)}
                  placeholder="Buscar produto pelo nome"
                  minimoCaracteres={1}
                  textoVazio="Nenhum produto encontrado."
                />
              </Campo>

              <Campo id="item-valor" rotulo="Valor unitário" erro={errosLinha.valor}>
                <InputMoeda
                  id="item-valor"
                  valor={linha.valor}
                  onChange={(valor) => setLinha((atual) => ({ ...atual, valor }))}
                />
              </Campo>

              <Campo id="item-quantidade" rotulo="Quantidade" erro={errosLinha.quantidade}>
                <input
                  id="item-quantidade"
                  ref={quantidadeRef}
                  type="number"
                  min="1"
                  step="1"
                  value={linha.quantidade}
                  onChange={(evento) => setLinha((atual) => ({ ...atual, quantidade: evento.target.value }))}
                  onKeyDown={(evento) => {
                    if (evento.key === 'Enter') {
                      evento.preventDefault()
                      adicionarItem()
                    }
                  }}
                />
              </Campo>

              <div className="campo">
                <span className="campo-rotulo">Subtotal</span>
                <output className="subtotal-linha">{formatarMoeda(subtotalLinha)}</output>
              </div>

              <button type="button" className="botao botao-primario linha-item-acao" onClick={adicionarItem}>
                <FiPlus /> Adicionar
              </button>
            </div>
          </div>

          <div className="cartao">
            <h2 className="cartao-titulo">Itens</h2>
            <DataTable
              colunas={colunasItens}
              linhas={itens}
              chave="produtoId"
              mensagemVazio="Nenhum item lançado ainda."
            />
            <div className="total-tabela">
              <span>Total</span>
              <strong>{formatarMoeda(total)}</strong>
            </div>
          </div>
        </div>

        <aside className="venda-resumo cartao">
          <h2 className="cartao-titulo">Resumo</h2>
          <dl className="resumo-lista">
            <div>
              <dt>Cliente</dt>
              <dd>{cliente?.nome ?? '—'}</dd>
            </div>
            <div>
              <dt>Data</dt>
              <dd>{formatarData(dataVenda)}</dd>
            </div>
            <div>
              <dt>Itens</dt>
              <dd>{itens.length}</dd>
            </div>
            <div>
              <dt>Unidades</dt>
              <dd>{totalUnidades}</dd>
            </div>
          </dl>

          <div className="resumo-total">
            <span>Total da venda</span>
            <strong>{formatarMoeda(total)}</strong>
          </div>

          <button
            type="button"
            className="botao botao-primario botao-bloco"
            onClick={finalizarVenda}
            disabled={!podeFinalizar}
          >
            {salvando ? 'Salvando...' : 'Finalizar venda (F2)'}
          </button>
          {motivoBloqueio && <p className="resumo-motivo">{motivoBloqueio}</p>}

          <button type="button" className="botao botao-secundario botao-bloco" onClick={pedirLimpeza}>
            Limpar
          </button>
        </aside>
      </div>

      <Modal
        aberto={modalCliente}
        titulo="Novo cliente"
        onFechar={() => setModalCliente(false)}
        largura="lg"
      >
        <ClienteForm
          cliente={null}
          onSalvo={aoSalvarNovoCliente}
          onCancelar={() => setModalCliente(false)}
        />
      </Modal>

      <Modal
        aberto={Boolean(vendaRegistrada)}
        titulo={`Venda #${vendaRegistrada?.id ?? ''} registrada`}
        onFechar={novaVenda}
        largura="sm"
      >
        <div className="resumo-sucesso">
          <FiCheckCircle className="icone-sucesso" />
          <dl className="resumo-lista">
            <div>
              <dt>Cliente</dt>
              <dd>{vendaRegistrada?.clienteNome}</dd>
            </div>
            <div>
              <dt>Data</dt>
              <dd>{formatarData(vendaRegistrada?.dataVenda)}</dd>
            </div>
            <div>
              <dt>Itens</dt>
              <dd>{vendaRegistrada?.itens?.length ?? 0}</dd>
            </div>
            <div>
              <dt>Total</dt>
              <dd>{formatarMoeda(vendaRegistrada?.valorTotal)}</dd>
            </div>
          </dl>
          <div className="acoes-modal">
            <button type="button" className="botao botao-secundario" onClick={() => navigate('/vendas')}>
              Ver vendas
            </button>
            <button type="button" className="botao botao-primario" onClick={novaVenda}>
              Nova venda
            </button>
          </div>
        </div>
      </Modal>

      <ConfirmDialog
        aberto={confirmarLimpeza}
        titulo="Limpar venda"
        mensagem="Todos os itens lançados e o cliente selecionado serão removidos. Deseja continuar?"
        textoConfirmar="Limpar"
        perigoso
        onConfirmar={limparTudo}
        onCancelar={() => setConfirmarLimpeza(false)}
      />
    </section>
  )
}
