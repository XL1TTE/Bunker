export type GameStartStepId =
  | 'validate-lobby'
  | 'request-content'
  | 'fetch-content'
  | 'validate-content'
  | 'create-game';

export const GAME_START_STEP_IDS: GameStartStepId[] = [
  'validate-lobby',
  'request-content',
  'fetch-content',
  'validate-content',
  'create-game',
];

export const GAME_START_STEP_LABELS: Record<GameStartStepId, string> = {
  'validate-lobby': 'Validating lobby',
  'request-content': 'Requesting game content',
  'fetch-content': 'Fetching card packs',
  'validate-content': 'Validating cards',
  'create-game': 'Creating the game',
};