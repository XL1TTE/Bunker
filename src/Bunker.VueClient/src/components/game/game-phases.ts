import type { GamePhase } from '@/types/game.types';

export const GAME_PHASE_ORDER: GamePhase[] = [
  'BunkerIntroduction',
  'IntroDiscussion',
  'Reveal',
  'Discussion',
  'DiscussionClosing',
  'Voting',
  'Roulette',
  'Finished',
];

export const PHASE_LABELS: Record<GamePhase, string> = {
  BunkerIntroduction: 'Bunker briefing',
  IntroDiscussion: 'Introductions',
  Reveal: 'Reveal',
  Discussion: 'Discussion',
  DiscussionClosing: 'Closing statements',
  Voting: 'Voting',
  Roulette: 'Roulette',
  Finished: 'Finished',
};

export const PHASE_DESCRIPTIONS: Record<GamePhase, string> = {
  BunkerIntroduction:
    'The bunker scenario is being revealed. Open the briefing card to see what you are surviving for.',
  IntroDiscussion:
    'Take turns introducing yourselves in the game chat. On your turn, make your opening case.',
  Reveal:
    'Each player reveals one hidden trait from their sheet. On your turn, pick a trait to reveal.',
  Discussion:
    'Chat is open to everyone. Make your case for why you belong in the bunker.',
  DiscussionClosing:
    'Take turns making your final case in the game chat before the vote.',
  Voting:
    'Cast a secret vote for who should be eliminated. Only the result is revealed at elimination.',
  Roulette:
    'The vote tied twice. The roulette will land on the player to eliminate.',
  Finished: 'The game is over.',
};

export const MODAL_PHASES: ReadonlySet<GamePhase> = new Set<GamePhase>([
  'IntroDiscussion',
  'Reveal',
  'Discussion',
  'DiscussionClosing',
  'Voting',
  'Roulette',
]);