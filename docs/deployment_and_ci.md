CI / Deployment notes

- Use GitHub Actions or Azure DevOps for CI. Basic pipeline steps:
  1. Checkout
  2. Setup .NET SDK (10.0)
  3. Build solution
  4. Run tests (if any)
  5. Publish server project and build Docker image
  6. Push to container registry and deploy to Azure App Service or AKS

- Example GitHub Actions (skeleton):
  - name: Setup .NET
    uses: actions/setup-dotnet@v4
    with:
      dotnet-version: '10.0.x'

  - name: Build
    run: dotnet build --configuration Release

  - name: Publish server
    run: dotnet publish src/Server/Server.csproj -c Release -o publish

  - name: Build Docker image
    run: docker build -f docker/Dockerfile.server -t myregistry/masvegas-server:latest .

- Logging & monitoring: use Application Insights or Prometheus + Grafana. Ensure structured logging with correlation ids for SignalR operations.

- Secrets: store JWT signing key, TURN credentials, DB connection strings in Azure Key Vault or GitHub Secrets.
