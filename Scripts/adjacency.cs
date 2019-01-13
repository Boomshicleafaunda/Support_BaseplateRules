//* Description *//
// Title: Adjacency
// Author: Boom (9740)
// Defines the adjacency portion of this add-on.

// /**
//  * Triggered when a brick is checked for baseplate neighbors.
//  *
//  * @param  {fxDTSBrick}  %brick   The brick performing the neighbor check.
//  *
//  * @return {boolean}
//  */
function fxDTSBrick::onAdjacencyCheck(%brick)
{
	// This method would be a good spot for a package to add any
	// overrides for admins or something if you wanted to have
	// scenarios where you could ignore the adjacency rules.

	// One quirk of adjacency is that if there aren't any bricks
	// planted, there's nothing to be adjacent to. If this is
	// the first brick, we should ignore adjacency rules.

	// If this is the first brick, ignore adjacency rules
	if(%brick.isFirstBrick()) {
		return false;
	}

	// Return whether or not to perform the neighbor check
	return true;
}

// /**
//  * Triggered when a baseplate passes a neighbor check.
//  *
//  * @param  {fxDTSBrick}  %brick  The brick that passed the neighbor check.
//  *
//  * @return {void}
//  */
function fxDTSBrick::onAdjacencyPass(%brick)
{
	// This method purely exists for package hooks, and serves no
	// purpose in this mod's default functionality. However, an
	// external add-on may choose to add a listener to this.
}

// /**
//  * Triggered when a baseplate fails a neighbor check.
//  *
//  * @param  {fxDTSBrick}  %brick  The brick that failed the neighbor check.
//  *
//  * @return {void}
//  */
function fxDTSBrick::onAdjacencyFail(%brick)
{
	// This method purely exists for package hooks, and serves no
	// purpose in this mod's default functionality. However, an
	// external add-on may choose to add a listener to this.
}

// /**
//  * Performs a neighbor check on the calling brick.
//  *
//  * @param  {fxDTSBrick}  %brick  The calling brick.
//  *
//  * @return {boolean}
//  */
function fxDTSBrick::doAdjacencyCheck(%brick)
{
	// Call the neighbor check to make sure we should do it
	%result = %brick.onAdjacencyCheck();

	// If the result is strictly false, then do not perform the check
	if(%result $= 0) {
		return true;
	}

	// Perform the neighbor check
	%success = %brick.hasAdjacentBaseplate();

	// Call the success trigger
	if(%success) {
		%brick.onAdjacencyPass();
	} else {
		%brick.onAdjacencyFail();
	}

	// Return whether or not the check was successful
	return %success;
}

