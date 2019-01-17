//* Description *//
// Title: Baseplates
// Author: Boom (9740)
// Defines the baseplate portion of this add-on.

// /**
//  * Performs the baseplate plant brick check on the calling client.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  * @param  {fxDTSBrick}      %brick   The brick attempting to be planted.
//  *
//  * @return {boolean}
//  */
function GameConnection::doBaseplatePlantBrickCheck(%client, %brick)
{
	// If no brick was provided, use the player temp brick
	if(%brick $= "") {
		%brick = %client.player.tempBrick;
	}

	// Make sure the brick exists
	if(!isObject(%brick)) {
		return true;
	}

	// Perform the baseplate check
	if((%reason = %brick.doBaseplateCheck(true)) !$= true) {

		// Send the reason to the client
		%client.sendBaseplateReasonMessage(%reason);

		// Prevent the brick from being planted
		return false;

	}

	// Use the default functionality
	return true;
}


// /**
//  * Returns whether or not the calling brick is on the ground.
//  *
//  * @param  {fxDTSBrick}  %brick  The calling brick.
//  *
//  * @return {boolean}
//  */
function fxDTSBrick::isGrounded(%brick)
{
	return %brick.getDistanceToGround() == 0;
}

// /**
//  * Returns the distance to the ground for the calling brick.
//  *
//  * @param  {fxDTSBrick}  %brick  The calling brick.
//  *
//  * @return {float}
//  */
function fxDTSBrick::getDistanceToGround(%brick)
{
	return getWord(%brick.getTransform(), 2) - %brick.getDatablock().brickSizeZ / 10;
}

// /**
//  * Performs the baseplate check for the calling brick.
//  *
//  * @param  {fxDTSBrick}  %brick   The calling brick.
//  * @param  {boolean}     %reason  Whether or not to return a reason.
//  *
//  * @return {string|boolean}
//  */
function fxDTSBrick::doBaseplateCheck(%brick, %reason)
{
	// If the reason flag wasn't provided, assume false
	if(%reason $= "") {
		%reason = false;
	}

	// The first thing that we need to check is whether or not we should
	// be enforcing baseplate rules to begin with. If the feature has
	// been disabled, or the brick is not on the ground, skip it.

	// If a baseplate is not required, use the default functionality
	if(!$Support::Baseplate::Required) {
		return true;
	}

	// If the baseplate is not on the ground, then use the default functionality
	if(!%brick.isGrounded()) {
		return true;
	}

	// At this point we know everyone must build on baseplates. If they
	// are not starting with a baseplate, we should point that out to
	// them. We'll get to the other rules once they learn this one.

	// Make sure the brick is a baseplate
	if(%brick.getDatablock().category !$= "Baseplates") {
		return %reason ? $Support::Baseplate::Reason::InvalidBrick : false;
	}

	// Now we know that they are working with a baseplate, but if it is
	// not large enough, we will not be able to align to the grid. We
	// can check the size ahead of time before we do other stuff.

	// Determine the size of the brick in grid units
	%size = %brick.getAlignmentGridSize();

	// Make sure the brick is wide enough
	if(getWord(%size, 0) == 0) {
		return %reason ? $Support::Baseplate::Reason::NotWideEnough : false;
	}

	// Make sure the brick is long enough
	if(getWord(%size, 1) == 0) {
		return %reason ? $Support::Baseplate::Reason::NotLongEnough : false;
	}

	// Just because the brick is a baseplate does not mean we can use it.
	// There are different types of baseplates that we must consider.
	// We must confirm that the right type of baseplate is used.

	// Make sure that the baseplate is the correct type
	if(!%brick.isCorrectBaseplateType()) {
		return %reason ? $Support::Baseplate::Reason::IncorrectType : false;
	}

	// With the basics out of the way, we can begin checking the first
	// major rule: the brick must be aligned to the grid. When it's
	// not aligned, we'll snap it for them, but also tell them.

	// Check for baseplate alignment
	if($Support::Baseplate::Alignment::Enabled) {

		// Make sure the brick is aligned
		if(!%brick.doAlignmentCheck()) {
			return %reason ? $Support::Baseplate::Reason::UnalignedBaseplate : false;
		}

	}

	// The last rule is adjacency. When enabled, the baseplate must be
	// adjancent to another baseplate. We'll exlude ghost bricks for
	// this, and we'll let them know about this rule if it fails.

	// Check for baseplate adjacency
	if($Support::Baseplate::Adjacency::Enabled)	{

		// Make sure the brick is adjacent to another baseplate
		if(!%brick.doAdjacencyCheck()) {
			return %reason ? $Support::Baseplate::Reason::NoBaseplateNeighbor : false;
		}

	}

	// Return success
	return true;
}

