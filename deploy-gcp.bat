@echo off
echo ========================================
echo   LFN Hauling App - GCP Deploy Script
echo ========================================
echo.

echo [1/5] Checking gcloud...
gcloud --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: gcloud not found. Please install Google Cloud SDK first.
    echo Download from: https://dl.google.com/dl/cloudsdk/channels/rapid/GoogleCloudSDKInstaller.exe
    pause
    exit /b 1
)

echo [2/5] Logging in to GCP...
gcloud auth login

echo [3/5] Setting project...
echo Enter your GCP Project ID:
set /p PROJECT_ID=
gcloud config set project %PROJECT_ID%

echo [4/5] Enabling required APIs...
gcloud services enable run.googleapis.com containerregistry.googleapis.com

echo [5/5] Deploying to Cloud Run...
echo.
echo Command will be:
echo gcloud run deploy lfn-hauling-app --source . --region asia-southeast1 --allow-unauthenticated --set-env-vars DATABASE_URL="Host=34.34.217.34;Port=5432;Database=lfn--postgres;Username=postgres;Password=keriboNO@1"
echo.

gcloud run deploy lfn-hauling-app --source . --region asia-southeast1 --allow-unauthenticated --set-env-vars DATABASE_URL="Host=34.34.217.34;Port=5432;Database=lfn--postgres;Username=postgres;Password=keriboNO@1"

echo.
echo ========================================
echo Deployment Complete!
echo ========================================
pause
