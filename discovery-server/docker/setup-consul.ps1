param($tokenResult)

function Restart-ContainerAndWait {
    param(
        [Parameter(Mandatory=$true)]
        [string]$ContainerName,
        [int]$MaxRetries = 10,
        [int]$SleepSeconds = 5
    )

    # Restart the container
    docker container restart $ContainerName

    # Wait for the container to be up again
    $containerIp = $null
    $retries = 0
    while (-not $containerIp -and $retries -lt $MaxRetries) {
        Write-Host "Waiting for container '$ContainerName' to be up again (retry: $($retries+1))..."
        Start-Sleep -Seconds $SleepSeconds
        $containerIp = (docker container inspect $ContainerName --format '{{.NetworkSettings.Networks.bridge.IPAddress}}') -replace '\r',''
        $retries++
    }

    if ($containerIp) {
        Write-Host "Container '$ContainerName' is up"
    } else {
        Write-Error "Failed to start container '$ContainerName' after $MaxRetries retries."
    }
}

function Replace-SubnetIp {
    param(
        [string]$FilePath,
        [string]$NewSubnetIp
    )

    $jsonString = Get-Content $FilePath -Raw
    $jsonObject = $jsonString | ConvertFrom-Json
    $jsonObject.bind_addr = '{{{{ GetPrivateInterfaces | include "network" "{0}" | attr "address" }}}}' -f $NewSubnetIp
    $jsonObject | ConvertTo-Json -Depth 5 | Set-Content $FilePath
}

# $subnetIP is provided externally
Replace-SubnetIp -FilePath "./server1.json" -NewSubnetIp $subnetIp
Replace-SubnetIp -FilePath "./server2.json" -NewSubnetIp $subnetIp
Replace-SubnetIp -FilePath "./server3.json" -NewSubnetIp $subnetIp

docker-compose up --detach

echo "Copying ACL config to consul servers"
docker cp consul-acl.json consul-server1:/consul/config/consul-acl.json
docker cp consul-acl.json consul-server2:/consul/config/consul-acl.json
docker cp consul-acl.json consul-server3:/consul/config/consul-acl.json

echo "Restarting consul-server2"
Restart-ContainerAndWait -ContainerName "consul-server2" -MaxRetries 10 -SleepSeconds 5 

echo "Restarting consul-server3"
Restart-ContainerAndWait -ContainerName "consul-server3" -MaxRetries 10 -SleepSeconds 5

echo "Restarting consul-server1"
Restart-ContainerAndWait -ContainerName "consul-server1" -MaxRetries 10 -SleepSeconds 5

echo "Bootstrapping ACL"
$output = docker exec consul-server1 consul acl bootstrap
$token = $output | Select-String -Pattern 'SecretID:\s+(.*)' | ForEach-Object { $_.Matches.Groups[1].Value }

echo "Setting agent token for consul-server1"
docker exec -e CONSUL_HTTP_TOKEN="$token" consul-server1 consul acl set-agent-token agent "$token"

echo "Setting agent token for consul-server1"
docker exec -e CONSUL_HTTP_TOKEN="$token" consul-server2 consul acl set-agent-token agent "$token"

echo "Setting agent token for consul-server1"
docker exec -e CONSUL_HTTP_TOKEN="$token" consul-server3 consul acl set-agent-token agent "$token"

echo "Done"
echo "Token: $token"

Set-Variable -Name $tokenResult -Value $token -Scope 1
