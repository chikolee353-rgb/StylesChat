Call UI and platform integration notes

- Use native controls for local/remote video surfaces provided by the chosen SDK.
- On Android/iOS use the SDK's renderer/view for embedding video. For WebView approach, render HTML5 video elements and wire JS events to native code.
- Handle permissions at runtime: CAMERA and RECORD_AUDIO on Android; request AV permissions on iOS and macOS.
- Maintain call state: Ringing, Connecting, InCall, Ended. Reflect that in the UI and signal peers via ChatHub events.
- Provide mute/unmute, switch camera, and end call controls.

Sample flow (high level):
1. User taps call button -> SignalR StartCall(targetId)
2. Callee receives IncomingCall and opens CallPage, shows accept/decline
3. On accept, initialize CallService, negotiate via CallSignal messages
4. When both peers have media, transition to InCall state
5. On hangup, send EndCall and cleanup

Remember: For production choose a tested SDK for reliability unless you are ready to operate TURN servers and manage more complex WebRTC behavior.
