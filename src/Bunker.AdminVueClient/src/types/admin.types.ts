// ContentService admin DTOs (camelCase over the wire — the backend's JsonConfiguration
// camelCases PascalCase C# records). Routes are gateway-prefixed /content/*.

export type CardTypeKey =
  | 'profession'
  | 'hobbies'
  | 'age'
  | 'sex'
  | 'fact'
  | 'health'
  | 'luggage';

export type CardValueType = 'string' | 'int';

export interface CardTypeConfig {
  key: CardTypeKey;
  label: string;
  // The single JSON field this card type carries (e.g. 'profession', 'age').
  fieldName: string;
  valueType: CardValueType;
  placeholder: string;
}

// The 7 character-card types share an identical list/create/update/delete shape and differ
// only in their one field, so the UI is driven by this descriptor.
export const CARD_TYPES: CardTypeConfig[] = [
  { key: 'profession', label: 'Profession', fieldName: 'profession', valueType: 'string', placeholder: 'e.g. Surgeon' },
  { key: 'hobbies', label: 'Hobbies', fieldName: 'hobbies', valueType: 'string', placeholder: 'e.g. Rock climbing' },
  { key: 'age', label: 'Age', fieldName: 'age', valueType: 'int', placeholder: 'e.g. 34' },
  { key: 'sex', label: 'Sex', fieldName: 'sex', valueType: 'string', placeholder: 'e.g. Female' },
  { key: 'fact', label: 'Fact', fieldName: 'fact', valueType: 'string', placeholder: 'e.g. Speaks four languages' },
  { key: 'health', label: 'Health', fieldName: 'health', valueType: 'string', placeholder: 'e.g. Mild asthma' },
  { key: 'luggage', label: 'Luggage', fieldName: 'luggage', valueType: 'string', placeholder: 'e.g. First-aid kit' },
];

export function findCardType(key: string): CardTypeConfig | undefined {
  return CARD_TYPES.find((t) => t.key === key);
}

// A card of any type — `id` plus the type-specific field, accessed by fieldName at runtime.
export type UnknownCard = { id: string } & Record<string, unknown>;

export interface BunkerCardDto {
  id: string;
  catastrophe: string;
  survivalDuration: string;
  bunkerEnvironment: string;
}

export interface CardPackDto {
  id: string;
  title: string;
  description: string;
  generationPrompt: string;
  cardIds: string[];
}

export interface CardPackPreviewDto {
  id: string;
  title: string;
  description: string;
}

export interface PersonalityPresetDto {
  id: string;
  title: string;
  description: string;
}

// --- Request bodies ---

export type CreateCardRequest = Record<string, string | number>;

export interface CreateBunkerCardRequest {
  catastrophe: string;
  survivalDuration: string;
  bunkerEnvironment: string;
}

export interface CreateCardPackRequest {
  title: string;
  description: string;
  generationPrompt: string;
  cardIds: string[];
}

export interface CreatePersonalityPresetRequest {
  title: string;
  description: string;
}

// --- Response wrappers (the backend wraps payloads in single-field structs) ---

export interface CardListResponse {
  total: number;
  cards: UnknownCard[];
}
export interface BunkerCardListResponse {
  cards: BunkerCardDto[];
}
export interface BunkerCardResponse {
  card: BunkerCardDto;
}
export interface PackListResponse {
  packs: CardPackDto[];
}
export interface PackResponse {
  pack: CardPackDto;
}
export interface PresetListResponse {
  presets: PersonalityPresetDto[];
}
export interface PresetResponse {
  preset: PersonalityPresetDto;
}