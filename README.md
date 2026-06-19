# EngineX

EngineX is a retro 2D space-lander game built in Unity. Pilot a small lander through rocky space caverns, conserve fuel, collect coins and fuel pickups, then touch down gently on multiplier landing pads for the best score.

![EngineX gameplay screenshot](Assets/Screenshots/game.png)

## Gameplay

Watch the gameplay video: [gameplay.mp4](Assets/Screenshots/gameplay.mp4)

The goal is simple: fly carefully, spend fuel wisely, and land cleanly. The lander uses physics-based thrust, so every movement affects your momentum and rotation. A successful landing depends on both speed and angle, with higher multiplier pads awarding larger landing scores.

## Features

- Physics-based 2D lander movement with thrust and rotation.
- Fuel management with a live HUD and low-fuel state.
- Coin pickups for score.
- Fuel pickups to extend each run.
- Landing pads with score multipliers.
- Thruster particle effects tied to player input.
- Sound effects and music for pickups, thrust, landing, and crashes.
- Pixel-art-inspired presentation with a space cave layout.

## Controls

| Action | Keyboard |
| --- | --- |
| Thrust up | `W` or `Up Arrow` |
| Rotate left | `A` or `Left Arrow` |
| Rotate right | `D` or `Right Arrow` |

## Tech Stack

- Unity `6000.5.0f1`
- C#
- Universal Render Pipeline `17.5.0`
- Unity 2D Renderer / 2D Physics
- Unity Input System `1.19.0`
- TextMesh Pro
- Unity Particle System
- Unity UI
- 2D sprite, Sprite Shape, Tilemap, and related Unity 2D packages

## Project Structure

```text
EngineX/
  Assets/
    Scenes/             Unity scenes
    Scripts/            Gameplay, HUD, pickup, and landing logic
    Prefabs/            Lander, pickups, landing pads, and particle systems
    Textures/           Sprites and visual assets
    Sounds/             Music and sound effects
    Screenshots/        README screenshot and gameplay video
  Packages/             Unity package manifest and lock file
  ProjectSettings/      Unity project settings
```

## Getting Started

1. Install Unity `6000.5.0f1` or a compatible Unity 6 editor.
2. Open Unity Hub.
3. Add the project folder: `EngineX/`.
4. Open the scene at `Assets/Scenes/SampleScene.unity`.
5. Press Play.

## Main Scripts

- `Lander.cs` handles player input, fuel consumption, physics thrust, pickups, and landing score calculation.
- `GameHUD.cs` builds and updates the in-game fuel and score display.
- `PickupSpawner.cs` places fuel and coin pickups in clear positions.
- `LandingPad.cs` stores landing pad score multipliers.
- `LanderVisuals.cs` controls thruster particle effects.

## Assets

The game includes custom textures, audio, prefabs, and a VT323-style pixel font to support the retro arcade look.
