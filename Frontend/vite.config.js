import { defineConfig, loadEnv } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '')
  const alvoProxy = env.API_PROXY_TARGET || process.env.API_PROXY_TARGET || 'http://localhost:8080'

  return {
    plugins: [react()],
    server: {
      host: 'localhost',
      port: 5173,
      proxy: {
        '/api': {
          target: alvoProxy,
          changeOrigin: true,
        },
      },
    },
  }
})
