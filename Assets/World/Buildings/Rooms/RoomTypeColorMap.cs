using System;
using UnityEngine;

namespace TowerBuilder
{
    using static RoomType;
    [Serializable]
    public class RoomTypeColorMap
    {
        // TODO - something more extensible - a dictionary or something
        //        this is fine for now though
        public Color commonAreaColor;
        public Color officeAreaColor;
        public Color residentialAreaColor;
        public Color transportationItemColor = new Color(0.992f, 0.811f, 0.721f);
        public Color recreationRoomColor;
        public Color defaultColor;

        // This class is designed for there to be only one of it
        public static Color GetForType(RoomType roomType)
        {
            var worldController = WorldController.Get();
            var roomTypeColorMap = worldController.roomTypeColorMap;

            switch (roomType)
            {
                case CommonArea:
                    return roomTypeColorMap.commonAreaColor;
                case Residential:
                    return roomTypeColorMap.residentialAreaColor;
                case Office:
                    return roomTypeColorMap.officeAreaColor;
                case TransportationItem:
                    return roomTypeColorMap.transportationItemColor;
                case Recreation:
                    return roomTypeColorMap.recreationRoomColor;
                default:
                    return roomTypeColorMap.defaultColor;
            }
        }
    }
}