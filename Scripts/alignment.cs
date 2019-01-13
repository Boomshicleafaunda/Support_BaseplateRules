//* Description *//
// Title: Alignment
// Author: Boom (9740)
// Defines the alignment portion of the add-on.

// /**
//  * Triggered when a brick is checked for grid alignment.
//  *
//  * @param  {fxDTSBrick}  %brick   The brick performing the alignment check.
//  *
//  * @return {boolean}
//  */
function fxDTSBrick::onAlignmentCheck(%brick)
{
	// This method would be a good spot for a package to add any
	// overrides for admins or something if you wanted to have
	// scenarios where you could ignore the alignment rules.
}

// /**
//  * Triggered when a baseplate passes an alignment check.
//  *
//  * @param  {fxDTSBrick}  %brick  The brick that passed the alignment check.
//  *
//  * @return {void}
//  */
function fxDTSBrick::onAlignmentPass(%brick)
{
	// This method purely exists for package hooks, and serves no
	// purpose in this mod's default functionality. However, an
	// external add-on may choose to add a listener to this.
}

// /**
//  * Triggered when a baseplate fails an alignment check.
//  *
//  * @param  {fxDTSBrick}  %brick  The brick that passed the alignment check.
//  *
//  * @return {void}
//  */
function fxDTSBrick::onAlignmentFail(%brick)
{
	// One quirk of alignment failure is auto alignment. When this
	// is enabled, we'll automatically align the ghost brick for
	// the player. Attempting to plant again should pass it.

	// Check for auto-align
	if($Support::Baseplate::Alignment::Auto) {
		%brick.align();
	}
}

// Performs an Alignment Check on the calling Brick
function fxDTSBrick::doAlignmentCheck(%brick)
{
	// Call the alignment check to make sure we should do it
	%result = %brick.onAlignmentCheck();

	// If the result is strictly false, then do not perform the check
	if(%result $= 0) {
		return true;
	}

	// Perform the alignment check
	%success = %brick.isAligned();

	// Call the success trigger
	if(%success) {
		%brick.onAlignmentPass();
	} else {
		%brick.onAlignmentFail();
	}

	// Return whether or not the check was successful
	return %success;
}

// Returns whether or not the calling Brick is Aligned to the Grid
function fxDTSBrick::isAligned(%brick)
{
	// Determine the Position of the Brick
	%transform = getWords(%brick.getTransform(), 0, 2);

	// Determine the Aligned Position of the Brick
	%alignment = %brick.getAlignment();

	// Check Alignment
	if(%transform !$= %alignment)
		return false;

	// Check for proper size
	%sizeX = %brick.getDatablock().brickSizeX;
	%sizeY = %brick.getDatablock().brickSizeY;

	// Flip Sizes if Necessary
	if(mAbs(mFloatLength(getWord(%brick.rotation, 3), 0)) == 90)
	{
		// Swap Sizes
		%temp = %sizeX;
		%sizeX = %sizeY;
		%sizeY = %temp;
	}

	// Make sure Brick can fill a Cell
	if(%sizeX < $Support::Baseplate::Alignment::Width || %sizeY < $Support::Baseplate::Alignment::Length)
		return false;

	return true;
}

// Moves the calling Brick to the nearest Aligned Position
function fxDTSBrick::align(%brick)
{
	// Align the Brick
	%brick.setTransform(%brick.getAlignment());
}

// /**
//  * Returns the position of the nearest alignment for the calling brick.
//  *
//  * @param  {fxDTSBrick}  %brick  The calling brick.
//  *
//  * @return {string}
//  */
function fxDTSBrick::getAlignment(%brick)
{
	// Determine the grid attributes
	%gridCellWidth = $Support::Baseplate::Alignment::Width;
	%gridCellLength = $Support::Baseplate::Alignment::Length;
	%gridXOffset = $Support::Baseplate::Alignment::XOffset;
	%gridYOffset = $Support::Baseplate::Alignment::YOffset;

	// The global position of bricks is not as neat as one would expect.
	// Every 1 meter counts as 2 blocks laterally. Because of this,
	// we need to scale down our grid to match the global size.

	// Scale the grid
	%gridCellWidth /= 2;
	%gridCellLength /= 2;
	%gridXOffset /= 2;
	%gridYOffset /= 2;

	// Determine the position of the brick
	%transform = %brick.getTransform();

	%x = getWord(%transform, 0);
	%y = getWord(%transform, 1);
	%z = getWord(%transform, 2);

	// Offset the position of the brick
	%x -= %gridXOffset;
	%y -= %gridYOffset;

	// Determine the size of the brick
	%data = %brick.getDatablock();

	%brickWidth = %data.brickSizeX;
	%brickLength = %data.brickSizeY;
	%brickHeight = %data.brickSizeZ;

	// Determine the rotation of the brick
	%rotation = %brick.angleID;

	// Determine the offset to the north west (top left) corner of the brick
	%centerXOffset = ((%rotation == 0 || %rotation == 2) ? %brickWidth : %brickLength) / 2;
	%centerYOffset = ((%rotation == 0 || %rotation == 2) ? %brickLength : %brickWidth) / 2;

	// We the center offset by two above to get from the center to the
	// corner. But again, real world distance is half that of studs.
	// To be accurate, we must half the center offset yet again.

	// Half the center offset
	%centerXOffset /= 2;
	%centerYOffset /= 2;

	// Offset the position to be at the north west (top left) corner of the brick
	%x -= %centerXOffset;
	%y -= %centerYOffset;

	// Use the bottom of the brick for vertical alignment
	// %z -= %brickHeight / 2;

	// Determine the aligned positions
	%xAlign = mFloor(%x / %gridCellWidth) * %gridCellWidth;
	%yAlign = mFloor(%y / %gridCellLength) * %gridCellLength;
	%zAlign = %z;

	// Reset the alignment to the center of the brick
	%xAlign += %centerXOffset;
	%yAlign += %centerYOffset;

	// Remove offset from the position of the brick
	%xAlign += %gridXOffset;
	%yAlign += %gridYOffset;

	// Return the alignment
	return %xAlign SPC %yAlign SPC %zAlign;
}

// /**
//  * Returns size of the calling brick in grid units.
//  *
//  * @param  {fxDTSBrick}  %brick  The calling brick.
//  *
//  * @return {float}
//  */
function fxDTSBrick::getAlignmentGridSize(%brick)
{
	// Determine the size of the brick
	%data = %brick.getDatablock();

	%sizeX = %data.brickSizeX;
	%sizeY = %data.brickSizeY;

	// Determine the rotation of the brick
	%rotation = mFloatLength(getWord(%brick.rotation, 3), 0);

	// Determine the size after rotation
	%brickWidth = mAbs(%rotation) == 90 ? %sizeY : %sizeX;
	%brickLength = mAbs(%rotation) == 90 ? %sizeX : %sizeY;

	// Determine the cell size of the grid
	%gridWidth = $Support::Baseplate::Alignment::Width;
	%gridLength = $Support::Baseplate::Alignment::Length;

	// Determine the brick size in grid units
	%cellWidth = mFloor(%brickWidth / %gridWidth);
	%cellLength = mFloor(%brickLength / %gridLength);

	// Return the size of the brick in grid units
	return %cellWidth SPC %cellLength;
}