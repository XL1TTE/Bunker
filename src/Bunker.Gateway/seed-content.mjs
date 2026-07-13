// Seeds the Bunker content database through the gateway's /content proxy.
// Run: double-click seed-content.cmd, or `node seed-content.mjs`.
// Prompts for a Keycloak access token (content-service.admin role) when run
// interactively; ACCESS_TOKEN env var still works for non-interactive use.
// Idempotent: fetches existing entries first and skips values that already exist,
// so it is safe to re-run.
import readline from "node:readline/promises";
import { stdin as input, stdout as output } from "node:process";

async function readAccessToken() {
  const envToken = (process.env.ACCESS_TOKEN ?? "").trim();
  if (envToken) {
    console.log("Using ACCESS_TOKEN from environment.");
    return envToken;
  }
  if (!process.stdin.isTTY) {
    console.error("No ACCESS_TOKEN env var and no interactive terminal. Set ACCESS_TOKEN or run via seed-content.cmd.");
    process.exit(1);
  }
  const rl = readline.createInterface({ input, output });
  try {
    const token = (await rl.question("Paste your Keycloak access token (content-service.admin role):\n> ")).trim();
    if (!token) {
      console.error("No token provided, aborting.");
      process.exit(1);
    }
    return token;
  } finally {
    rl.close();
  }
}

const TOKEN = await readAccessToken();

const BASE = (process.env.GATEWAY_URL ?? "http://127.0.0.1:5174").replace(/\/$/, "");
const headers = { Authorization: `Bearer ${TOKEN}`, "Content-Type": "application/json" };

const sleep = (ms) => new Promise((r) => setTimeout(r, ms));

async function request(method, path, body, attempts = 4) {
  for (let i = 0; ; i++) {
    const res = await fetch(`${BASE}${path}`, {
      method,
      headers,
      body: body === undefined ? undefined : JSON.stringify(body),
    });
    const text = await res.text();
    if (res.ok) return text ? JSON.parse(text) : null;
    if ((res.status === 500 || res.status === 408 || res.status === 429) && i < attempts) {
      await sleep(400 * (i + 1));
      continue;
    }
    throw new Error(`${method} ${path} -> ${res.status}: ${text}`);
  }
}

const get = (path) => request("GET", path);
const post = (path, body) => request("POST", path, body);

// --- Cards -----------------------------------------------------------------

const CARD_KINDS = [
  { kind: "profession", field: "profession", values: [
    "Software Engineer","Civil Engineer","Paramedic","Surgeon","Firefighter",
    "Police Officer","Chef","Electrician","Plumber","Mechanic","Farmer",
    "Biologist","Chemist","School Teacher","Nurse","Architect","Commercial Pilot",
    "Dentist","Veterinarian","Locksmith",
  ]},
  { kind: "hobbies", field: "hobbies", values: [
    "Rock climbing","Oil painting","Playing chess","Knitting","Gardening",
    "Photography","Astronomy","Playing guitar","Baking bread","Woodworking",
    "Martial arts","Video gaming","Reading novels","Fishing","Cycling",
  ]},
  { kind: "age", field: "age", values: [18,21,25,30,35,42,50,58,65,72,80,88] },
  { kind: "sex", field: "sex", values: ["Male","Female"] },
  { kind: "fact", field: "fact", values: [
    "Has a twin sibling","Cannot swim at all","Speaks five languages",
    "Owns a gun license","Allergic to peanuts","Has a pacemaker",
    "Was a child prodigy","Survived a plane crash","Knows morse code",
    "Has eidetic memory","Former military sniper","Trained in CPR",
    "Never uses the internet","Once won a marathon","Keeps a pet tarantula",
  ]},
  { kind: "health", field: "health", values: [
    "Perfect health","Mild asthma","Chronic back pain","Diabetic type one",
    "Severe pollen allergy","Color blind","High blood pressure","Missing left hand",
    "Deaf in one ear","Chronic insomnia","Anxiety disorder","Needs strong glasses",
  ]},
  { kind: "luggage", field: "luggage", values: [
    "First aid kit","Pocket knife","Water filter","Toolbox","Holy bible",
    "Hunting rifle","Seeds pack","Medicine supply","Flashlight","Gas mask",
    "Radio receiver","Solar charger","Dried food rations","Multi-tool","Sleeping bag",
  ]},
];

const sameValue = (a, b) =>
  typeof a === "string" && typeof b === "string"
    ? a.toLowerCase() === b.toLowerCase()
    : a === b;

async function seedCards() {
  const ids = {};
  for (const { kind, field, values } of CARD_KINDS) {
    const existing = await get(`/content/cards/${kind}?skip=0&take=1000`);
    const have = new Map();
    for (const c of existing.cards ?? []) have.set(c[field], c.id);
    ids[kind] = [];
    for (const v of values) {
      let id = undefined;
      for (const [ev, eid] of have) if (sameValue(ev, v)) id = eid;
      if (id) {
        ids[kind].push(id);
        continue;
      }
      const data = await post(`/content/cards/${kind}`, { [field]: v });
      id = data.card?.id ?? data.id;
      ids[kind].push(id);
      console.log(`  ${kind.padEnd(11)} ${String(v).padEnd(26)} -> ${id}`);
    }
  }
  return ids;
}

