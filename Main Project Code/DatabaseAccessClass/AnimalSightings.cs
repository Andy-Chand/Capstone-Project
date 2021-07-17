using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccessClass
{

    public class AnimalSightings
    {
        public int AnimalID { get; set; }
        public int SiteNumber { get; set; }
        public int DateYear { get; set; }
        public int DateMonth { get; set; }
        public int DateDay { get; set; }
        public string SpeciesCode { get; set; }
        public string Genus { get; set; }
        public string Species { get; set; }
        public string CommonName { get; set; }
        public int SiteID { get; set; }
        public Location SiteLocation { get; private set; }

        public AnimalSightings()
        {

        }

        public AnimalSightings(string SpeciesCode, string Genus, string Species, string CommonName)
        {
            this.SpeciesCode = SpeciesCode;
            this.Genus = Genus;
            this.Species = Species;
            this.CommonName = CommonName;
        }

        public AnimalSightings(int AnimalID, string CommonName, string Genus, int SiteID, int SiteNumber, Location L)
        {
            this.AnimalID = AnimalID;
            this.CommonName = CommonName;
            this.Genus = Genus;
            this.SiteID = SiteID;
            this.SiteNumber = SiteNumber;
            this.SiteLocation = L;


        }
        /// <summary>
        /// Animal Sighting constructor. Takes all parameters relating to Animal and location (not site)
        /// Vers        Date        Coder       Comments
        /// 1.0                                 Initial
        /// 1.1         2021-03-29  Taylor      Removed closest emissions
        /// </summary>
        /// <param name="AnimalID"></param>
        /// <param name="L"></param>
        /// <param name="DateYear"></param>
        /// <param name="DateMonth"></param>
        /// <param name="DateDay"></param>
        /// <param name="SpeciesCode"></param>
        /// <param name="Genus"></param>
        /// <param name="Species"></param>
        /// <param name="CommonName"></param>
        public AnimalSightings(int AnimalID, Location L, int DateYear, int DateMonth, int DateDay, string SpeciesCode,
                            string Genus, string Species, string CommonName)
        {
            this.AnimalID = AnimalID;
            this.SiteNumber = SiteNumber;
            this.DateYear = DateYear;
            this.DateMonth = DateMonth;
            this.DateDay = DateDay;
            this.SpeciesCode = SpeciesCode;
            this.Genus = Genus;
            this.Species = Species;
            this.CommonName = CommonName;

        }
        public AnimalSightings(int AnimalID, string CommonName, string Genus, string SpeciesCode, int DateYear, int DateMonth, int DateDay, int SiteID, int SiteNumber,
          Location L)
        {

            this.AnimalID = AnimalID;
            this.SiteNumber = SiteNumber;
            this.DateYear = DateYear;
            this.DateMonth = DateMonth;
            this.DateDay = DateDay;
            this.SpeciesCode = SpeciesCode;
            this.Genus = Genus;
            this.Species = Species;
            this.CommonName = CommonName;
            this.SiteID = SiteID;
            this.SiteLocation = L;

        }


    }
}
