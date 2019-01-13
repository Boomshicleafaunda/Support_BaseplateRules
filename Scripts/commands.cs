//* Description *//
// Title: Commands
// Author: Boom (9740)
// Defines the commands for this add-on.

/////////////////////////////
//* Full List of Commands *//
/////////////////////////////

// "/baseplaterules"
// "/baseplaterules <enable|disable>"
// "/baseplaterules commands"
// "/baseplaterules type"
// "/baseplaterules type set <type>"
// "/baseplaterules adjacency"
// "/baseplaterules adjacency <enable|disable>"
// "/baseplaterules adjacency diagonals"
// "/baseplaterules adjacency diagonals <enable|disable>"
// "/baseplaterules alignment"
// "/baseplaterules alignment <enable|disable>"
// "/baseplaterules alignment auto"
// "/baseplaterules alignment auto <enable|disable>"
// "/baseplaterules alignment grid"
// "/baseplaterules alignment grid set <X> <Y>"
// "/baseplaterules alignment offset"
// "/baseplaterules alignment offset set <X> <Y>"

////////////////////
//* Base Command *//
////////////////////

// /**
//  * Triggered when the calling client uses the "/baseplaterules" command
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  * @param  {string}          %arg0    The first argument.
//  * @param  {string}          %arg1    The second argument.
//  * @param  {string}          %arg2    The third argument.
//  * @param  {string}          %arg3    The fourth argument.
//  * @param  {string}          %arg4    The fifth argument.
//  *
//  * @return {void}
//  */
function serverCmdBaseplaterules(%client, %arg0, %arg1, %arg2, %arg3, %arg4)
{
	return %client.handleBaseplateRulesCommand(%arg0, %arg1, %arg2, %arg3, %arg4);
}

// /**
//  * Handles the specified baseplate rules command from the calling client.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  * @param  {string}          %arg0    The first argument.
//  * @param  {string}          %arg1    The second argument.
//  * @param  {string}          %arg2    The third argument.
//  * @param  {string}          %arg3    The fourth argument.
//  * @param  {string}          %arg4    The fifth argument.
//  *
//  * @return {void}
//  */
function GameConnection::handleBaseplateRulesCommand(%client, %arg0, %arg1, %arg2, %arg3, %arg4)
{
	// If no arguments were provided, display the help text
	if(%arg0 $= "") {
		return %client.showBaseplateRulesHelpMessage();
	}

	// Check the enable or disable command
	switch$(%arg0) {

		// Enable
		case "enable":
			return %client.enableBaseplateRules();

		// Disable
		case "disable":
			return %client.disableBaseplateRules();

	}

	// At this point, all other commands will require baseplate rules to
	// be enabled. We should make sure of that here, so that we do not
	// have to check this within each command. It makes life simple.

	// Make sure the baseplate rules are enabled
	if(!%client.enforceBaseplateRulesAreEnabled()) {
		return;
	}

	// Determine the command type
	switch$(%arg0) {

		// Commands
		case "commands":
			return %client.showBaseplateRulesCommandsListMessage();

		// Type
		case "type":
			return %client.handleBaseplateTypeRuleCommand(%arg1, %arg2);

		// Adjacency
		case "adjacency":
			return %client.handleBaseplateAdjacencyRulesCommand(%arg1, %arg2);

		// Alignment
		case "alignment":
			return %client.handleBaseplateAlignmentRulesCommand(%arg1, %arg2, %arg3, %arg4);

	}

	// Unknown command type
	return %client.showBaseplateRulesUnknownCommandTypeMessage();
}

// /**
//  * Displays the baseplate rules help text to the calling client.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {void}
//  */
function GameConnection::showBaseplateRulesHelpMessage(%client)
{
	// Display the message header
	messageClient(%client, '', "\c7=== \c3Baseplate Rules \c7===");

	// Display the add-on description
	messageClient(%client, '', "\c6Adds \c3Baseplate Alignment\c6 and \c3Adjacency Rules\c6 for building.");

	// Display the status of the individual rules
	%client.showBaseplateRulesListMessage();

	// Enforce that the add-on is enabled
	%client.enforceBaseplateRulesAreEnabled();

	// Display the commands command
	messageClient(%client, '', "\c6Type \c4/baseplaterules commands \c6 to view all commands.");
}

// /**
//  * Displays the current status of the baseplate rules to the calling client.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {void}
//  */
function GameConnection::showBaseplateRulesListMessage(%client)
{
	%client.showBaseplateTypeRuleStatus();
	%client.showBaseplateAdjacencyRulesStatus();
	%client.showBaseplateAlignmentRulesStatus();
}

// /**
//  * Enables the baseplate rules.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {void}
//  */
function GameConnection::enableBaseplateRules(%client)
{
	// Make sure the client is able to enable the baseplate rules
	if(!%client.canToggleBaseplateRules()) {
		return messageClient(%client, '', "You are not allowed to use this command.");
	}

	// Make sure the baseplate rules are not already enabled
	if($Support::Baseplate::Required == 1) {
		return messageClient(%client, '', "\c3Baseplate Rules \c6are already \c2enabled\c6.");
	}

	// Enable the baseplate rules
	$Support::Baseplate::Required = 1;

	// Display the success message
	messageAll('MsgAdminForce', "\c3Baseplate Rules \c6have been \c2enabled\c6.", %client.name);
}

// /**
//  * Disables the baseplate rules.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {void}
//  */
function GameConnection::disableBaseplateRules(%client)
{
	// Make sure the client is able to disable the baseplate rules
	if(!%client.canToggleBaseplateRules()) {
		return messageClient(%client, '', "You are not allowed to use this command.");
	}

	// Make sure the baseplate rules are not already disabled
	if($Support::Baseplate::Required == 0) {
		return messageClient(%client, '', "\c3Baseplate Rules \c6are already \c2disabled\c6.");
	}

	// Enable the baseplate rules
	$Support::Baseplate::Required = 0;

	// Display the success message
	messageAll('MsgAdminForce', "\c3Baseplate Rules \c6have been \c0disabled\c6.", %client.name);
}

