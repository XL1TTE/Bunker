<script setup lang="ts">
import type { DirectiveBinding } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth.store';
import BunkerAtmosphere from '@/components/landing/BunkerAtmosphere.vue';
import FeatureIcon from '@/components/icons/FeatureIcon.vue';
import AiCardIcon from '@/components/icons/AiCardIcon.vue';
import styles from '@/views/landing.module.css';

type FeatureName = 'realtime' | 'bots' | 'packs' | 'stats' | 'lobbies' | 'durability';
type AiCardName = 'shuffle' | 'sheet' | 'complete';

const authStore = useAuthStore();
const router = useRouter();

const aiCards: { icon: AiCardName; title: string; body: string }[] = [
  {
    icon: 'shuffle',
    title: 'A Unique Bunker Every Game',
    body: 'Every match draws a fresh Bunker Card — a catastrophe, a survival window, and what the bunker has and lacks.',
  },
  {
    icon: 'sheet',
    title: 'Coherent Character Sheets',
    body: 'The Bunker Card and every player’s seven-attribute sheet are generated together — coherent with each other and with the bunker.',
  },
  {
    icon: 'complete',
    title: 'Always Playable',
    body: 'Every match runs to its conclusion — no stalls, no dead ends, just a complete game from start to finish.',
  },
];

const steps = [
  { title: 'Join or Host a Lobby', desc: 'Browse public lobbies and join with a code, or create your own. The host picks card packs, sets capacity, and can fill empty slots with auto-ready bots.' },
  { title: 'Get Ready and Start', desc: 'Everyone toggles Ready. The host starts the game — the AI generates the Bunker Card and each player’s secret Character Sheet.' },
  { title: 'See the Bunker', desc: 'Round 1 reveals the Bunker Card to everyone, plus a 1-minute-per-player intro to introduce yourself.' },
  { title: 'Reveal an Attribute', desc: 'Each round, reveal one attribute from your sheet. What you reveal shapes how others judge your value. On timeout, a random attribute auto-opens.' },
  { title: 'Discuss and Persuade', desc: 'A 2-minute open chat, then 1 minute per player to make your closing case for why you belong.' },
  { title: 'Vote — Secret Ballot', desc: 'Vote for who should be eliminated. Only a “player has voted” signal shows during the phase; the full tally is revealed at elimination.' },
  { title: 'Elimination and Tie-Break', desc: 'Most votes is eliminated. A tie triggers a second vote; a second tie resolves with the Roulette — a cosmetic highlight that lands on the player to eliminate.' },
  { title: 'Survive or Lose', desc: 'Rounds repeat until the remaining players fit the bunker’s capacity. Those inside win; the rest are eliminated. Bots take their turns automatically.' },
];

const features: { name: FeatureName; title: string; desc: string }[] = [
  { name: 'realtime', title: 'Real-time multiplayer', desc: 'Live lobbies and game rooms with real-time chat and instant state updates.' },
  { name: 'bots', title: 'AI bots', desc: 'Fill slots with personality-preset bots that reveal and vote on their own.' },
  { name: 'packs', title: 'Card packs', desc: 'Mix packs (Default, 18+, Sci-Fi…) to theme a game.' },
  { name: 'stats', title: 'Profiles and stats', desc: 'Track games played, wins, and losses; customize your in-game nickname.' },
  { name: 'lobbies', title: 'Private and public lobbies', desc: 'Play with friends via invite code or open up to the browser.' },
  { name: 'durability', title: 'Survives interruptions', desc: 'A dropped connection or server hiccup won’t cut your match short — pick up right where you left off.' },
];

// The marquee duplicates the step list once so the track can translate by
// -50% for a seamless infinite loop (see .howTrack / @keyframes howMarquee).
const howLoop = [...steps, ...steps];

