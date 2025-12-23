# suitsTerminal by darmuh (v73)

## Description

Created as an *alternative* solution to having too many suits on the suits rack! This mod will remove all suits from the rack and store them in the terminal.
You can also leave up to 13 suits (configurable) to remain on the rack via this mod's configuration options, or remove none if you have another mod taking care of the excess suits.

### Suits Terminal Menu:

 - A singular ``suits`` command will be generated.
	- duplicate suit names will be listed with their unique suitID.
 - The ``suits`` command will bring up an interactive menu that can be navigated by arrow keys.
	- Please note that any other mod that uses the arrow keys for functionality may interfere with this menu.
 - Favoriting system and separate favorites menu is now available.
	- Favorites will be saved to the ``Profile Favorites List`` configuration item.
	- Enable ``Personalized Favorites`` to have persisting personal favorites across all mod profiles. 
		- This information is personal to you and will not be shared via profile code or in a modpack
 - The ``Start Page`` configuration item determines what page the suits command opens to.
	- By default, you will open into the main menu where you can select from various pages.
 - By default, a picture-in-picture style Mirror Camera is generated as part of this menu so you can see what the suit you are equipping looks like.
	- The ``Picture-In-Picture Style`` configuration item will determine what kind of camera, if any, is created.
	- If set to utilize OpenBodyCams, the ``OpenBodyCams Resolution`` configuration item has been provided for further customization.
	- The below are configurable keys to modify the mirror camera view by zoom, height, rotation, or even disable it!
		- ``Camera Toggle``: This key will disable the camera display.
		- ``Camera Zoom``: This key will be used to cycle between 4 different zoom options.
		- ``Camera Height``: This key will allow you to change the height of the mirror camera to see different parts of the suit you've equipped.
		- ``Camera Rotation``: This key will allow you to cycle between 4 different angles around the player so you can see each side of yourself and the back.
 - Utilize the ``Suit Sorting Style`` configuration item to determine how the suits menu items are sorted, if at all.

### Rack Configuration
 - Use ``SuitsOnRack`` to determine the number of suits to display on the rack.
 - Use ``Rack Offset`` to determine the amount of space between each suit on the rack.
 - Add only specific suits to the rack's available spots.
	- Use ``Rack Suits (ONLY)`` to specify a specific list of suit names to show on the rack.
	- Use ``DONT Add To Rack`` to specify a list of suit names that should NOT show on the rack.
	- Leave both lists blank if you dont want to specify specific suit names.
 - Determine what if anything is removed via the ``Rack Removal`` setting.
	- Note that if you choose to set this to DontRemoveAnything, the rack will not be modified by this mod (and can result in suits extending past the rack)

### Terminal Configuration
 - Specify suits that should NOT be available in the terminal via ``DontAddToTerminal``
	- Leave blank if you would like all suits added to the terminal
 - Controls for the terminal menu are completely configurable, any key except for tab is acceptable.
	- See valid key names here - https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/UnityEngine.InputSystem.Key.html
 - Pages will be generated based on how many suits are detected at load in.
	- I have tested up to 320 suits without any issues. Feel free to try and break things.

### Default Suit Configuration
 - Set a new default suit with the ``Profile Default Suit`` configuration item.
	- This feature will automatically disable itself if SuitSaver is present to avoid conflicts.
 - Enable ``Personalized Default Suit`` to have a persisting personal default suit across all mod profiles. 
	- This information is personal to you and will not be shared via profile code or in a modpack
	- NOTE: If a profile does not contain your personalized default suit your suit will not change from the standard default.


### Compatibility
 - Completely compatibile with TooManySuits or any other mod that adjusts the suit rack by setting ``Rack Removal`` to ``DontRemoveAnything``.
	- When TooManySuits is detected, the rack will never be touched.
 - OpenBodyCams Support has been added for the Picture-In-Picture Mirror Camera
	- The camera created with OpenBodyCams will be used in place of the base camera that has been used in this mod.
 - Compatible with mods that move the suit rack, like BiggerShip.
	
### Chat Commands:
 - These commands utilize some of the advanceTerminalMenu's handling, while still requiring commands to be run like the legacy terminal commands.
 - Type ``!suits`` to see the first page, and ``!suits (page number)`` for each page after that.
 - Type ``!wear`` and then the number associated with the suit name to equip it.
	- So if you see ``!wear 3 (Shrek)`` you will type ``!wear 3`` in chat to equip it.

FYI - This mod is more than capabale of adding more than the default maximum 100 suits from More_Suits. Feel free to change that configuration option and try to find the max possible suits you can add.

