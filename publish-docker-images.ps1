param(
    [string]$version,
    [string]$repo
)

echo "Pushing Frontend App"
docker push $repo/dpm-frontend-app:latest
docker push $repo/dpm-frontend-app:$version

echo "Pushing Api Gateway"
docker push $repo/dpm-api-gateway:latest
docker push $repo/dpm-api-gateway:$version

echo "Pushing Company API"
docker push $repo/dpm-company-api:latest
docker push $repo/dpm-company-api:$version

echo "Pushing Project API"
docker push $repo/dpm-project-api:latest
docker push $repo/dpm-project-api:$version

echo "Pushing Health Checks Dashboard"
docker push $repo/dpm-health-checks-dashboard:latest
docker push $repo/dpm-health-checks-dashboard:$version