// --- Bunker cards ----------------------------------------------------------

async function seedBunkerCards() {
  const bunkers = [
    {
      catastrophe:
        "A global pandemic of a highly contagious airborne virus wiped out 90% of the population in three months.",
      survivalDuration: "18 months",
      bunkerEnvironment:
        "A repurposed Soviet-era underground silo with two levels. It has a hand-cranked air filter, a single diesel generator with limited fuel, a hydroponic grow rack, a water well, and a stockpile of canned food. There is no internet, no working radio transmitter, and only one pressure suit for surface trips.",
    },
    {
      catastrophe:
        "A sudden solar flare fried every electronic grid on Earth, collapsing supply chains and triggering mass famine.",
      survivalDuration: "2 years",
      bunkerEnvironment:
        "A reinforced basement under a rural farmhouse. It stores rainwater in a cistern, has a wood-burning stove, a small library of printed survival manuals, hand tools, seeds, and a sealed ammunition box. The group must ration firewood and patrol the perimeter nightly against desperate outsiders.",
    },
  ];
  const existing = (await get("/content/bunker-cards/")).cards ?? [];
  for (const b of bunkers) {
    if (existing.some((e) => e.catastrophe === b.catastrophe)) {
      console.log(`  bunker  (exists) ${b.catastrophe.slice(0, 40)}...`);
      continue;
    }
    const data = await post("/content/bunker-cards/", b);
    console.log(`  bunker  -> ${data.card?.id ?? data.id}`);
  }
}

// --- Bot personality presets ----------------------------------------------

async function seedBots() {
  const presets = [
    { title: "The Optimist", description:
      "Always looks on the bright side, tries to keep morale high, and assumes the best of every survivor. Tends to vote against excluding people." },
    { title: "The Strategist", description:
      "Cold and calculating, weighs every survivor by practical usefulness to the bunker. Will readily vote out anyone who consumes more than they contribute." },
    { title: "The Paranoid", description:
      "Distrusts everyone, suspects sabotage and secret stashes, and votes out anyone whose story has even a small inconsistency." },
    { title: "The Diplomat", description:
      "Tries to build consensus and avoid conflict, often abstains or flips to the majority to keep the group together." },
  ];
  const existing = (await get("/content/bots/")).presets ?? [];
  for (const p of presets) {
    if (existing.some((e) => (e.title ?? "").toLowerCase() === p.title.toLowerCase())) {
      console.log(`  bot     (exists) ${p.title}`);
      continue;
    }
    const data = await post("/content/bots/", p);
    console.log(`  bot     ${p.title.padEnd(18)} -> ${data.preset?.id ?? data.id}`);
  }
}

// --- Card packs ------------------------------------------------------------

async function seedPacks(ids) {
  const packs = [
    {
      title: "Classic Bunker Set",
      description:
        "The default balanced card set for a standard game of Bunker, mixing everyday professions and common conditions.",
      generationPrompt:
        "Generate grounded, realistic bunker-survivor content. Keep professions and facts plausible for ordinary modern people. Avoid supernatural or sci-fi elements.",
      cardIds: [
        ...ids.profession.slice(0, 10),
        ...ids.hobbies.slice(0, 8),
        ...ids.age,
        ...ids.sex,
        ...ids.fact.slice(0, 8),
        ...ids.health.slice(0, 6),
        ...ids.luggage.slice(0, 8),
      ],
    },
    {
      title: "Hardcore Survival Pack",
      description:
        "A punishing card set weighted toward harsh health conditions, scarce luggage, and extreme facts for tougher rounds.",
      generationPrompt:
        "Generate tense, survival-focused content. Emphasize scarce resources, physical hardship, and moral dilemmas. Characters should feel like a liability or a gamble.",
      cardIds: [
        ...ids.profession.slice(10),
        ...ids.fact.slice(8),
        ...ids.health.slice(6),
        ...ids.luggage.slice(8),
      ],
    },
    {
      title: "Professions Only",
      description:
        "A minimal pack containing only profession cards, useful for quick rounds focused on roles and contributions.",
      generationPrompt:
        "Generate diverse professions from blue-collar, white-collar, medical, and trades backgrounds. Each profession should hint at survival value.",
      cardIds: [...ids.profession],
    },
  ];
  const existing = (await get("/content/packs/")) ?? [];
  for (const p of packs) {
    if (existing.some((e) => (e.title ?? "").toLowerCase() === p.title.toLowerCase())) {
      console.log(`  pack    (exists) ${p.title}`);
      continue;
    }
    const data = await post("/content/packs/", p);
    console.log(`  pack    ${p.title.padEnd(22)} (${p.cardIds.length} cards) -> ${data.pack?.id ?? data.id}`);
  }
}

// --- main ------------------------------------------------------------------

console.log(`Seeding via ${BASE} ...`);
const ids = await seedCards();
await seedBunkerCards();
await seedBots();
await seedPacks(ids);

console.log("\nDone. Card counts (including pre-existing):");
for (const [k, v] of Object.entries(ids)) console.log(`  ${k.padEnd(11)} ${v.length}`);