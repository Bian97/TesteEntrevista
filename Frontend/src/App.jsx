import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import { ToastContainer } from 'react-toastify'
import 'react-toastify/dist/ReactToastify.css'
import MainLayout from './layouts/MainLayout'
import Clientes from './pages/Clientes'
import NaoEncontrado from './pages/NaoEncontrado'
import NovaVenda from './pages/NovaVenda'
import Produtos from './pages/Produtos'
import Vendas from './pages/Vendas'

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Navigate to="/vendas/nova" replace />} />
        <Route element={<MainLayout />}>
          <Route path="/vendas" element={<Vendas />} />
          <Route path="/vendas/nova" element={<NovaVenda />} />
          <Route path="/clientes" element={<Clientes />} />
          <Route path="/produtos" element={<Produtos />} />
          <Route path="*" element={<NaoEncontrado />} />
        </Route>
      </Routes>
      <ToastContainer position="top-right" autoClose={4000} newestOnTop />
    </BrowserRouter>
  )
}