// /**
//  * Returns whether or not the calling client can disable baseplate rules.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {boolean}
//  */
function GameConnection::canToggleBaseplateRules(%client)
{
	return %client.canUpdateBaseplateRulesPreference("required");
}

// /**
//  * Returns whether or not the calling client update the specified baseplate rules preference.
//  *
//  * @param  {GameConnection}  %client      The calling client.
//  * @param  {string}          %preference  The specified preference.
//  *
//  * @return {boolean}
//  */
function GameConnection::canUpdateBaseplateRulesPreference(%client, %preference)
{
	return %client.isAdmin;
}

// /**
//  * Enforces that the baseplate rules are enabled.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {boolean}
//  */
function GameConnection::enforceBaseplateRulesAreEnabled(%client)
{
	// If baseplate rules are enabled, return success
	if($Support::Baseplate::Required) {
		return true;
	}

	// Tell them that baseplate rules are currently disabled
	messageClient(%client, '', "\c3Baseplate Rules \c6are currently \c0disabled\c6.");

	// If the client is able to enable baseplate rules, tell them how
	if(%client.canToggleBaseplateRules()) {
		messageClient(%client, '', "\c6Use the \c4/baseplaterules enable\c6 command to turn them on.");
	}

	// Return failure
	return false;
}

// /**
//  * Displays the baseplate rules unknown command type message to the calling client.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {void}
//  */
function GameConnection::showBaseplateRulesUnknownCommandTypeMessage(%client)
{
	// Display error message
	messageClient(%client, '', "\c0That is not a valid baseplate rule command. Please use one of the following:");

	// List the commands
	return %client.showBaseplateRulesCommandsListMessage();
}

// /**
//  * Displays the baseplate rules commands to the calling client.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {void}
//  */
function GameConnection::showBaseplateRulesCommandsListMessage(%client)
{
	// List the enable/disable command if allowed
	if(%client.canToggleBaseplateRules()) {
		messageClient(%client, '', "\c4/baseplaterules \c7[enable|disable]");
	}

	// List the type command
	if(%client.canUpdateBaseplateTypeRule()) {
		messageClient(%client, '', "\c4/baseplaterules \c6type set \c7[type]");
	} else {
		messageClient(%client, '', "\c4/baseplaterules \c6type");
	}

	// List the adjacency command
	if(%client.canToggleBaseplateAdjacencyRules()) {
		messageClient(%client, '', "\c4/baseplaterules \c6adjacency \c7[enable|disable]");
	} else {
		messageClient(%client, '', "\c4/baseplaterules \c6adjacency");
	}

	// List the adjacency diagonals command
	if(%client.canToggleBaseplateAdjacencyDiagonalRule()) {
		messageClient(%client, '', "\c4/baseplaterules \c6adjacency diagonals \c7[enable|disable]");
	} else {
		messageClient(%client, '', "\c4/baseplaterules \c6adjacency diagonals");
	}

	// List the alignment command
	if(%client.canToggleBaseplateAlignmentRules()) {
		messageClient(%client, '', "\c4/baseplaterules \c6alignment \c7[enable|disable]");
	} else {
		messageClient(%client, '', "\c4/baseplaterules \c6alignment");
	}

	// List the alignment auto command
	if(%client.canToggleBaseplateAlignmentAutoRule()) {
		messageClient(%client, '', "\c4/baseplaterules \c6alignment auto \c7[enable|disable]");
	} else {
		messageClient(%client, '', "\c4/baseplaterules \c6alignment auto");
	}

	// List the alignment grid command
	if(%client.canUpdateBaseplateAlignmentGridRule()) {
		messageClient(%client, '', "\c4/baseplaterules \c6alignment grid set \c7[X] [Y]");
	} else {
		messageClient(%client, '', "\c4/baseplaterules \c6alignment grid");
	}

	// List the alignment offset command
	if(%client.canUpdateBaseplateAlignmentOffsetRule()) {
		messageClient(%client, '', "\c4/baseplaterules \c6alignment offset set \c7[X] [Y]");
	} else {
		messageClient(%client, '', "\c4/baseplaterules \c6alignment offset");
	}
}

///////////////////////////
//* Base - Type Command *//
///////////////////////////

// /**
//  * Handles the specified baseplate type rule command from the calling client.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  * @param  {string}          %arg0    The first argument.
//  * @param  {string}          %arg1    The second argument.
//  *
//  * @return {void}
//  */
function GameConnection::handleBaseplateTypeRuleCommand(%client, %arg0, %arg1)
{
	// If the "set" command was not provided, display the help text
	if(%arg0 $= "") {
		return %client.showBaseplateTypeHelpMessage();
	}

	// If a non "set" command was provided, display the commands
	if(%arg0 !$= "set") {
		return %client.showBaseplateRulesUnknownCommandTypeMessage();
	}

	// Set the baseplate type
	return %client.setBaseplateTypeRule(%arg1);
}

// /**
//  * Displays the baseplate rules help text to the calling client.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {void}
//  */
function GameConnection::showBaseplateTypeHelpMessage(%client)
{
	// Display the message header
	messageClient(%client, '', "\c7=== \c3Baseplate Type \c7===");

	// Display the add-on description
	messageClient(%client, '', "\c6There are three different types of baseplates:");
	%client.showBaseplateTypeListMessage();
}

