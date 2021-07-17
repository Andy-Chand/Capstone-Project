using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace DatabaseAccessClass
{
    public class DataAccess
    {

        public List<string> DistinctSites;

        public DataAccess()
        {

        }

        /// <summary>
        /// Gets the connection string 
        /// Vers        Date        Coder       Comments
        /// 1.0         2021-03-22  Allan       Initial
        /// </summary>
        /// <returns></returns>
        public string GetConnectionString()
        {
            string fileName = "Connection.txt";
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            List<String> Connection = File.ReadAllLines(path).ToList();
            return Connection[0];
        }

        /// <summary>
        /// Uses lambda expression to sort out distinct sites from list
        /// Vers     Date           Coder       Comments
        /// 1.0      2021-02-22     Taylor      Initial
        /// 
        /// <param name="csvList"></param>
        /// <returns>SiteList<list type="<String>>"</returns>
        /// 
        public List<String> FindDistinctSites(List<String> csvList)
        {
            // list that will be populated with only the information pertaining to sites
            List<String> SiteList = new List<String>();

            foreach (String S in csvList)
            {
                // splits the string, and adds site number, latitude, and longitude to the SiteString List
                string[] entries = S.Split(',');
                //ProjectID, Cluster, SITE, STATION, Latitude, Longitude, RECORDING_DATE, RECORD_TIME, Method, SPECIES_CODE, NumberDetected, Genus, Species, ENGLISH_NAME
                //SITE, Latitude, Longitude
                string P = entries[2] + "," + entries[4] + "," + entries[5];
                SiteList.Add(P);
            }

            // groups SiteString list by site number, and then selects the first entry from each new group
            // Sites list is populated with this data and returned
            SiteList = SiteList.GroupBy(i => i.Split(',')[0]).Select(grp => grp.First()).ToList();

            return SiteList;
        }


        /// <summary>
        /// Inserts Rare Animal data from the CSV file. Uses a SQL bulk insert
        /// Vers     Date           Coder       Comments
        /// 1.0      2021-02-03     Taylor      Initial
        /// 1.1      2021-02-22     Taylor      Seperate the CSV into tbl_RareAnimal, and tbl_Sites. Added SELECT to find FK
        /// 1.2      2021-02-23     Taylor      Using LINQ to find FK instead of SELECT. Reduces need to open connection 
        /// 
        /// <param name="FileName"></param>
        /// <returns></returns>

        public void InsertAnimalMarker(string FileName)
        {
            SqlConnection conn = new SqlConnection(GetConnectionString());

            // reads the csv file for RareAnimal data. Each row is entered into the TempEntry list as an entry
            List<String> TempEntry = File.ReadAllLines(FileName).ToList();
            // removes the first line which includes the column names
            TempEntry.RemoveAt(0);

            try
            {
                // create a new datatable for Sites
                DataTable sites = new DataTable();

                // map the data table to match tbl_Sites
                sites.Columns.Add(new DataColumn("SiteID", typeof(int)));
                sites.Columns.Add(new DataColumn("Latitude", typeof(double)));
                sites.Columns.Add(new DataColumn("Longitude", typeof(double)));
                sites.Columns.Add(new DataColumn("SiteNumber", typeof(int)));

                // calls FindDistinctSites to sort out site specific information
                DistinctSites = FindDistinctSites(TempEntry);

                for (int i = 0; i < DistinctSites.Count; i++)
                {
                    //order of the elements
                    //SITE, Latitude, Longitude
                    string[] siteElements = DistinctSites[i].Split(',');

                    // create a new row for sites
                    DataRow rowSites = sites.NewRow();

                    // populate row with data (map the above data table with the appropriate split string index)
                    rowSites["SiteID"] = i;
                    rowSites["Latitude"] = Convert.ToDouble(siteElements[1]);
                    rowSites["Longitude"] = Convert.ToDouble(siteElements[2]);
                    rowSites["SiteNumber"] = Convert.ToInt32(siteElements[0]);

                    // add row to sites datatable
                    sites.Rows.Add(rowSites);
                }

                //create object of SqlBulkCopy which help to insert  
                SqlBulkCopy sitebulk = new SqlBulkCopy(conn);

                //assign Destination table name  
                sitebulk.DestinationTableName = "tbl_Sites";

                // map the datatable columns to the tbl_RareAnimal columns
                sitebulk.ColumnMappings.Add("SiteID", "SiteID");
                sitebulk.ColumnMappings.Add("Latitude", "Latitude");
                sitebulk.ColumnMappings.Add("Longitude", "Longitude");
                sitebulk.ColumnMappings.Add("SiteNumber", "SiteNumber");

                conn.Open();
                //insert bulk Records into DataBase.  
                sitebulk.WriteToServer(sites);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                conn.Close();

            }

            try
            {

                // create a new datatable for Animals
                DataTable animals = new DataTable();

                // map the data table to match tbl_RareAnimal
                animals.Columns.Add(new DataColumn("RareAnimalID", typeof(int)));
                animals.Columns.Add(new DataColumn("DateYear", typeof(int)));
                animals.Columns.Add(new DataColumn("DateMonth", typeof(int)));
                animals.Columns.Add(new DataColumn("DateDay", typeof(int)));
                animals.Columns.Add(new DataColumn("SpeciesCode", typeof(string)));
                animals.Columns.Add(new DataColumn("Genus", typeof(string)));
                animals.Columns.Add(new DataColumn("Species", typeof(string)));
                animals.Columns.Add(new DataColumn("CommonName", typeof(string)));
                animals.Columns.Add(new DataColumn("SitesFK", typeof(int)));

                for (int i = 0; i < TempEntry.Count; i++)
                {

                    // split the TempEntry rows into columns
                    //order of the elements
                    //ProjectID, Cluster, SITE, STATION, Latitude, Longitude, RECORDING_DATE, RECORD_TIME, Method, SPECIES_CODE, NumberDetected, Genus, Species, ENGLISH_NAME
                    string[] animalElements = TempEntry[i].Split(',');
                    // split the date further into three sections (dd mmm yy)
                    string[] date = animalElements[6].Split('-');

                    // create a new row for animals
                    DataRow rowAnimals = animals.NewRow();
                    // populate row with data (map the above data table with the appropriate split string index)
                    rowAnimals["RareAnimalID"] = i;
                    rowAnimals["DateYear"] = (Convert.ToInt32(date[2]));
                    // use ternary operator to convert string month to number. Default is NULL
                    rowAnimals["DateMonth"] = (date[1] == "Jan") ? 1 :
                                      (date[1] == "Feb") ? 2 :
                                      (date[1] == "Mar") ? 3 :
                                      (date[1] == "Apr") ? 4 :
                                      (date[1] == "May") ? 5 :
                                      (date[1] == "Jun") ? 6 :
                                      (date[1] == "Jul") ? 7 :
                                      (date[1] == "Aug") ? 8 :
                                      (date[1] == "Sep") ? 9 :
                                      (date[1] == "Oct") ? 10 :
                                      (date[1] == "Nov") ? 11 :
                                      // want to change default to NULL not 0
                                      (date[1] == "Dec") ? 12 : 0;
                    rowAnimals["DateDay"] = Convert.ToInt32(date[0]);
                    rowAnimals["SpeciesCode"] = animalElements[9];
                    rowAnimals["Genus"] = animalElements[11];
                    rowAnimals["Species"] = animalElements[12];
                    rowAnimals["CommonName"] = animalElements[13];
                    // This uses LINQ to find the index of the DistinctSites list (+1 because of index 0) which corresponds to the site number
                    rowAnimals["SitesFK"] = (DistinctSites.FindIndex(a => a.Split(',')[0] == animalElements[2])) + 1;

                    // add row to datatable
                    animals.Rows.Add(rowAnimals);
                }

                //create object of SqlBulkCopy which help to insert  
                SqlBulkCopy animalbulk = new SqlBulkCopy(conn);

                //assign Destination table name  
                animalbulk.DestinationTableName = "tbl_RareAnimal";

                // map the datatable columns to the tbl_RareAnimal columns
                animalbulk.ColumnMappings.Add("RareAnimalID", "RareAnimalID");
                animalbulk.ColumnMappings.Add("DateYear", "DateYear");
                animalbulk.ColumnMappings.Add("DateMonth", "DateMonth");
                animalbulk.ColumnMappings.Add("DateDay", "DateDay");
                animalbulk.ColumnMappings.Add("SpeciesCode", "SpeciesCode");
                animalbulk.ColumnMappings.Add("Genus", "Genus");
                animalbulk.ColumnMappings.Add("Species", "Species");
                animalbulk.ColumnMappings.Add("CommonName", "CommonName");
                animalbulk.ColumnMappings.Add("SitesFK", "SitesFK");

                conn.Open();
                //insert bulk Records into DataBase.  
                animalbulk.WriteToServer(animals);

            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                conn.Close();

            }

        }


        /// <summary>
        /// Inserts Resp Particules data from the CSV file. Uses a SQL bulk insert
        /// Vers     Date           Coder       Comments
        /// 1.0      2021-02-03     Taylor      Initial
        /// 1.1      2021-02-06     Jill        Added in "FacilityName"
        /// 
        /// 
        /// <param name="FileName"></param>
        /// <returns></returns>

        public void InsertRespParticulates(string FileName)
        {
            SqlConnection conn = new SqlConnection(GetConnectionString());

            // reads the csv file for RespParticules data. Each row is entered into the TempEntry list as an entry
            List<String> TempEntry = File.ReadAllLines(FileName).ToList();
            // removes the first line which includes the column names
            TempEntry.RemoveAt(0);

            try
            {
                // create a new datatable
                DataTable tbl = new DataTable();

                // map the data table to match tbl_RareAnimal
                tbl.Columns.Add(new DataColumn("RespID", typeof(int)));
                tbl.Columns.Add(new DataColumn("FacilityName", typeof(string)));
                tbl.Columns.Add(new DataColumn("Latitude", typeof(double)));
                tbl.Columns.Add(new DataColumn("Longitude", typeof(double)));
                tbl.Columns.Add(new DataColumn("DateYear", typeof(int)));
                tbl.Columns.Add(new DataColumn("Emissions_Tonnes", typeof(double)));


                for (int i = 0; i < TempEntry.Count; i++)
                {
                    // split the TempEntry rows into columns
                    //order of the elements
                    //NPRI_ID, Facility_name, Company_name, Address, City, Province, PostalCode, Latitude, Longitude, Emissions, Units, Report_year
                    string[] elements = TempEntry[i].Split(',');
                    double emissions;

                    // create a new row
                    DataRow dr = tbl.NewRow();

                    // populate row with data (map the above data table with the appropriate split string index)
                    dr["RespID"] = i;
                    dr["FacilityName"] = (elements[1]);
                    dr["Latitude"] = Convert.ToDouble(elements[7]);
                    dr["Longitude"] = Convert.ToDouble(elements[8]);
                    dr["DateYear"] = Convert.ToInt32(elements[11]);
                    // use ternary operator to sort out doubles from "-" (aka. NULL)
                    dr["Emissions_Tonnes"] = (Double.TryParse(elements[9], out emissions)) ? emissions : 0;


                    // add row to datatable
                    tbl.Rows.Add(dr);
                }

                //create object of SqlBulkCopy which help to insert  
                SqlBulkCopy objbulk = new SqlBulkCopy(conn);

                //assign Destination table name  
                objbulk.DestinationTableName = "tbl_RespParticles";

                // map the datatable columns to the tbl_RareAnimal columns
                objbulk.ColumnMappings.Add("RespID", "RespID");
                objbulk.ColumnMappings.Add("FacilityName", "FacilityName");
                objbulk.ColumnMappings.Add("Latitude", "Latitude");
                objbulk.ColumnMappings.Add("Longitude", "Longitude");
                objbulk.ColumnMappings.Add("DateYear", "DateYear");
                objbulk.ColumnMappings.Add("Emissions_Tonnes", "Emissions_Tonnes");

                conn.Open();
                //insert bulk Records into DataBase.  
                objbulk.WriteToServer(tbl);
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                conn.Close();

            }

        }

        /// <summary>
        /// Runs the Stored Procedures scripts. Executes the script incrementally based on the "GO" command
        /// Vers        Date            Coder       Comments
        /// 1.0         2021-02-17      Taylor      Initial
        /// 
        /// <param name="FileName"></param>
        /// <returns></returns>
        public void RunNonQueries(string FileName)
        {
            // read the Stored Procedure file
            string script = File.ReadAllText(FileName);
            // sqlBatch will hold the script up until the "GO" is used. It will then be executed, and cleared to hold the next batch
            string sqlBatch = string.Empty;

            SqlConnection conn = new SqlConnection(GetConnectionString());
            SqlCommand cmd = new SqlCommand(string.Empty, conn);

            try
            {
                conn.Open();
                foreach (string line in script.Split(new string[2] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    // If line == "GO", execute sqlBatch command up until this point
                    // Then clear the sqlBatch to be populated with the next section
                    if (line.ToUpperInvariant().Trim() == "GO")
                    {
                        cmd.CommandText = sqlBatch;
                        cmd.ExecuteNonQuery();
                        sqlBatch = string.Empty;
                    }
                    // Else, add line to the end of sqlBatch
                    else
                    {
                        sqlBatch += line + "\n";
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                conn.Close();
            }
        }


        /// <summary>
        /// Purpose: This function runs the DDL_Create file. It is very similar to RunNonQueries except it splits the connection string to exclude
        /// the database name since the database hasn't been created yet.
        /// 
        /// Vers        Date            Coder       Comments
        /// 1.0         2021-03-08      Allan       Initial
        /// </summary>
        /// <param name="FileName"></param>
        public void DDL_Create(string FileName)
        {
            // read the Stored Procedure file
            string script = File.ReadAllText(FileName);
            // sqlBatch will hold the script up until the "GO" is used. It will then be executed, and cleared to hold the next batch
            string sqlBatch = string.Empty;

            string[] GetConnection = GetConnectionString().Split(';');

            SqlConnection conn = new SqlConnection(GetConnection[0] + ';' + GetConnection[2] + ';' + GetConnection[3]);

            SqlCommand cmd = new SqlCommand(string.Empty, conn);

            try
            {
                conn.Open();
                foreach (string line in script.Split(new string[2] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    // If line == "GO", execute sqlBatch command up until this point
                    // Then clear the sqlBatch to be populated with the next section
                    if (line.ToUpperInvariant().Trim() == "GO")
                    {
                        cmd.CommandText = sqlBatch;
                        cmd.ExecuteNonQuery();
                        sqlBatch = string.Empty;
                    }
                    // Else, add line to the end of sqlBatch
                    else
                    {
                        sqlBatch += line + "\n";
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                conn.Close();
            }
        }
    

    /// <summary>
    /// Imports Site Markers using sp_GetAllSites to populate a Master list for markers
    /// Vers     Date           Coder       Comments
    /// 1.0      2021-03-14     Jill        Initial
    /// 1.1      2021-03-17     Taylor      Refactored to fit V1.6 changes
    /// 
    /// </summary>
    /// <returns>MasterSiteMarkers</returns>
    /// 
    public List<HeatMap> GetAllRespParticles()
        {
            //MasterHeatMarkers List will store all the information from the SQL Database
            List<HeatMap> MasterHeatMarkers = new List<HeatMap>();

            try
            {
                //Accesses the connection and executes the stored procedure to get all Resp Particulate data
                System.Data.SqlClient.SqlDataReader t = PDM.Data.SqlHelper.ExecuteReader(GetConnectionString(), "sp_GetAllRespParticulates");
                //If the database still has rows, it will continue
                if (t.HasRows)
                {
                    //While rows are present C# will continue to read and add the SQL database to the C# list
                    while (t.Read())
                    {

                        //Grabs the variables from Resp Particulates and parses them appropriately 
                        // Order = SiteID, Name, Lat, Long, Year, Emissions

                        HeatMap tempHeat = new HeatMap(int.Parse(t[0].ToString()), t[1].ToString(), new Location(double.Parse(t[2].ToString()), double.Parse(t[3].ToString())), int.Parse(t[4].ToString()), double.Parse(t[5].ToString()));

                        //HeatMarker(int ID, int E, Location L, String F)
                        //Adds the row to the list
                        MasterHeatMarkers.Add(tempHeat);
                    }
                }
            }
            //This is the catch block
            catch (SqlException SQLE)
            {
                //No Implementation yet...

                throw SQLE;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {

            }
            //Returns the filled list
            return MasterHeatMarkers;
        }

        /// <summary>
        /// Uses sp_InsertRespParticulate to insert a new entry into tbl_RespParticules
        /// Vers     Date           Coder       Comments
        /// 1.0      2021-03-14     Taylor      Initial
        /// </summary>
        /// <params>InsertChoice, Lat, Long, SiteNumber, Year, Month, Day, SpeciesCode, Genus, Species, CommonName)</params>
        public string InsertRespParticulate(string Facility, Location Loc, int Year, double Emissions)
        {
            try
            {
                //Accesses the connection and executes the stored procedure to insert the new marker
                System.Data.SqlClient.SqlDataReader t = PDM.Data.SqlHelper.ExecuteReader(GetConnectionString(), "SP_InsertRespParticulate", Facility, Loc.Latitude, Loc.Longitude, Year, Emissions);
                return "Emissions data successfully entered into the database";
            }

            catch (SqlException SQLE)
            {
                return SQLE.Message + " Emissions data not entered into the database";
            }
        }


        /// <summary>
        /// Imports Site Markers using sp_GetAllSites to populate a Master list for markers
        /// Vers     Date           Coder       Comments
        /// 1.0      2021-02-16     Andy        Initial
        /// 1.1      2021-02-23     Taylor      Refactored to populate site marker list rather than animal markers
        /// </summary>
        /// <returns>MasterSiteMarkers</returns>
        /// 
        public List<SiteMarker> GetAllSites()
        {
            //Lists the TempLocationList which will store all the information from the SQL Database
            List<SiteMarker> MasterSiteMarkers = new List<SiteMarker>();

            try
            {
                //Accesses the connection and executes the stored procedure to get all sites
                System.Data.SqlClient.SqlDataReader t = PDM.Data.SqlHelper.ExecuteReader(GetConnectionString(), "sp_GetAllSites");
                //If the database still has rows, it will continue
                if (t.HasRows)
                {
                    //While rows are present C# will continue to read and add the SQL database to the C# list
                    while (t.Read())
                    {

                        //Grabs the variables from Location and parses them appropriately 
                        // Order = SiteID, Lat, Long, SiteNumber, Genus

                        SiteMarker tempSite = new SiteMarker(int.Parse(t[0].ToString()), int.Parse(t[3].ToString()), new Location(double.Parse(t[1].ToString()), double.Parse(t[2].ToString())), SiteSightings(Convert.ToInt32(t[0])));


                        //Adds the row to the list
                        MasterSiteMarkers.Add(tempSite);
                    }
                }
            }
            //This is the catch block
            catch (SqlException SQLE)
            {
                //No Implementation yet...
                
                throw SQLE;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {

            }
            //Returns the filled list
            return MasterSiteMarkers;
        }


        /// <summary>
        /// Imports the Animal sighting information for a specific site ID, and specific genus
        /// Vers     Date           Coder       Comments
        /// 1.0      2021-03-01     Allan       Initial
        /// 1.1      2021-03-02     Allan       Added day, month, year and other info to animal sightings
        ///
        /// </summary>
        /// <params>GenusName, SiteID</params>
        /// <returns>GenusSiteAnimalMarkers</returns>
        /// 
        public List<AnimalSightings> AnimalsByMarkerAndGenus(string GenusName, int SiteID)
        {
            List<AnimalSightings> GenusSiteAnimalMarkers = new List<AnimalSightings>();

            try
            {
                //Accesses the connection and executes the stored procedure to get all sites
                System.Data.SqlClient.SqlDataReader t = PDM.Data.SqlHelper.ExecuteReader(GetConnectionString(), "sp_GetAnimalIDByGenusAndSiteID", GenusName, SiteID);
                //If the database still has rows, it will continue
                if (t.HasRows)
                {
                    //While rows are present C# will continue to read and add the SQL database to the C# list
                    while (t.Read())
                    {

                        //Grabs the variables from Location and parses them appropriately 
                        // Order = SiteID, SiteNumber, Lat, Long

                        AnimalSightings tempAnimal = new AnimalSightings(Convert.ToInt32(t[0]), t[1].ToString(), t[2].ToString(), t[3].ToString(), Convert.ToInt32(t[4]), Convert.ToInt32(t[5]), Convert.ToInt32(t[6]), Convert.ToInt32(t[7]), Convert.ToInt32(t[8]), new Location(double.Parse(t[9].ToString()), double.Parse(t[10].ToString())));


                        //Adds the row to the list
                        GenusSiteAnimalMarkers.Add(tempAnimal);
                    }
                }
            }
            //This is the catch block
            catch (SqlException SQLE)
            {
                //No Implementation yet...
                throw SQLE;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {

            }
            //Returns the filled list
            return GenusSiteAnimalMarkers;
        }


        /// <summary>
        /// Imports the SiteMarkers which contain a specific genus. Returns GenusSiteMarkers list
        /// Vers     Date           Coder       Comments
        /// 1.0      2021-03-01     Allan       Initial
        ///
        /// </summary>
        /// <params>GenusName</params>
        /// <returns>GenusSiteMarkers</returns>
        public List<SiteMarker> SiteMarkersByGenus(string GenusName)
        {
            //Lists the TempLocationList which will store all the information from the SQL Database
            List<SiteMarker> GenusSiteMarkers = new List<SiteMarker>();

            try
            {
                //Accesses the connection and executes the stored procedure to get all sites
                System.Data.SqlClient.SqlDataReader t = PDM.Data.SqlHelper.ExecuteReader(GetConnectionString(), "sp_GetSiteMarkerByGenus", GenusName);
                //If the database still has rows, it will continue
                if (t.HasRows)
                {
                    //While rows are present C# will continue to read and add the SQL database to the C# list
                    while (t.Read())
                    {

                        //Grabs the variables from Location and parses them appropriately 
                        // Order = SiteID, SiteNumber, Lat, Long

                        SiteMarker tempSite = new SiteMarker(Convert.ToInt32(t[0]), Convert.ToInt32(t[1]), new Location(Convert.ToDouble(t[2]), Convert.ToDouble(t[3])), (t[4].ToString()));


                        //Adds the row to the list
                        GenusSiteMarkers.Add(tempSite);
                    }
                }
            }
            //This is the catch block
            catch (SqlException SQLE)
            {
                //No Implementation yet...
                throw SQLE;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {

            }
            //Returns the filled list
            return GenusSiteMarkers;
        }


        /// <summary>
        /// Imports the information using sp_GetAllAnimals to populate a list for markers
        /// Vers     Date           Coder       Comments
        /// 1.0      2021-02-16     Andy        Initial
        /// 1.1      2021-03-29     Taylor      Re-factored to suit the refactored SP
        /// 
        /// <param name="FileName"></param>
        /// <returns></returns>

        public List<AnimalSightings> GetAllAnimals()
        {
            //Lists the TempLocationList which will store all the information from the SQL Database
            List<AnimalSightings> MasterAnimalMarkers = new List<AnimalSightings>();

            try
            {
                //Accesses the connection and executes the stored procedure to get all animals
                System.Data.SqlClient.SqlDataReader t = PDM.Data.SqlHelper.ExecuteReader(GetConnectionString(), "sp_GetAllAnimals");
                //If the database still has rows, it will continue
                if (t.HasRows)
                {
                    //While rows are present C# will continue to read and add the SQL database to the C# list
                    while (t.Read())
                    {

                        //Grabs the variables from Location and parses them appropriately 
                        // SQL Order  RA.ID, RA.DateYear, RA.DateMonth, RA.DateDay, RA.SpeciesCode, RA.Genus, RA.Species, RA.CommonName, S.Latitude, S.Longitude
                        //Constructor Order ( AnimalID,  L,  DateYear,  DateMonth,  DateDay,  SpeciesCode, Genus,  Species,  CommonName)
                        AnimalSightings AS = new AnimalSightings(int.Parse(t[0].ToString()), new Location(double.Parse(t[8].ToString()), double.Parse(t[9].ToString())),
                                                                  int.Parse(t[1].ToString()), int.Parse(t[2].ToString()), int.Parse(t[3].ToString()), t[4].ToString(),
                                                                  t[5].ToString(), t[6].ToString(), t[7].ToString());

                        //Adds the row to the list
                        MasterAnimalMarkers.Add(AS);
                    }
                }
            }
            //This is the catch block
            catch (SqlException SQLE)
            {
                //No Implementation yet...
                throw SQLE;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {

            }
            //Returns the filled list
            return MasterAnimalMarkers;
        }


        /// <summary>
        /// Uses SP_DeleteSiteMarkerByID to delete a site marker
        /// Vers     Date           Coder       Comments
        /// 1.0      2021-02-26     Allan       Initial
        /// 1.1      2021-03-02     Taylor      Added finally{con.Close();}
        ///
        /// </summary>
        /// <params>ID</params>
        public void DeleteSiteByID(int ID)
        //the function returns a void and accepts only int's
        {                
            SqlConnection con = new SqlConnection(GetConnectionString());
            try
            {
                using (con)
                {
                    con.Open();
                    SqlCommand command = new SqlCommand("sp_DeleteSiteMarkerByID", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "sp_DeleteSiteMarkerByID";
                    command.Parameters.Add("@SiteMarkerID", SqlDbType.Int).Value = ID;
                    //this is the connection between C# and SQL. this function will perform in the exact same
                    //way as SP_DeleteAnimalMarkerbyID in that it will delete ALL instances of that particular AnimalID
                    using (command)
                    {
                        int G = command.ExecuteNonQuery();
                        //when C# tries to delete an animal marker that doesn't exist, command.ExecuteNonQuery() = -1
                        //therefore we can alert the user that they inputed an invalid value (I have it in Console commands
                        //right now but later it will be in HTML Alert's or something
                        //if (G == -1)
                        //{
                        //    throw NotSupportedException;
                        //}

                    }
                }
            }
            catch (SqlException SQLE)
            {
                //No Implementation yet...
                throw SQLE;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Creates a list of animal sightings based on the site ID number using sp_GetAnimalsBySiteID.
        /// Vers     Date           Coder       Comments
        /// 1.0                                 Initial
        /// 1.1     2021-03-02      Taylor      Added finally {con.Close();}
        /// 1.2     2021-03-02      Allan       Changed the catch
        /// </summary>
        /// <param name="ID"></param>
        /// <returns>tempSightings</returns>
        public List<AnimalSightings> SiteSightings(int ID)
        {

            DataTable dt = new DataTable();
            List<AnimalSightings> tempSightings = new List<AnimalSightings>();
            SqlConnection con = new SqlConnection(GetConnectionString());

            try
            {
                using (con)
                {
                    con.Open();
                    SqlCommand command = new SqlCommand("sp_GetAnimalsBySiteID", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "sp_GetAnimalsBySiteID";
                    command.Parameters.Add("@SiteNumberInput", SqlDbType.Int).Value = ID;
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dt);

                    foreach (DataRow dr in dt.Rows)
                    {
                        AnimalSightings obj = new AnimalSightings();

                        //It only takes three parameters. Room for improvement here if needed.

                        obj.AnimalID = Convert.ToInt32(dr["RareAnimalID"]);
                        obj.SiteNumber = Convert.ToInt32(dr["SiteNumber"]);
                        obj.CommonName = dr["CommonName"].ToString();
                        obj.DateDay = Convert.ToInt32(dr["DateDay"]);
                        obj.DateMonth = Convert.ToInt32(dr["DateMonth"]);
                        obj.DateYear = Convert.ToInt32(dr["DateYear"]);
                        obj.Genus = dr["Genus"].ToString();
                        obj.SpeciesCode = dr["SpeciesCode"].ToString();

                        tempSightings.Add(obj);
                    }
                }

            }
            catch (SqlException SQLE)
            {
                //No Implementation yet...
                throw SQLE;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                con.Close();
            }

            return tempSightings;
        }


        /// <summary>
        /// Uses SP_DeleteAnimalMarkerByID to delete a site marker
        /// Vers     Date           Coder       Comments
        /// 1.0      2021-02-26     Allan       Initial
        /// 1.1      2021-03-02     Taylor      Added finally{con.Close();}
        /// 1.2      2021-03-02     Allan       Changed the catch
        /// </summary>
        /// <params>ID</params>
        public void DeleteAnimalMarkerbyID(int ID)
        //the function returns a void and accepts only int's
        {
            SqlConnection con = new SqlConnection(GetConnectionString());

            try
            {
                using (con)
                {
                    con.Open();
                    SqlCommand command = new SqlCommand("sp_DeleteAnimalMarkerByID", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "sp_DeleteAnimalMarkerbyID";
                    command.Parameters.Add("@AnimalMarkerID", SqlDbType.Int).Value = ID;
                    //this is the connection between C# and SQL. this function will perform in the exact same
                    //way as SP_DeleteAnimalMarkerbyID in that it will delete ALL instances of that particular AnimalID
                    using (command)
                    {
                        int G = command.ExecuteNonQuery();
                        //when C# tries to delete an animal marker that doesn't exist, command.ExecuteNonQuery() = -1
                        //therefore we can alert the user that they inputed an invalid value (I have it in Console commands
                        //right now but later it will be in HTML Alert's or something
                        //if (G == -1)
                        //{
                        //    throw NotSupportedException;
                        //}

                    }
                }
            }
            catch (SqlException SQLE)
            {
                //No Implementation yet...
                throw SQLE;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Uses sp_GenusByAnimalCommonName
        /// Vers     Date           Coder       Comments
        /// 1.0      2021-03-01     Allan       Initial
        ///
        /// </summary>
        /// <params>CommonName</params>
        /// <returns>TempList</returns>
        public List<AnimalSightings> AnimalInfo(string CommonName)
        {

            List<AnimalSightings> TempList = new List<AnimalSightings>();

            try
            {
                System.Data.SqlClient.SqlDataReader t = PDM.Data.SqlHelper.ExecuteReader(GetConnectionString(), "sp_GenusByAnimalCommonName", CommonName);
                //If the database still has rows, it will continue
                if (t.HasRows)
                {
                    //While rows are present C# will continue to read and add the SQL database to the C# list


                    while (t.Read())
                    {

                        //Grabs the variables from Location and parses them appropriately 
                        AnimalSightings tempAnimal = new AnimalSightings(t[0].ToString(), t[1].ToString(), t[2].ToString(), t[3].ToString());

                        //Adds the row to the list
                        TempList.Add(tempAnimal);
                    }
                }
            }
            //This is the catch block
            catch (SqlException SQLE)
            {
                //No Implementation yet...
                throw SQLE;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {

            }
            return TempList;
        }

        /// <summary>
        /// Uses sp_InsertAnimalMarker to insert either a new Site Marker or a new Animal Sighting
        /// Vers     Date           Coder       Comments
        /// 1.0      2021-02-28     Andy        Initial
        /// 1.1      2021-03-01     Taylor      Refactored to allow for both inserting sites and animal markers.
        /// 1.2      2021-03-02     Andy        Changed variable to nullable
        /// 1.3      2021-03-11     Allan       Changed variable type to string. Now if an error occurs in the SQL layer
        ///                                     it will return the error string. If successful it will return the common
        ///                                     name the fact that the function passed successfully. Also the 'if'
        ///                                     statement was added so that the returned string reflects whether an animal
        ///                                     sighting is being added (or has failed) or a site has been added (or has failed).
        /// </summary>
        /// <params>InsertChoice, Lat, Long, SiteNumber, Year, Month, Day, SpeciesCode, Genus, Species, CommonName)</params>
        public string InsertNewAnimalMarker(bool InsertChoice, double? Lat, double? Long, int? SiteNumber, int? Year, int? Month, int? Day, string SpeciesCode, string Genus, string Species, string CommonName)
        {
            // bool InsertChoice determines whether data will be inserted into tbl_Sites (true) or tbl_RareAnimal (false)
            // use event handlers of button clicks to set the bool value
            if (InsertChoice == false)
            {
                string ERROR = "Common Name: " + CommonName + " successfully added to the database";
                try
                {
                    //Accesses the connection and executes the stored procedure to insert the new marker
                    System.Data.SqlClient.SqlDataReader t = PDM.Data.SqlHelper.ExecuteReader(GetConnectionString(), "sp_InsertAnimalMarker", InsertChoice, Lat, Long, SiteNumber, Year, Month, Day, SpeciesCode, Genus, Species, CommonName);
                }
                //This is the catch block
                catch (Exception e)
                {
                    ERROR = e.Message + "Animal sighting was not added to the database";
                }

                return ERROR;

            }
            else
            {
                string ERROR = "Site was added";
                try
                {
                    //Accesses the connection and executes the stored procedure to insert the new marker
                    System.Data.SqlClient.SqlDataReader t = PDM.Data.SqlHelper.ExecuteReader(GetConnectionString(), "sp_InsertAnimalMarker", InsertChoice, Lat, Long, SiteNumber, Year, Month, Day, SpeciesCode, Genus, Species, CommonName);
                }
                //This is the catch block
                catch (Exception e)
                {
                    ERROR = e.Message + " Site was not added";
                }

                return ERROR;
            }
            
          
            
        }

        /// <summary>
        /// Uses sp_UpdateAnimalMarkerByID to either update site information or update animal sighting information.
        /// Vers     Date           Coder       Comments
        /// 1.0      2021-03-11     Allan       Initial (maybe not the initial but initial is unknown)
        /// 1.1      2021-03-11     Allan       Added the if statement to taylor the return string value to reflect whether
        ///                                     a site is being updated or if it is the animal sighting that is being updated.
        /// </summary>

        /// <returns></returns>
        public string UpdateAnimalMarker(bool InsertChoice, int ID, double? Lat, double? Long, int? SiteNumber, int? AnimalID, int? Year, int? Month, int? Day, string SpeciesCode, string Genus, string Species, string CommonName)
        {
            // bool InsertChoice determines whether data will be inserted into tbl_Sites (true) or tbl_RareAnimal (false)
            // use event handlers of button clicks to set the bool value
            if (InsertChoice == false)
            {
                string ERROR = "Common Name: " + CommonName +
                          "\nSpecies Code : " + SpeciesCode +
                          "\nSpecies: " + Species +
                          "\nGenus: " + Genus +
                          "\n successfully updated in the database";
                try
                {
                    //Accesses the connection and executes the stored procedure to insert the new marker
                    System.Data.SqlClient.SqlDataReader t = PDM.Data.SqlHelper.ExecuteReader(GetConnectionString(), "sp_UpdateAnimalMarkerByID", InsertChoice, ID, Lat, Long, SiteNumber, AnimalID, Year, Month, Day, SpeciesCode, Genus, Species, CommonName);
                }
                //This is the catch block
                catch (SqlException SQLE)
                {
                    //No Implementation yet...
                    ERROR = SQLE.Message + "\nCommon Name: " + CommonName + " sighting was not updated in the database";
                }
                catch (Exception e)
                {

                    ERROR = e.Message + "\nCommon Name: " + CommonName + " sighting was not updated in the database";
                }
                return ERROR;
            }
            else
            {
                string ERROR = "Site Number: " + SiteNumber +
                       "\nLatitude : " + Lat +
                       "\nLongitude: " + Long +
                       "\n successfully updated in the database";
                try
                {
                    //Accesses the connection and executes the stored procedure to insert the new marker
                    System.Data.SqlClient.SqlDataReader t = PDM.Data.SqlHelper.ExecuteReader(GetConnectionString(), "sp_UpdateAnimalMarkerByID", InsertChoice, ID, Lat, Long, SiteNumber, AnimalID, Year, Month, Day, SpeciesCode, Genus, Species, CommonName);
                }
                //This is the catch block
                catch (SqlException SQLE)
                {
                    //No Implementation yet...
                    ERROR = SQLE.Message + "\nSite Number: " + SiteNumber + " was not updated in the database";
                }
                catch (Exception e)
                {

                    ERROR = e.Message + "\nSite Number: " + SiteNumber + " was not updated in the database";
                }
                return ERROR;
            }
        }


        /// <summary>
        /// Selects distinct genus from tbl_RareAnimal. Adds them to a list
        /// Vers     Date           Coder       Comments
        /// 1.0      2021-03-03     Taylor        Initial
        /// </summary>
        /// <returns>GenusList</returns>
        public List<string> GetDistinctGenus()
        {
            List<string> GenusList = new List<string>();
            SqlConnection con = new SqlConnection(GetConnectionString());
            SqlCommand GenusSelect = new SqlCommand("SELECT DISTINCT Genus FROM tbl_RareAnimal ORDER BY Genus", con);

            try
            {
                con.Open();
                SqlDataReader t = GenusSelect.ExecuteReader();
                while (t.Read())
                {
                    string G = t[0].ToString();
                    GenusList.Add(G);
                }

            }

            catch (SqlException SQLE)
            {
                //No Implementation yet...
                throw SQLE;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                con.Close();
            }
                return GenusList;

        }


        /// <summary>
        /// Uses SP_DeleteAirParticulateByID to delete a resp site marker
        /// Vers     Date           Coder       Comments
        /// 1.0      2021-02-26     Allan       Initial
        /// 
        ///
        /// </summary>
        /// <params>ID</params>
        public void DeleteSiteRespByID(int ID)
        {
            SqlConnection con = new SqlConnection(GetConnectionString());

            try
            {
                using (con)
                {
                    con.Open();
                    SqlCommand command = new SqlCommand("sp_DeleteAirParticulateByID", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "sp_DeleteAirParticulateByID";
                    command.Parameters.Add("@AirParticulateID", SqlDbType.Int).Value = ID;
                    //this is the connection between C# and SQL. this function will perform in the exact same
                    //way as SP_DeleteRespParticulateByID in that it will delete ALL instances of that particular RespID
                    using (command)
                    {
                        int G = command.ExecuteNonQuery();
                        //when C# tries to delete an animal marker that doesn't exist, command.ExecuteNonQuery() = -1
                        //therefore we can alert the user that they inputed an invalid value (I have it in Console commands
                        //right now but later it will be in HTML Alert's or something
                        //if (G == -1)
                        //{
                        //    throw NotSupportedException;
                        //}

                    }
                }
            }
            catch (SqlException SQLE)
            {
                //No Implementation yet...
                throw SQLE;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Purpose: This function takes an int from the MainPage.aspx page, finds the path for Connection.txt, then writes the
        /// appropriate connection string to it. Then it runs the function BuildDatabase.
        /// 
        /// Vers        Date            Coder       Comments
        /// 1.0         2021-03-21      Allan       Initial
        /// </summary>
        /// <param name="i"></param>
        public void SetConnectionStringAndBuildDatabase(int i)
        {
            if (i == 1)
            {
                string fileName = "Connection.txt";
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

                List<String> ConnectionStrings = new List<string>();
                ConnectionStrings.Add(@"");
                File.WriteAllLines(path, ConnectionStrings.ToArray());
                BuildDatabase();
            }
            else if (i == 2)
            {
                string fileName = "Connection.txt";
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

                List<String> ConnectionStrings = new List<string>();
                ConnectionStrings.Add(@"");
                File.WriteAllLines(path, ConnectionStrings.ToArray());
                BuildDatabase();
            }
            else if (i == 3)
            {
                string fileName = "Connection.txt";
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

                List<String> ConnectionStrings = new List<string>();
                ConnectionStrings.Add(@"");
                File.WriteAllLines(path, ConnectionStrings.ToArray());
                BuildDatabase();
            }
            else if (i == 4)
            {
                string fileName = "Connection.txt";
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

                List<String> ConnectionStrings = new List<string>();
                ConnectionStrings.Add(@"");
                File.WriteAllLines(path, ConnectionStrings.ToArray());
                BuildDatabase();
            }
            else if (i == 5)
            {
                string fileName = "Connection.txt";
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

                List<String> ConnectionStrings = new List<string>();
                ConnectionStrings.Add(@"");
                File.WriteAllLines(path, ConnectionStrings.ToArray());
                BuildDatabase();
            }
        }


        /// <summary>
        /// Purpose: This function takes an int from the MainPage.aspx page, finds the path for Connection.txt, then writes the
        /// appropriate connection string to it.
        /// </summary>
        /// <param name="i"></param>
        public void SetConnectionString(int i)
        {
            if (i == 1)
            {
                string fileName = "Connection.txt";
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

                List<String> ConnectionStrings = new List<string>();
                ConnectionStrings.Add(@"");
                File.WriteAllLines(path, ConnectionStrings.ToArray());
            }
            else if (i == 2)
            {
                string fileName = "Connection.txt";
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

                List<String> ConnectionStrings = new List<string>();
                ConnectionStrings.Add(@"");
                File.WriteAllLines(path, ConnectionStrings.ToArray());
            }
            else if (i == 3)
            {
                string fileName = "Connection.txt";
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

                List<String> ConnectionStrings = new List<string>();
                ConnectionStrings.Add(@"");
                File.WriteAllLines(path, ConnectionStrings.ToArray());
            }
            else if (i == 4)
            {
                string fileName = "Connection.txt";
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

                List<String> ConnectionStrings = new List<string>();
                ConnectionStrings.Add(@"");
                File.WriteAllLines(path, ConnectionStrings.ToArray());
            }
            else if (i == 5)
            {
                string fileName = "Connection.txt";
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

                List<String> ConnectionStrings = new List<string>();
                ConnectionStrings.Add(@"");
                File.WriteAllLines(path, ConnectionStrings.ToArray());
            }
        }

        /// <summary>
        /// Purpose: This builds the database using the functions above. It must use the PathFinder function to get the path
        /// to the sql files and datasets.
        /// 
        /// Vers        Date            Coder       Comments
        /// 1.0       2021-03-21      Allan       Initial
        /// </summary>
        public void BuildDatabase()
        {
            // Running the DDL
            DDL_Create(PathFinder("SQL Promote/DB_Radagast_DDL_CREATE_V1.5.sql"));

            // Insert the Rare Animal data from the csv file
            InsertAnimalMarker(PathFinder("Datasets/RareAnimalsSpeciesOnly.csv"));

            // Insert the Resp Particulate data from the csv file
            InsertRespParticulates(PathFinder("Datasets/ParticulateEmissionsLat53.csv"));

            //Rare Animal(Marker) Stored Procedures
            //CREATE
            RunNonQueries(PathFinder("SQL Promote/sp_InsertAnimalMarker_V1.7.sql"));

            RunNonQueries(PathFinder("SQL Promote/sp_GetAllAnimals_V1.5.sql"));

            RunNonQueries(PathFinder("SQL Promote/sp_GetAllSites_V1.0.sql"));

            RunNonQueries(PathFinder("SQL Promote/sp_GetAnimalByGenusAndSite_V1.1.sql"));

            RunNonQueries(PathFinder("SQL Promote/sp_GetAnimalsBySiteID_V1.1.sql"));

            RunNonQueries(PathFinder("SQL Promote/sp_GetSiteMarkerByGenus_V1.0.sql"));

            RunNonQueries(PathFinder("SQL Promote/sp_GenusByAnimalCommonName_V1.0.sql"));

            //UPDATE
            RunNonQueries(PathFinder("SQL Promote/sp_UpdateAnimalMarkerByID_V1.7.sql"));

            // DELETE
            RunNonQueries(PathFinder("SQL Promote/sp_DeleteAnimalMarkerByID_V1.0.sql"));

            RunNonQueries(PathFinder("SQL Promote/sp_DeleteSiteByID_V1.0.sql"));

            // Respirable Particulates (HeatMap) Stored Procedures
            //CREATE
            RunNonQueries(PathFinder("SQL Promote/SP_InsertRespParticulate_V1.3.sql"));

            //READ
            RunNonQueries(PathFinder("SQL Promote/sp_GetAllRespParticulates_V1.0.sql"));

            RunNonQueries(PathFinder("SQL Promote/sp_GetRespParticulateByID_V1.2.sql"));

            //UPDATE
            RunNonQueries(PathFinder("SQL Promote/sp_UpdateRespParticulateByID_V1.2.sql"));

            // DELETE
            RunNonQueries(PathFinder("SQL Promote/sp_DeleteRespParticulateByID_V1.1.sql"));
        }

        /// <summary>
        /// Purpose: This function uses takes a string and uses Path.Combine to find the absolute path name
        /// of the file and then returns it.
        /// 
        /// Vers        Date        Coder       Comments
        /// 1.0         2021-03-21  Allan       Initial
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public string PathFinder(string file)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file);
            string[] pathArray = path.Split('\\');
            string shorterPath = "";
            for (int i = 0; i < pathArray.Length; i++)
            {
                if (i == pathArray.Length - 2 || i == pathArray.Length - 3)
                {

                }
                else if (i == 0)
                {
                    shorterPath = shorterPath + pathArray[i];
                }
                else
                {
                    shorterPath = shorterPath +"\\"+ pathArray[i];
                }
            }
            return shorterPath;
        }

    }

}
