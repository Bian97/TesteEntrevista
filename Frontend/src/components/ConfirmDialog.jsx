import Modal from './Modal'

export default function ConfirmDialog({
  aberto,
  titulo,
  mensagem,
  textoConfirmar = 'Confirmar',
  textoCancelar = 'Cancelar',
  perigoso = false,
  carregando = false,
  onConfirmar,
  onCancelar,
}) {
  return (
    <Modal aberto={aberto} titulo={titulo} onFechar={onCancelar} largura="sm">
      <p className="confirmacao-mensagem">{mensagem}</p>
      <div className="acoes-modal">
        <button type="button" className="botao botao-secundario" onClick={onCancelar} disabled={carregando}>
          {textoCancelar}
        </button>
        <button
          type="button"
          className={`botao ${perigoso ? 'botao-perigo' : 'botao-primario'}`}
          onClick={onConfirmar}
          disabled={carregando}
        >
          {textoConfirmar}
        </button>
      </div>
    </Modal>
  )
}
