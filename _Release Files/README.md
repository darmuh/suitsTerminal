# suitsTerminal by darmuh

## Description

Created as an *alternative* solution to having too many suits on the suits rack! This mod will remove all suits from the rack and store them in the terminal.
You can also leave up to 13 suits (configurable) to remain on the rack via this mod's configuration options, or remove none if you have another mod taking care of the excess suits.

### advancedTerminalMenu:

 - A singular 'suits' command will be generated.
	- duplicate suit names will be listed with their unique suitID.

 - The 'suits' command will bring up an interactive menu that can be navigated by arrow keys. (with advancedTerminalMenu enabled)
	- Please note that any other mod that uses the arrow keys for functionality may interfere with this menu.
	- The specialMenusActive bool in this mod is public and accessible.
	- The public bool can be used by other mods to indicate not to listen for shortcut during this specialmenu.

 - Favoriting system and separate favorites menu is now available.
	- Simply press the [favItemKey] to add the currently selected suit to your favorite suits.
	- To see only your favorited suits, press the [favMenuKey] to toggle the menu to the favorites list.
	- Favorites will be saved to the [favoritesMenuList] configuration item.
	- Default keybinds are [favItemKey = F] and [favMenuKey = F1].

 - Add only specific suits to the rack's 13 available spots.
	- Use [SuitsOnRackOnly] to specify a specific list of suit names to show on the rack.
	- Use [DontAddToRack] to specify a list of suit names that should NOT show on the rack.
	- Leave both lists blank if you dont want to specify specific suit names.
	- Favorite some suits and look at favoritesMenuList for examples of suit names.
	- These settings do not override any of the following config options: [suitsOnRack], [dontRemove], [hideRack]

 - Specify suits that should NOT be available in the terminal via [DontAddToTerminal]
	- Leave blank if you would like all suits added to the terminal

 - Set a new default suit with the [DefaultSuit] configuration item.
	- This feature will automatically disable itself if SuitSaver is present to avoid conflicts.

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
 
 - [NEW IN 1.3.0] OpenBodyCams Support has been added for the Mirror Camera
	- The camera created with OpenBodyCams will be used in place of the base camera that has been used in this mod.
	- All of the above binds will continue to apply to this camera.
	- The configuration item [useOpenBodyCams] will determine whether OpenBodyCams is used to create the MirrorCamera.
	- The configuration item [obcResolution] determines the resolution the camera created by OpenBodyCams will use.

 - [NEW IN 1.3.0] More configuration items have been added to customize the advancedTerminalMenu experience.
	- [menuKeyPressDelay] is the regular delay when checking for key presses in the advancedTerminalMenu. (This delay will be added ontop of menuPostSelectDelay)
	- [menuPostSelectDelay] is the delay used after a key press is registered in the advancedTerminalMenu.
	- I have modified the default values for these two. They were originally both set to 0.1 in previous versions of suitsTerminal.
	
 - Here is some videos from dev testing showcasing this mod's progression as newer features were developed. (note these videos are not the final product) 
	- [v1.0.0 video demo (youtube)](https://www.youtube.com/watch?v=4qNo0Qn6zJk)
	- [v1.1.0 video demo (youtube)](https://www.youtube.com/watch?v=bOm86ieLVfM)
	- [v1.1.3 video demo (youtube)](https://www.youtube.com/watch?v=6fJ2Vm1iekQ)
	- [v1.2.0 video demo (youtube)](https://www.youtube.com/watch?v=lCWSDqoQolU)
	- [v1.3.0 video demo (youtube)](https://www.youtube.com/watch?v=7QvVdqLiAsg)
	- [TooManySuits v1.1.0 Compatibility demo (youtube)](https://www.youtube.com/watch?v=CzcAo6pFM4k)
	
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