// /**
//  * Displays the valid baseplate types to the calling client.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {void}
//  */
function GameConnection::showBaseplateTypeListMessage(%client)
{
	messageClient(%client, '', "\c7 (1) \c5Any\c6: Any brick from the baseplate category.");
	messageClient(%client, '', "\c7 (2) \c5Plate\c6: Any 1/3x tall plate brick from the baseplate category.");
	messageClient(%client, '', "\c7 (3) \c5Plain\c6: Any brick from the Plain baseplate subcategory.");
}

// /**
//  * Sets the baseplate type rule.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  * @param  {string}          %type    The new baseplate type to be required.
//  *
//  * @return {void}
//  */
function GameConnection::setBaseplateTypeRule(%client, %type)
{
	// Make sure the client is able to set the baseplate type rule
	if(!%client.canUpdateBaseplateTypeRule()) {
		return messageClient(%client, '', "You are not allowed to use this command.");
	}

	// The specified type may be the display value of the type, which we
	// cannot use for the preference. We must instead convert the type
	// to its system value, before we then set it to the preference.

	// Determine the baseplate type system value
	%value = %client.getBaseplateTypeMapSystemValue(%type);

	// Make sure the baseplate type is supported
	if(%value < 0) {
		return %client.showInvalidBaseplateTypeCommandMessage(%type);
	}

	// Make sure the baseplate type rule will change
	if($Support::Baseplate::Type == %value) {
		return messageClient(%client, '', "\c6The required \c3Baseplate Type \c6is already \c5" @ %type @ "\c6.");
	}

	// Set the baseplate type rule
	$Support::Baseplate::Type = %value;

	// Display the success message
	messageAll('MsgAdminForce', "\c6The required \c3Baseplate Type \c6has been changed to \c5" @ %type @ "\c6.", %client.name);
}

// /**
//  * Returns whether or not the calling client can set the baseplate type rule.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {boolean}
//  */
function GameConnection::canUpdateBaseplateTypeRule(%client)
{
	return %client.canUpdateBaseplateRulesPreference("type");
}

// /**
//  * Returns whether or not the specified type is a valid baseplate type.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  * @param  {string}          %type    The specified type.
//  *
//  * @return {integer}
//  */
function GameConnection::getBaseplateTypeMapIndex(%client, %type)
{
	// Determine the type map
	%map = $Support::Baseplate::TypeMap;

	// Iterate through the words of the type map
	for(%i = 0; %i < getWordCount(%map); %i++) {

		// If the current word matches the type, return the index
		if(getWord(%map, %i) $= %type) {
			return %i;
		}

	}

	// The specified type is not within the type map
	return -1;
}

// /**
//  * Returns the baseplate type map system value for the specified type.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  * @param  {string}          %type    The specified type.
//  *
//  * @return {integer}
//  */
function GameConnection::getBaseplateTypeMapSystemValue(%client, %type)
{
	// Determine the type map index
	%index = %client.getBaseplateTypeMapIndex(%type);

	// If the index wasn't found, there is no system value
	if(%index < 0) {
		return -1;
	}

	// The type map is constructed such that every even indexed word,
	// starting at 0, is the display value, and every odd indexed
	// word is the system value. We need the system value here.

	// Check if the index is even
	if(%index % 2 == 0) {

		// Return the word after the type index, which should be the system value
		return getWord($Support::Baseplate::TypeMap, %index + 1);

	}

	// The type is already the system value
	return %type;
}

// /**
//  * Returns the baseplate type map display value for the specified type.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  * @param  {string}          %type    The specified type.
//  *
//  * @return {string}
//  */
function GameConnection::getBaseplateTypeMapDisplayValue(%client, %type)
{
	// If no type was provided, use the current type
	if(%type $= "") {
		%type = $Support::Baseplate::Type;
	}

	// Determine the type map index
	%index = %client.getBaseplateTypeMapIndex(%type);

	// If the index wasn't found, there is no display value
	if(%index < 0) {
		return "";
	}

	// The type map is constructed such that every even indexed word,
	// starting at 0, is the display value, and every odd indexed
	// word is the system value. We need the display value here.

	// Check if the index is odd
	if(%index % 2 == 1) {

		// Return the word before the type index, which should be the display value
		return getWord($Support::Baseplate::TypeMap, %index - 1);

	}

	// The type is already the display value
	return %type;
}

// /**
//  * Displays the error message for providing an invalid baseplate type to the "/baseplaterules type set <type>" command.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  * @param  {string}          %type    The baseplate type the client tried to use.
//  *
//  * @return {void}
//  */
function GameConnection::showInvalidBaseplateTypeCommandMessage(%client, %type)
{
	// Tell them that the type they chose is not valid
	messageClient(%client, '', "\c6\"\c5" @ %type @ "\c6\" is not a valid \c3Baseplate Type\c6. Please use one of the following:");

	// Tell them about the different baseplate types
	%client.showBaseplateTypeListMessage();
}

// /**
//  * Displays the current status of the baseplate type rule to the calling client.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {void}
//  */
function GameConnection::showBaseplateTypeRuleStatus(%client)
{
	// Determine the display value of the baseplate type rule
	%value = %client.getBaseplateTypeMapDisplayValue();

	// Tell them that the type they chose is not valid
	messageClient(%client, '', "\c3Baseplate Type\c6: \c5" @ %value);
}

/////////////////////////
//* Adjacency Command *//
/////////////////////////

