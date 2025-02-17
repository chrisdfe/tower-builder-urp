# TODO

## currently:

- [ ] rooms that aren't resizable but don't build a wall between itself and another room of its type

  - [ ] Room 'groups'
  - [ ] eg entrance/exit, lobby, stairs

- [ ] routing for occupants

  - [x] ability to tell a occupant to go here or there

- [ ] add ability to change windows etc of rooms to inspect tool

- [ ] divide tiles up into 'subtiles' for occupants to stand on. maybe 3-5 subtiles per tile
  - [ ] once no more subtiles are free, room is "full"
  - [ ] the more occupant per tile the more 'cramped' (ultimately)

## next:

- [ ] Resizable rooms
  - [ ] It can just be rooms merge when rooms of the same type are placed next to each other for now
- [ ] destroy rooms on a per-block basis
- [ ] BUG: the UI scales weirdly when I resize the window
- [ ] time
- [ ] transportation items
  - [ ] each should have 1+ 'entrance' and 'exit'
- [ ] schedules for residents
- [ ] More specific info in inspect view
- [ ] 'undeletable' rooms (starting entrance/exit)
- [ ] center blueprint tile to cursor
  - [ ] I could refactor/reuse room.GetInspectTargetOrigin for this
- [ ] tooltip
  - [ ] tooltip for resident name etc
- [ ] cleanup: think about just calling cameraController._ or tooltipController._ directly instead of using delegates
- [ ] sfx/music system (controller)
- [ ] room validator: stairs must be fully on top of other rooms
- [ ] room validator: only x of a room allowed per building
- [ ] room validator: stairs should be able to overlap on the bottom/top but not both
- [ ] BUG: seperate buildings don't appear to be getting created when they should
- [ ] inspect panel should list residents/workers in inspected room
- [ ] resident entry point (to be replaced at some point)
- [ ] wallet/money
- [ ] overlays
  - [ ] 'connectedness' overlay (i.e rooms accessable from entrance via transportation items)
- [ ] building exterior - like a 3rd of a tile of extra stuff on the outside
- [ ] camera improvement: camera zoom with scroll wheel
- [ ] camera imrovement: hold middle mouse down to move around
- [ ] camera improvement: should have a 'current tile' that it snaps to
- [ ] camera improvement: inspect zoom
  - [ ] it should last longer, but slow down a lot towards the end
  - [ ] you should be able to use camera movement keys while the zoom out is happening

## room ideas

- [ ] garbage room
  - [ ] each resident/worker creates a certain amount of garbage that has to be stored and unloaded
- [ ] garden
  - [ ] both makes people happy and makes money
- [ ] recreation rooms

# Done

- [x] Rename "Resident" to "Villager" or "Occupant" or something
- [x] CLEANUP: roomTile should have 'coordinates' instead of just tranform.position
- [x] make stairs on the same level as others for now
- [x] Inpect tool should be the default
- [x] basic room tile meshes that render walls/ceiling/floors in the correct place for what position they are in the room
- [x] inspect-able residents
- [x] refactor ToolsController to use InspectTool, BuildTool, and DestroyTool classes
- [x] camera shake when building/destroying a room
- [x] room validators
  - [x] room must be touching another room (except entrance/exit)
- [x] camera zoom on the inspected target
  - [x] IInspectTarget should have a "centerpoint" vector to use for this
- [x] BUG: build tool option buttons don't show up when you use keyboard shortcuts
- [x] CLEANUP: InspectTarget could probably be an interface - SetInspectState, SetInspectHoverdState, etc
- [x] re-implement inspecting rooms
- [x] BUG: clicking on a room when no tool is selected deletes the room?
- [x] 'building entrance/exit'
- [x] residents
  - [x] resident gameobject/prefab
  - [x] room capacity/resident capacity per tile
- [x] inspect tool
- [x] destroy tool
- [x] transportation items
- [x] different colors for rooms
- [x] notification when player tries to build invalid room
- [x] split buildings stuff out into Buildings controller
- [x] pull blueprint/room stuff out of WorldController into new class
- [x] active tool option button should be 'is active'
- [x] 'tool option buttons' (room definitions for build, building/room/resident for inspect, etc)
- [x] bug: blueprint room creates a building, and no new buildings get added when they should
- [x] don't build room when user is clicking on ui/remove blueprint when user hovers over ui
