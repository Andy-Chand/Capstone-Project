/****************************************************************************
ScriptName: Giving a new definition to Location
Coder: Andy Chand 

Date: 2020-11-02
vers     Date                    Coder       Issue
1.0      2020-02-16              Andy        Initial
1.1      2020-03-13              Allan       Added in the Opacity to the overlay object

*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseAccessClass
{
    public class Location
    {
        //Declares all the variables present in the Animal Database
        //Declared as public, so it is accessible
        //public int AnimalID { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Location(double lat, double lng)
        {
            // Add in checks for valid lat and longs?
            Latitude = lat;
            Longitude = lng;
        }
    }
}
