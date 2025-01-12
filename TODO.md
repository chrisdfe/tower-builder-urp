# TODO

## currently:

## next:

- time
- room validator: stairs must be fully on top of other rooms
- room validator: only x of a room allowed per building
- routing for residents
- More specific info in inspect view
- IMPROVEMENT: inspect zoom
  - it should last longer, but slow down a lot towards the end
  - you should be able to use camera movement keys while the zoom out is happening
- center blueprint tile to cursor
  - I could refactor/reuse room.GetInspectTargetOrigin for this
- stairs should be able to overlap on the bottom/top
- refactor ToolsController to use InspectTool, BuildTool, and DestroyTool classes
- stairs should have an 'entrance' and 'exit'
- BUG: seperate buildings don't appear to be getting created when they should
- inspect panel should list residents/workers
- inspect-able residents
- schedules for residents
- resident entry point (to be replaced at some point)
- wallet/money
- overlays
  - 'connectedness' overlay (i.e rooms accessable from entrance via transportation items)
- building exterior - like a 3rd of a tile of extra stuff on the outside

# Done

- camera shake when building/destroying a room
- room validators
  - room must be touching another room (except entrance/exit)
- camera zoom on the inspected target
  - IInspectTarget should have a "centerpoint" vector to use for this
- BUG: build tool option buttons don't show up when you use keyboard shortcuts
- CLEANUP: InspectTarget could probably be an interface - SetInspectState, SetInspectHoverdState, etc
- re-implement inspecting rooms
- BUG: clicking on a room when no tool is selected deletes the room?
- 'building entrance/exit'
- residents
  - resident gameobject/prefab
  - room capacity/resident capacity per tile
- inspect tool
- destroy tool
- transportation items
- different colors for rooms
- notification when player tries to build invalid room
- split buildings stuff out into Buildings controller
- pull blueprint/room stuff out of WorldController into new class
- active tool option button should be 'is active'
- 'tool option buttons' (room definitions for build, building/room/resident for inspect, etc)
- bug: blueprint room creates a building, and no new buildings get added when they should
- don't build room when user is clicking on ui/remove blueprint when user hovers over ui
