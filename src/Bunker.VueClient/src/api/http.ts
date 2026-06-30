export function getApiBaseUrl(): string {
  const url = import.meta.env.VITE_API_BASE_URL;
  if (!url) {
    throw new Error('VITE_API_BASE_URL is not configured. Set it in .env.');
  }
  return url.replace(/\/+$/, '');
}

export interface AuthTokenProvider {
  getAccessToken(): Promise<string | null>;
}

export class ApiError extends Error {
  constructor(public readonly status: number, public readonly body: string, message?: string) {
    super(message ?? `API error ${status}: ${body}`);
    this.name = 'ApiError';
  }
}

export async function apiRequest<T>(
  tokenProvider: AuthTokenProvider,
  method: string,
  path: string,
  body?: unknown,
): Promise<T> {
  const token = await tokenProvider.getAccessToken();
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
  };
  if (token) {
    headers.Authorization = `Bearer ${token}`;
  }
  const response = await fetch(`${getApiBaseUrl()}${path}`, {
    method,
    headers,
    body: body !== undefined ? JSON.stringify(body) : undefined,
  });
  if (!response.ok) {
    const text = await response.text();
    throw new ApiError(response.status, text);
  }
  if (response.status === 204) {
    return undefined as T;
  }
  return (await response.json()) as T;
}