# Change Log

## [1.6.0]
 - Moved menu system to OpenLib's BetterMenu system
	- offloads a lot of the menu handling logic from this individual mod to OpenLib
	- new menu is also a general improvement in comparison to suitsTerminal's old menu
	- menu now has a main menu page with sub menus for favorites, picking a suit to wear, and the help page
	- Updated duplicate names in advancedmenu to show the suit's syncedsuitid for the duplicate suit
 - Moved the majority of command creation to OpenLib's new CommandManager system
	- should be no noticeable difference, the new system is just a refactor of the old class that was used
 - Optimized TogglePiP method to early return when state is what it's being set to
 - Removed TerminalStuff soft compat (handled by OpenLib now)

### If you experience issues with OpenLib version 0.3.0 or suitsTerminal 1.6.0, please report the issues with logs and revert to previous versions of both.	

## [1.5.12]
 - Fixed soft compat for darmuhsTerminalStuff (again)
 - including correct dll file, 1.5.11 release looks to have had 1.5.10

## [1.5.11]
 - Fixed soft compat for darmuhsTerminalStuff

## [1.5.10]
 - Fixed darmuhsTerminalStuff GUID

## [1.5.9]
 - Fixed issue with homebrew camera not updating LOD shadow level due to an early return that was added in 1.5.7 to fix another issue.
	- Thank you @duskwise for the report and help with troubleshooting the issue.
 - Added support for networked nodes when using darmuhsTerminalStuff.
	- This should fix an issue where it appeared you were in the menu but the menu had not been started by the current player.
	- Should also now sync the terminal display for other players when you are navigating the menus

## [1.5.8]
 - Fixed issue with advanced suits menu not working since switching to the new input handling system when darmuhsTerminalStuff was not present.
 - Added a check for TooManySuits. If present, this mod will not adjust the suits on the rack in anyway (even if DontRemove is set to false).
 - Fixed issue where suits that were supposed to be on the rack were out of position in various ways.
 - Fixed issue where, after being fired, paid suits would break the advancedsuitsmenu until a suit had been paid for.
 - Fixed issue where sometimes the ``[EQUIPPED]`` tag would not show for the suit you're currently wearing when first opening the suits menu.

## [1.5.7]
 - Fixed an error related to using the help menu that would throw errors related to the PictureInPicture camera.
	- You can now toggle the favorites menus from the help menu.
 - Fixed issue that would cause additional text to leak into other pages from inputs in the advancedsuitsmenu
	- This also fixes the suits command sometimes not working (there was actually an extra empty space character in the input section for some reason lol)
 - Removed some old redundant code from when I initially moved this mod to Openlib
 - Added logic to make the terminal text caret (the blinking line) transparent when in the advancedsuitsmenu and return to the color it was when you first spawned.
	- If you have mods that change this color (like darmuhsTerminalStuff) it will not update to the latest customization refresh but only what it was when you first spawned.
		- In regards to darmuhsTerminalStuff, i'll add some compatibility for this on terminalstuff's side soon.

## [1.5.6]
 - Switched to new input handling system for advancedsuitsmenu using OpenLib's new TerminalKeyPressed Event
	- Input should now feel one to one with each individual key press.
	- You can no longer hold down a key to scroll through the menu.
	- The ``MenuKeyPressDelay`` and ``MenuPostSelectDelay`` have been removed as they are no longer necessary.
 - Updated to OpenLib 0.2.10

## [1.5.5]
 - Updated menu handling to not throw the "Fatal Error" message into the terminal when a suit in the menu is NULL.
	- Will now list the specific item in the menu as ``**MISSING SUIT**``
	- Selecting a missing suit will not do anything.
	- FYI, when host/client do not share the same amount of extra suits added by mods the suits will be desynced between players. This is not something that will be fixable by suitsTerminal.
		- This is due to mismatched unlockable ID numbers from when the suit is added to the game by the mod that adds it.
	- Changed duplicate name handling to show specific suit ID number rather than adding (1) to the name.

## [1.5.4]
 - Fixed various issues with favorites menu such as
	- Fatal index error when the config item has 0 favorited suits.
	- Fatal index error due to duplicate favorite items causing extra pages than needed
 - Fixed various issues when changing rack setting config items between lobby loads.
 - Fixed issue with pagination system that would increase the page counter even if there was not a new page displayed
 - Added new config item [PersonalizedFavorites] which will NOT save favorites to the config item.
	- This will save your favorites to a text file located in the following folder 'AppData\LocalLow\ZeekerssRBLX\Lethal Company\suitsTerminal'
	- This file will persist between different mod profiles so you do not need to re-favorite when using a new profile code.
	- If a suit in your favorites does not exist in the current profile it will be ignored (not cause issues)
 - Added some more descriptive logging messages under extensive logging.
 - Added some warning logs to indicate the mod is not touching the suits rack. If you have another mod taking care of the rack, like TooManySuits, this can be ignored and is just informative.
	- I added this for any potential reported issues with the rack not adjusting properly due to user config error.

