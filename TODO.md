# TODO

## currently:

- camera zoom on the inspected target
  - IInspectTarget should have a "centerpoint" vector to use for this

## next:

- time
- stairs should be able to overlap on the bottom/top
- rooms must be next to another room EXCEPT entrance/exit
- refactor ToolsController to use InspectTool, BuildTool, and DestroyTool classes
- stairs should have an 'entrance' and 'exit'
- BUG: seperate buildings don't appear to be getting created when they should
- inspect panel should list residents/workers
- inspect-able residents
- schedules for residents
- routing for residents
- resident entry point (to be replaced at some point)
- wallet/money

# Done

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