function onCta(): void {
  // While the boot-time profile fetch is in flight the user is authenticated
  // but `profile` is still null. Without this guard we'd call `login()` —
  // starting a second Keycloak flow on top of the one already resolving. The
  // prominent CTAs are also :disabled during this window (see template), so
  // this is a backstop for the footer "Play Now" link, which isn't disabled.
  if (authStore.initializing) return;
  if (authStore.profile) {
    void router.push('/lobbies');
  } else {
    void authStore.login();
  }
}

function scrollToHowTo(): void {
  const reduce = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
  document.getElementById('how-to-play')?.scrollIntoView({
    behavior: reduce ? 'auto' : 'smooth',
    block: 'start',
  });
}

// Scroll-triggered reveal. Applied per block (and per item, staggered by the
// binding value in ms). The directive adds .reveal on mount and toggles
// .revealVisible every time the element enters OR leaves the viewport, so the
// animation replays on each re-entry (scroll down to the footer and back up and
// the blocks fade back in again — it is not a one-shot). The observer is kept
// alive for the element's lifetime (disconnect on unmount). Reduced-motion
// leaves elements visible (CSS keeps them in their final state). The delay is
// exposed as transition-delay so wrapper-based reveals (cards, features)
// stagger on every replay.
const vReveal = {
  mounted(el: HTMLElement, binding: DirectiveBinding<number | undefined>): void {
    const reduce = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    if (reduce) return;
    el.classList.add(styles.reveal);
    const delay = typeof binding.value === 'number' ? binding.value : 0;
    if (delay) {
      el.style.transitionDelay = `${delay}ms`;
    }
    const io = new IntersectionObserver(
      (entries) => {
        for (const entry of entries) {
          entry.target.classList.toggle(styles.revealVisible, entry.isIntersecting);
        }
      },
      { threshold: 0.12, rootMargin: '0px 0px -8% 0px' },
    );
    io.observe(el);
    (el as HTMLElement & { _io?: IntersectionObserver })._io = io;
  },
  unmounted(el: HTMLElement): void {
    (el as HTMLElement & { _io?: IntersectionObserver })._io?.disconnect();
  },
};
</script>

