# Easy MES Backend (Skyhard side-business)

> **New Claude session: read `docs/CLAUDE_ONBOARDING.md` first.** Covers the full picture across all 5 repos (2 BE + 3 FE), infrastructure, deploy commands, Sprint 3 outcomes, workflow rules, gotchas, and Milos's preferences. This file is per-repo coding conventions only.
>
> **This repo is a mirror of `algreen-tracker-be`** (the skysoft BE source of truth). All BE code is identical between the two — `cp` between them is fine (no per-product branding in BE). Every security/code fix lands in both repos.

## Project Overview

Same .NET 9 modular monolith as `algreen-tracker-be`. Modules: Tenancy, Identity, Production, Orders. PostgreSQL, JWT auth, SignalR.

## Differences from algreen-tracker-be

- Deploy target: easy-mes droplet `46.101.125.31`, not the skysoft droplet
- DB: `easy_mes` (Postgres container `easy-mes-postgres`)
- Branch: `main` (skysoft uses `staging` for alblue, `master` for algreen pilot)
- Sentry environment tag: `easy-mes-prod`
- Serilog log path: `/var/log/easy-mes/api-YYYYMMDD.log`

## Code conventions

See `algreen-tracker-be/CLAUDE.md` — the conventions are the same for both BE repos. Don't fork them.

## Deploy

```bash
./deploy.sh easymes        # → /opt/easy-mes/api/, easy-mes-api service
```

Same `deploy.sh` flow as the skysoft BE: dirty-tree refusal → branch checkout/pull → `dotnet publish` → `rsync` → `--migrate` step → `systemctl restart`.

## Mirror discipline

Every BE change in `algreen-tracker-be` lands here too in a separate commit. Don't skip — security drift between the two would be a real problem. The audit work in Sprint 3.0 explicitly mirrored across both.
