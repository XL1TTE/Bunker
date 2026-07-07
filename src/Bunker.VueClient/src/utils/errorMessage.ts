import { ApiError } from '@/api/http';

const STATUS_FALLBACK: Record<number, string> = {
  400: 'The request was rejected by the server.',
  401: 'You need to be signed in. Please log in again.',
  403: "You don't have permission to do that.",
  404: 'Not found.',
  409: 'Conflict — someone else changed this first.',
  429: 'Too many requests. Slow down a bit.',
  500: 'Server error. Please try again later.',
  502: 'The server is unavailable right now.',
  503: 'The server is temporarily unavailable.',
  504: 'The server took too long to respond.',
};

function joinValidationErrors(errors: unknown): string {
  if (!errors || typeof errors !== 'object') return '';
  const lines: string[] = [];
  for (const [field, value] of Object.entries(errors as Record<string, unknown>)) {
    const messages = Array.isArray(value) ? value.map(String) : [String(value)];
    for (const m of messages) {
      const trimmed = m.trim();
      if (!trimmed) continue;
      lines.push(field ? `${field}: ${trimmed}` : trimmed);
    }
  }
  return lines.join('\n');
}

function parseApiErrorBody(error: ApiError): string | null {
  const body = error.body?.trim();
  if (!body) return null;

  // Most backend errors are JSON. Try that first.
  try {
    const parsed = JSON.parse(body);
    if (typeof parsed === 'string') return parsed.trim() || null;
    if (parsed && typeof parsed === 'object') {
      const obj = parsed as Record<string, unknown>;
      // Middleware shape: { "error": "..." }
      if (typeof obj.error === 'string' && obj.error.trim()) return obj.error.trim();
      // RFC 7807 ProblemDetails / TypedResults.Problem: { "title", "detail", "status" }
      if (typeof obj.detail === 'string' && obj.detail.trim()) return obj.detail.trim();
      if (typeof obj.title === 'string' && obj.title.trim()) return obj.title.trim();
      // TypedResults.ValidationProblem: { "errors": { Field: ["msg", ...] } }
      if (obj.errors) {
        const joined = joinValidationErrors(obj.errors);
        if (joined) return joined;
      }
      if (typeof obj.message === 'string' && obj.message.trim()) return obj.message.trim();
    }
  } catch {
    // Not JSON — fall through to the raw body below.
  }

  // Plain-text body (e.g. a minimal API string result that wasn't serialized as JSON).
  return body;
}

export function extractErrorMessage(error: unknown): string {
  if (error instanceof ApiError) {
    const parsed = parseApiErrorBody(error);
    if (parsed) return parsed;
    return STATUS_FALLBACK[error.status] ?? `Request failed (HTTP ${error.status}).`;
  }

  if (error instanceof Error) {
    // fetch() throws a TypeError on network/transport failures.
    if (/failed to fetch|networkerror|load failed/i.test(error.message)) {
      return 'Network error — could not reach the server. Check your connection.';
    }
    return error.message || 'Something went wrong.';
  }

  return 'Something went wrong.';
}