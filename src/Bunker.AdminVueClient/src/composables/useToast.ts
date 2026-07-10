import { useToastStore } from '@/stores/toast.store';
import { extractErrorMessage } from '@/utils/errorMessage';

/**
 * Run an async action, surfacing any failure as a toast.
 *
 * Returns the action's result on success, or `null` on failure (after toasting).
 * Call sites can branch on `null` to avoid proceeding — e.g. skipping a
 * navigation after a failed `createLobby`.
 */
export function useToast() {
  const toasts = useToastStore();

  async function run<T>(fn: () => Promise<T>, opts?: { message?: string }): Promise<T | null> {
    try {
      return await fn();
    } catch (e) {
      toasts.error(opts?.message ?? extractErrorMessage(e));
      return null;
    }
  }

  return {
    toasts,
    run,
    error: (m: string) => toasts.error(m),
    success: (m: string) => toasts.success(m),
    info: (m: string) => toasts.info(m),
    dismiss: (id: number) => toasts.dismiss(id),
  };
}