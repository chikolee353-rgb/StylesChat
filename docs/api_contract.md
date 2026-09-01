Mas Vegas Content Creators Chat — API Contract (MVP)

Base URL
- https://{api-host}/api

Authentication
- Use JWT access tokens in Authorization header: Authorization: Bearer {token}
- /api/auth/login -> POST { username, password } -> { accessToken, refreshToken, expiresIn }
- /api/auth/register -> POST { username, password, displayName } -> 201 Created
- /api/auth/refresh -> POST { refreshToken } -> { accessToken, refreshToken }

User endpoints
- GET /api/users/me -> returns profile for authenticated user
- GET /api/users/{id} -> returns public profile
- POST /api/users/{id}/contacts -> add contact
- DELETE /api/users/{id}/contacts -> remove contact

Messaging
- GET /api/messages/conversations -> list conversations with metadata (last message, unread count)
- GET /api/messages/{conversationId}?page={n}&pageSize={s} -> paginated messages
- POST /api/messages -> { conversationId | recipientId, text, attachments[] } -> 201

SignalR (Real-time)
- Hub path: /hubs/chat
- Client connect: include access token in query string or bearer header
- Hub methods (client -> server):
  - SendMessage(payload) -> deliver message to recipient(s) and persist
  - Typing(conversationId, isTyping)
  - StartCall(targetUserId) -> sends call invitation
  - CallSignal(targetUserId, data) -> used for WebRTC offer/answer/ICE candidates
  - EndCall(targetUserId)
- Server -> Client events:
  - MessageReceived(message)
  - MessageDelivered(messageId)
  - PresenceUpdated(userId, status)
  - IncomingCall(callInfo)
  - CallSignal(fromUserId, data)
  - CallEnded(callId)

Video call signaling (WebRTC)
- Use SignalR to exchange SDP offers/answers and ICE candidates between peers
- Server may provide TURN credentials via an authenticated endpoint: GET /api/rtc/turn

Error handling
- Standard HTTP status codes
- Error body: { code: string, message: string, details?: object }

Notes
- Message objects should include { id, conversationId, senderId, text, attachments[], timestamp, status }
- Conversations can be 1:1 or group; for 1:1, conversationId may be derived from ordered pair of userIds
- Keep payloads minimal for realtime messages (send IDs and small metadata)

Security & Rate Limits
- All endpoints require HTTPS
- Sensitive endpoints (auth, signaling) should be rate-limited

Deployment considerations
- Use Redis for SignalR scale-out
- Use a TURN server for WebRTC NAT traversal
- Use managed database (PostgreSQL/Azure SQL) for persistence

This contract is intentionally minimal to speed up the MVP implementation. It will be expanded as features are added.
