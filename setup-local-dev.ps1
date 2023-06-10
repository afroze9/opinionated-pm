class NexusConfig
{
    [string]$ProjectName
    [FrameworkConfiguration]$Framework
    [ServiceConfiguration[]]$Services
}

class FrameworkConfiguration
{
    [ApiGatewayConfiguration]$ApiGateway
    [HealthChecksDashboardConfiguration]$HealthChecksDashboard
}

class ApiGatewayConfiguration
{
    [string]$ServiceName
    [string]$ConsulConfigDirectory
    [string]$OcelotDirectory
    [string]$AppSettingsConfigPath
}

class HealthChecksDashboardConfiguration
{
    [string]$ConsulConfigDirectory
}

class ServiceConfiguration
{
    [string]$Name
}

class NexusHelper
{
    [string]$Globaltoken
    [string]$Network
    [string]$SubnetIp
    [NexusConfig]$NexusConfig
    
    [void]
    SetupDiscoveryServer()
    {
        Write-Host "`nStarting Consul"
        Push-Location ".\discovery-server\docker\"
        $token = ""
        &".\setup-consul.ps1" "token" $this.subnetIp
        Pop-Location
        $Globaltoken = $token
    }
    
    [void]
    PreconfigureDocker()
    {
        Write-Host "`nIncreasing WSL memory for ELK"
        wsl -d docker-desktop sysctl -w "vm.max_map_count=262144"

        Write-Host "`nChecking Docker Networks"
        $networkList = docker network ls --filter "name=$this.network" --format "{{.Name}}"
        if ($networkList -contains $this.network)
        {
            Write-Host "`nThe '$this.network' network already exists."
        }
        else
        {
            docker network create $this.network
            Write-Host "`nThe '$this.network' network has been created."
        }

        $networkId = docker network inspect $this.network "--format={{.Id}}"
        $this.SubnetIp = docker network inspect $networkId "--format={{(index .IPAM.Config 0).Subnet}}"
        Write-Host "The Subnet IP is $this.SubnetIp"
    }

    SetupApiGatewayConsul(
            [string]$serviceName,
            [string]$consulConfigDirectory,
            [string]$ocelotConfigDirectory,
            [string]$appSettingsPath,
            [string]$token,
            [string]$jaegerHost,
            [string]$consulKvHost,
            [string]$consulKvPort)
    {
        # Create Policy
        $policy = Create-Consul-Policy $this.Globaltoken $this.NexusConfig.Framework.ApiGateway.ServiceName $this.NexusConfig.Framework.ApiGateway.ConsulConfigDirectory

        # Create Token
        $apiGatewayToken = Create-Consul-Token $token $serviceName $policy.Name

        # Update Configs
        $appConfigPath = Join-Path -Path $consulConfigDirectory "app-config.json"
        $appConfig = Get-Content -Raw -Path $appConfigPath | ConvertFrom-Json
        $appConfig.Consul.Token = $token
        $appConfig.TelemetrySettings.Endpoint = $jaegerHost
        $appConfig | ConvertTo-Json -Depth 10 | Set-Content -Path $appConfigPath

        # Create KV
        $kvBody = $appConfig | ConvertTo-Json -Depth 10
        Create-Consul-KV $serviceName $kvBody $token

        # Update OcelotJson
        $ocelotGlobalConfigPath = Join-Path -Path $ocelotConfigDirectory "ocelot.global.json"
        $ocelotGlobalContent = Get-Content -Raw $ocelotGlobalConfigPath | ConvertFrom-Json
        $ocelotGlobalContent.GlobalConfiguration.ServiceDiscoveryProvider.Host = $consulKvHost
        $ocelotGlobalContent.GlobalConfiguration.ServiceDiscoveryProvider.Token = $apiGatewayToken
        $ocelotGlobalContent | ConvertTo-Json -Depth 10 | Set-Content $ocelotGlobalConfigPath

        # Update appsettings.json
        $appsettingsContent = Get-Content -Raw $appSettingsPath | ConvertFrom-Json
        $appsettingsContent.ConsulKV.Url = "http://{0}:{1}" -f $consulKvHost, $consulKvPort
        $appsettingsContent.ConsulKV.Token = $apiGatewayToken
        $appsettingsContent | ConvertTo-Json -Depth 10 | Set-Content $appSettingsPath

        return @{
            PolicyName = $policy.Name
            Token = $apiGatewayToken
        }
    }

