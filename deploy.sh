#!/bin/bash
set -e

TARGET=${1:-}

if [ "$TARGET" = "pilot" ]; then
  BRANCH=master
  REMOTE_PATH=/opt/algreen/api/
  SERVICE=algreen-api
elif [ "$TARGET" = "staging" ]; then
  BRANCH=staging
  REMOTE_PATH=/opt/alblue/api/
  SERVICE=alblue-api
else
  echo "Usage: ./deploy.sh [staging|pilot]"
  echo "  staging → branch=staging  → /opt/alblue/api/   + alblue-api"
  echo "  pilot   → branch=master   → /opt/algreen/api/  + algreen-api"
  exit 1
fi

if [ -n "$(git status --porcelain)" ]; then
  echo "❌ Working tree has uncommitted changes — commit or stash before deploying."
  exit 1
fi

echo "🌿 Switching to $BRANCH and pulling latest..."
git fetch origin "$BRANCH"
git checkout "$BRANCH"
git pull --ff-only origin "$BRANCH"

echo "🔨 Building backend (commit: $(git rev-parse --short HEAD))..."
dotnet publish AlgreenMES.API/AlgreenMES.API.csproj -c Release -o ./publish

echo "📦 Uploading to server ($TARGET → $REMOTE_PATH)..."
rsync -az --delete --exclude='appsettings.Production.json' --exclude='uploads/' ./publish/ root@46.101.166.137:$REMOTE_PATH

echo "🔄 Restarting $SERVICE..."
ssh root@46.101.166.137 "systemctl restart $SERVICE"

echo "✅ Backend deployed to $TARGET (branch: $BRANCH, commit: $(git rev-parse --short HEAD))"
