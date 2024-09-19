import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import fs from 'fs';
import { fileURLToPath } from 'url';
import path from 'path';

// Obtém o diretório atual
const __dirname = path.dirname(fileURLToPath(import.meta.url));

export default defineConfig({
    plugins: [react()],
    server: {
        https: {
            key: fs.readFileSync(path.resolve(__dirname, './127.0.0.1+1-key.pem')),
            cert: fs.readFileSync(path.resolve(__dirname, './127.0.0.1+1.pem'))
        }
    }
});
