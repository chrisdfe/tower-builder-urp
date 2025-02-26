# TODO

## currently:

- [ ] schedules for occupants
  - [ ] sleep time
  - [ ] work time
- [ ] ability to cancel tasks
  - e.g interupting wander task with 'travel to destination' task
  - e.g when a room is destroyed
  - [ ] BUG: if you interrupt an occupant while it's traveling to a tile by telling it to go to another tile, it will do so but
        you will not be able to give it any further commands.
- [ ] BUG: I broke Room.GetInspectFocalPoint

## next:

- [ ] TileAddress - tile, room, building
- [ ] sky that responds to time of day
- [ ] get rid of room layers? if transportation rooms (stairs elevators) are all going to be on the same layer
- [ ] inspect camera should move with the resident as it's moving
- [ ] occupants should arrive via entrance/exit
- [ ] build/destroy mode should pause time
- [ ] BUG: occupants poke through walls at extreme subTileOffsets
- [ ] refactor occupants to exist primarily on buildingsController
  - right now they belong to rooms.residents, which doesn't account
- [ ] "travelers"
  - [ ] create room for this ("hotel room" or "overnight cabin" or something)
  - [ ] periodically travelers arrive, stay, and then get off
- [ ] BUG: changing speeds while resident is moving messes walking/movement transitions up
- [ ] should be able to move camera in inspect mode
- [ ] segment variant improvements:
  - [ ] apply segment variant to entire room group as well
  - [ ] apply segment variant to entire building as well
  - [ ] save/reuse last used segment variant? Settings for building?
  - [ ] large window that spans multiple tiles/tile position aware
- [ ] BUG: figure out why when I de-focus then re-focus the unity editor while the game is running I get a bunch of errors. Probably an Awake() thing
- [ ] BUG: fix building-adding code - right now it's hard coded as max 1 building
- [ ] ability to fix how stairs still room group even if not aligned vertically
  - [ ] maybe just a validator that prevents you from placing them right next to each other or askew
- [ ] BUG: the UI scales weirdly when I resize the window
- [ ] transportation items
  - [ ] maybe just rooms
  - [ ] each should have 1+ 'entrance' and 'exit'
- [ ] More specific info in inspect view
- [ ] 'undeletable' rooms (starting entrance/exit)
  - [ ] could do this with a destroy validator that always returns false
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
- [ ] room furniture

## room ideas

- [ ] garbage room
  - [ ] each resident/worker creates a certain amount of garbage that has to be stored and unloaded
- [ ] garden
  - [ ] both makes people happy and makes money
- [ ] recreation rooms

# Done

- [x] in traveling task, make sure occupant.currentRoom gets updated as well
- [x] time
- [x] destroy rooms on a per-block basis
  - [x] maybe the room group situation is already a solution for this
- [x] make sure random tile selected in 'wander' task isn't the tile the occupant is currently on
- [x] 'wander' task
- [x] randomize occupants' position on tile slightly -0.5f-0.5f
- [x] place new residents randomly within room, instead of all on one tile
- [x] center blueprint tile to cursor
  - [x] I could refactor/reuse room.GetInspectTargetOrigin for this
- [x] routing for occupants
  - [x] ability to tell a occupant to go here or there
  - [x] occupant 'tasks' - wandering, standing walking
    - [ ] subtasks too - e.g wandering subtasks: standing, walking to another subtile, etc
      - these can probably just be IOccupantTasks, I can't think of why they would need to be seperate right now
  - [x] display current occupant task in inspect mode
  - [x] move occupant along path, 1 tile per tick
  - [x] animate transition between tiles
- [x] add ability to change windows etc of rooms to inspect tool
- [x] Resizable rooms
  - [x] It can just be rooms merge when rooms of the same type are placed next to each other for now
- [x] rooms that aren't resizable but don't build a wall between itself and another room of its type
  - [x] Room 'groups'
  - [x] BUG: only the first roomGroup is counted
  - [x] eg entrance/exit, lobby, stairs
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
