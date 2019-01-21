//* Description *//
// Title: Package
// Author: Boom (9740)
// Defines the package for this add-on.

// Disable Existing Package
if(isPackage(Support_BaseplateAlignment_Package)) {
	deactivatePackage(Support_BaseplateAlignment_Package);
}

//* Package *//
package Support_BaseplateAlignment_Package
{
	// /**
	//  * Triggered when a client plants a brick.
	//  *
	//  * @param  {GameConnection}  %client  The client attempting to plant a brick.
	//  *
	//  * @return {void}
	//  */
	function serverCmdPlantBrick(%client)
	{
		// Perform the baseplate plant brick check
		if(!%client.doBaseplatePlantBrickCheck()) {
			return;
		}

		// Use the default functionality
		return Parent::serverCmdPlantBrick(%client);
	}

	// Certain mods will plant bricks without calling the plant brick
	// server command. Just to cover our bases, we're going to use
	// overrides for both "/plantbrick" and fxDTSBrick.plant().

	// /**
	//  * Attempts to plant the calling brick.
	//  *
	//  * @param  {fxDTSBrick}  %brick  The calling brick.
	//  *
	//  * @return {integer}
	//  */
	function fxDTSBrick::plant(%brick)
	{
		// Determine the client
		%client = %brick.client;

		// If the brick has a client, perform the baseplate plant brick check
		if(isObject(%client) && !%client.doBaseplatePlantBrickCheck(%brick)) {
			return 4; // Unstable
		}

		// If there is no client, still perform the baseplate plant brick check
		if(!isObject(%client) && !%brick.doBaseplateCheck()) {
			return 4; // Unstable
		}

		// Use the default functionality
		return Parent::plant(%brick);
	}

	// If you are a cheeky bastard, then you may have noticed that a
	// brick in its death animation still counts as adjaceny. Even
	// though the brick can be killed later on, it's not clean.

	// /**
	//  * Triggered when the calling brick is being killed.
	//  *
	//  * @param  {fxDTSBrick}  %brick  The calling brick.
	//  *
	//  * @return {integer}
	//  */
	function fxDTSBrick::onDeath(%brick)
	{
		// Mark the brick as dead
		%brick.isDead = true;

		// Use the default functionality
		return Parent::onDeath(%brick);
	}

	// /**
	//  * Triggered for each tick of a save file being loaded.
	//  *
	//  * @return {void}
	//  */
	function ServerLoadSaveFile_Tick()
	{
		// If baseplate rules are disabled, carry on
		if(!$Support::Baseplate::Required) {
			return Parent::ServerLoadSaveFile_Tick();
		}

		// When baseplate rules are enabled, certain bricks may get
		// loaded that violate baseplate rules. However, we can't
		// assume that the bricks are supposed to follow rules.

		// Temporarily disable baseplate rules
		$Support::Baseplate::Required = 0;

		// Call the parent function
		%result = Parent::ServerLoadSaveFile_Tick();

		// Enable baseplate rules
		$Support::Baseplate::Required = 1;

		// Return the result
		return %result;
	}

};

// Activate the package
activatePackage(Support_BaseplateAlignment_Package);