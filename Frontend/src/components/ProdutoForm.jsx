import { useRef, useState } from 'react'
import { toast } from 'react-toastify'
import Campo from './Campo'
import InputMoeda from './InputMoeda'
import { atualizarProduto, criarProduto, listarProdutos } from '../services/produtos'

const IDS_CAMPOS = {
  nome: 'produto-nome',
  valorUnitario: 'produto-valor',
}

const VALOR_MAXIMO = 9999999.99
const MENSAGEM_NOME_DUPLICADO = 'Já existe um produto com este nome.'

export default function ProdutoForm({ produto, onSalvo, onCancelar }) {
  const editando = Boolean(produto)

  const [nome, setNome] = useState(produto?.nome ?? '')
  const [valorUnitario, setValorUnitario] = useState(produto?.valorUnitario ?? 0)
  const [erros, setErros] = useState({})
  const [salvando, setSalvando] = useState(false)
  const emAndamentoRef = useRef(false)

  const valores = { nome, valorUnitario }

  function validarCampo(campo, valoresAtuais) {
    if (campo === 'nome') {
      const nomeLimpo = valoresAtuais.nome.trim()
      if (!nomeLimpo) return 'Informe o nome.'
      if (nomeLimpo.length > 150) return 'Nome deve ter no máximo 150 caracteres.'
    }

    if (campo === 'valorUnitario') {
      if (!(valoresAtuais.valorUnitario > 0)) return 'Informe um valor maior que zero.'
      if (valoresAtuais.valorUnitario > VALOR_MAXIMO) return 'O valor máximo é R$ 9.999.999,99.'
    }

    return ''
  }

  function atualizarErro(campo, mensagem) {
    setErros((anteriores) => ({ ...anteriores, [campo]: mensagem }))
  }

  function revalidarSeComErro(campo, valoresAtuais) {
    if (erros[campo]) atualizarErro(campo, validarCampo(campo, valoresAtuais))
  }

  async function consultarNomeDuplicado(nomeLimpo) {
    try {
      const encontrados = await listarProdutos(nomeLimpo)
      return encontrados.some((item) => item.nome.trim().toLowerCase() === nomeLimpo.toLowerCase() && item.id !== produto?.id)
    } catch {
      return false
    }
  }

  function tratarMudancaNome(evento) {
    const novoNome = evento.target.value
    setNome(novoNome)
    revalidarSeComErro('nome', { ...valores, nome: novoNome })
  }

  function tratarBlurNome() {
    atualizarErro('nome', validarCampo('nome', valores))
  }

  function tratarMudancaValor(novoValor) {
    setValorUnitario(novoValor)
    revalidarSeComErro('valorUnitario', { ...valores, valorUnitario: novoValor })
  }

  function tratarBlurValor() {
    atualizarErro('valorUnitario', validarCampo('valorUnitario', valores))
  }

  async function tratarSubmit(evento) {
    evento.preventDefault()
    if (emAndamentoRef.current) return
    emAndamentoRef.current = true
    setSalvando(true)

    try {
      const novosErros = {}
      for (const campo of Object.keys(IDS_CAMPOS)) {
        const mensagem = validarCampo(campo, valores)
        if (mensagem) novosErros[campo] = mensagem
      }
      if (!novosErros.nome && await consultarNomeDuplicado(nome.trim())) {
        novosErros.nome = MENSAGEM_NOME_DUPLICADO
      }
      setErros(novosErros)

      const primeiroInvalido = Object.keys(IDS_CAMPOS).find((campo) => novosErros[campo])
      if (primeiroInvalido) {
        document.getElementById(IDS_CAMPOS[primeiroInvalido])?.focus()
        return
      }

      const corpo = {
        nome: nome.trim(),
        valorUnitario: Math.round(valorUnitario * 100) / 100,
      }

      const salvo = editando
        ? await atualizarProduto(produto.id, corpo)
        : await criarProduto(corpo)
      toast.success(editando ? 'Produto atualizado.' : 'Produto cadastrado.')
      onSalvo(salvo)
    } catch {
      return
    } finally {
      emAndamentoRef.current = false
      setSalvando(false)
    }
  }

  return (
    <form className="formulario" onSubmit={tratarSubmit} noValidate>
      <div className="grid-formulario">
        <Campo id="produto-nome" rotulo="Nome" erro={erros.nome} className="campo-largo">
          <input
            id="produto-nome"
            type="text"
            autoComplete="off"
            maxLength={150}
            autoFocus
            value={nome}
            onChange={tratarMudancaNome}
            onBlur={tratarBlurNome}
          />
        </Campo>

        <Campo id="produto-valor" rotulo="Valor unitário" erro={erros.valorUnitario}>
          <InputMoeda
            id="produto-valor"
            valor={valorUnitario}
            onChange={tratarMudancaValor}
            onBlur={tratarBlurValor}
            onFocus={(evento) => evento.target.select()}
            maxDigitos={9}
          />
        </Campo>
      </div>

      <div className="acoes-modal">
        <button type="button" className="botao botao-secundario" onClick={onCancelar} disabled={salvando}>
          Cancelar
        </button>
        <button type="submit" className="botao botao-primario" disabled={salvando}>
          {salvando ? 'Salvando...' : 'Salvar'}
        </button>
      </div>
    </form>
  )
}
