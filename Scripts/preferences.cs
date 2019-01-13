//* Description *//
// Title: Preferences
// Author: Boom (9740)
// Defines the preferences for this add-on.

// Check if RTB is enabled
if(isFile("Add-Ons/System_ReturnToBlockland/server.cs")) {

	// Make sure the preferences hook is enabled
	if(!$RTB::Hooks::ServerControl) {
		exec("Add-Ons/System_ReturnToBlockland/hooks/serverControl.cs");
	}

	// Register the RTB preferences
	RTB_registerPref("Baseplate Required", "Baseplate Rules | Baseplates", "$Support::Baseplate::Required", "bool", "Support_BaseplateRules", 1, false, false);
	RTB_registerPref("Baseplate Type", "Baseplate Rules | Baseplates", "$Support::Baseplate::Type", "list Any 1 Plate 2 Plain 3", "Support_BaseplateRules", 1, false, false);

	RTB_registerPref("Use Adjacency",     "Baseplate Rules | Adjacency", "$Support::Baseplate::Adjacency::Enabled",          "bool", "Support_BaseplateRules", 1, false, false);
	RTB_registerPref("Include Diagonals", "Baseplate Rules | Adjacency", "$Support::Baseplate::Adjacency::IncludeDiagonals", "bool", "Support_BaseplateRules", 1, false, false);

	RTB_registerPref("Use Alignment", "Baseplate Rules | Alignment", "$Support::Baseplate::Alignment::Enabled", "bool",       "Support_BaseplateRules", 1,  false, false);
	RTB_registerPref("Auto Align",    "Baseplate Rules | Alignment", "$Support::Baseplate::Alignment::Auto",    "bool",       "Support_BaseplateRules", 1,  false, false);
	RTB_registerPref("Cell Width", 	  "Baseplate Rules | Alignment", "$Support::Baseplate::Alignment::Width",   "int 1 1024", "Support_BaseplateRules", 16, false, false);
	RTB_registerPref("Cell Length",   "Baseplate Rules | Alignment", "$Support::Baseplate::Alignment::Length",  "int 1 1024", "Support_BaseplateRules", 16, false, false);
	RTB_registerPref("Grid X Offset", "Baseplate Rules | Alignment", "$Support::Baseplate::Alignment::XOffset", "int 0 1024", "Support_BaseplateRules", 0,  false, false);
	RTB_registerPref("Grid Y Offset", "Baseplate Rules | Alignment", "$Support::Baseplate::Alignment::YOffset", "int 0 1024", "Support_BaseplateRules", 0,  false, false);

}

// No preference manager is enabled
else {

	// Use Default Preference Values
	$Support::Baseplate::Required = true;
	$Support::Baseplate::Type = 1;

	$Support::Baseplate::Adjacency::Enabled = true;
	$Support::Baseplate::Adjacency::IncludeDiagonals = false;

	$Support::Baseplate::Alignment::Enabled = true;
	$Support::Baseplate::Alignment::Auto = true;
	$Support::Baseplate::Alignment::Width = 16;
	$Support::Baseplate::Alignment::Length = 16;
	$Support::Baseplate::Alignment::XOffset = 0;
	$Support::Baseplate::Alignment::YOffset = 0;

}

// Define the type enumerables system values
$Support::Baseplate::Type::Any = 1;
$Support::Baseplate::Type::Plate = 2;
$Support::Baseplate::Type::Plain = 3;
$Support::Baseplate::TypeMap = "Any 1 Plate 2 Plain 3";

// Define the distinct reason codes
$Support::Baseplate::Reason::InvalidBrick = "Invalid Brick";
$Support::Baseplate::Reason::NotWideEnough = "Not Wide Enough";
$Support::Baseplate::Reason::NotLongEnough = "Not Long Enough";
$Support::Baseplate::Reason::IncorrectType = "Incorrect Type";
$Support::Baseplate::Reason::UnalignedBaseplate = "Unaligned Baseplate";
$Support::Baseplate::Reason::NoBaseplateNeighbor = "No Baseplate Neighbor";