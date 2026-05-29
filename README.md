# Darkroom Escape

A first-person puzzle escape room built in Unity 6  

## Concept
The player is locked in a dark scholar's study. The room is dim. To escape, they must complete five visual puzzles, lighting candles, examining a table, flipping switches in the correct order, triggering the chandelier, and matching a code to candles by the door all before a 5-minute timer expires.

## Puzzle Tasks
1. **Light the first candle** - illuminates the room in red, revealing the developing table.
2. **Examine the table** - uncovers a book and lights three wall candles.
3. **Touch the three wall candles in correct order** (hinted by their brightness, brightest first). Wrong order resets the entire sequence and the room goes dark.
4. **Pull the chandelier** - replaces the red light with white light, revealing a 3-digit code on the bookshelf and three numbered candles by the door.
5. **Click the three numbered candles in the order shown by the bookshelf code** to unlock the door.

## Controls
- WASD - move
- Mouse - look
- Left Click - interact
- Esc - release cursor (testing only)

## Win Condition
All 5 puzzles solved → door unlocks → click to open → "YOU ESCAPED" screen.

## Lose Conditions
- 5-minute timer expires → "TIME'S UP" screen with restart.
- Wrong candle order in puzzle 3 → full sequence resets, room goes dark.

## Custom Scripts
- **PuzzleManager.cs** - central singleton; tracks progress, runs timer, handles win/lose, manages full-sequence reset on failure, validates order for both switch and final-candle puzzles.
- **Interactable.cs** - every clickable object uses this. An enum dispatches to one of seven interaction types (RedChain, Tray, Switch, WhiteChain, Dial, FinalCandle, Door), each gated on previous puzzle progress.
- **PlayerInteraction.cs** - camera raycast that calls Interact() on click and updates the interaction prompt text.

## Asset Sources
- Free Medieval Room by Omni Studio - https://assetstore.unity.com/packages/3d/environments/free-medieval-room-131004 - Unity Asset Store EULA
- Mini First Person Controller by Simon Serge Pasi - Unity Asset Store EULA
- Audio: Youtube

## How to Run
1. Clone this repo
2. Open in Unity 6 (or Unity 2022.3 LTS)
3. Open `Assets/_Project/Scenes/Darkroom.unity`
4. Press Play

