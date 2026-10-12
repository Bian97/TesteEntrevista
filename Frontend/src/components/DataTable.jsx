import { FiInbox } from 'react-icons/fi'

export default function DataTable({
  colunas,
  linhas,
  carregando = false,
  mensagemVazio = 'Nenhum registro encontrado.',
  chave = 'id',
  onClickLinha,
}) {
  const totalColunas = colunas.length

  function tratarTeclaLinha(evento, linha) {
    if (evento.key === 'Enter') onClickLinha(linha)
  }

  return (
    <div className="tabela-wrapper">
      <table className="tabela">
        <thead>
          <tr>
            {colunas.map((coluna, indice) => (
              <th key={indice} className={coluna.className}>{coluna.titulo}</th>
            ))}
          </tr>
        </thead>
        <tbody>
          {carregando && (
            <tr>
              <td colSpan={totalColunas} className="celula-centralizada">
                <span className="spinner" />
              </td>
            </tr>
          )}
          {!carregando && linhas.length === 0 && (
            <tr>
              <td colSpan={totalColunas}>
                <div className="estado-vazio">
                  <FiInbox size={28} />
                  <p>{mensagemVazio}</p>
                </div>
              </td>
            </tr>
          )}
          {!carregando &&
            linhas.map((linha) => (
              <tr
                key={linha[chave]}
                className={onClickLinha ? 'linha-clicavel' : undefined}
                onClick={onClickLinha ? () => onClickLinha(linha) : undefined}
                onKeyDown={onClickLinha ? (evento) => tratarTeclaLinha(evento, linha) : undefined}
                tabIndex={onClickLinha ? 0 : undefined}
              >
                {colunas.map((coluna, indice) => (
                  <td key={indice} className={coluna.className}>
                    {coluna.render ? coluna.render(linha) : linha[coluna.chave]}
                  </td>
                ))}
              </tr>
            ))}
        </tbody>
      </table>
    </div>
  )
}
