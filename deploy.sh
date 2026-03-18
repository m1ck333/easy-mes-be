#!/bin/bash
set -e

echo "🔨 Building backend..."
dotnet publish AlgreenMES.API/AlgreenMES.API.csproj -c Release -o ./publish

echo "📦 Uploading to server..."
rsync -az --delete --exclude='appsettings.Production.json' ./publish/ root@46.101.166.137:/opt/algreen/api/

echo "🔄 Restarting API..."
ssh root@46.101.166.137 "systemctl restart algreen-api"

echo "✅ Backend deployed!"