// /**
//  * Returns whether or not the calling brick is the correct baseplate type.
//  *
//  * @param  {fxDTSBrick}  %brick  The calling brick.
//  *
//  * @return {boolean}
//  */
function fxDTSBrick::isCorrectBaseplateType(%brick)
{
	// Determine the correct baseplate type
	%type = $Support::Baseplate::Type;

	// Check each type
	switch(%type) {

		// Any
		case $Support::Baseplate::Type::Any:
			return true;

		// Plate
		case $Support::Baseplate::Type::Plate:
			return %brick.getDatablock().brickSizeZ == 1;

		// Plain
		case $Support::Baseplate::Type::Plain:
			return %brick.getDatablock().subCategory $= "Plain";

	}

	// Unsupported type
	return false;
}

// /**
//  * Sends the message for the specified reason to the calling client.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  * @param  {string}          %reason  The reason code.
//  *
//  * @return {void}
//  */
function GameConnection::sendBaseplateReasonMessage(%client, %reason)
{
	// Determine the reason message
	%message = %client.getBaseplateReasonMessage(%reason);

	// Send the baseplate reason message
	commandToClient(%client, 'MessageBoxOK', %reason, %message);
}

// /**
//  * Returns the message for the specified reason.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  * @param  {string}          %reason  The reason code.
//  *
//  * @return {string}
//  */
function GameConnection::getBaseplateReasonMessage(%client, %reason)
{
	// Determine by reason
	switch$(%reason) {

		// Invalid Brick
		case $Support::Baseplate::Reason::InvalidBrick:
			return "You must start with a baseplate!<bitmap:base/client/ui/brickIcons/16x16 Base>";

		// Not Wide Enough
		case $Support::Baseplate::Reason::NotWideEnough:
			return "You must start with a baseplate at least" SPC $Support::Baseplate::Alignment::Width SPC "blocks wide!";

		// Not Long Enough
		case $Support::Baseplate::Reason::NotLongEnough:
			return "You must start with a baseplate at least" SPC $Support::Baseplate::Alignment::Length SPC "blocks long!";

		// Incorrect Type
		case $Support::Baseplate::Reason::IncorrectType:
			return %client.getBaseplateIncorrectTypeReasonMessage();
			return "You must use a %type baseplate!";

		// Unaligned Baseplate
		case $Support::Baseplate::Reason::UnalignedBaseplate:
			return "You must align the baseplate to the existing grid!";

		// No Baseplate Neighbor
		case $Support::Baseplate::Reason::NoBaseplateNeighbor:
			return "You must place the baseplate adjacent to another baseplate!";

	}
}

// /**
//  * Returns the incorrect type reason message.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {string}
//  */
function GameConnection::getBaseplateIncorrectTypeReasonMessage(%client)
{
	// Determine the correct baseplate type
	%type = $Support::Baseplate::Type;

	// Check each type
	switch(%type) {

		// Plate
		case $Support::Baseplate::Type::Plate:
			return "You must start with a plate baseplate!";

		// Plain
		case $Support::Baseplate::Type::Plain:
			return "You must start with a plain baseplate!";

	}

	// Unsupported type
	return "You must start with the correct type of baseplate!";
}