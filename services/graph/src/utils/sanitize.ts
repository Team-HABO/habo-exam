import xss from "xss";

// Remove any HTML tags
export function sanitizeInput(input: string): string {
    return xss(input, {
        whiteList: {}, // Empty whitelist - remove all HTML tags
        stripIgnoreTag: true,
    });
}
