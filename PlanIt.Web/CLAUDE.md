# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Development
bun run dev          # Start dev server

# Build
bun run build        # Production build
bun run preview      # Preview production build

# Code Quality
bun run check        # Type-check with svelte-check
bun run check:watch  # Type-check in watch mode
bun run lint         # Prettier + ESLint check
bun run format       # Auto-format with Prettier

# Testing
bun run test         # Run all tests once
bun run test:unit    # Run tests in watch mode
```

Use `bun` as the package manager (not npm or yarn).

## App Purpose

**PlanIt** is an outing planner for Salt Lake City. Users browse and pick from pre-inserted schedules ("outings"), each containing an ordered list of attractions (venues, activities, stops). The core loop is: discover an outing → view its attractions → go.

## Theme

The visual theme is **Planetarium** — deep space aesthetic with a dark navy/black background (`#04060f`), twinkling star animations, nebula blob glows (purple/blue blurred radial gradients), and glassmorphism cards.

All UI copy must fit both the planetarium aesthetic **and** the outing-planner purpose. Use space/exploration language that maps naturally to outing planning. Examples:

| Context | Copy |
|---|---|
| Login heading | "Welcome back, Explorer" |
| Login subheading | "Ready to continue your journey?" |
| Login CTA | "Launch Mission" |
| Register heading | "Begin your journey" |
| Register CTA | "Enter the Universe" |
| Empty state (no outings) | "No expeditions charted yet" |
| Outing card | "Expedition: [name]" |
| Attractions list | "Waypoints" |
| Schedule/date | "Departure" |

Avoid generic SaaS copy ("Get started", "Sign up", "Dashboard"). Every string should feel like it belongs in a planetarium that also runs city tours.

## Architecture

**Stack**: Svelte 5, SvelteKit 2, TypeScript, TailwindCSS 4, Vitest

**Routing**: File-based via `src/routes/`. Pages are `+page.svelte`, layouts are `+layout.svelte`, server-only data loading is `+layout.server.ts` / `+page.server.ts`.

**Data fetching**: `@sveltestack/svelte-query` (React Query for Svelte) is configured globally in `src/routes/+layout.ts` and `+layout.svelte`. Queries are browser-only (`enabled: browser`), stale after 60s.

**Svelte 5 runes**: Enabled globally via `svelte.config.js` (`runes: true`). Use `$state`, `$derived`, `$effect`, `$props` etc. instead of the legacy reactive syntax.

**Styling**: TailwindCSS v4 with `@tailwindcss/forms` and `@tailwindcss/typography` plugins. Global styles live in `src/routes/layout.css`. Prettier is configured with the Tailwind plugin to auto-sort class names.

**Path alias**: `$lib` resolves to `src/lib/`. Public library exports go through `src/lib/index.ts`.

## Testing

Two parallel Vitest projects run simultaneously:

- **Client** (browser/Playwright/Chromium): files matching `src/**/*.svelte.{test,spec}.{js,ts}` — for component tests
- **Server** (Node.js): files matching `src/**/*.{test,spec}.{js,ts}` excluding `.svelte.test` — for utility/server logic

All tests must include at least one assertion (enforced by Vitest config).

## Icons

Use **`@lucide/svelte`** for all icons. Import named icons directly:

```svelte
import { LogOut, User, CalendarDays } from '@lucide/svelte';
// Usage:
<LogOut class="h-4 w-4" />
```

The brand planet SVG in `Brand.svelte` and `Navbar.svelte` is a custom logo — do not replace it with a Lucide icon.

## Code Style

- **Indentation**: tabs
- **Quotes**: single
- **Trailing commas**: none
- **Print width**: 100 characters
- ESLint uses flat config (v9+) — config is in `eslint.config.js`