    static
    [string]
    GetEnvValue([string]$name)
    {
        $envFile = Get-Content -Path ".env"
        foreach ($line in $envFile)
        {
            if ( $line.Trim().StartsWith($name + "="))
            {
                return $line.Split("=")[1]
            }
        }
        return ""
    }

    static
    [void]
    SetupDevCerts([string]$password)
    {
        Write-Host "`nGenerating Dev Certs"
        $CreateCert = (dotnet dev-certs https -c | Select-String -SimpleMatch "No valid certificate found.")
        if ($CreateCert)
        {
            dotnet dev-certs https --trust
        }
        dotnet dev-certs https -ep ./devcerts/aspnetapp.pfx -p $password
    }

    static
    [string]
    GetNexusConfig()
    {
        $content = Get-Content -Raw "nexus.config.json" | ConvertFrom-Json
        return $content
    }

}

######################### Consul stuff temp comment

function Get-Consul-Headers([string]$token)
{
    $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
    $headers.Add("X-Consul-Token", "$token")

    return $headers
}

function Get-Consul-Policy-Body(
        [string]$serviceName,
        [string]$rulesDirectory)
{
    $rules = Get-Content "$rulesDirectory/rules.hcl" -Raw
    $policyName = "kv-$serviceName"
    $bodyJson = @{
        Name = "$policyName"
        Description = "Policy for $serviceName key prefix"
        Rules = $rules
    } | ConvertTo-Json

    return @{
        Json = $bodyJson
        Name = $policyName
    }
}

function Create-Consul-Policy(
        [string]$token,
        [string]$serviceName,
        [string]$rulesDirectory)
{
    $headers = Get-Consul-Headers $token
    $policy = Get-Consul-Policy-Body $serviceName $rulesDirectory

    $response = Invoke-RestMethod `
        -Uri "http://localhost:8500/v1/acl/policy" `
        -Method PUT `
        -Headers $headers `
        -Body $policy.Json

    $policy.Id = $response.ID

    Write-Host "`nPolicy Created: $policy.Name : $policy.Id"
    return $policy
}

function Get-Consul-Token-Body(
        [string]$serviceName,
        [string]$policyName
)
{
    $body = @{
        Description = "Token for $serviceName service"
        Policies = @(
        @{
            Name = "$policyName"
        }
        )
        ServiceIdentities = @(
        @{
            ServiceName = "$serviceName"
        }
        )
    } | ConvertTo-Json

    return $body
}

function Create-Consul-Token(
        [string]$token,
        [string]$serviceName,
        [string]$policyName
)
{
    $headers = Get-Consul-Headers $token
    $jsonBody = Get-Consul-Token-Body $serviceName $policyName

    $response = Invoke-RestMethod `
        -Uri "http://localhost:8500/v1/acl/token" `
        -Method PUT `
        -Headers $headers `
        -Body $jsonBody

    $secretId = $response.SecretID

    Write-Host "`nToken created for Service: $serviceName | $secretId"
    return $secretId
}

