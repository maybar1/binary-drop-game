# Binary Drop

A fast, puzzle-y binary path game built in Unity (5x5 grid). Drag across adjacent tiles to form a binary number; match the target decimals to win. Each level increases bit-length (3, 4, 5 bits).

## How to Run 
1. Clone this repo.
2. Open the project in **Unity 6.x** (or the version you used).
3. Open scene: `Assets/Scenes/level1.unity`.
4. Press ▶️ Play.

> Make sure the `Assets/`, `ProjectSettings/`, and `Packages/` folders exist in your clone.

## Play the Build
You can download the full game build along with the complete Unity project from Google Drive:  
- **[Download from Google Drive]([https://drive.google.com/your-link-here](https://drive.google.com/file/d/1IoLRbsZUk9llXo8aqq-UBMmcYgLeYHVC/view?usp=drive_link))**

The ZIP contains:
- Windows executable (`.exe`) for running the game without Unity
- Full Unity project (Assets, ProjectSettings, Packages) for development

## Game Rules 
- Select **adjacent** tiles (up/down/left/right) to form a binary number of the required bit-length (Level 1: 3 bits, Level 2: 4 bits, Level 3: 5 bits).
- The decimal value is shown live while dragging.
- When you match a target number, it turns green. Find all targets before time runs out.

## Team
- May — Lead developer: gameplay programming, UI/UX design, level logic, audio integration, bug fixing, build & deployment.
- Vered — Assisted in game concept, provided feedback on level design, contributed to SFX selection.
- Ayelet — Assisted in visual theme decisions, tested gameplay, provided feedback for improvements.

## Tech
- Unity 6, TextMeshPro
- Simple grid generator + DFS path sampling for valid targets

## Credits & Assets

- Code assistance: ChatGPT (prompt engineering & refactors)