<template>
  <div :class="styles.landing">
    <!-- Section 1 — Hero -->
    <section :class="styles.hero">
      <BunkerAtmosphere />
      <div :class="['container', styles.heroContent]">
        <span :class="styles.eyebrow">Social deduction party game</span>
        <h1 :class="styles.title">Survive by Persuasion, Not by Luck</h1>
        <p :class="styles.subtitle">
          Argue, reveal, and vote your way into a seat. Convince the others you’re
          worth keeping, or get left outside.
        </p>
        <div :class="styles.actions">
          <button
            :class="styles.cta"
            type="button"
            :disabled="authStore.initializing"
            @click="onCta"
          >
            Play Now
          </button>
          <button :class="styles.ctaGhost" type="button" @click="scrollToHowTo">
            How to Play
          </button>
        </div>
      </div>
    </section>

    <!-- Section 2 — AI-generated content -->
    <section id="ai" :class="[styles.section, styles.sectionAlt]">
      <div class="container">
        <div v-reveal :class="styles.sectionHead">
          <h2 :class="styles.sectionTitle">A Different Game Every Time, Powered by AI</h2>
          <p :class="styles.sectionIntro">
            Every game’s content is generated fresh by an AI model themed to the host’s
            chosen card packs — so no two bunkers, professions, or player dilemmas are ever
            the same.
          </p>
        </div>
        <div :class="styles.cards">
          <div
            v-for="(card, i) in aiCards"
            :key="card.title"
            v-reveal="i * 90"
            :class="styles.cardWrap"
          >
            <article :class="styles.card">
              <span :class="styles.cardIcon" aria-hidden="true">
                <AiCardIcon :name="card.icon" />
              </span>
              <h3 :class="styles.cardTitle">{{ card.title }}</h3>
              <p :class="styles.cardBody">{{ card.body }}</p>
            </article>
          </div>
        </div>
        <p v-reveal :class="styles.footnote">
          Identity stays private — the AI builds each sheet anonymously, and no player
          identity is ever sent to the model.
        </p>
      </div>
    </section>

    <!-- Section 3 — How to play (horizontal marquee of step titles) -->
    <section id="how-to-play" :class="styles.section">
      <div class="container">
        <div v-reveal :class="styles.sectionHead">
          <h2 :class="styles.sectionTitle">How to Play</h2>
        </div>
        <div :class="styles.howScroll">
          <div :class="styles.howTrack">
            <article
              v-for="(step, i) in howLoop"
              :key="i"
              :class="styles.howChip"
            >
              <span :class="styles.howDot" aria-hidden="true"></span>
              <span :class="styles.howLabel">{{ step.title }}</span>
            </article>
          </div>
        </div>
        <p v-reveal :class="styles.footnote">
          The game is automatic and turn-based — per-player and group timers move things
          forward even if someone steps away.
        </p>
      </div>
    </section>

    <!-- Section 4 — What you get -->
    <section :class="[styles.section, styles.sectionAlt]">
      <div class="container">
        <div v-reveal :class="styles.sectionHead">
          <h2 :class="styles.sectionTitle">What You Get</h2>
        </div>
        <div :class="styles.features">
          <div
            v-for="(f, i) in features"
            :key="f.title"
            v-reveal="i * 70"
            :class="styles.featureWrap"
          >
            <article :class="styles.feature">
              <FeatureIcon :name="f.name" :class="styles.featureIcon" />
              <h3 :class="styles.featureTitle">{{ f.title }}</h3>
              <p :class="styles.featureDesc">{{ f.desc }}</p>
            </article>
          </div>
        </div>
      </div>
    </section>

    <footer :class="styles.footer">
      <div :class="['container', styles.footerInner]">
        <div :class="styles.footerCta">
          <h2 :class="styles.footerCtaTitle">
            Gather your group. Make your case. Earn your seat in the bunker.
          </h2>
          <button
            :class="styles.cta"
            type="button"
            :disabled="authStore.initializing"
            @click="onCta"
          >
            Play Now
          </button>
        </div>
        <div :class="styles.footerTop">
          <div :class="styles.footerBrandBlock">
            <span :class="styles.footerMark" aria-hidden="true">B</span>
            <div>
              <span :class="styles.footerName">Bunker</span>
              <p :class="styles.footerTag">A social deduction party game</p>
            </div>
          </div>
          <nav :class="styles.footerCols" aria-label="Footer navigation">
            <div :class="styles.footerCol">
              <h4 :class="styles.footerColTitle">Play</h4>
              <ul :class="styles.footerList">
                <li>
                  <button type="button" :class="styles.footerLink" @click="scrollToHowTo">
                    How to Play
                  </button>
                </li>
                <li>
                  <button type="button" :class="styles.footerLink" @click="onCta">Play Now</button>
                </li>
              </ul>
            </div>
            <div :class="styles.footerCol">
              <h4 :class="styles.footerColTitle">Legal</h4>
              <ul :class="styles.footerList">
                <li><a :class="styles.footerLink" href="#">Privacy Policy</a></li>
                <li><a :class="styles.footerLink" href="#">Terms of Service</a></li>
                <li><a :class="styles.footerLink" href="#">Cookie Policy</a></li>
              </ul>
            </div>
            <div :class="styles.footerCol">
              <h4 :class="styles.footerColTitle">Social</h4>
              <ul :class="styles.footerList">
                <li><a :class="styles.footerLink" href="#">Discord</a></li>
                <li><a :class="styles.footerLink" href="#">X</a></li>
                <li><a :class="styles.footerLink" href="#">GitHub</a></li>
              </ul>
            </div>
          </nav>
        </div>
        <div :class="styles.footerBottom">
          <span>© 2026 Bunker. All rights reserved.</span>
          <span :class="styles.footerMade">Made with persuasion, not luck.</span>
        </div>
      </div>
    </footer>
  </div>
</template>