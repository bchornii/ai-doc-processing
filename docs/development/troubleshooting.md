Problem:
Files owned by root

Cause:
An earlier container or command created files as root.

Recovery:
```sh
sudo chown -R vscode:vscode .
./scripts/clean.sh
```