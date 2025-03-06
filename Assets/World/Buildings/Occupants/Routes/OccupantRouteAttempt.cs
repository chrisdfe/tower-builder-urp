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
            List<OccupantRouteNode> path,
            HashSet<Room> visitedRooms,
            List<OccupantRouteAttempt> allAttempts
        )
        {
            this.building = building;
            this.currentTile = currentTile;
            currentRoom = building.FindRoomAtTile(currentTile);
            Assert.IsNotNull(currentRoom);

            this.destinationTile = destinationTile;
            destinationRoom = building.FindRoomAtTile(destinationTile);
            Assert.IsNotNull(destinationRoom);

            this.path = path;

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

                var unvisitedTransportationRoomGroups = unvisitedTransportationRoomsOnFloor
                    .Aggregate(new HashSet<RoomGroup>(), (result, room) =>
                    {
                        result.Add(building.FindRoomGroupByRoom(room));
                        return result;
                    }).ToList();

                foreach (var transportationRoomGroup in unvisitedTransportationRoomGroups)
                {
                    var branch = CreateBranch();

                    var bottomLeftTile = transportationRoomGroup.GetLowestXTileOnFloor(currentTile.y);
                    branch.GoToTile(bottomLeftTile);

                    var unvisitedFloors = transportationRoomGroup.GetFloors().FindAll(floor => floor != currentTile.y);

                    // Go to each floor this transportation item services
                    // create a new branch for each floor
                    foreach (var floor in unvisitedFloors)
                    {
                        var branchForFloor = branch.CreateBranch();

                        //
                        branchForFloor.GoToTileAndContinue(new Tile(branch.currentTile.x, floor));
                    }
                }
            }
        }

        public OccupantRouteAttempt CreateBranch() =>
            new OccupantRouteAttempt(
                building,
                currentTile,
                destinationTile,
                new(path),
                new(visitedRooms),
                allAttempts
            );

        void GoToTile(Tile tile)
        {
            path.Add(new() { tile = tile });
            currentTile = tile;
            currentRoom = building.FindRoomAtTile(currentTile);

            visitedRooms.Add(currentRoom);
        }
    }
}