// /**
//  * Handles the specified baseplate adjacency rules command from the calling client.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  * @param  {string}          %arg0    The first argument.
//  * @param  {string}          %arg1    The second argument.
//  *
//  * @return {void}
//  */
function GameConnection::handleBaseplateAdjacencyRulesCommand(%client, %arg0, %arg1)
{
	// If no arguments were provided, display the help text
	if(%arg0 $= "") {
		return %client.showBaseplateAdjacencyRulesHelpMessage();
	}

	// Check the enable or disable command
	switch$(%arg0) {

		// Enable
		case "enable":
			return %client.enableBaseplateAdjacencyRules();

		// Disable
		case "disable":
			return %client.disableBaseplateAdjacencyRules();

	}

	// At this point, all other commands will require adjacency rules to
	// be enabled. We should make sure of that here, so that we do not
	// have to check this within each command. It makes life simple.

	// Make sure the baseplate adjacency rules are enabled
	if(!%client.enforceBaseplateAdjacencyRulesAreEnabled()) {
		return;
	}

	// Determine the command type
	switch$(%arg0) {

		// Diagonals
		case "diagonals":
			return %client.handleBaseplateAdjacencyDiagonalRuleCommand(%arg1);

	}

	// Unknown command type
	return %client.showBaseplateRulesUnknownCommandTypeMessage();
}

// /**
//  * Displays the baseplate adjacency rules help text to the calling client.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {void}
//  */
function GameConnection::showBaseplateAdjacencyRulesHelpMessage(%client)
{
	// Display the feature header
	messageClient(%client, '', "\c7=== \c3Baseplate Adjacency Rules \c7===");

	// Display the feature description
	messageClient(%client, '', "\c6Requires baseplates to be placed adjacent (directly next to) one another.");

	// Display the status of the individual rules
	%client.showBaseplateAdjacencyRulesListMessage();

	// Enforce that the feature is enabled
	%client.enforceBaseplateAdjacencyRulesAreEnabled();
}

// /**
//  * Displays the baseplate adjacency rules help text to the calling client.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {void}
//  */
function GameConnection::showBaseplateAdjacencyRulesListMessage(%client)
{
	%client.showBaseplateAdjacencyDiagonalRuleStatus();
}

// /**
//  * Enforces that the baseplate adjacency rules are enabled.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {boolean}
//  */
function GameConnection::enforceBaseplateAdjacencyRulesAreEnabled(%client)
{
	// If baseplate adjacency rules are enabled, return success
	if($Support::Baseplate::Adjacency::Enabled) {
		return true;
	}

	// Tell them that baseplate adjacency rules are currently disabled
	messageClient(%client, '', "\c3Baseplate Adjacency Rules \c6are currently \c0disabled\c6.");

	// If the client is able to enable baseplate adjacency rules, tell them how
	if(%client.canToggleBaseplateAdjacencyRules()) {
		messageClient(%client, '', "\c6Use the \c4/baseplaterules adjacency enable\c6 command to turn them on.");
	}

	// Return failure
	return false;
}

// /**
//  * Enables the baseplate adjacency rules.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {void}
//  */
function GameConnection::enableBaseplateAdjacencyRules(%client)
{
	// Make sure the client is able to enable the baseplate adjacency rules
	if(!%client.canToggleBaseplateAdjacencyRules()) {
		return messageClient(%client, '', "You are not allowed to use this command.");
	}

	// Make sure the baseplate adjacency rules are not already enabled
	if($Support::Baseplate::Adjacency::Enabled == 1) {
		return messageClient(%client, '', "\c3Baseplate Adjacency Rules \c6are already \c2enabled\c6.");
	}

	// Enable the baseplate adjacency rules
	$Support::Baseplate::Adjacency::Enabled = 1;

	// Display the success message
	messageAll('MsgAdminForce', "\c3Baseplate Adjacency Rules \c6have been \c2enabled\c6.", %client.name);
}

// /**
//  * Returns whether or not the calling client can enable baseplate adjacency rules.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {boolean}
//  */
function GameConnection::canToggleBaseplateAdjacencyRules(%client)
{
	return %client.canUpdateBaseplateRulesPreference("adjacency");
}

// /**
//  * Disables the baseplate adjacency rules.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {void}
//  */
function GameConnection::disableBaseplateAdjacencyRules(%client)
{
	// Make sure the client is able to disable the baseplate adjacency rules
	if(!%client.canToggleBaseplateAdjacencyRules()) {
		return messageClient(%client, '', "You are not allowed to use this command.");
	}

	// Make sure the baseplate adjacency rules are not already disabled
	if($Support::Baseplate::Adjacency::Enabled == 0) {
		return messageClient(%client, '', "\c3Baseplate Adjacency Rules \c6are already \c2disabled\c6.");
	}

	// Enable the baseplate adjacency rules
	$Support::Baseplate::Adjacency::Enabled = 0;

	// Display the success message
	messageAll('MsgAdminForce', "\c3Baseplate Adjacency Rules \c6have been \c0disabled\c6.", %client.name);
}

// /**
//  * Displays the current status of the baseplate adjacency rules to the calling client.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {boolean}
//  */
function GameConnection::showBaseplateAdjacencyRulesStatus(%client)
{
	if($Support::Baseplate::Adjacency::Enabled) {
		messageClient(%client, '', "\c3Adjacency\c6: \c2Enabled");
	} else {
		messageClient(%client, '', "\c3Adjacency\c6: \c0Disabled");
	}
}

////////////////////////////////////
//* Adjacency - Diagonal Command *//
////////////////////////////////////

// /**
//  * Handles the specified baseplate adjacency diagonal rule command from the calling client.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  * @param  {string}          %arg0    The first argument.
//  *
//  * @return {void}
//  */
function GameConnection::handleBaseplateAdjacencyDiagonalRuleCommand(%client, %arg0)
{
	// If the no command was provided, display the help text
	if(%arg0 $= "") {
		return %client.showBaseplateAdjacencyDiagonalRuleHelpMessage();
	}

	// Determine the command type
	switch$(%arg0) {

		// Enable
		case "enable":
			return %client.enableBaseplateAdjacencyDiagonalRule();

		// Disable
		case "disable":
			return %client.disableBaseplateAdjacencyDiagonalRule();

	}

	// Unknown command type
	return %client.showBaseplateRulesUnknownCommandTypeMessage();
}