## [1.5.3]
 - Adjusted camera mask to use layer 23 instead of layer 30 (in line with most other mods)
	- This should fix [issue #15](https://github.com/darmuh/suitsTerminal/issues/15) on github
 - Fixed SuitsOnRackOnly config item, thanks CoolLKKPS for the report on [github](https://github.com/darmuh/suitsTerminal/issues/14)

## [1.5.2]
 - Fixed indexing issue when suits contain the exact same name
	- some backend changes related to this, now tracking unique suit ID numbers
 - Fixed similar but unrelated issue where non-host clients were getting an indexing error
 - Removed config option for keeping negative suit IDs as this just creates a duplicate of the default suit and nothing else.
 - Fixed some compatibility issues with terminalstuff
	- Thank you @moroxide for the help in troubleshooting this.
 - Minor fix for old terminal command creation

## [1.5.1]
 - Fixed fatal error that broke advancedsuitsmenu, sorry for missing this.
 - Added some more logging for when these fatal errors occur to directly point to the exact issue.

## [1.5.0]
 - Slight backend rework. Utilizing a new class that tracks relevant information for each suit.
	- Fixes issue of suits that were hidden from terminal causing odd desyncs in the menus
	- Should also be much more efficient (resource-wise)
 - Fixed issues with different camera implementations
	- OpenBodyCams cameras will no longer randomly change positions when equipping a suit
	- Homebrew cams have been moved to OpenLib with built in compatibility for TooManyEmotes, ModelReplacementAPI, and MirrorDecor.
 - Changed mirror camera type to a non-orthographic view
	- Zoom steps now change camera fov
	- Set height positions have also been changed completely
 - General code cleanup

## [1.4.5]
 - Fixed error with favorites menu that would break suitsTerminal completely.
	- Thanks @Lunxara for the report.

## [1.4.4]
 - Added [DefaultSuit] configuration item which will allow for setting a new default suit to wear when loading in to the game.
	- If SuitSaver is present, this will be completely disabled to avoid conflicts.

## [1.4.3]
 - Updated configuration item names, descriptions, and default values for clarity.
 - Added TransformHotfix to mirror camera for reproducible issue that I cant seem to solve.
	- The issue is if you switch suits from the rack and then go to the terminal and enter the suits menu it will almost always break the mirror camera position/rotation.
	- The TransformHotfix will basically reset the camera position/angles the frame after you enter the suits menu.
 - Added [DontAddToTerminal] config item to specify suit names that should NOT be added to the terminal.
	- You will only be able to wear this suit if it's in the rack.
 - Added [DontAddToRack] config item to specify suits that should NOT be added to the rack.
	- These suits will only be able to be worn if they are listed in the terminal.
 - Changed [suitsOnRackList] to [SuitsOnRackOnly] & removed [suitsOnRackCustom] config item.
	- If [SuitsOnRackOnly] has a value, only suits in this list will be added to the rack.
	- Handling has been updated so that this does not need to be case sensitive and leaving it empty will leave it disabled.
 - Moved some more redundant methods over to OpenLib and performed some general code cleanup.
 - Updated advancedSuitsMenu formatting, will now show currently equipped suit name at the bottom of the listing above page numbers
 - Added nullable to project and better error handling messages

## [1.4.2]
 - Added fix for compatibility with upcoming terminalstuff update
 - Hopefully fixed NRE error with addcommand method (unable to replicate on my end)
 - Updated to OpenLib 0.1.8
 - Update advancedSuitsMenu to accept 0 input to hopefully fix some issues where other mods would place words into the node anyway

## [1.4.1]
 - Fix manifest to include "darmuh-OpenLib-0.1.3"

## [1.4.0]
 - This mod is now utilizing OpenLib for it's interactions with the terminal
 - Fixed issue where homebrew mirror camera would not show the player.
	- Thanks Zaggy for the pointer
 - Updated OpenBodyCams handling to be in-line with how it is done with darmuhsTerminalStuff.
 - Some terminal commands added with this mod will be added to the "other" command listing
 - Version information will now autopopulate to the file properties

## [1.3.0]
 - Legonzaur on Github fixed an issue where all suits were still getting loaded by the game despite being hidden, causing a large amount of RAM usage.
	- Huge thankyou to all who put effort into this fix!
	- While it was a relatively small change in the code, a lot of troubleshooting/effort was put into discovering this issue.
	- I welcome anyone who would like to contribute to this project. I did originally make this just as a backup to TooManySuits if/when it failed.
 - Removed TerminalAPI dependency utilizing same command creation method as the latest version of darmuhsTerminalStuff.
	- This mod is now hard-dependency free!
	- Please report any issues with the new terminal command creation system if you find any :)
 - Updated mirror camera culling mask calculation
	- Mask should now be up to standards with most other mods that use cameras. Just like darmuhsTerminalStuff
	- removed the config item that lets you set your own culling mask integer in favor of this new system.
 - Added compatibility with OpenBodyCams, including using OBC for the mirrorcam
	- Thanks again to Zaggy for working with me to get this working.
	- Please note you need to be on OpenBodyCams version 2.2.1 or later to utilize this feature.
	- Configuration items [useOpenBodyCams] and [obcResolution] have been added.
		- [useOpenBodyCams] can be set to false to continue to use the built-in camera in suitsTerminal.
		- [obcResolution] will determine the camera resolution.
 - Moved the majority of the controls hints to a separate help menu page.
	- This page can be accessed by the new [helpMenu] configuration key.
	- While in the help menu, all controls except for [leaveMenu] and [helpMenu] are paused.
 - While using the built-in camera, your first person hand models will be hidden temporarily when the camera is being displayed.
 - Configuration items have been added to modify the time advancedTerminalMenu will wait between checking for key presses as well as after a key has been pressed.
	- [menuKeyPressDelay] is the Regular delay when checking for key presses in the advancedTerminalMenu. (This delay will be added ontop of menuPostSelectDelay)
	- [menuPostSelectDelay] is the delay used after a key press is registered in the advancedTerminalMenu.
	- I have modified the default values for these two. They were originally both set to 0.1 in previous versions of suitsTerminal.
 - The commands added by this mod no longer display in the "Other" command menu.
	- I may add support for this later, it was a TerminalAPI feature.
 - Updated chat command handling to not try to run the last chat message sent every single time enter is pressed.
 - Verified compatibility with latest version of TooManySuits when [dontRemove] is set to TRUE.
	- This allows for TooManySuits to be the only mod touching the suits on the rack, while still adding commands to the terminal.
	- One odd interaction I did find is that some suits could not be equipped from the terminal until you've at least cycled through all pages of TooManySuits' rack.
		- Not sure if this is something I can fix on my end alone.

## [1.2.0]
 - Fixed issue where equipping suits from the terminal would not save the suit with the suit saver mod enabled.
	- This issue was resolved by using a different vanilla function to change a player's suit.
		- This also simplified my code a bit more and I no longer need to grab a player's playerID when changing the suit.
 - Fixed issue where buying the new bunny & bee suits would not add them to all player's suits terminal listing.
	- This issue seems to be caused by the fact that the bunny/bee suits do not update to be unlocked properly in the terminal and sometimes will continue to show in the store after being purchased.
 - Added better duplicate name handling for suits with the same display name.
	- Suits with the same unlockable name will now be displayed with their unique suit id value in the menu.
	- This will ensure you get all of the suits you've added to the game, even if they share the same name
		- If a model is replacing a specific suit name however, like NEON from valorant, then the model will replace all instances of that suit name.
		- This is something that has to do with the model replacement mod, I can retrieve the different suit IDs but the model replaces all of them lol.
 - Added config option to choose sorting method, [suitsSortingStyle]
	- Choose between alphabetical, numerical, and none.
	- Alphabetical goes off of the UnlockableName of the suit.
	- Numerical goes off of the syncedSuitID value for each suit.
	- None will apply no specific sorting and can result in host/client racks not being synced.
 - Added config options to display specific suits on the rack (up to 13), [suitsOnRackCustom] [suitsOnRackList]
	- When [suitsOnRackCustom] is enabled, will only show suits that match the suit names listed in suitsOnRackList.
	- Note that each suit name should be separated by a comma and that the suit names ARE case sensitive.
	- Favorite some suits and look at favoritesMenuList for examples of suit names.
	- These settings do not override any of the following config options: [suitsOnRack], [dontRemove], [hideRack]
 - Added new favorites system and menu page.
	- Added [favItemKey] to select suits to add to favorites menu.
	- Added [favMenuKey] to show favorites menu.
	- Updated menu handling for this new system.
	- Added new config item to save favorited suits [favoritesMenuList]. These will be stored/loaded each play session.
 - Adjusted menu slightly to make room for new key bindings, may find an alternative solution to make the page feel less cramped in the future.

## [1.1.3] 
 - Added new configuration options for removing the suit rack and the boots below the suit rack
	- Credits to Hamster (author of LethalPipeRemoval) for finding the specific objects.
	- enable hideRack to remove suit rack (this will ignore the suitsOnRack configuration since the entire rack is being deleted).
	- enable hideBoots to remove the boots below the suit rack.
	- Thanks also to seol.jihu on discord for pointing me towards Hamster's mod for an idea on how to implement this.
 - Added the ability to move/rotate/zoom the mirror camera to different fixed positions with related keybind config options.
	- togglePiPZoom: This key will be used to cycle between 4 different zoom options.
	- togglePiPHeight: This key will allow you to change the height of the mirror camera to see different parts of the suit you've equipped.
	- togglePiPRotation: This key will allow you to cycle between 4 different angles around the player so you can see each side of yourself and the back.
 - Cleaned up the menu a bit, moved all controls to the bottom of the menu below the suits list.
	- Also added some more checks to make sure the terminal isn't accepting input while in this menu.
	- Attached the mirror camera to a specific object on the terminal so that it scrolls with the text.
	- Increased the size of the mirror camera and added a slight opacity so that text behind it can still be seen.

## [1.1.2]
 - Actually added the logic for handling the new configuration items for the advancedMenuSystem navigation. (lined out in below changes)
	- Also modified some of the keybinding logic to better handle errors & standardize keybinds to a few re-usable methods.

## [1.1.1]

 - Added new key binding configuration items for menu system:
	- menuLeft: Change key used to navigate to previous page in advancedMenuSystem.
	- menuRight: Change key used to navigate to next page in advancedMenuSystem.
	- menuDown: Change key used to navigate to next item in advancedMenuSystem.
	- menuUp: Change key used to navigate to previous item in advancedMenuSystem.
	- Added the above bindings since lots of different mods like to use the arrow keys, and also maybe some of you would like to use WASD instead.
    - Also removed key limitations in bindings as these binds are ONLY active when in the menu system. 

 - Added new configuration item for Mirror Camera:
	- setPiPCullingMask: Only modify this if you know what you are doing. Changes what is rendered on the camera.

 - Changed default culling mask for camera and added methods to ensure the body/arms of the player are shown on the camera when the camera is viewable.
	- When the player is no longer in the advancedMenuSystem and the camera is disabled the layer information that is changed will be reverted back to their last known state.
	
 - Changed all TerminalAPI related keywords to nouns instead of verbs. This should fix odd errors that were occuring whenever trying to purchase a suit that had suit in the name.

## [1.1.0]

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

## [1.0.6]

 - Added configuration option to disable terminal commands.

 - Added configuration option to disable this mod's interactions with the rack at all (will make this mod compatible with TooManySuits or any other future alternatives)
 
 - Updated chat command handling and added more specific messages when loading in based on how the mod is configured.

## [1.0.5]
 - Fixed purchase-able suits appearing floating next to the rack and not being wearable from the terminal.
 
 - Added chat commands similar to terminal commands. (!suits/!wear)
 
 - Added configuration option to disable chat commands.

## [1.0.4] 
 - Added character limit to suit names to resolve some issues with extra long suit names, thanks B1adeWo1f for the report on discord.
 
 - Added configuration option for enabling/disabling randomsuit command.

## [1.0.3]
 - Updated compatibility for any mod that for whatever reason removes keywords like Advanced Company was doing.
 
 - Added configuration option to allow for leaving up to 13 suits on the rack. Default is 0.
 
 - Added configuration option to replace negative suitIDs with positive ones, default is disabled as this may cause issues.
 
 - Added command 'randomsuit' which will allow you to change to a random suit from the list.

## [1.0.2]
 - Added compatibility check with Advanced Company that fixes the issue where it would break the terminal commands in this mod.
 
 - Removed method for replacing negative suitIDs and now just completely removing them. As far as I've seen the only suit that has this issue is the green suit from vanilla.

## [1.0.1]
 - Fixed issue where the mod would break if the host closed and opened a new lobby (sorry I left this in 1.0.0, thought i'd put a fix in before publishing)
 
 - Added Hud/Chat hints when loading in to inform player that the suits were moved off the rack to the terminal.

## [1.0.0]
*Initial release version*