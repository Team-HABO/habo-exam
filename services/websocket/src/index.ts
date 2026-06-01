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

function parseEnvelope(raw: Buffer): { type: string; payload?: unknown } | null {
	try {
		const parsed = JSON.parse(raw.toString());
		if (typeof parsed?.type !== 'string') return null;
		return parsed as { type: string; payload?: unknown };
	} catch {
		return null;
	}
}

const wss = new WebSocketServer({
	server,
	verifyClient({ origin }: { origin: string }) {
		if (origin && !ALLOWED_ORIGINS.some((o) => origin.startsWith(o))) {
			console.warn(`[ws] Rejected connection from disallowed origin: ${origin}`);
			return false;
		}
		return true;
	},
});

wss.on('connection', (ws: WebSocket) => {
	console.log(`[ws] Client connected — active: ${wss.clients.size}`);

	ws.on('message', async (rawMessage) => {
		const envelope = parseEnvelope(rawMessage as Buffer);

		if (!envelope) {
			ws.send(JSON.stringify({ type: 'error', message: 'Invalid message — expected JSON with a "type" field' }));
			return;
		}

		try {
			switch (envelope.type) {
				case 'getMessages':
					await handleGetMessages(ws);
					break;
				case 'sendMessage':
					await handleSendMessage(envelope.payload, wss.clients);
					break;
				default:
					ws.send(JSON.stringify({ type: 'error', message: `Unknown message type: "${envelope.type}"` }));
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
