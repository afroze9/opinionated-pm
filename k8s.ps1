kubectl apply -f .\k8s\consul.yaml
kubectl wait --for=condition=ready pod -l app=consul -n nexus --timeout=300s

sleep 10

kubectl apply -f .\services\company-api\k8s\policies.yaml
kubectl apply -f .\services\project-api\k8s\policies.yaml
kubectl apply -f .\services\people-api\k8s\policies.yaml
kubectl apply -f .\api-gateway\k8s\policies.yaml
kubectl apply -f .\health-checks-dashboard\k8s\policies.yaml

sleep 10
kubectl apply -f .\services\company-api\k8s\company-api.yaml

echo done

