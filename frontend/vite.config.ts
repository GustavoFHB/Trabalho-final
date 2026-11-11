import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// Em desenvolvimento, /api é encaminhado para a API .NET (porta 5080).
// No build, o resultado vai para o wwwroot da API, que passa a servir o front.
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: { '/api': 'http://localhost:5080' },
  },
  build: {
    outDir: '../backend/src/SimuladorFormal.Api/wwwroot',
    emptyOutDir: true,
  },
})
