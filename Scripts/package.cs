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
		// Determine the brick attempting to being planted
		%brick = %client.player.tempBrick;

		// Make sure the brick exists
		if(!isObject(%brick)) {
			return Parent::serverCmdPlantBrick(%client);
		}

		// Perform the baseplate check
		if((%reason = %brick.doBaseplateCheck(true)) !$= true) {

			// Send the reason to the client
			%client.sendBaseplateReasonMessage(%reason);

			// Prevent the brick from being planted
			return;

		}

		// Use the default functionality
		return Parent::serverCmdPlantBrick(%client);
	}
};

// Activate the package
activatePackage(Support_BaseplateAlignment_Package);