# 1.2.0
* **Changes:**
  * Added a unique music track - 'Castle of Memories' by Cane B! It's unreleased at the time of writing this, but you can check out his other work on [YouTube](https://www.youtube.com/@caneb4) and [SoundCloud](https://soundcloud.com/vgmcb)! Thanks to Cane B for letting me use it for the stage!
    * Also added dependency on R2API Sound
  * Starstorm 2: Security Chests can now appear in the stage! (also added a config option to toggle them)
  * Slightly desaturated the sky
  * Removed a bunch of unused assets, which should reduce the mod's file size somewhat
* **Fixes:**
  * Attempted to fix an issue where the ocean could appear to disappear when viewed from certain angles
  * Fixed a few floating ground nodes in the metal platform area
  * Fixed geysers being silent
  * Increased the distance required for willow trees and logs to transition from LOD2 to culled

# 1.1.1
* Adjusted vertical placements for each of the manually placed escape pod spawns to align them with the ground (one in particular was partially submerged, which may or may not have been causing players to spawn under the map)
* Added Colossus to the stage after looping if EnemiesReturns is enabled
  * Also added a config option to toggle Colossus

# 1.1.0
* **Layout changes:**
  * Added a waterfall in the sloped area near the big wooden structure and moved/added a few rocks and pillars surrounding it
    * The waterfall is not present in the Simulacrum version. This is intentional
  * Replaced some props with new metal platforms
  * Added a fence to a ledge while the large wooden building is boarded up to make it clearer that it's a dead end
* **Lighting changes:**
  * Made the map's fog and lighting purple-pink-ish
    * The original lighting was more blue/aqua, which did not feel especially night-time-ish and generally looked kind of dull and flat. This new look's a bit more unique and I think it looks nice but feel free to beat me to death with hammers if it sucks
    * We got Purple Berry, okay? You know we got Purple Berry all day, and it's got those Purple Berries in there. So That's great, man. Ten outta Ten. You can't go wrong with Purple Berry. Tastes just like Purple, man. Tent outta Tent
  * Slightly increased shadow intensity (0.5 -> 0.66)
  * Fixed the Simulacrum variant using the wrong skybox material
* **Other changes:**
  * Doubled the selection weight for every basic monster except Blind Pests, which should in theory make Blind Pests less likely to spawn
  * Wayfarers will now only appear after 1 stage completion
  * Added ground nodes to a small corner where I forgot to put them

# 1.0.0
* Initial Release