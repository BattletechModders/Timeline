# Timeline
BattleTech mod that helps ground the campaign into the timeline of the universe.

* Pops up a timeline selection event or uses the default starting data (set in SimGameConstants)
* Allows for limiting mechs to an appearance date by using `MinAppearanceDate` in the MechDef
  * `"MinAppearanceDate" : "3025-06-28T00:00:00Z",` is used by the Hatchetman in vanilla 1.4
* Displays the date in the event popup and in the game timeline
* Allows for any RequirementDef to contain tags of format `timeline_{year}_{month}_{day}` or `timeline_{year}_{month}` or `timeline_{year}`
  * RequiredTag date has passed
  * ExcludedTag date has not passed

## Requires

[ModTek](https://github.com/BattletechModders/ModTek/releases) -- [installation guide here](https://github.com/BattletechModders/ModTek/wiki/The-Drop-Dead-Simple-Guide-to-Installing-BTML-&-ModTek-&-ModTek-mods).

## Credits

* Zathoth for the [AddYearToTimeline mod](https://github.com/Zathoth/AddYearToTimeline) that served as initial inspiration
* HBS for making a great game

## Future Plans

* Add tags to the company/game to allow for time to unlock content (events/mechs/etc.)
* Allow for star systems to change over time based on the BattleTech universe
