export default function PageHeader({ titulo, subtitulo, children }) {
  return (
    <header className="page-header">
      <div>
        <h1>{titulo}</h1>
        {subtitulo && <p className="page-header-subtitulo">{subtitulo}</p>}
      </div>
      {children && <div className="page-header-acoes">{children}</div>}
    </header>
  )
}
