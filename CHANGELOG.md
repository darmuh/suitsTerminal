# Change Log

### [1.1.2] **CURRENT VERSION**
 - Actually added the logic for handling the new configuration items for the advancedMenuSystem navigation. (lined out in below changes)
	- Also modified some of the keybinding logic to better handle errors & standardize keybinds to a few re-usable methods.

### [1.1.1]

 - <del> Added new key binding configuration items for menu system:
	- menuLeft: Change key used to navigate to previous page in advancedMenuSystem.
	- menuRight: Change key used to navigate to next page in advancedMenuSystem.
	- menuDown: Change key used to navigate to next item in advancedMenuSystem.
	- menuUp: Change key used to navigate to previous item in advancedMenuSystem.
	- Added the above bindings since lots of different mods like to use the arrow keys, and also maybe some of you would like to use WASD instead.
    - Also removed key limitations in bindings as these binds are ONLY active when in the menu system. </del>

 - Added new configuration item for Mirror Camera:
	- setPiPCullingMask: Only modify this if you know what you are doing. Changes what is rendered on the camera.

 - Changed default culling mask for camera and added methods to ensure the body/arms of the player are shown on the camera when the camera is viewable.
	- When the player is no longer in the advancedMenuSystem and the camera is disabled the layer information that is changed will be reverted back to their last known state.
	
 - Changed all TerminalAPI related keywords to nouns instead of verbs. This should fix odd errors that were occuring whenever trying to purchase a suit that had suit in the name.

### [1.1.0]

 - Moved changelog to separate file from readme.

 - Added advancedTerminalMenu and related configuration options.
	- leaveMenu: Keybind for exiting the advancedTerminalMenu (does not override exit terminal binding)
	- selectMenu: Keybind for selecting a suit to wear
	- togglePiP: Keybind for toggling the mirror-camera in the advancedTerminalMenu
	- enablePiPCamera: Set this to false to disable the mirror-camera preview completely.
	- advancedTerminalMenu: Set this to false to use the legacy terminal commands.

 - Compartmentalized some code away from working directly in patches
	- Should fix a handful of errors that were happening previously

 - Updated chat command handling once again. It's much more simplified and works very similarly to the advancedTerminalMenu
	- Uses a different vanilla method for adding chat messages.
	- To make things easier, you only type the number associated to the suit to wear it, i.e !wear 9 for the 9th suit.
	- Suit names and numbers are grabbed using the same logic as advancedTerminalMenu
	- Added invalid input error handling cases for when commands are misused.

 - Moved majority of logs to extensiveLogging, this is disabled by default.

 - Added configuration options to enable/disable the hints that display when first loading in.
	- chatHints: Disable or Enable hints that display in chat.
	- bannerHints: Disable or Enable hints that display as banners when you load in (similar to the modlist check by LC_API).

 - Updated handling method for suits bought in the store.
	- Locked suits that have a store node should not be added to the suits list.
	- The suits list is updated each time a purchase is made in the terminal to account for suits being unlocked.

 - Fixed a long-time known issue of host/client having different suits on the rack
	- Sorting is now done alphabetical instead of numerical by suitID to accomplish this.

### [1.0.6]

 - Added configuration option to disable terminal commands.

 - Added configuration option to disable this mod's interactions with the rack at all (will make this mod compatible with TooManySuits or any other future alternatives)
 
 - Updated chat command handling and added more specific messages when loading in based on how the mod is configured.

### [1.0.5]
 - Fixed purchase-able suits appearing floating next to the rack and not being wearable from the terminal.
 
 - Added chat commands similar to terminal commands. (!suits/!wear)
 
 - Added configuration option to disable chat commands.

### [1.0.4] 
 - Added character limit to suit names to resolve some issues with extra long suit names, thanks B1adeWo1f for the report on discord.
 
 - Added configuration option for enabling/disabling randomsuit command.

### [1.0.3]
 - Updated compatibility for any mod that for whatever reason removes keywords like Advanced Company was doing.
 
 - Added configuration option to allow for leaving up to 13 suits on the rack. Default is 0.
 
 - Added configuration option to replace negative suitIDs with positive ones, default is disabled as this may cause issues.
 
 - Added command 'randomsuit' which will allow you to change to a random suit from the list.

### [1.0.2]
 - Added compatibility check with Advanced Company that fixes the issue where it would break the terminal commands in this mod.
 
 - Removed method for replacing negative suitIDs and now just completely removing them. As far as I've seen the only suit that has this issue is the green suit from vanilla.

### [1.0.1]
 - Fixed issue where the mod would break if the host closed and opened a new lobby (sorry I left this in 1.0.0, thought i'd put a fix in before publishing)
 
 - Added Hud/Chat hints when loading in to inform player that the suits were moved off the rack to the terminal.

### [1.0.0]
*Initial release version*