using System;
using System.Collections.Generic;
using System.Text;

namespace BFV.Common {
    public enum Location {

        UNDEFINED = 0,
        HLT = 1,
        MT_IN = 2,
        MT = 3,
        MT_OUT = 4,
        BK = 5,
        HEX_IN = 6,
        HEX_OUT = 7,
        FERM = 8

    }

    public static class LocationHelper {

        public static IList<Location> AllLocations = new List<Location>(new[] {
            Location.HLT,
            Location.MT_IN,
            Location.MT,
            Location.MT_OUT,
            Location.BK,
            Location.HEX_IN,
            Location.HEX_OUT,
            Location.FERM
        });

        public static IList<Location> PidLocations = new List<Location>(new[] {
            Location.HLT,
            Location.MT,
            Location.BK,
        });

        public static IList<Location> SsrLocations = new List<Location>(new[]{
            Location.HLT,
            Location.BK
        });

        public static IList<Location> PumpComponentIds = new List<Location>(new[] {
            Location.HLT,
            Location.MT,
            Location.BK,
        });

        public static Location ToLocation(this string locationStr) {
            return (Location)Enum.Parse(typeof(Location), locationStr);
        }

        public static Location ToLocation(this int locationInt) {
            return ToLocation(locationInt.ToString());
        }

    }
}