// /**
//  * Displays the baseplate adjacency rules help text to the calling client.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {void}
//  */
function GameConnection::showBaseplateAdjacencyDiagonalRuleHelpMessage(%client)
{
	// Display the feature header
	messageClient(%client, '', "\c7=== \c3Baseplate Adjacency Diagonal Rule \c7===");

	// Display the feature description
	messageClient(%client, '', "\c6Allows baseplates positioned diagonally to each other to be considered as adjacent.");

	// Enforce that the feature is enabled
	%client.enforceBaseplateAdjacencyDiagonalRuleIsEnabled();
}

// /**
//  * Enforces that the baseplate adjacency diagonal rule is enabled.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {boolean}
//  */
function GameConnection::enforceBaseplateAdjacencyDiagonalRuleIsEnabled(%client)
{
	// If baseplate adjacency diagonal rule is enabled, return success
	if($Support::Baseplate::Adjacency::IncludeDiagonals) {
		return true;
	}

	// Tell them the baseplate adjacency diagonal rule is currently disabled
	messageClient(%client, '', "\c3Baseplate Adjacency Diagonal Rule \c6is currently \c0disabled\c6.");

	// If the client is able to enable the baseplate adjacency diagonal rule, tell them how
	if(%client.canToggleBaseplateAdjacencyDiagonalRule()) {
		messageClient(%client, '', "\c6Use the \c4/baseplaterules adjacency diagonals enable\c6 command to turn it on.");
	}

	// Return failure
	return false;
}

// /**
//  * Enables the baseplate adjacency diagonal rule.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {void}
//  */
function GameConnection::enableBaseplateAdjacencyDiagonalRule(%client)
{
	// Make sure the client is able to enable the baseplate adjacency diagonal rule
	if(!%client.canToggleBaseplateAdjacencyDiagonalRule()) {
		return messageClient(%client, '', "You are not allowed to use this command.");
	}

	// Make sure the baseplate adjacency diagonal rule is not already enabled
	if($Support::Baseplate::Adjacency::IncludeDiagonals == 1) {
		return messageClient(%client, '', "\c3Baseplate Adjacency Diagonal Rule \c6is already \c2enabled\c6.");
	}

	// Enable the baseplate adjacency diagonal rule
	$Support::Baseplate::Adjacency::IncludeDiagonals = 1;

	// Display the success message
	messageAll('MsgAdminForce', "\c3Baseplate Adjacency Diagonal Rule \c6has been \c2enabled\c6.", %client.name);
}

// /**
//  * Returns whether or not the calling client can enable the baseplate adjacency diagonal rule.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {boolean}
//  */
function GameConnection::canToggleBaseplateAdjacencyDiagonalRule(%client)
{
	return %client.canUpdateBaseplateRulesPreference("adjacency_diagonals");
}

// /**
//  * Disables the baseplate adjacency diagonal rule.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {void}
//  */
function GameConnection::disableBaseplateAdjacencyDiagonalRule(%client)
{
	// Make sure the client is able to disable the baseplate adjacency diagonal rule
	if(!%client.canToggleBaseplateAdjacencyDiagonalRule()) {
		return messageClient(%client, '', "You are not allowed to use this command.");
	}

	// Make sure the baseplate adjacency diagonal rule is not already disabled
	if($Support::Baseplate::Adjacency::IncludeDiagonals == 0) {
		return messageClient(%client, '', "\c3Baseplate Adjacency Diagonal Rule \c6is already \c2disabled\c6.");
	}

	// Enable the baseplate adjacency diagonal rule
	$Support::Baseplate::Adjacency::IncludeDiagonals = 0;

	// Display the success message
	messageAll('MsgAdminForce', "\c3Baseplate Adjacency Diagonal Rule \has been \c0disabled\c6.", %client.name);
}

// /**
//  * Displays the current status of the baseplate adjacency diagonal rule to the calling client.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {boolean}
//  */
function GameConnection::showBaseplateAdjacencyDiagonalRuleStatus(%client)
{
	if($Support::Baseplate::Adjacency::IncludeDiagonals) {
		messageClient(%client, '', "\c3Diagonals\c6: \c2Enabled");
	} else {
		messageClient(%client, '', "\c3Diagonals\c6: \c0Disabled");
	}
}

/////////////////////////
//* Alignment Command *//
/////////////////////////

// /**
//  * Handles the specified baseplate alignment rules command from the calling client.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  * @param  {string}          %arg0    The first argument.
//  * @param  {string}          %arg1    The second argument.
//  * @param  {string}          %arg2    The third argument.
//  * @param  {string}          %arg4    The fourth argument.
//  *
//  * @return {void}
//  */
function GameConnection::handleBaseplateAlignmentRulesCommand(%client, %arg0, %arg1, %arg2, %arg3)
{
	// If no arguments were provided, display the help text
	if(%arg0 $= "") {
		return %client.showBaseplateAlignmentRulesHelpMessage();
	}

	// Check the enable or disable command
	switch$(%arg0) {

		// Enable
		case "enable":
			return %client.enableBaseplateAlignmentRules();

		// Disable
		case "disable":
			return %client.disableBaseplateAlignmentRules();

	}

	// At this point, all other commands will require alignment rules to
	// be enabled. We should make sure of that here, so that we do not
	// have to check this within each command. It makes life simple.

	// Make sure the baseplate alignment rules are enabled
	if(!%client.enforceBaseplateAlignmentRulesAreEnabled()) {
		return;
	}

	// Determine the command type
	switch$(%arg0) {

		// Auto
		case "auto":
			return %client.handleBaseplateAlignmentAutoRuleCommand(%arg1);

		// Grid
		case "grid":
			return %client.handleBaseplateAlignmentGridRuleCommand(%arg1, %arg2, %arg3);

		// Offset
		case "offset":
			return %client.handleBaseplateAlignmentOffsetRuleCommand(%arg1, %arg2, %arg3);

	}

	// Unknown command type
	return %client.showBaseplateRulesUnknownCommandTypeMessage();
}

