Security & Production readiness checklist

1. Secrets and keys
- Move JWT signing keys, TURN credentials, DB connection strings to a secret store (Azure Key Vault). Do not keep secrets in appsettings for prod.

2. HTTPS everywhere
- Configure TLS for all endpoints. Use managed certificates in Azure App Service or ingress controller for AKS.

3. Authentication & Authorization
- Consider using ASP.NET Core Identity with EF Core or an external provider (Azure AD B2C) for production.
- Implement refresh tokens and rotate keys.

4. Data protection
- Encrypt sensitive data at rest when required.
- Avoid storing call media server-side unless necessary; if storing, use a secure blob store with limited access and retention policies.

5. SignalR scale-out
- Use Redis backplane (Azure Cache for Redis) or Azure SignalR Service for scaling SignalR.

6. TURN
- Deploy coturn with authentication or use a managed TURN provider. Monitor bandwidth and connections.

7. Logging & Monitoring
- Add structured logs, correlation IDs, and collect metrics for call failures, connection drops, and message delivery rates.

8. Rate limiting & abuse prevention
- Implement rate limits on auth and signaling endpoints. Consider per-user throttling.

9. Load testing
- Simulate concurrent connections and calls using a load testing tool to validate scaling plan.

10. Privacy & compliance
- Implement account deletion and data export flows.
- Publish privacy policy and terms of service; comply with local regulations (GDPR, CCPA if applicable).

11. CI/CD
- Protect production deployments with approvals; use separate staging environment.

12. Backups
- Backup databases and configuration regularly.

This checklist is a starting point for hardening and production readiness.
