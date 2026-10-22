# DROPSHIP TYCOON + NAYA NAGAR

Unity Android project by ANI STUDIO. This repository includes an experimental Indian open-world roleplay mode alongside the original dropshipping tycoon source.

## Naya Nagar open-world prototype
- Compact procedural Indian city, roads, colourful low-poly buildings and third-person avatar.
- Touch movement, sprint/stamina, rupee HUD, delivery jobs, snack shop, Mandir blessing, vehicle toggle and fast travel.
- Three mall landmarks, stadium, Mandir, bus/auto stand, ten food-outlet signs and Diwali mode.
- Design document: Docs/NAYA-NAGAR-GDD.md.

## Open in Unity
Use Unity 2022.3 LTS with Android Build Support, Android SDK & NDK Tools, and OpenJDK. Open the repository as a Unity project and run Assets/Scenes/Main.unity.

## Build APK with GitHub Actions
The workflow .github/workflows/unity-android.yml builds and uploads NAYA-NAGAR.apk. Unity license secrets are required: UNITY_LICENSE, UNITY_EMAIL, and UNITY_PASSWORD (GameCI activation guide: https://game.ci/docs/github/activation).

Status: source files are committed, but an APK is not confirmed until Actions succeeds. This prototype uses placeholder geometry and needs playtesting and art polish.
