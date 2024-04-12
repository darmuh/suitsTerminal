# suitsTerminal by darmuh

## Description

Created as an alternative solution to having too many suits, this mod will remove all suits from the rack and store them in the terminal.
You can also leave up to 13 suits to remain on the rack via this mod's configuration options, or remove none if you have another mod taking care of the excess suits.


### advancedTerminalMenu:

 - A singular 'suits' command will be generated.
	- This command will also be shown in the 'Other' command listing.

 - The 'suits' command will bring up an interactive menu that can be navigated by arrow keys.
	- Please note that any other mod that uses the arrow keys for functionality may interfere with this menu.
	- I plan to add compatibility with my mod (darmuhsTerminalStuff) in a later update, however there aren't any major issues between the two.
	- The specialMenusActive bool in this mod is public and accessible.
	- The public bool can be used by other mods to indicate not to listen for shortcut during this specialmenu, which is likely what I'll do for TerminalStuff.

 - Controls for the advancedTerminalMenu are completely configurable, any key except for tab is acceptable.
	- See valid key names here - https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/UnityEngine.InputSystem.Key.html

 - Pages will be generated based on how many suits are detected at load in.
	- I have tested up to 320 suits without any issues. Feel free to try and break things.

 - A Mirror Camera is generated as part of this menu so you can see what the suit you are equipping looks like.
	- This can be toggled by the togglePiP key binding config option.
	- Or disabled completely via the enablePiPCamera config option.
	- See also the below configurable keys to modify the mirror camera view by zoom, height, or rotation.
		- togglePiPZoom: This key will be used to cycle between 4 different zoom options.
		- togglePiPHeight: This key will allow you to change the height of the mirror camera to see different parts of the suit you've equipped.
		- togglePiPRotation: This key will allow you to cycle between 4 different angles around the player so you can see each side of yourself and the back.
	
 - Here is some videos from dev testing showcasing this mod's progression as newer features were developed. (note these videos are not the final product) 
	- [v1.0.0 video demo (youtube)](https://www.youtube.com/watch?v=4qNo0Qn6zJk)
	- [v1.1.0 video demo (youtube)](https://www.youtube.com/watch?v=bOm86ieLVfM)
	- [v1.1.3 video demo (youtube)](https://www.youtube.com/watch?v=6fJ2Vm1iekQ)

	
### Chat Commands:

 - These commands utilize some of the advanceTerminalMenu's handling, while still requiring commands to be run like the legacy terminal commands.

 - Type !suits to see the first page, and !suits (page number) for each page after that.

 - Type !wear and then the number associated with the suit name to equip it.
	- So if you see '!wear 3' (Shrek) you will type !wear 3 in chat to equip it.

### Legacy Terminal Commands:

 - A command will be generated for every page of suits (6 suits per terminal page)

 - 'suits' command is always the first page, each page after is 'suits (pagenumber)'

 - Generates a command for every suit

 - To pick a suit to wear you will use the command 'wear (suitname)'

 - you can see each suit name in the suits page commands

 - use command 'randomsuit' to wear a random suit from the list.

FYI - This mod is more than capabale of adding more than the default maximum 100 suits from More_Suits. Feel free to change that configuration option and try to find the max possible suits you can add.

