#!/bin/bash
set -e

TARGET=${1:-}

if [ "$TARGET" = "pilot" ]; then
  REMOTE_PATH=/opt/algreen/api/
  SERVICE=algreen-api
elif [ "$TARGET" = "staging" ]; then
  REMOTE_PATH=/opt/alblue/api/
  SERVICE=alblue-api
else
  echo "Usage: ./deploy.sh [staging|pilot]"
  echo "  staging → /opt/alblue/api/ + alblue-api"
  echo "  pilot   → /opt/algreen/api/ + algreen-api"
  exit 1
fi

echo "🔨 Building backend..."
dotnet publish AlgreenMES.API/AlgreenMES.API.csproj -c Release -o ./publish

echo "📦 Uploading to server ($TARGET → $REMOTE_PATH)..."
rsync -az --delete --exclude='appsettings.Production.json' --exclude='uploads/' ./publish/ root@46.101.166.137:$REMOTE_PATH

echo "🔄 Restarting $SERVICE..."
ssh root@46.101.166.137 "systemctl restart $SERVICE"

echo "✅ Backend deployed to $TARGET!"
