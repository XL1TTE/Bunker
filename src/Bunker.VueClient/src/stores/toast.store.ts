import { defineStore } from 'pinia';
import { ref } from 'vue';

export type ToastType = 'error' | 'success' | 'info';

export interface Toast {
  id: number;
  type: ToastType;
  message: string;
}

let nextId = 1;

export const useToastStore = defineStore('toast', () => {
  const toasts = ref<Toast[]>([]);

  function dismiss(id: number): void {
    toasts.value = toasts.value.filter((t) => t.id !== id);
  }

  function push(type: ToastType, message: string, timeoutMs: number): number {
    const id = nextId++;
    toasts.value.push({ id, type, message });
    if (timeoutMs > 0) {
      window.setTimeout(() => dismiss(id), timeoutMs);
    }
    return id;
  }

  function error(message: string, timeoutMs = 8000): number {
    return push('error', message, timeoutMs);
  }

  function success(message: string, timeoutMs = 4000): number {
    return push('success', message, timeoutMs);
  }

  function info(message: string, timeoutMs = 5000): number {
    return push('info', message, timeoutMs);
  }

  function clear(): void {
    toasts.value = [];
  }

  return { toasts, push, error, success, info, dismiss, clear };
});