// /**
//  * Displays the baseplate alignment rules help text to the calling client.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {void}
//  */
function GameConnection::showBaseplateAlignmentRulesHelpMessage(%client)
{
	// Display the feature header
	messageClient(%client, '', "\c7=== \c3Baseplate Alignment Rules \c7===");

	// Display the feature description
	messageClient(%client, '', "\c6Requires baseplates to be placed in alignment with a grid.");

	// Display the status of the individual rules
	%client.showBaseplateAlignmentRulesListMessage();

	// Enforce that the feature is enabled
	%client.enforceBaseplateAlignmentRulesAreEnabled();
}

// /**
//  * Displays the baseplate alignment rules help text to the calling client.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {void}
//  */
function GameConnection::showBaseplateAlignmentRulesListMessage(%client)
{
	%client.showBaseplateAlignmentAutoRuleStatus();
	%client.showBaseplateAlignmentGridRuleStatus();
	%client.showBaseplateAlignmentOffsetRuleStatus();
}

// /**
//  * Enforces that the baseplate alignment rules are enabled.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {boolean}
//  */
function GameConnection::enforceBaseplateAlignmentRulesAreEnabled(%client)
{
	// If baseplate alignment rules are enabled, return success
	if($Support::Baseplate::Alignment::Enabled) {
		return true;
	}

	// Tell them that baseplate alignment rules are currently disabled
	messageClient(%client, '', "\c3Baseplate Alignment Rules \c6are currently \c0disabled\c6.");

	// If the client is able to enable baseplate alignment rules, tell them how
	if(%client.canToggleBaseplateAlignmentRules()) {
		messageClient(%client, '', "\c6Use the \c4/baseplaterules alignment enable\c6 command to turn them on.");
	}

	// Return failure
	return false;
}

// /**
//  * Enables the baseplate alignment rules.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {void}
//  */
function GameConnection::enableBaseplateAlignmentRules(%client)
{
	// Make sure the client is able to enable the baseplate alignment rules
	if(!%client.canToggleBaseplateAlignmentRules()) {
		return messageClient(%client, '', "You are not allowed to use this command.");
	}

	// Make sure the baseplate alignment rules are not already enabled
	if($Support::Baseplate::Alignment::Enabled == 1) {
		return messageClient(%client, '', "\c3Baseplate Alignment Rules \c6are already \c2enabled\c6.");
	}

	// Enable the baseplate alignment rules
	$Support::Baseplate::Alignment::Enabled = 1;

	// Display the success message
	messageAll('MsgAdminForce', "\c3Baseplate Alignment Rules \c6have been \c2enabled\c6.", %client.name);
}

// /**
//  * Returns whether or not the calling client can enable baseplate alignment rules.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {boolean}
//  */
function GameConnection::canToggleBaseplateAlignmentRules(%client)
{
	return %client.canUpdateBaseplateRulesPreference("alignment");
}

// /**
//  * Disables the baseplate alignment rules.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {void}
//  */
function GameConnection::disableBaseplateAlignmentRules(%client)
{
	// Make sure the client is able to disable the baseplate alignment rules
	if(!%client.canToggleBaseplateAlignmentRules()) {
		return messageClient(%client, '', "You are not allowed to use this command.");
	}

	// Make sure the baseplate alignment rules are not already disabled
	if($Support::Baseplate::Alignment::Enabled == 0) {
		return messageClient(%client, '', "\c3Baseplate Alignment Rules \c6are already \c2disabled\c6.");
	}

	// Enable the baseplate alignment rules
	$Support::Baseplate::Alignment::Enabled = 0;

	// Display the success message
	messageAll('MsgAdminForce', "\c3Baseplate Alignment Rules \c6have been \c0disabled\c6.", %client.name);
}

// /**
//  * Displays the current status of the baseplate alignment rules to the calling client.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {boolean}
//  */
function GameConnection::showBaseplateAlignmentRulesStatus(%client)
{
	if($Support::Baseplate::Alignment::Enabled) {
		messageClient(%client, '', "\c3Alignment\c6: \c2Enabled");
	} else {
		messageClient(%client, '', "\c3Alignment\c6: \c0Disabled");
	}
}

////////////////////////////////
//* Adjacency - Auto Command *//
////////////////////////////////

// /**
//  * Handles the specified baseplate alignment auto rule command from the calling client.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  * @param  {string}          %arg0    The first argument.
//  *
//  * @return {void}
//  */
function GameConnection::handleBaseplateAlignmentAutoRuleCommand(%client, %arg0)
{
	// If the no command was provided, display the help text
	if(%arg0 $= "") {
		return %client.showBaseplateAlignmentAutoRuleHelpMessage();
	}

	// Determine the command type
	switch$(%arg0) {

		// Enable
		case "enable":
			return %client.enableBaseplateAlignmentAutoRule();

		// Disable
		case "disable":
			return %client.disableBaseplateAlignmentAutoRule();

	}

	// Unknown command type
	return %client.showBaseplateRulesUnknownCommandTypeMessage();
}

// /**
//  * Displays the baseplate alignment rules help text to the calling client.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {void}
//  */
function GameConnection::showBaseplateAlignmentAutoRuleHelpMessage(%client)
{
	// Display the feature header
	messageClient(%client, '', "\c7=== \c3Baseplate Alignment Auto Rule \c7===");

	// Display the feature description
	messageClient(%client, '', "\c6Automatically aligns baseplates to the grid.");

	// Enforce that the feature is enabled
	%client.enforceBaseplateAlignmentAutoRuleIsEnabled();
}

