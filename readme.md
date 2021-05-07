# UnrealTournamentHotfixer

Quick WPF application for applying some common .INI hotfixes to Unreal Tournament 2004. 
Includes a rough draft of an .INI parser based off of the defunct [ini-parser](https://github.com/rickyah/ini-parser) written by Ricardo Hernández

## What it takes care of

- Sets `ConfiguredInternetSpeed` to `10001` to "unlock" the 85 fps limit
- Sets a bind for setting the internet speed (`netspeed 10001`) to the `p` key if the above .INI setting doesn't hold
- Sets the `MaxClientFrameRate` to 112 (middle ground for stability without framerate dips)
- Adds a section to fix the "UT2004 is not yet authorized to connect to the internet" message when clicking "Join Game" from the main menu

