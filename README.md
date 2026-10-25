# StylesChat

StylesChat is an Android chat app with a shared live room, username/password registration, and saved message history.

## Deploy the demo to Render

1. Push this repository to GitHub and create a Blueprint from `render.yaml` in the [Render Dashboard](https://dashboard.render.com/blueprints). This creates a free Docker web service and a free PostgreSQL database in Oregon.
2. The configured service URL is `https://styleschat-chikolee353-rgb.onrender.com`. If Render requires a different globally unique service name, update `ApiBaseUrl` in `.github\workflows\build-android.yml` to match after the service is created.
3. After the Render service is live, push a version tag such as `v1.0.0` to build an Android APK and publish it as a GitHub release. The Render landing page links to the latest release APK.

The free web service spins down after 15 minutes without traffic and may take about a minute to start again. Free PostgreSQL databases expire 30 days after creation, so this is for a short-lived demo: database expiry deletes registrations and chat history. Render's free web-service filesystem is ephemeral; the app therefore stores data in PostgreSQL rather than local SQLite when `DATABASE_URL` is configured.

The APK uses the standard .NET Android debug signing key and is for testing/sideloading only. Do not distribute it as a production release or publish it to Google Play; production distribution requires a private release signing key.

## Local development

Build with `dotnet build StylesChat.slnx`. To run the API locally, set `JWT_SIGNING_KEY` to a private secret of at least 32 bytes and start `src\Server\Server.csproj`. The default Android emulator API URL is `https://10.0.2.2:5001`; override `ApiBaseUrl` when building for another server.
