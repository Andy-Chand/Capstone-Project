/****************************************************************************
ScriptName: HeatMap class to hold the data from tbl_RespParticles

Date: 2020-11-02
vers     Date                   Coder       Issue
1.0      2021-03-14             Taylor      Initial
1.1      2021-03-29             Taylor      moved setWeight class to HeatMap

*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccessClass
{
    public class HeatMap
    {
        public int ID { get; private set; }
        public string FacilityName { get; private set; }
        public Location Loc { get; private set; }
        public int Year { get; private set; }
        public double Emissions { get; private set; }
		public double Weight;

        public HeatMap()
        {

        }


        /// <summary>
        /// HeatMap constructor that takes FaciltyName, Location, Year, and Emissions
        /// Vers        Date            Coder       Issue
        /// 1.0         2021-03-14      Taylor      Initial   
        /// </summary>
        /// <param name="FN"></param>
        /// <param name="L"></param>
        /// <param name="Y"></param>
        /// <param name="E"></param>
        public HeatMap(int id, string FN, Location L, int Y, double E)
        {
            ID = id;
            FacilityName = FN;
            Loc = L;
            Year = Y;
            Emissions = E;
        }


        /// <summary>
        /// Calculates standard deviation of emission data from MasterHeatMarkerList, and assigns weighted values
        /// Vers        Date            Coder       Issue
        /// 1.0         2021-03-18      Taylor      Initial
        ///
        /// </summary>
        /// <params> MasterHeatMarkerList </params>
        public void setWeight(List<HeatMap> MasterHeatMarkerList)
        {
            // Calculate the average    
            double avg = MasterHeatMarkerList.Average(n => n.Emissions);

            // Calculate Sum of (emissions-avg)^2    
            double sum = MasterHeatMarkerList.Sum(n => Math.Pow(n.Emissions - avg, 2));

            // Calculate std dev.    
            double standardDeviation = Math.Sqrt((sum) / (MasterHeatMarkerList.Count() - 1));

            // assign weight property depending on st dev of emissions
            foreach (HeatMap HM in MasterHeatMarkerList)
            {
                HM.Weight = (HM.Emissions < standardDeviation) ? 1.0 :
                         (HM.Emissions > standardDeviation && HM.Emissions < 2 * standardDeviation) ? 2.5 :
                         (HM.Emissions > 2 * standardDeviation && HM.Emissions < 3 * standardDeviation) ? 4.0 :
                         (HM.Emissions > 3 * standardDeviation && HM.Emissions < 4 * standardDeviation) ? 5.5 :
                         (HM.Emissions > 4 * standardDeviation && HM.Emissions < 5 * standardDeviation) ? 7.0 :
                         (HM.Emissions > 5 * standardDeviation && HM.Emissions < 6 * standardDeviation) ? 8.5 :
                         (HM.Emissions > 6 * standardDeviation && HM.Emissions < 7 * standardDeviation) ? 10.0 :
                         (HM.Emissions > 7 * standardDeviation) ? 11.5 : 0.0;
            }
        }
    }
}
