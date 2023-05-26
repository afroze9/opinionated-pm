# Deploy on K8s
helm repo update
helm install consul hashicorp/consul --create-namespace --namespace consul --values values.yaml

# Get Token
$token = kubectl get secrets/consul-bootstrap-acl-token --template='{{.data.token | base64decode }}' --namespace consul

# Setup Policies
Run the `setup-consul.ps1` file in the `Consul` folder for each project.

