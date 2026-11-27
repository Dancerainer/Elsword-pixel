# Elsword Pixel 1.0.2

An independently developed, noncommercial fan project for a single-player pixel action RPG. This English-language build covers Ruben and Elder. Choose a dungeon from the map and fight through its rooms to the final boss.

## Install the APK

1. Copy the 1.0.2 APK to an Android 6.0 (API 23) or newer device.
2. Open the APK in the device's file manager. If prompted, allow installation from that source and confirm **Install**.
3. Open **Elsword Pixel Ruben and Elder** and choose a dungeon from the map.

## Dungeons

| Region | Dungeons |
| --- | --- |
| Ruben | Tree of El, Forest Ruins, White Mist Swamp |
| Elder | Dark Forest, Banthus Cave, Underground Waterway, Wally Castle Outskirts, Wally Castle Center |

Clear the current objective or break the exit gate to advance. After a dungeon clear, the results show your clear time, highest combo, hits taken, and rank. You can return to the map immediately or wait for the 30-second countdown. Best results, equipped skills, and settings save locally on the device.

## Controls

| Action | Touch | Hardware keyboard |
| --- | --- | --- |
| Move and sprint | Drag the stick left or right. Push it to the outer ring to sprint. | Left / Right arrows. Hold Shift or double-tap a direction to sprint. |
| Jump or drop | Flick the stick up or down. | Up / Down arrows |
| Attacks | **Light** and **Heavy** | `Z` and `X` |
| Skills | Tap a slot on the skill wheel. | `Q W E R` and `A S D C` |
| Awakening | **Awaken** | Left Ctrl |
| Potions | Tap the potion rack. | `1`, `2`, `3`. `T` also uses the full recovery potion. |
| Menus | Use the on-screen **Map**, **Skills**, and **Commands** buttons. | `Tab` for the map, `K` for skills, `F1` for commands, `F2` for settings. |

Select **Commands** for combo instructions or **Skills** to change your loadout. You can switch input modes in Settings.

## Build the Android APK

Install the Unity Editor version in [ProjectVersion.txt](unity/ProjectSettings/ProjectVersion.txt) with Android Build Support, its SDK and NDK tools, and OpenJDK. From the repository root, run PowerShell with your installed Unity executable path.

```powershell
.\unity\tools\build-unity.ps1 -Target Android -EditorPath "C:\path\to\Unity.exe"
```

The build script prints the APK path after validation. The Editor build log is saved to `unity/Reports/editor-android.log`.

## License

The original project code is available under the [MIT License](LICENSE). Elsword game audio, trademarks, and game-derived assets retain the terms of their respective rights holders.

Silkscreen is by the Silkscreen Project Authors and distributed under the [SIL Open Font License 1.1](unity/Assets/Resources/Pixel/ui/OFL.txt).
