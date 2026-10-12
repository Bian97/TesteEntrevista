import { useRef, useState } from 'react'
import { toast } from 'react-toastify'
import Campo from './Campo'
import { atualizarCliente, criarCliente, listarClientes } from '../services/clientes'
import { DATA_MINIMA_NASCIMENTO, dataMaximaNascimento, mascaraCpf, mascaraTelefone, somenteDigitos } from '../utils/formatadores'
import { cpfValido, emailValido } from '../utils/validadores'

const IDS_CAMPOS = {
  cpf: 'cliente-cpf',
  nome: 'cliente-nome',
  dataNascimento: 'cliente-nascimento',
  telefone: 'cliente-telefone',
  email: 'cliente-email',
}

export default function ClienteForm({ cliente, onSalvo, onCancelar }) {
  const editando = Boolean(cliente)
  const cpfOriginal = somenteDigitos(cliente?.cpf)
  const dataOriginal = cliente?.dataNascimento ? String(cliente.dataNascimento).slice(0, 10) : ''

  const [cpf, setCpf] = useState(() => mascaraCpf(cliente?.cpf ?? ''))
  const [nome, setNome] = useState(cliente?.nome ?? '')
  const [dataNascimento, setDataNascimento] = useState(dataOriginal)
  const [email, setEmail] = useState(cliente?.email ?? '')
  const [telefone, setTelefone] = useState(() => mascaraTelefone(cliente?.telefone ?? ''))
  const [erros, setErros] = useState({})
  const [salvando, setSalvando] = useState(false)
  const emAndamentoRef = useRef(false)

  const valores = { cpf, nome, dataNascimento, email, telefone }

  function validarCampo(campo, valoresAtuais) {
    if (campo === 'cpf') {
      const cpfDigitos = somenteDigitos(valoresAtuais.cpf)
      if (!cpfDigitos) return 'Informe o CPF.'
      if (!cpfValido(cpfDigitos)) return 'CPF inválido.'
    }

    if (campo === 'nome') {
      const nomeLimpo = valoresAtuais.nome.trim()
      if (!nomeLimpo) return 'Informe o nome.'
      if (nomeLimpo.length > 150) return 'Nome deve ter no máximo 150 caracteres.'
    }

    if (campo === 'dataNascimento') {
      const data = valoresAtuais.dataNascimento
      if (data && data < DATA_MINIMA_NASCIMENTO) return 'Data de nascimento deve ser a partir de 01/01/1900.'
      if (data && data > dataMaximaNascimento()) return 'A data de nascimento deve ser anterior à data de hoje.'
    }

    if (campo === 'telefone') {
      const telefoneDigitos = somenteDigitos(valoresAtuais.telefone)
      if (telefoneDigitos && telefoneDigitos.length !== 10 && telefoneDigitos.length !== 11) return 'Telefone deve ter 10 ou 11 dígitos.'
    }

    if (campo === 'email') {
      if (valoresAtuais.email.trim() && !emailValido(valoresAtuais.email)) return 'E-mail inválido.'
    }

    return ''
  }

  function atualizarErro(campo, mensagem) {
    setErros((anteriores) => ({ ...anteriores, [campo]: mensagem }))
  }

  function revalidarSeComErro(campo, valoresAtuais) {
    if (erros[campo]) atualizarErro(campo, validarCampo(campo, valoresAtuais))
  }

  function alterar(campo, setter, novoValor) {
    setter(novoValor)
    revalidarSeComErro(campo, { ...valores, [campo]: novoValor })
  }

  function validarNoBlur(campo) {
    atualizarErro(campo, validarCampo(campo, valores))
  }

  async function consultarCpfDuplicado(cpfDigitos) {
    try {
      const encontrados = await listarClientes(cpfDigitos)
      const duplicado = encontrados.find((item) => item.cpf === cpfDigitos && item.id !== cliente?.id)
      return duplicado ? `CPF já cadastrado para ${duplicado.nome}.` : ''
    } catch {
      return ''
    }
  }

  function tratarMudancaCpf(evento) {
    const novoCpf = mascaraCpf(evento.target.value)
    const digitos = somenteDigitos(novoCpf)
    setCpf(novoCpf)
    if (erros.cpf || digitos.length === 11) atualizarErro('cpf', validarCampo('cpf', { ...valores, cpf: novoCpf }))
  }

  function tratarBlurEmail() {
    const emailLimpo = email.trim()
    setEmail(emailLimpo)
    atualizarErro('email', validarCampo('email', { ...valores, email: emailLimpo }))
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
      const cpfDigitos = somenteDigitos(cpf)
      if (!novosErros.cpf && cpfDigitos !== cpfOriginal) {
        const mensagem = await consultarCpfDuplicado(cpfDigitos)
        if (mensagem) novosErros.cpf = mensagem
      }
      setErros(novosErros)

      const primeiroInvalido = Object.keys(IDS_CAMPOS).find((campo) => novosErros[campo])
      if (primeiroInvalido) {
        document.getElementById(IDS_CAMPOS[primeiroInvalido])?.focus()
        return
      }

      const corpo = {
        cpf: cpfDigitos,
        nome: nome.trim(),
        dataNascimento: dataNascimento || null,
        email: email.trim() || null,
        telefone: somenteDigitos(telefone) || null,
      }

      const salvo = editando
        ? await atualizarCliente(cliente.id, corpo)
        : await criarCliente(corpo)
      toast.success(editando ? 'Cliente atualizado.' : 'Cliente cadastrado.')
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
        <Campo id="cliente-cpf" rotulo="CPF" erro={erros.cpf}>
          <input
            id="cliente-cpf"
            type="text"
            inputMode="numeric"
            autoComplete="off"
            autoFocus={!editando}
            placeholder="000.000.000-00"
            value={cpf}
            onChange={tratarMudancaCpf}
            onBlur={() => validarNoBlur('cpf')}
          />
        </Campo>

        <Campo id="cliente-nome" rotulo="Nome" erro={erros.nome}>
          <input
            id="cliente-nome"
            type="text"
            autoComplete="off"
            autoCapitalize="words"
            maxLength={150}
            autoFocus={editando}
            value={nome}
            onChange={(e) => alterar('nome', setNome, e.target.value)}
            onBlur={() => validarNoBlur('nome')}
          />
        </Campo>

        <Campo id="cliente-nascimento" rotulo="Nascimento" erro={erros.dataNascimento}>
          <input
            id="cliente-nascimento"
            type="date"
            min={DATA_MINIMA_NASCIMENTO}
            max={dataMaximaNascimento()}
            value={dataNascimento}
            onChange={(e) => alterar('dataNascimento', setDataNascimento, e.target.value)}
            onBlur={() => validarNoBlur('dataNascimento')}
          />
        </Campo>

        <Campo id="cliente-telefone" rotulo="Telefone" erro={erros.telefone}>
          <input
            id="cliente-telefone"
            type="text"
            inputMode="tel"
            autoComplete="off"
            placeholder="(00) 00000-0000"
            value={telefone}
            onChange={(e) => alterar('telefone', setTelefone, mascaraTelefone(e.target.value))}
            onBlur={() => validarNoBlur('telefone')}
          />
        </Campo>

        <Campo id="cliente-email" rotulo="E-mail" erro={erros.email} className="campo-largo">
          <input
            id="cliente-email"
            type="email"
            inputMode="email"
            autoComplete="off"
            maxLength={200}
            value={email}
            onChange={(e) => alterar('email', setEmail, e.target.value)}
            onBlur={tratarBlurEmail}
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
