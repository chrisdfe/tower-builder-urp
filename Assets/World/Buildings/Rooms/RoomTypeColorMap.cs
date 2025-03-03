using System;
using UnityEngine;

namespace TowerBuilder
{
    [Serializable]
    public class RoomTypeColorMap
    {
        // TODO - something more extensible - a dictionary or something
        //        this is fine for now though
        public Color commonAreaColor;
        public Color officeAreaColor;
        public Color residentialAreaColor;
        public Color transportationItemColor = new Color(0.992f, 0.811f, 0.721f);
        public Color defaultColor;

        // This class is designed for there to be only one of it
        public static Color GetForType(RoomType roomType)
        {
            var worldController = WorldController.Get();
            var roomTypeColorMap = worldController.roomTypeColorMap;

            switch (roomType)
            {
                case RoomType.CommonArea:
                    return roomTypeColorMap.commonAreaColor;
                case RoomType.Residential:
                    return roomTypeColorMap.residentialAreaColor;
                case RoomType.Office:
                    return roomTypeColorMap.officeAreaColor;
                case RoomType.TransportationItem:
                    return roomTypeColorMap.transportationItemColor;
                default:
                    return roomTypeColorMap.defaultColor;
            }
        }
    }
}