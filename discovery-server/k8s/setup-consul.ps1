param($tokenResult)

helm repo update
helm install consul hashicorp/consul --create-namespace --namespace consul --values values.yaml

$tokenResult = kubectl get secrets/consul-bootstrap-acl-token --template='{{.data.token | base64decode }}' --namespace consul

Set-Variable -Name $tokenResult -Value $secret_id -Scope 1
