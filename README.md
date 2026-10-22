# DROPSHIP TYCOON MOBILE

A Unity Android tycoon starter with an interactive ecommerce business loop.

## Gameplay
- Start with ₹10,000.
- Buy stock, set a selling price, and fulfill customer orders.
- Orders consume inventory and add sales revenue; cash flow accounts for stock costs and shipping.
- Upgrade marketing, warehouse capacity, and automation.
- Save progress locally on the device.

## Open in Unity
1. Install Unity Hub and Unity **2022.3 LTS** with Android Build Support, Android SDK & NDK Tools, and OpenJDK.
2. Open this folder as a Unity project.
3. Open `Assets/Scenes/Main.unity` and press Play.
4. To build locally: File → Build Settings → Android → Switch Platform → Build.

## GitHub Actions APK build
The workflow `.github/workflows/unity-android.yml` builds an APK and uploads it as an Actions artifact. Unity licensing is required. Configure repository secrets `UNITY_LICENSE`, `UNITY_EMAIL`, and `UNITY_PASSWORD` according to the GameCI activation guide: https://game.ci/docs/github/activation

This is the Unity project source, not a precompiled APK. A real APK must be compiled by Unity with a valid license and Android toolchain.
