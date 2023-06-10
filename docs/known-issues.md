# Known Issues
## docker-compose fails to start es01

Run the following script:
```powershell
wsl -d docker-desktop sysctl -w vm.max_map_count=262144
```
