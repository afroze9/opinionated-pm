param(
    [string]$version,
    [string]$repo
)

echo ""
echo "Building version $version for repository $repo"

echo ""
echo "Building Frontend App"
docker build -t nexus-frontend-app:latest -t nexus-frontend-app:$version -t "$repo/nexus-frontend-app:latest" -t "$repo/nexus-frontend-app:$version" .\frontend-app\

echo ""
echo "Building API Gateway"
docker build -t nexus-api-gateway:latest -t nexus-api-gateway:$version -t "$repo/nexus-api-gateway:latest" -t "$repo/nexus-api-gateway:$version" -f .\api-gateway\src\Nexus.ApiGateway\Dockerfile .

echo ""
echo "Building Company API"
docker build -t nexus-company-api:latest -t nexus-company-api:$version -t "$repo/nexus-company-api:latest" -t "$repo/nexus-company-api:$version" -f .\services\company-api\src\Nexus.Company.Api\Dockerfile .

echo ""
echo "Building Project API"
docker build -t nexus-project-api:latest -t nexus-project-api:$version -t "$repo/nexus-project-api:latest" -t "$repo/nexus-project-api:$version" -f .\services\project-api\src\Nexus.Project.Api\Dockerfile .

echo ""
echo "Building Health Checks Dashboard"
docker build -t nexus-health-checks-dashboard:latest -t nexus-health-checks-dashboard:$version -t "$repo/nexus-health-checks-dashboard:latest" -t "$repo/nexus-health-checks-dashboard:$version" -f .\health-checks-dashboard\src\Nexus.HealthChecksDashboard\Dockerfile .
