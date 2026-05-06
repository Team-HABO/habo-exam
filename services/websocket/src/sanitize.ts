import xss from 'xss';

export function sanitize(input: unknown): string {
	if (typeof input !== 'string') {
		throw new TypeError('Expected a string');
	}
	return xss(input.trim());
}
