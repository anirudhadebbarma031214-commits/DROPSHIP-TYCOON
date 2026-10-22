# DROPSHIP TYCOON — Godot 4 Android

**ANI STUDIO** · 3D mobile tycoon starter

This repository now includes a Godot 4 project alongside the original Unity source. The Android APK build uses Godot and does **not** require a Unity license or the `UNITY_LICENSE` secret.

## Starter gameplay
- 3D town, roads, buildings, starter shop and car display
- Third-person character movement
- On-screen touch controls for Android; WASD/arrow keys on desktop
- Starting cash and passive income
- Buy business expansions to increase income

## Build an Android APK with GitHub Actions
1. Open the repository's **Actions** tab.
2. Select **Build Godot Android APK**.
3. Click **Run workflow** on the `main` branch.
4. Open the completed run and scroll to **Artifacts**.
5. Download `Dropship-Tycoon-Android-APK` and extract `DropshipTycoon.apk`.

The workflow uses the Godot 4.4.1 CI image and exports an ARM64 Android APK.

## Open locally
Install Godot 4.4.1 and its matching Android export templates, open this folder as a Godot project, then run `scenes/main.tscn`.

## Project files
- `project.godot` — Godot project settings
- `scenes/main.tscn` — main scene
- `scripts/main.gd` — procedural 3D world, movement, UI and tycoon loop
- `export_presets.cfg` — Android export preset
- `.github/workflows/godot-android.yml` — APK workflow

Note: the original Unity project files have been left in the repository as a backup; the new Godot workflow does not use them.
