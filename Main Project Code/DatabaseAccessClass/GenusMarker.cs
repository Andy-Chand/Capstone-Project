using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccessClass
{
    public class GenusMarker
    {
        public int RareAnimalID { get; private set; }
        public int SiteNumber { get; private set; }
        public Location SiteLocation { get; private set; }
        public int DateYear { get; private set; }
        public int DateMonth { get; private set; }
        public int DateDay { get; private set; }
        public string SpeciesCode { get; private set; }
        public string Genus { get; private set; }
        public string Species { get; private set; }
        public string CommonName { get; private set; }
        public int SiteID { get; private set; }

        /// <summary>
        /// Constructor that builds a Genus Marker when we use "sp_GetAnimalMarkerBySpecies"
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="L"></param>
        /// <param name="S"></param>
        /// <param name="DY"></param>
        /// <param name="DM"></param>
        /// <param name="DD"></param>
        /// <param name="SC"></param>
        /// <param name="G"></param>
        /// <param name="SP"></param>
        /// <param name="CN"></param>
        public GenusMarker(int ID, Location L, int S, int DY, int DM, int DD, string SC, string G, string SP, string CN, int SI)
        {
            RareAnimalID = ID;
            SiteLocation = L;
            SiteNumber = S;
            DateYear = DY;
            DateMonth = DM;
            DateDay = DD;
            SpeciesCode = SC;
            Genus = G;
            Species = SP;
            CommonName = CN;
            SiteID = SI;
        }
    }
}
