In order to use Asset Loader for a new locomotive, simply create a zip with your Bundle, Catalog.json and Definitions.json files, with the addition of an info.json file for Unity Mod Manager.
File structure, using Greenninja's A-12 4-4-0 as an example:
GN-A12-440_v1.0.0.zip
  -> info.json
  -> Bundle
  -> Catalog.json
  -> Definitions.json

info.json:
{
	"Id": "GN-A12-440",
	"Version": "1.0.0",
	"DisplayName": "GreenNinja2404's A-12 4-4-0",
	"Author": "Greenninja2404",
	"ManagerVersion": "0.27.12",
	"HomePage": "https://www.nexusmods.com/railroader/mods/XXX"	
}

Repalce HomePage with whatever you like, delete the entry or replace XXX with your Nexus mod id.

In order to use Asset Loader for tender swaps, you zip file structure should contain sub-folders for each loco you are providing a Definitions.json fole for. Eg:
CjaneTenderSwap
  -> info.json
  -> ls-060-s23
    -> Definitions.json
  -> ls-080-s51
    -> Definitions.json

The info.json is the same format as above.

Asset Loader will inject your Definitions.json instead of the original when loading an asset with the same name as the folder name.
