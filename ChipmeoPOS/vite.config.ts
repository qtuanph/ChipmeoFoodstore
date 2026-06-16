import devtoolsJson from 'vite-plugin-devtools-json';
import tailwindcss from '@tailwindcss/vite';
import { sveltekit } from '@sveltejs/kit/vite';
import { defineConfig } from 'vite';

export default defineConfig({
	plugins: [tailwindcss(), sveltekit(), devtoolsJson()],
	server: {
		allowedHosts: ['demo.chipmeo.io.vn', 'localhost', 'api.chipmeo.io.vn', 'food.chipmeo.io.vn'],
		// PROXY CONFIGURATION (LOCAL DEV ONLY)
		// Khi chạy "npm run dev", frontend chạy ở localhost:5173.
		// Proxy này giúp chuyển hướng các request /api sang backend đang chạy ở localhost:5142
		// để tránh lỗi CORS và giả lập môi trường như production.
		// LƯU Ý: Cấu hình này KHÔNG có tác dụng khi build ra production (npm run build).
		proxy: {
			'/api': {
				target: 'http://localhost:5142',
				changeOrigin: true,
				secure: false
			},
			'/hubs': {
				target: 'http://localhost:5142',
				changeOrigin: true,
				secure: false,
				ws: true // Important for SignalR WebSockets
			}
		}
	},
	build: {
		sourcemap: false,
		rollupOptions: {
			onwarn(warning, warn) {
				if (
					warning.code === 'INVALID_ANNOTATION' &&
					warning.message.includes('@microsoft/signalr')
				) {
					return;
				}
				warn(warning);
			}
		}
	}
});
