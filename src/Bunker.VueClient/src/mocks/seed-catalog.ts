import type { CardPackPreview, PersonalityPresetPreview } from '@/types/content.types';

export const SEED_PACKS: CardPackPreview[] = [
  {
    id: '11111111-1111-1111-1111-111111111111',
    title: 'Default',
    description: 'The standard Bunker experience — balanced professions and traits.',
  },
  {
    id: '22222222-2222-2222-2222-222222222222',
    title: '18+',
    description: 'Mature professions, dark humour, and adult situations.',
  },
  {
    id: '33333333-3333-3333-3333-333333333333',
    title: 'Sci-Fi',
    description: 'Space stations, exotic professions, off-world hazards.',
  },
];

export const SEED_PRESETS: PersonalityPresetPreview[] = [
  {
    id: 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
    title: 'The Paranoiac',
    description: 'Convinced everyone is lying. Will accuse players at every turn.',
  },
  {
    id: 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
    title: 'The Optimist',
    description: 'Believes the best in everyone. Bad at reading lies, great at morale.',
  },
  {
    id: 'cccccccc-cccc-cccc-cccc-cccccccccccc',
    title: 'The Pragmatist',
    description: 'Votes based on utility, never emotion. Cold but consistent.',
  },
];