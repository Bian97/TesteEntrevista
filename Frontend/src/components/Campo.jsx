export default function Campo({ id, rotulo, erro, dica, className = '', children }) {
  return (
    <div className={`campo${erro ? ' campo-com-erro' : ''} ${className}`.trim()}>
      <label htmlFor={id} className="campo-rotulo">{rotulo}</label>
      {children}
      {dica && !erro && <span className="campo-dica">{dica}</span>}
      <span className="campo-erro-texto">{erro || ''}</span>
    </div>
  )
}
