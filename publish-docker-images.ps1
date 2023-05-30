param(
    [string]$version,
    [string]$repo
)

echo "Pushing Frontend App"
docker push $repo/nexus-frontend-app:latest
docker push $repo/nexus-frontend-app:$version

echo "Pushing Api Gateway"
docker push $repo/nexus-api-gateway:latest
docker push $repo/nexus-api-gateway:$version

echo "Pushing Company API"
docker push $repo/nexus-company-api:latest
docker push $repo/nexus-company-api:$version

echo "Pushing Project API"
docker push $repo/nexus-project-api:latest
docker push $repo/nexus-project-api:$version

echo "Pushing Health Checks Dashboard"
docker push $repo/nexus-health-checks-dashboard:latest
docker push $repo/nexus-health-checks-dashboard:$version
