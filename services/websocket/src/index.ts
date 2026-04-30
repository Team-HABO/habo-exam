import 'dotenv/config';
import { createServer } from 'node:http';
import { WebSocketServer, WebSocket } from 'ws';
import { handleGetMessages, handleSendMessage } from './messageHandlers.js';

const PORT = Number(process.env.PORT) || 3000;

const ALLOWED_ORIGINS = ['http://localhost', 'http://127.0.0.1'];

const server = createServer((_req, res) => {
	// Any plain HTTP request gets rejected — this server is WebSocket-only
	res.writeHead(426, { 'Content-Type': 'text/plain' });
	res.end('426 Upgrade Required — this server only accepts WebSocket connections.');
});

const wss = new WebSocketServer({
	server,
	verifyClient({ origin }: { origin: string }) {
		const allowed = !origin || ALLOWED_ORIGINS.some((o) => origin.startsWith(o));

		if (!allowed) {
			console.warn(`[ws] Rejected connection from disallowed origin: ${origin}`);
		}

		return allowed;
	},
});

wss.on('connection', (ws: WebSocket) => {
	console.log(`[ws] Client connected — active: ${wss.clients.size}`);

	ws.on('message', async (rawMessage) => {
		let parsed: unknown;

		try {
			parsed = JSON.parse(rawMessage.toString());
		} catch {
			ws.send(JSON.stringify({ type: 'error', message: 'Message must be valid JSON' }));
			return;
		}

		// Validate the message envelope
		if (
			typeof parsed !== 'object' ||
			parsed === null ||
			!('type' in parsed) ||
			typeof (parsed as Record<string, unknown>).type !== 'string'
		) {
			ws.send(JSON.stringify({ type: 'error', message: "Missing or invalid 'type' field" }));
			return;
		}

		const { type, payload } = parsed as { type: string; payload?: unknown };

		// Route to the correct handler
		try {
			switch (type) {
				case 'getMessages':
					await handleGetMessages(ws);
					break;
				case 'sendMessage':
					await handleSendMessage(payload, wss.clients);
					break;
				default:
					ws.send(JSON.stringify({ type: 'error', message: `Unknown message type: "${type}"` }));
			}
		} catch (err) {
			const message = err instanceof Error ? err.message : 'Internal server error';
			ws.send(JSON.stringify({ type: 'error', message }));
		}
	});

	ws.on('close', () => {
		console.log(`[ws] Client disconnected — active: ${wss.clients.size}`);
	});

	ws.on('error', (err) => {
		console.error('[ws] Socket error:', err.message);
	});
});

server.listen(PORT, () => {
	console.log(`[ws] Server listening on ws://localhost:${PORT}`);
});
