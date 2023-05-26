function Get-EnvValue([string]$name)
{
    $envFile = Get-Content -Path ".env"
    foreach ($line in $envFile)
    {
        if ( $line.Trim().StartsWith($name + "="))
        {
            return $line.Split("=")[1]
        }
    }
    return $null
}

# Get docker image variables
$date = Get-Date
$version = $date.ToString("yyyy.MM.dd.HHmmss")
$repo = "afroze9"

$api_gateway_port = Get-EnvValue "API_GATEWAY_PORT_EXTERNAL"
$company_api_port = Get-EnvValue "COMPANY_API_PORT_EXTERNAL"
$project_api_port = Get-EnvValue "PROJECT_API_PORT_EXTERNAL"
$health_checks_dashboard_port = Get-EnvValue "HEALTH_CHECKS_DASHBOARD_PORT_EXTERNAL"

echo "Generating Dev Certs"
$CreateCert = (dotnet dev-certs https -c | Select-String -SimpleMatch "No valid certificate found.")
if ($CreateCert)
{
    dotnet dev-certs https --trust
}
dotnet dev-certs https -ep ./devcerts/aspnetapp.pfx -p $devCertPassword

echo ""
echo "Increasing WSL memory for ELK"
wsl -d docker-desktop sysctl -w vm.max_map_count = 262144

echo ""
echo "Building Docker Images"
.\build-docker-images.ps1 -version $version -repo $repo

echo ""
echo "Publishing Docker Images"
.\publish-docker-images.ps1 -version $version -repo $repo

echo ""
echo "Starting Consul"
Push-Location ".\discovery-server\k8s\"
&".\setup-consul.ps1" "token"
Pop-Location

echo ""
echo "Setting up ACL for api-gateway"
Push-Location ".\api-gateway\src\ProjectManagement.ApiGateway\Consul\"
&".\setup-consul-docker.ps1" "api_gateway_token"
Pop-Location

#echo ""
#echo "Setting up ACL for company-api"
#Push-Location ".\company-api\src\ProjectManagement.Company.Api\Consul"
#&".\setup-consul-docker.ps1" "company_api_token"
#Pop-Location
#
#echo ""
#echo "Setting up ACL for project-api"
#Push-Location ".\project-api\src\ProjectManagement.Project.Api\Consul"
#&".\setup-consul-docker.ps1" "project_api_token"
#Pop-Location
#
#echo ""
#echo "Setting up ACL for health-checks-dashboard"
#Push-Location ".\health-checks-dashboard\src\ProjectManagement.HealthChecksDashboard\Consul"
#&".\setup-consul-docker.ps1" "health_checks_dashboard_token"
#Pop-Location
#
#echo ""
#echo "Copying tokens to .env file"
#(Get-Content .env) |
#        ForEach-Object { $_ -replace "^API_GATEWAY_TOKEN=.*", "API_GATEWAY_TOKEN=$api_gateway_token" } |
#        ForEach-Object { $_ -replace "^COMPANY_API_TOKEN=.*", "COMPANY_API_TOKEN=$company_api_token" } |
#        ForEach-Object { $_ -replace "^PROJECT_API_TOKEN=.*", "PROJECT_API_TOKEN=$project_api_token" } |
#        ForEach-Object { $_ -replace "^HEALTH_CHECKS_DASHBOARD_TOKEN=.*", "HEALTH_CHECKS_DASHBOARD_TOKEN=$health_checks_dashboard_token" } |
#        Set-Content .env
#
#echo ""
#echo "Setting up databases, elk, and jaeger"
#docker-compose -f .\docker-compose.yml up -d
#
#$tokens = @{
#    "Global Token" = $token
#    "API Gateway Token" = $api_gateway_token
#    "Company API Token" = $company_api_token
#    "Project API Token" = $project_api_token
#    "Health Checks Dashboard Token" = $health_checks_dashboard_token
#}
#
#$services = @{
#    "API Gateway" = "https://localhost:$api_gateway_port"
#    "Company API" = "https://localhost:$company_api_port"
#    "Project API" = "https://localhost:$project_api_port"
#    "Health Checks Dashboard" = "https://localhost:$health_checks_dashboard_port"
#    "Frontend App" = "http://localhost:3000"
#    "Consul" = "http://localhost:8500"
#    "Kibana" = "http://localhost:5601"
#    "Jaeger" = "http://localhost:16686"
#}
#
#echo ""
#echo "----------------------------------------------"
#echo "Setup complete"
#echo "----------------------------------------------"
#echo "Tokens"
#echo "----------------------------------------------"
#$tokens.GetEnumerator() | Format-Table -AutoSize -Property Name, Value
#echo ""
#echo "`nServices"
#echo "----------------------------------------------"
#$services.GetEnumerator() | Format-Table -AutoSize -Property Name, Value
