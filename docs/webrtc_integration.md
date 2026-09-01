WebRTC integration guidance — Mas Vegas Content Creators Chat

Overview
- WebRTC handles real-time media (audio/video) peer-to-peer.
- The server's role: signaling (exchange SDP offers/answers and ICE candidates) and optionally provide TURN credentials.
- Use SignalR hub (/hubs/chat) to relay signaling messages between peers.

Signaling flow (1:1 call)
1. Caller initiates call by invoking StartCall(targetUserId) on ChatHub.
2. Callee receives IncomingCall event and accepts/rejects.
3. If accepted, the peers create RTCPeerConnection and caller creates an SDP offer.
4. Caller sends the SDP offer via ChatHub.CallSignal(targetUserId, { type: "offer", sdp: ... }).
5. Callee receives CallSignal, sets remote description, creates answer, and returns via CallSignal.
6. Both peers exchange ICE candidates via CallSignal messages.

TURN
- For NAT traversal in real-world conditions, a TURN server is needed.
- Options:
  - Deploy coturn and provide credentials via an authenticated API endpoint (GET /api/rtc/turn).
  - Use a managed provider (Twilio, Xirsys, Agora) to avoid operating TURN servers.

MAUI and WebRTC
- There is no first-class managed WebRTC library for MAUI. Two practical approaches:
  1. Use a third-party cross-platform SDK with native bindings that supports MAUI (e.g., Agora, Twilio). These provide simplified APIs and reliability at a cost.
  2. Use platform WebView + WebRTC-based web client loaded inside the app and communicate with native code via JS bridge. Simpler but less integrated.

Security
- Always use HTTPS/WSS for signaling and DTLS/SRTP for media (handled by WebRTC).
- Do not log raw SDP or ICE candidates in production.

Scaling
- Signaling via SignalR scales with Redis backplane for multiple server instances.
- TURN server needs capacity planning based on expected concurrent calls.

Next steps for implementation
- Decide between self-hosted WebRTC (with coturn) vs third-party SDK.
- Implement a small JS-based WebRTC page for prototyping and load it in MAUI WebView for proof-of-concept.
- Add an authenticated endpoint to issue temporary TURN credentials if hosting coturn with REST credentials (or configure long-lived credentials securely).

This guidance helps integrate WebRTC with the existing SignalR-based signaling hub created in the server skeleton.