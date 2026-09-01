Mas Vegas Content Creators Chat — Requirements (MVP)

Overview
- App name: Mas vegas content creators chat
- Cross-platform client using .NET MAUI (Android, iOS, Windows, macOS)
- Backend using ASP.NET Core (.NET 10)
- Real-time text messaging, presence, simple group chats, and 1:1 video calls over mobile data/Wi-Fi

Functional requirements (MVP)
- User accounts: register, login, logout
- Contact list: add, remove, block users
- 1:1 text messaging with delivery status (sent, delivered, read)
- Basic group chat (create group, add members, send messages)
- Real-time presence (online/last-seen)
- 1:1 video calls between users using WebRTC (signaling via SignalR)
- Message persistence (server-side database) with pagination
- Push notifications for new messages/call invites (FCM/APNs)
- User profile (display name, avatar)

Non-functional requirements
- Secure transport (TLS everywhere)
- JWT-based authentication with refresh tokens
- Scalable SignalR (Redis backplane) for multiple server instances
- TURN server support for WebRTC NAT traversal; optional third-party provider (e.g., Agora/Twilio) to reduce ops
- Data protection at rest for sensitive data (e.g., encryption for media storage)
- GDPR/Privacy considerations: option to delete account and messages
- High availability and monitoring (logging, metrics)

Constraints & notes
- Target .NET 10 and .NET MAUI (no Xamarin.Forms)
- This initial implementation focuses on scaffolding and the core messaging + signaling path; native WebRTC integration on MAUI requires platform bindings or third-party SDKs and will be addressed in later steps.

MVP Scope to deliver first
1. Backend API + SignalR host (authentication, user, messages, SignalR hub)
2. MAUI client: login, contact list, 1:1 text chat with real-time update
3. Signaling for 1:1 video calls (peer negotiation via SignalR), with guidance on TURN / SDK options

Out-of-scope for MVP
- End-to-end encryption of messages (E2EE)
- Media file storage and CDN integration (beyond simple uploads)
- Advanced call features (recording, multi-party video)

Security checklist (initial)
- Require HTTPS on all endpoints
- Validate and sanitize input server-side
- Store secrets (JWT signing keys, TURN credentials) securely in environment or secret store
- Rate-limit authentication and signaling endpoints to mitigate abuse
