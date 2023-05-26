param(
    [string]$version,
    [string]$repo
)

echo ""
echo "Building version $version for repository $repo"

echo ""
echo "Building Frontend App"
docker build -t dpm-frontend-app:latest -t dpm-frontend-app:$version -t "$repo/dpm-frontend-app:latest" -t "$repo/dpm-frontend-app:$version" .\frontend-app\

echo ""
echo "Building API Gateway"
Push-Location ".\api-gateway\"
docker build -t dpm-api-gateway:latest -t dpm-api-gateway:$version -t "$repo/dpm-api-gateway:latest" -t "$repo/dpm-api-gateway:$version" -f "src\ProjectManagement.ApiGateway\Dockerfile" .
Pop-Location

echo ""
echo "Building Company API"
Push-Location ".\company-api\"
docker build -t dpm-company-api:latest -t dpm-company-api:$version -t "$repo/dpm-company-api:latest" -t "$repo/dpm-company-api:$version" -f "src\ProjectManagement.Company.Api\Dockerfile" .
Pop-Location

echo ""
echo "Building Project API"
Push-Location ".\project-api\"
docker build -t dpm-project-api:latest -t dpm-project-api:$version -t "$repo/dpm-project-api:latest" -t "$repo/dpm-project-api:$version" -f "src\ProjectManagement.Project.Api\Dockerfile" .
Pop-Location

echo ""
echo "Building Health Checks Dashboard"
Push-Location ".\health-checks-dashboard\"
docker build -t dpm-health-checks-dashboard:latest -t dpm-health-checks-dashboard:$version -t "$repo/dpm-health-checks-dashboard:latest" -t "$repo/dpm-health-checks-dashboard:$version" -f "src\ProjectManagement.HealthChecksDashboard\Dockerfile" .
Pop-Location