// /**
//  * Returns whether or not the calling brick has a baseplate neighbor.
//  *
//  * @param  {fxDTSBrick}  %brick  The calling brick.
//  *
//  * @return {boolean}
//  */
function fxDTSBrick::hasAdjacentBaseplate(%brick)
{
	// Before we can perform an adjacency check, there's a lot of info
	// that we need to know. We'll pull everything together ahead of
	// time, and perform the check near the end of this function.

	// Determine the alignment of the brick
	%alignment = %brick.getAlignment();

	%alignmentX = getWord(%alignment, 0);
	%alignmentY = getWord(%alignment, 1);
	%alignmentZ = getWord(%alignment, 2);

	// Determine the center of the brick
	%brickPos = getWords(%brick.getTransform(), 0, 2);

	// Determine the grid size of the brick
	%brickCellSize = %brick.getAlignmentGridSize();

	%brickCellWidth = getWord(%brickCellSize, 0);
	%brickCellLength = getWord(%brickCellSize, 1);

	// Determine the grid cell size
	%gridCellWidth = $Support::Baseplate::Alignment::Width;
	%gridCellLength = $Support::Baseplate::Alignment::Length;

	// The global position of bricks is not as neat as one would expect.
	// Every 1 meter counts as 2 blocks laterally. Because of this,
	// we need to scale down our grid to match the global size.

	// Scale the grid
	%gridCellWidth /= 2;
	%gridCellLength /= 2;

	// Determine the starting positions for checking (left and top)
	%startX = %alignmentX - (%brickCellWidth / 2 + 0.5) * %gridCellWidth;
	%startY = %alignmentY - (%brickCellLength / 2 + 0.5) * %gridCellLength;

	// Determine the ending positions for checking (right and bottom)
	%endX = %startX + (%brickCellWidth + 1) * %gridCellWidth;
	%endY = %startY + (%brickCellLength + 1) * %gridCellLength;

	// To determine the bottom of the brick, we must first calculate the
	// height in meters. Every 1 meter counts as 5 plates vertically.
	// We then need half that value to go from center to bottom.

	// Use the bottom of the brick the current height position
	%currentZ = getWord(%brickPos, 2) - %brick.getDatablock().brickSizeZ / 10;

	// The absolute bottom of the brick is not actually what we want. A
	// raycast would skim just slightly under the brick and be unable
	// to find anything. We should go up by half a plate instead.

	// Increase the current height position by half a plate
	%currentZ += 0.1;

	// We are going to raycast from the bottom origin of the brick to a
	// possible location of an adjacent brick. Before we can do that,
	// we must of course calculate the origin we should be using.

	// Determine the raycast origin
	%origin = getWord(%brickPos, 0) SPC getWord(%brickPos, 1) SPC %currentZ;

	// Iterate through the top alignment
	for(%currentX = %startX; %currentX <= %endX; %currentX += %gridCellWidth) {

		// Exclude diagonals when we're told to
		if(!$Support::Baseplate::Adjacency::IncludeDiagonals && (%currentX == %startX || %currentX == %endX)) {
			continue;
		}

		// Determine the current position
		%currentPos = %currentX SPC (%startY - 1) SPC %currentZ;

		// Check for an adjacent baseplate
		if(%brick.checkForAdjacentBaseplate(%origin, %currentPos)) {
			return true;
		}

	}

	// Iterate through the bottom alignment
	for(%currentX = %startX; %currentX <= %endX; %currentX += %gridCellWidth) {

		// Exclude diagonals when we're told to
		if(!$Support::Baseplate::Adjacency::IncludeDiagonals && (%currentX == %startX || %currentX == %endX)) {
			continue;
		}

		// Determine the current position
		%currentPos = %currentX SPC (%endY + 1) SPC %currentZ;

		// Check for an adjacent baseplate
		if(%brick.checkForAdjacentBaseplate(%origin, %currentPos)) {
			return true;
		}

	}

	// Iterate through the left alignment
	for(%currentY = %startY; %currentY <= %endY; %currentY += %gridCellLength) {

		// Exclude diagonals when we're told to
		if(!$Support::Baseplate::Adjacency::IncludeDiagonals && (%currentY == %startY || %currentY == %endY)) {
			continue;
		}

		// Determine the current position
		%currentPos = (%startX - 1) SPC %currentY SPC %currentZ;

		// Check for an adjacent baseplate
		if(%brick.checkForAdjacentBaseplate(%origin, %currentPos)) {
			return true;
		}

	}

	// Iterate through the right alignment
	for(%currentY = %startY; %currentY <= %endY; %currentY += %gridCellLength) {

		// Exclude diagonals when we're told to
		if(!$Support::Baseplate::Adjacency::IncludeDiagonals && (%currentY == %startY || %currentY == %endY)) {
			continue;
		}

		// Determine the current position
		%currentPos = (%endX + 1) SPC %currentY SPC %currentZ;

		// Check for an adjacent baseplate
		if(%brick.checkForAdjacentBaseplate(%origin, %currentPos)) {
			return true;
		}

	}

	// No adjacent baseplates were found
	return false;
}

// /**
//  * Returns whether or not an adjancent baseplate exists at the specified position.
//  *
//  * @param  {fxDTSBrick}  %brick     The calling brick.
//  * @param  {string}      %origin    The origin (bottom center) of the brick.
//  * @param  {string}      %position  The position to check for an adjacent baseplate.
//  *
//  * @return {boolean}
//  */
function fxDTSBrick::checkForAdjacentBaseplate(%brick, %origin, %position)
{
	// Perform a raycast from the center of the brick to the current position
	%object = firstWord(containerRaycast(%origin, %position, $TypeMasks::FxBrickAlwaysObjectType, %brick));

	// If an object was not found, then no baseplate exists
	if(!isObject(%object)) {
		return false;
	}

	// If the object is not a baseplate, then it doesn't count
	if(%object.getDatablock().category !$= "Baseplates") {
		return false;
	}

	// Object is an adjacent baseplate
	return true;
}

// /**
//  * Returns whether or not the calling brick is the first brick on the server.
//  *
//  * @param  {fxDTSBrick}  %brick  The calling brick.
//  *
//  * @return {boolean}
//  */
function fxDTSBrick::isFirstBrick(%brick)
{
	// Determine the total count of existing bricks
	%brickCount = getBrickCount();

	// Determine all of the connected clients
	%count = ClientGroup.getCount();

	// Iterate through each client
	for(%i = 0; %i < %count; %i++) {

		// If any player has a ghost brick, remove it from the total count
		if(isObject(ClientGroup.getObject(%i).player.tempBrick)) {
			%brickCount--;
		}

	}

	// Return whether or not the brick count is zero
	return %brickCount == 0;
}