import { NavLink, Outlet } from 'react-router-dom'
import { FiList, FiPackage, FiPlusCircle, FiShoppingBag, FiUsers } from 'react-icons/fi'

const LINKS_MENU = [
  { to: '/vendas/nova', rotulo: 'Nova venda', icone: FiPlusCircle, destaque: true },
  { to: '/vendas', rotulo: 'Vendas', icone: FiList, end: true },
  { to: '/clientes', rotulo: 'Clientes', icone: FiUsers },
  { to: '/produtos', rotulo: 'Produtos', icone: FiPackage },
]

export default function MainLayout() {
  return (
    <div className="layout">
      <aside className="sidebar">
        <div className="sidebar-logo">
          <FiShoppingBag />
          <span>Vendas</span>
        </div>
        <nav className="sidebar-nav">
          {LINKS_MENU.map((link) => {
            const Icone = link.icone
            return (
              <NavLink
                key={link.to}
                to={link.to}
                end={link.end}
                title={link.rotulo}
                className={({ isActive }) =>
                  `nav-link${link.destaque ? ' nav-link-destaque' : ''}${isActive ? ' ativo' : ''}`
                }
              >
                <Icone />
                <span className="nav-texto">{link.rotulo}</span>
              </NavLink>
            )
          })}
        </nav>
      </aside>
      <main className="conteudo">
        <Outlet />
      </main>
    </div>
  )
}