function Create-Consul-KV(
        [string]$serviceName,
        [string]$jsonBody,
        [string]$token
)
{
    $header = Get-Consul-Headers $token
    $response = Invoke-RestMethod `
        -Uri "http://localhost:8500/v1/kv/$serviceName/app-config" `
        -Method PUT `
        -Headers $headers `
        -ContentType "application/json" `
        -Body $jsonBody
}

function

# Variables
$nexusConfig = Get-Nexus-Config
$api_gateway_port = Get-EnvValue "API_GATEWAY_PORT_EXTERNAL"
$company_api_port = Get-EnvValue "COMPANY_API_PORT_EXTERNAL"
$project_api_port = Get-EnvValue "PROJECT_API_PORT_EXTERNAL"
$health_checks_dashboard_port = Get-EnvValue "HEALTH_CHECKS_DASHBOARD_PORT_EXTERNAL"
$networkName = "consul_external"

# Dev Certs
Setup-DevCerts $devCertPassword

# Docker Config
$subnetIp = [NexusHelper]::PreconfigureDocker($networkName)

# Discovery Server
$token = [NexusHelper]::SetupDiscoveryServer($subnetIp)

# Framework
$apiGatewayConsulConfig = Setup-ApiGateway-Consul `
    -serviceName $nexusConfig.Framework.ApiGateway.ServiceName `
    -consulConfigDirectory $nexusConfig.Framework.ApiGateway.ConsulConfigDirectory `
    -ocelotConfigDirectory $nexusConfig.Framework.ApiGateway.OcelotDirectory `
    -appSettingsPath $nexusConfig.Framework.ApiGateway.AppSettingsConfigPath `
    -token $token `
    -jaegerHost "localhost" `
    -consulKvHost "localhost" `
    -consulKvPort "8500"

#Write-Host ""
#Write-Host "Setting up ACL for api-gateway"
#Push-Location ".\api-gateway\src\Nexus.ApiGateway\Consul\"
#&".\setup-consul.ps1" "api_gateway_token"
#Pop-Location

Write-Host ""
Write-Host "Setting up ACL for health-checks-dashboard"
Push-Location ".\health-checks-dashboard\src\Nexus.HealthChecksDashboard\Consul"
&".\setup-consul.ps1" "health_checks_dashboard_token"
Pop-Location

# Services
Write-Host ""
Write-Host "Setting up ACL for company-api"
Push-Location ".\services\company-api\src\Nexus.Company.Api\Consul"
&".\setup-consul.ps1" "company_api_token"
Pop-Location

Write-Host ""
Write-Host "Setting up ACL for project-api"
Push-Location ".\services\project-api\src\Nexus.Project.Api\Consul"
&".\setup-consul.ps1" "project_api_token"
Pop-Location

# Updating Global Configs
Write-Host ""
Write-Host "Copying tokens to .env file"
(Get-Content .env) |
        ForEach-Object { $_ -replace "^API_GATEWAY_TOKEN=.*", "API_GATEWAY_TOKEN=$api_gateway_token" } |
        ForEach-Object { $_ -replace "^COMPANY_API_TOKEN=.*", "COMPANY_API_TOKEN=$company_api_token" } |
        ForEach-Object { $_ -replace "^PROJECT_API_TOKEN=.*", "PROJECT_API_TOKEN=$project_api_token" } |
        Set-Content .env

# Starting Docker Infrastructure
Write-Host ""
Write-Host "Setting up databases, elk, and jaeger"
docker-compose -f .\docker-compose-local.yml up -d

# Finish
$tokens = @{
    "Global Token" = $token
    "API Gateway Token" = $api_gateway_token
    "Company API Token" = $company_api_token
    "Project API Token" = $project_api_token
    "Health Checks Dashboard Token" = $health_checks_dashboard_token
}

$services = @{
    "Consul" = "http://localhost:8500"
    "Kibana" = "http://localhost:5601"
    "Jaeger" = "http://localhost:16686"
}

Write-Host ""
Write-Host "----------------------------------------------"
Write-Host "Setup complete"
Write-Host "----------------------------------------------"
Write-Host "Tokens"
Write-Host "----------------------------------------------"
$tokens.GetEnumerator() | Format-Table -AutoSize -Property Name, Value
Write-Host ""
Write-Host "`nServices"
Write-Host "----------------------------------------------"
$services.GetEnumerator() | Format-Table -AutoSize -Property Name, Value
