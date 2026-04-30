import { WebSocket } from 'ws';
import { prisma } from './prisma.js';
import { sanitize } from './sanitize.js';

type SendMessagePayload = {
	username: string;
	content: string;
};

function isValidPayload(value: unknown): value is SendMessagePayload {
	return typeof value === 'object' && value !== null && 'username' in value && 'content' in value;
}

export async function handleGetMessages(ws: WebSocket): Promise<void> {
	const messages = await prisma.message.findMany({
		orderBy: { createdAt: 'asc' },
	});

	ws.send(JSON.stringify({ type: 'messages', data: messages }));
}

export async function handleSendMessage(payload: unknown, clients: Set<WebSocket>): Promise<void> {
	if (!isValidPayload(payload)) {
		throw new Error('Invalid payload: expected { username, content }');
	}

	const username = sanitize(payload.username);
	const content = sanitize(payload.content);

	if (!username || !content) {
		throw new Error('username and content must not be empty');
	}

	const message = await prisma.message.create({
		data: { username, content },
	});

	const outbound = JSON.stringify({ type: 'newMessage', data: message });

	for (const client of clients) {
		if (client.readyState === WebSocket.OPEN) {
			client.send(outbound);
		}
	}
}
