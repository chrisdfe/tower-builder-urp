using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerBuilder
{
    public class OccupantRouteAttempt
    {
        public List<OccupantRouteNode> path = new();

        Building building;
        HashSet<Room> visitedRooms = new();

        Tile currentTile;
        Room currentRoom;

        Tile destinationTile;
        Room destinationRoom;

        public bool hasReachedDestination { get; private set; }
        List<OccupantRouteAttempt> allAttempts;

        public OccupantRouteAttempt(
            Building building,
            Tile currentTile,
            Tile destinationTile,
            HashSet<Room> visitedRooms,
            List<OccupantRouteAttempt> allAttempts
        )
        {
            Debug.Log(
                "Creating route attempt.\n" +
                $"Building: {building}\n" +
                $"current tile: {currentTile}\n" +
                $"current room: {currentRoom}\n" +
                $"destination tile: {destinationTile}\n" +
                $"destination room: {destinationRoom}\n" +
                $"visited rooms: {visitedRooms.Count}"
            );

            this.building = building;
            this.currentTile = currentTile;
            currentRoom = building.FindRoomAtTile(currentTile);
            Assert.IsNotNull(currentRoom);

            this.destinationTile = destinationTile;
            destinationRoom = building.FindRoomAtTile(destinationTile);
            Assert.IsNotNull(destinationRoom);

            this.visitedRooms = visitedRooms;

            this.allAttempts = allAttempts;
            allAttempts.Add(this);
        }

        public void Start()
        {
            GoToTileAndContinue(currentTile);
        }

        public void GoToTileAndContinue(Tile tile)
        {
            GoToTile(tile);

            if (currentRoom == destinationRoom)
            {
                if (currentTile == destinationTile)
                {
                    // route is complete!
                    hasReachedDestination = true;
                    Debug.Log("I made it.");
                }
                else
                {
                    GoToTileAndContinue(destinationTile);
                }
            }
            // TODO - look for adjacent rooms instead (this assumes all rooms on the same floor are connected)
            else if (currentTile.y == destinationTile.y)
            {
                GoToTileAndContinue(destinationTile);
            }
            else
            {
                // TODO - look for adjacent rooms instead (this assumes all rooms on the same floor are connected)
                var roomsOnFloor = building.FindRoomsOnFloor(currentTile.y);

                var unvisitedTransportationRoomsOnFloor = roomsOnFloor.FindAll(room =>
                    // find transportation items
                    room.definition.type == RoomType.TransportationItem &&
                    // that we haven't visted yet
                    !visitedRooms.Contains(room)
                );

                Debug.Log($"unvisitedTransportationRoomsOnFloor: {unvisitedTransportationRoomsOnFloor.Count}");

                var unvisitedTransportationRoomGroups = unvisitedTransportationRoomsOnFloor
                    .Aggregate(new HashSet<RoomGroup>(), (result, room) =>
                    {
                        result.Add(building.FindRoomGroupByRoom(room));
                        return result;
                    }).ToList();

                Debug.Log($"unvisitedTransportationRoomGroups: {unvisitedTransportationRoomGroups.Count}");

                foreach (var transportationRoomGroup in unvisitedTransportationRoomGroups)
                {
                    Debug.Log($"creating branch for transportation room group {transportationRoomGroup}");
                    var branch = CreateBranch();

                    var bottomLeftTile = transportationRoomGroup.GetBottomLeftTile();
                    branch.GoToTile(new Tile(bottomLeftTile.x, currentTile.y));
                    Debug.Log($"bottomLeftTile: {bottomLeftTile}");

                    var unvisitedFloors = transportationRoomGroup.GetFloors().FindAll(floor => floor != currentTile.y);
                    Debug.Log($"unvisitedFloors: {unvisitedFloors.Aggregate("", (result, floor) => result + $"{floor}, ")}");

                    // Go to each floor this transportation item services
                    // create a new branch for each floor
                    foreach (var floor in unvisitedFloors)
                    {
                        var branchForFloor = branch.CreateBranch();

                        //
                        branch.GoToTileAndContinue(new Tile(branch.currentTile.x, floor));
                    }
                }
            }
        }

        public OccupantRouteAttempt CreateBranch() =>
            new OccupantRouteAttempt(
                building,
                currentTile,
                destinationTile,
                new(visitedRooms),
                allAttempts
            );

        void GoToTile(Tile tile)
        {
            Debug.Log($"going to tile {tile}");

            path.Add(new() { tile = tile });
            currentTile = tile;
            currentRoom = building.FindRoomAtTile(currentTile);

            visitedRooms.Add(currentRoom);
        }
    }
}