// /**
//  * Enforces that the baseplate alignment auto rule is enabled.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {boolean}
//  */
function GameConnection::enforceBaseplateAlignmentAutoRuleIsEnabled(%client)
{
	// If baseplate alignment auto rule is enabled, return success
	if($Support::Baseplate::Alignment::Auto) {
		return true;
	}

	// Tell them the baseplate alignment auto rule is currently disabled
	messageClient(%client, '', "\c3Baseplate Alignment Auto Rule \c6is currently \c0disabled\c6.");

	// If the client is able to enable the baseplate alignment auto rule, tell them how
	if(%client.canToggleBaseplateAlignmentAutoRule()) {
		messageClient(%client, '', "\c6Use the \c4/baseplaterules alignment auto enable\c6 command to turn it on.");
	}

	// Return failure
	return false;
}

// /**
//  * Enables the baseplate alignment auto rule.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {void}
//  */
function GameConnection::enableBaseplateAlignmentAutoRule(%client)
{
	// Make sure the client is able to enable the baseplate alignment auto rule
	if(!%client.canToggleBaseplateAlignmentAutoRule()) {
		return messageClient(%client, '', "You are not allowed to use this command.");
	}

	// Make sure the baseplate alignment auto rule is not already enabled
	if($Support::Baseplate::Alignment::Auto == 1) {
		return messageClient(%client, '', "\c3Baseplate Alignment Auto Rule \c6is already \c2enabled\c6.");
	}

	// Enable the baseplate alignment auto rule
	$Support::Baseplate::Alignment::Auto = 1;

	// Display the success message
	messageAll('MsgAdminForce', "\c3Baseplate Alignment Auto Rule \c6has been \c2enabled\c6.", %client.name);
}

// /**
//  * Returns whether or not the calling client can enable the baseplate alignment auto rule.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {boolean}
//  */
function GameConnection::canToggleBaseplateAlignmentAutoRule(%client)
{
	return %client.canUpdateBaseplateRulesPreference("alignment_auto");
}

// /**
//  * Disables the baseplate alignment auto rule.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {void}
//  */
function GameConnection::disableBaseplateAlignmentAutoRule(%client)
{
	// Make sure the client is able to disable the baseplate alignment auto rule
	if(!%client.canToggleBaseplateAlignmentAutoRule()) {
		return messageClient(%client, '', "You are not allowed to use this command.");
	}

	// Make sure the baseplate alignment auto rule is not already disabled
	if($Support::Baseplate::Alignment::Auto == 0) {
		return messageClient(%client, '', "\c3Baseplate Alignment Auto Rule \c6is already \c2disabled\c6.");
	}

	// Enable the baseplate alignment auto rule
	$Support::Baseplate::Alignment::Auto = 0;

	// Display the success message
	messageAll('MsgAdminForce', "\c3Baseplate Alignment Auto Rule \c6has been \c0disabled\c6.", %client.name);
}

// /**
//  * Displays the current status of the baseplate alignment auto rule to the calling client.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {boolean}
//  */
function GameConnection::showBaseplateAlignmentAutoRuleStatus(%client)
{
	if($Support::Baseplate::Alignment::Auto) {
		messageClient(%client, '', "\c3Auto\c6: \c2Enabled");
	} else {
		messageClient(%client, '', "\c3Auto\c6: \c0Disabled");
	}
}

////////////////////////////////
//* Adjacency - Grid Command *//
////////////////////////////////

// /**
//  * Handles the specified baseplate alignment grid rule command from the calling client.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  * @param  {string}          %arg0    The first argument.
//  * @param  {string}          %arg1    The second argument.
//  * @param  {string}          %arg2    The third argument.
//  *
//  * @return {void}
//  */
function GameConnection::handleBaseplateAlignmentGridRuleCommand(%client, %arg0, %arg1, %arg2)
{
	// If the no command was provided, display the help text
	if(%arg0 $= "") {
		return %client.showBaseplateAlignmentGridRuleHelpMessage();
	}

	// Determine the command type
	switch$(%arg0) {

		// Set
		case "set":
			return %client.setBaseplateAlignmentGridRule(%arg1, %arg2);

	}

	// Unknown command type
	return %client.showBaseplateRulesUnknownCommandTypeMessage();
}

// /**
//  * Displays the baseplate alignment rules help text to the calling client.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {void}
//  */
function GameConnection::showBaseplateAlignmentGridRuleHelpMessage(%client)
{
	// Display the feature header
	messageClient(%client, '', "\c7=== \c3Baseplate Alignment Grid Rule \c7===");

	// Determine the grid cell size
	%width = $Support::Baseplate::Alignment::Width;
	%length = $Support::Baseplate::Alignment::Length;

	// Display the feature description
	messageClient(%client, '', "\c6Baseplates must align to the \c5" @ %width @ "x" @ %length @ "\c6 grid.");
}

// /**
//  * Sets the baseplate alignment grid size.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {void}
//  */
function GameConnection::setBaseplateAlignmentGridRule(%client, %width, %length)
{
	// Make sure the client is able to enable the baseplate alignment grid rule
	if(!%client.canUpdateBaseplateAlignmentGridRule()) {
		return messageClient(%client, '', "You are not allowed to use this command.");
	}

	// Make sure the baseplate alignment grid is going to change
	if(%width $= $Support::Baseplate::Alignment::Width && %length $= $Support::Baseplate::Alignment::Length) {
		return messageClient(%client, '', "\c3Baseplate Alignment Grid \c6is already \c5" @ %width @ "x" @ %length @ "\c6.");
	}

	// Make sure the width is a valid integer between 1 and 1024
	if(%width $= "" || mFloatLength(%width, 0) !$= %width || %width < 1 || %width > 1024) {
		return messageClient(%client, '', "The width must be a valid integer between 1 and 1024.");
	}

	// Make sure the length is a valid integer between 1 and 1024
	if(%length $= "" || mFloatLength(%length, 0) !$= %length || %length < 1 || %length > 1024) {
		return messageClient(%client, '', "The length must be a valid integer between 1 and 1024.");
	}

	// Update the baseplate alignment grid size
	$Support::Baseplate::Alignment::Width = %width;
	$Support::Baseplate::Alignment::Length = %length;

	// Display the success message
	messageAll('MsgAdminForce', "\c3Baseplate Alignment Grid \c6has been updated to \c5" @ %width @ "x" @ %length @ "\c6.", %client.name);
}

// /**
//  * Returns whether or not the calling client can enable the baseplate alignment grid rule.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {boolean}
//  */
function GameConnection::canUpdateBaseplateAlignmentGridRule(%client)
{
	return %client.canUpdateBaseplateRulesPreference("alignment_grid");
}

// /**
//  * Displays the current status of the baseplate alignment grid rule to the calling client.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {boolean}
//  */
function GameConnection::showBaseplateAlignmentGridRuleStatus(%client)
{
	// Determine the grid cell size
	%width = $Support::Baseplate::Alignment::Width;
	%length = $Support::Baseplate::Alignment::Length;

	// Display the feature description
	messageClient(%client, '', "\c3Grid\c6: \c5" @ %width @ "x" @ %length);
}

//////////////////////////////////
//* Adjacency - Offset Command *//
//////////////////////////////////

// /**
//  * Handles the specified baseplate alignment offset rule command from the calling client.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  * @param  {string}          %arg0    The first argument.
//  * @param  {string}          %arg1    The second argument.
//  * @param  {string}          %arg2    The third argument.
//  *
//  * @return {void}
//  */
function GameConnection::handleBaseplateAlignmentOffsetRuleCommand(%client, %arg0, %arg1, %arg2)
{
	// If the no command was provided, display the help text
	if(%arg0 $= "") {
		return %client.showBaseplateAlignmentOffsetRuleHelpMessage();
	}

	// Determine the command type
	switch$(%arg0) {

		// Set
		case "set":
			return %client.setBaseplateAlignmentOffsetRule(%arg1, %arg2);

	}

	// Unknown command type
	return %client.showBaseplateRulesUnknownCommandTypeMessage();
}

// /**
//  * Displays the baseplate alignment rules help text to the calling client.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {void}
//  */
function GameConnection::showBaseplateAlignmentOffsetRuleHelpMessage(%client)
{
	// Display the feature header
	messageClient(%client, '', "\c7=== \c3Baseplate Alignment Offset Rule \c7===");

	// Determine the offset size
	%xoffset = $Support::Baseplate::Alignment::XOffset;
	%yoffset = $Support::Baseplate::Alignment::YOffset;

	// Display the feature description
	messageClient(%client, '', "\c6Baseplates must align to the grid offset by \c5" @ %xoffset @ "x" @ %yoffset @ "\c6.");
}

// /**
//  * Sets the baseplate alignment offset size.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {void}
//  */
function GameConnection::setBaseplateAlignmentOffsetRule(%client, %xoffset, %yoffset)
{
	// Make sure the client is able to enable the baseplate alignment offset rule
	if(!%client.canUpdateBaseplateAlignmentOffsetRule()) {
		return messageClient(%client, '', "You are not allowed to use this command.");
	}

	// Make sure the baseplate alignment offset is going to change
	if(%xoffset $= $Support::Baseplate::Alignment::XOffset && %yoffset $= $Support::Baseplate::Alignment::YOffset) {
		return messageClient(%client, '', "\c3Baseplate Alignment Offset \c6is already \c5" @ %xoffset @ "x" @ %yoffset @ "\c6.");
	}

	// Make sure the xoffset is a valid integer between 0 and 1024
	if(%xoffset $= "" || mFloatLength(%xoffset, 0) !$= %xoffset || %xoffset < 0 || %xoffset > 1024) {
		return messageClient(%client, '', "The x-offset must be a valid integer between 0 and 1024.");
	}

	// Make sure the yoffset is a valid integer between 0 and 1024
	if(%yoffset $= "" || mFloatLength(%yoffset, 0) !$= %yoffset || %yoffset < 0 || %yoffset > 1024) {
		return messageClient(%client, '', "The y-offset must be a valid integer between 0 and 1024.");
	}

	// Update the baseplate alignment offset size
	$Support::Baseplate::Alignment::XOffset = %xoffset;
	$Support::Baseplate::Alignment::YOffset = %yoffset;

	// Display the success message
	messageAll('MsgAdminForce', "\c3Baseplate Alignment Offset \c6has been updated to \c5" @ %xoffset @ "x" @ %yoffset @ "\c6.", %client.name);
}

// /**
//  * Returns whether or not the calling client can enable the baseplate alignment offset rule.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {boolean}
//  */
function GameConnection::canUpdateBaseplateAlignmentOffsetRule(%client)
{
	return %client.canUpdateBaseplateRulesPreference("alignment_offset");
}

// /**
//  * Displays the current status of the baseplate alignment offset rule to the calling client.
//  *
//  * @param  {GameConnection}  %client  The calling client.
//  *
//  * @return {boolean}
//  */
function GameConnection::showBaseplateAlignmentOffsetRuleStatus(%client)
{
	// Determine the offset size
	%xoffset = $Support::Baseplate::Alignment::XOffset;
	%yoffset = $Support::Baseplate::Alignment::YOffset;

	// Display the feature description
	messageClient(%client, '', "\c3Offset\c6: \c5" @ %xoffset @ "x" @ %yoffset);
}