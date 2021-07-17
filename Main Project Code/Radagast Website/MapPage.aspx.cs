using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DatabaseAccessClass;
using System.Data.SqlClient;


namespace Radagast_Website
{
    public partial class MapPage : System.Web.UI.Page
    {
        public static List<SiteMarker> MasterSiteMarkerList;
        public static List<HeatMap> MasterHeatMarkerList;
        public static List<SiteMarker> GenusSiteList;
        public static List<AnimalSightings> AllAnimals;
        public static List<string> AllNames;
        public static List<String> GenusList;
        public static DataAccess DB;
        public static HeatMap HM;
        public static double Opacity;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // establish connections to classes
                DB = new DataAccess();
                HM = new HeatMap();

                // Creates list of all Site Markers. MasterSiteMarkerList, Populates list from DB.GetAllSites.
                MasterSiteMarkerList = new List<SiteMarker>(DB.GetAllSites());

                // Creates list of all Site Markers. MasterSiteMarkerList, Populates list from DB.GetAllSites.
                MasterHeatMarkerList = new List<HeatMap>(DB.GetAllRespParticles());

                // calls setWeight() to calculate weight classes for MasterHeatMarkerList based on Emissions
                HM.setWeight(MasterHeatMarkerList);

                //List of all animal information
                AllAnimals = new List<AnimalSightings>(DB.GetAllAnimals());

                //List of distinct animal common names
                AllNames = AllAnimals.Select(i => i.CommonName).Distinct().ToList();

                //DropDownGenusList has a default so when a selection is made the selection is saved to an invisible textbox on the MapPage.aspx
                //so that it can be used in the event of an accidental refresh (like when you add the heatmap and/or overlay)
                if (DropDownGenusList.SelectedItem.Value == "DefaultSelection" && LastSelection.Text == "")
                {
                    GenusSiteList = new List<SiteMarker>(DB.SiteMarkersByGenus(DropDownGenusList.SelectedItem.Value));
                }
                else if (DropDownGenusList.SelectedItem.Value == "DefaultSelection" && LastSelection.Text != null)
                {
                    GenusSiteList = new List<SiteMarker>(DB.SiteMarkersByGenus(LastSelection.Text.ToString()));
                }
                else
                {
                    GenusSiteList = new List<SiteMarker>(DB.SiteMarkersByGenus(DropDownGenusList.SelectedItem.Value));

                    LastSelection.Text = DropDownGenusList.SelectedItem.Value;
                }


                // complete list of distinct genus
                // loops list and adds genus to the drop down menu
                GenusList = new List<String>(DB.GetDistinctGenus());

                foreach (String S in GenusList)
                {
                    DropDownGenusList.Items.Remove(S);
                    DropDownGenusList.Items.Add(S);
                }


                //Creates an overlay with the Bottom left coordinate, top right coordinate, and the image path
                try
                {
                    Overlay L = new Overlay(new Location(49.45142, -120.12540), new Location(60.00970, -109.95450), "../../MapImages/ABeco.png", Opacity);

                    System.Web.UI.HtmlControls.HtmlGenericControl createDiv = new System.Web.UI.HtmlControls.HtmlGenericControl("DIV");
                    createDiv.Attributes.Add("BottomLeftLat", L.BottomLeft.Latitude.ToString());
                    createDiv.Attributes.Add("BottomLeftLng", L.BottomLeft.Longitude.ToString());
                    createDiv.Attributes.Add("TopRightLat", L.TopRight.Latitude.ToString());
                    createDiv.Attributes.Add("TopRightLng", L.TopRight.Longitude.ToString());
                    createDiv.Attributes.Add("ImagePath", L.ImagePath);
                    createDiv.Attributes.Add("Opacity", L.Opacity.ToString());
                    DivOverlayList.Controls.Add(createDiv);
                    DivOverlayList.Visible = OverlayCheckbox.Checked;
                }
                catch (Exception E)
                {
                    ShowAlert(E.Message + "\n Please see the support team to resolve this issue");
                }
            }
            catch (Exception E)
            {
                ShowAlert("A database error has occured. Return to the home page to reset your connection string.");
            }

        }



        /// <summary>
        /// Throws an alert to the screen 
        /// Vers        Date            Coder       Issue
        /// 1.0         2021-03-04      Taylor      Initial
        /// </summary>
		/// <params>alert</params>
        public void ShowAlert(string alert)
        {
            Response.Write("<script>alert('" + alert + "')</script>");
        }


        /// <summary>
        /// Controls Overlay visibility in reaction to the Overlay Checkbox
        /// Vers        Date            Coder       Issue
        /// 1.0         2021-03-08      Jill        Initial
        /// 1.1         2021-03-13      Allan       Added in the sliderstuff visibility
        /// 1.2         2021-03-18      Allan       Added the location marker compatibility loops
        /// 1.3         2021-03-24      Taylor      De-nested if / else
        /// </summary>
        /// <params>sender, e</params>
        protected void OverlayCheckboxChange(object sender, EventArgs e)
        {
            try
            {
                if (OverlayCheckbox.Checked == true)
                {
                    Legend.Visible = true;
                    DivOverlayList.Visible = true;
                    sliderstuff.Visible = true;
                    br5.Visible = true;
                    br8.Visible = false;
                }
                else if (OverlayCheckbox.Checked == false)
                {
                    DivOverlayList.Visible = false;
                    sliderstuff.Visible = false;
                    Legend.Visible = false;
                    br5.Visible = false;
                    br8.Visible = true;
                }
                if (HeatMapCheckbox.Checked == true)
                {
                    PopulateHeatMap();
                }
                else if (HeatMapCheckbox.Checked == false)
                {

                }

                if (DropDownGenusList.SelectedItem.Value == "All")
                {
                    // call PopulateAllMarkers function
                    PopulateAllMarkers();
                }
                else if (DropDownGenusList.SelectedItem.Value == "Empty")
                {

                }
                else
                {
                    PopulateGenusMarkers();
                }
            }
            catch (Exception)
            {

            }


        }

        /// <summary>
        /// Calls heatmap markers to populate in reaction to HeatMapCheckbox
        /// Vers        Date            Coder       Issue
        /// 1.0         2021-03-14      Jill        Initial
        /// 1.1         2021-03-18      Allan       Added the location marker compatibility loops
        /// 1.2         2021-03-24      Taylor      De-nested if / else
        /// </summary>
        /// <params>sender, e</params>
        protected void HeatMapCheckboxChange(object sender, EventArgs e)
        {
            try
            {
                if (HeatMapCheckbox.Checked == true)
                {
                    PopulateHeatMap();
                }

                if (OverlayCheckbox.Checked == true)
                {
                    Legend.Visible = true;
                    DivOverlayList.Visible = true;
                    sliderstuff.Visible = true;
                    br5.Visible = true;
                    br8.Visible = false;
                }
                else if (OverlayCheckbox.Checked == false)
                {
                    DivOverlayList.Visible = false;
                    sliderstuff.Visible = false;
                    Legend.Visible = false;
                    br5.Visible = false;
                    br8.Visible = true;
                }
                if (DropDownGenusList.SelectedItem.Value == "All")
                {
                    // call PopulateAllMarkers function
                    PopulateAllMarkers();
                }
                else if (DropDownGenusList.SelectedItem.Value == "Empty")
                {

                }
                else
                {
                    PopulateGenusMarkers();
                }
            }
            catch (Exception)
            {

            }

        }



        /// <summary>
        /// Takes a list of Heat Markers and populates the data to the map
        /// Vers        Date            Coder       Issue
        /// 1.0         2021-03-14      Jill      	Initial
        /// 1.1			2021-03-18		Taylor		Added in weight
        /// 
        /// </summary>
        protected void PopulateHeatMap()
        {
            for (int i = 0; i < MasterHeatMarkerList.Count; ++i)
            {
                System.Web.UI.HtmlControls.HtmlGenericControl createDiv = new System.Web.UI.HtmlControls.HtmlGenericControl("DIV");

                createDiv.Attributes.Add("RespID", MasterHeatMarkerList[i].ID.ToString());
                createDiv.Attributes.Add("FacilityName", MasterHeatMarkerList[i].FacilityName.ToString());
                createDiv.Attributes.Add("Latitude", MasterHeatMarkerList[i].Loc.Latitude.ToString());
                createDiv.Attributes.Add("Longitude", MasterHeatMarkerList[i].Loc.Longitude.ToString());
                createDiv.Attributes.Add("Emissions", MasterHeatMarkerList[i].Emissions.ToString());
                createDiv.Attributes.Add("Weight", MasterHeatMarkerList[i].Weight.ToString());


                // create the div with all the information at the end
                DivHeatMapList.Controls.Add(createDiv);
            }
        }


        /// <summary>
        /// Takes a list of Site Markers and populates the data to the map
        /// Vers        Date            Coder       Issue
        /// 1.0         2021-03-02      Taylor      Initial(Function-ified Andy's code)
        /// 1.1         2021-03-02      Allan       Added in the Year, Month and Day into the lists and added webMethods at
        ///                                         the bottom of the page
        /// 1.2         2021-03-03      Taylor      Removed parameters so only valid Site Marker lists are used
        /// 1.3         2021-03-11      Allan       Added the animal names 'for' loop in the 'else' so that sites with no 
        ///                                         animal sightings (newly made sites) can have a list of distinct animal
        ///                                         names for when the user wants to add known animal sightings to the site.
        /// </summary>
        protected void PopulateAllMarkers()
        {
            for (int i = 0; i < MasterSiteMarkerList.Count; ++i)
            {
                System.Web.UI.HtmlControls.HtmlGenericControl createDiv = new System.Web.UI.HtmlControls.HtmlGenericControl("DIV");

                createDiv.Attributes.Add("SiteID", MasterSiteMarkerList[i].SiteID.ToString());
                createDiv.Attributes.Add("SiteNumber", MasterSiteMarkerList[i].SiteNumber.ToString());
                createDiv.Attributes.Add("Latitude", MasterSiteMarkerList[i].SiteLocation.Latitude.ToString());
                createDiv.Attributes.Add("Longitude", MasterSiteMarkerList[i].SiteLocation.Longitude.ToString());

                // if the site marker has associated animal sightings, add animal sighting information to the Div
                if (MasterSiteMarkerList[i].Sightings.Count > 0)
                {
                    for (int j = 0; j < MasterSiteMarkerList[i].Sightings.Count; ++j)
                    {
                        createDiv.Attributes.Add("Sighting" + j.ToString(), (MasterSiteMarkerList[i].Sightings[j].AnimalID.ToString() + ": " + MasterSiteMarkerList[i].Sightings[j].CommonName));
                        createDiv.Attributes.Add("Year" + j.ToString(), (MasterSiteMarkerList[i].Sightings[j].DateYear.ToString()));
                        createDiv.Attributes.Add("Month" + j.ToString(), (MasterSiteMarkerList[i].Sightings[j].DateMonth.ToString()));
                        createDiv.Attributes.Add("Day" + j.ToString(), (MasterSiteMarkerList[i].Sightings[j].DateDay.ToString()));

                        if (j == MasterSiteMarkerList[i].Sightings.Count - 1)
                        {
                            createDiv.Attributes.Add("SightingCount", (j + 1).ToString());
                        }
                    }
                    for (int k = 0; k < AllNames.Count; ++k)
                    {
                        createDiv.Attributes.Add("CommonName" + k.ToString(), AllNames[k].ToString());
                    }
                    createDiv.Attributes.Add("NameCount", (AllNames.Count).ToString());
                }
                // else just add all the common animal names as attributes for later use.
                else
                {
                    for (int k = 0; k < AllNames.Count; ++k)
                    {
                        createDiv.Attributes.Add("CommonName" + k.ToString(), AllNames[k].ToString());
                    }
                    createDiv.Attributes.Add("NameCount", (AllNames.Count).ToString());
                }
                // create the div with all the information at the end
                DivMarkerList.Controls.Add(createDiv);
            }
        }

        /// <summary>
        /// Takes a list of Site Markers which contain a specific genus, and populates the data to the map
        /// Vers        Date            Coder       Issue
        /// 1.0         2021-03-02      Taylor      Initial (Function-ified Andy's code)
        /// 1.1         2021-03-02      Allan       Added in the Year, Month and Day
        /// 1.2         2021-03-03      Taylor      Removed parameters so only valid Site Marker lists are used

        /// </summary>
        protected void PopulateGenusMarkers()
        {
            for (int i = 0; i < GenusSiteList.Count; ++i)
            {

                //Generates the list using the DataAccess class by genus and the appropriate stored procedure
                List<AnimalSightings> SightingsByGenusAndSite = new List<AnimalSightings>(DB.AnimalsByMarkerAndGenus(GenusSiteList[i].Genus, GenusSiteList[i].SiteID));

                System.Web.UI.HtmlControls.HtmlGenericControl createDiv = new System.Web.UI.HtmlControls.HtmlGenericControl("DIV");

                createDiv.Attributes.Add("SiteID", GenusSiteList[i].SiteID.ToString());
                createDiv.Attributes.Add("SiteNumber", GenusSiteList[i].SiteNumber.ToString());
                createDiv.Attributes.Add("Latitude", GenusSiteList[i].SiteLocation.Latitude.ToString());
                createDiv.Attributes.Add("Longitude", GenusSiteList[i].SiteLocation.Longitude.ToString());
                createDiv.Attributes.Add("Genus", GenusSiteList[i].Genus.ToString());
                createDiv.Attributes.Add("AnimalCount", SightingsByGenusAndSite.Count.ToString());

                for (int j = 0; j < SightingsByGenusAndSite.Count; ++j)
                {
                    createDiv.Attributes.Add("Animal" + j.ToString(), (SightingsByGenusAndSite[j].AnimalID.ToString() + ": " + SightingsByGenusAndSite[j].CommonName));
                    createDiv.Attributes.Add("Year" + j.ToString(), (SightingsByGenusAndSite[j].DateYear.ToString()));
                    createDiv.Attributes.Add("Month" + j.ToString(), (SightingsByGenusAndSite[j].DateMonth.ToString()));
                    createDiv.Attributes.Add("Day" + j.ToString(), (SightingsByGenusAndSite[j].DateDay.ToString()));
                }

                for (int k = 0; k < AllNames.Count; ++k)
                {
                    createDiv.Attributes.Add("CommonName" + k.ToString(), AllNames[k].ToString());
                }

                createDiv.Attributes.Add("NameCount", (AllNames.Count).ToString());

                DivMarkerList.Controls.Add(createDiv);

            }
        }

        /// <summary>
        /// Event handler for InsertHM_True button (shows all fields for inserting emissions data)
        /// Vers        Date        Coder       Comments
        /// 1.0         2021-03-14  Taylor      Initial
        /// 1.1         2021-03-18  Allan       Added code to help with style and spacing and synergized with the
        ///                                     site markers, heatmap and overlay
        /// 1.2         2021-03-24  Taylor      De-nested if / else
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void InsertHM_True(object sender, EventArgs e)
        {
            try
            {
                // hides the site fields
                LatitudeText.Visible = false;
                LongitudeText.Visible = false;
                SubmitSite.Visible = false;
                CancelSiteInsert.Visible = false;
                br1.Visible = false;
                br2.Visible = false;

                // shows the emissions fields
                FacilityTB.Visible = true;
                LatitudeTB.Visible = true;
                LongitudeTB.Visible = true;
                YearTB.Visible = true;
                EmissionsTB.Visible = true;
                SubmitHM.Visible = true;
                CancelTB.Visible = true;
                br3.Visible = true;
                br4.Visible = true;
                br6.Visible = true;
                if (HeatMapCheckbox.Checked == true)
                {
                    PopulateHeatMap();
                }

                if (OverlayCheckbox.Checked == true)
                {
                    Legend.Visible = true;
                    DivOverlayList.Visible = true;
                    sliderstuff.Visible = true;
                    br5.Visible = true;
                    br8.Visible = false;
                }
                else if (OverlayCheckbox.Checked == false)
                {
                    DivOverlayList.Visible = false;
                    sliderstuff.Visible = false;
                    Legend.Visible = false;
                    br5.Visible = false;
                    br8.Visible = true;
                }
                if (DropDownGenusList.SelectedItem.Value == "All")
                {
                    // call PopulateAllMarkers function
                    PopulateAllMarkers();
                }
                else if (DropDownGenusList.SelectedItem.Value == "Empty")
                {

                }
                else
                {
                    PopulateGenusMarkers();

                }
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Event handler for SubmitHM_Click button (inserts data into tbl_RespParticules from form)
        /// 
        /// Vers        Date        Coder       Comments
        /// 1.0         unknown     unknown     initial
        /// 1.1         2021-03-19  Allan       added the synergy with heatmap, overlay and location markers
        /// 1.2         2021-03-24  Taylor      Added call to DB to populate HeatMap after entry is inserted, and resets weight
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SubmitHM_Click(object sender, EventArgs e)
        {
            try
            {
                string message;
                try
                {
                    // converts the text input from each field into their appropriate variable type
                    string FacilityInput = FacilityTB.Text;
                    double LatitudeInput = Math.Round(Convert.ToDouble(LatitudeTB.Text), 8);
                    double LongitudeInput = Math.Round(Convert.ToDouble(LongitudeTB.Text), 8);
                    int YearInput = Convert.ToInt32(YearTB.Text);
                    double EmissionsInput = Math.Round(Convert.ToDouble(EmissionsTB.Text), 2);

                    // call to the database to insert the users data
                    message = DB.InsertRespParticulate(FacilityInput, new Location(LatitudeInput, LongitudeInput), YearInput, EmissionsInput);
                    ShowAlert(message);

                    // add new heatmap entry to the list
                    MasterHeatMarkerList = DB.GetAllRespParticles();
                    // reset the weight for all heatmap
                    HM.setWeight(MasterHeatMarkerList);
                }

                catch (Exception E)
                {
                    ShowAlert(Server.HtmlEncode(E.Message) + " Emission was not inserted into database");
                }

                // re-set the map to how the user had it before the insert
                finally
                {
                    if (HeatMapCheckbox.Checked == true)
                    {
                        // then re-populate markers
                        PopulateHeatMap();
                    }

                    if (OverlayCheckbox.Checked == true)
                    {
                        Legend.Visible = true;
                        DivOverlayList.Visible = true;
                        sliderstuff.Visible = true;
                        br5.Visible = true;
                        br8.Visible = false;
                    }
                    else if (OverlayCheckbox.Checked == false)
                    {
                        DivOverlayList.Visible = false;
                        sliderstuff.Visible = false;
                        Legend.Visible = false;
                        br5.Visible = false;
                        br8.Visible = true;

                    }
                    if (DropDownGenusList.SelectedItem.Value == "All")
                    {
                        // call PopulateAllMarkers function
                        PopulateAllMarkers();
                    }
                    else if (DropDownGenusList.SelectedItem.Value == "Empty")
                    {

                    }
                    else
                    {
                        PopulateGenusMarkers();
                    }


                }
            }
            catch (Exception)
            {

            }
        }



        /// <summary>
        /// Event handler for InsertSite_True button (shows all fields for inserting site data)
        /// Vers        Date        Coder       Comments
        /// 1.0         2021-03-14  Taylor      Button-ized Andy's code
        /// 1.1         2021-03-18  Allan       Added code to help with style and spacing and synergized with the
        ///                                     site markers, heatmap and overlay
        /// 1.2         2021-03-24  Taylor      De-nested if / else statements
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void InsertSite_True(object sender, EventArgs e)
        {
            try
            {
                // hides the emissions fields
                FacilityTB.Visible = false;
                LatitudeTB.Visible = false;
                LongitudeTB.Visible = false;
                YearTB.Visible = false;
                EmissionsTB.Visible = false;
                SubmitHM.Visible = false;
                CancelTB.Visible = false;
                br3.Visible = false;
                br4.Visible = false;


                // shows the site fields
                LatitudeText.Visible = true;
                LongitudeText.Visible = true;
                SubmitSite.Visible = true;
                CancelSiteInsert.Visible = true;

                br1.Visible = true;
                br2.Visible = true;
                br6.Visible = true;

                if (HeatMapCheckbox.Checked == true)
                {
                    PopulateHeatMap();
                }

                if (OverlayCheckbox.Checked == true)
                {
                    Legend.Visible = true;
                    DivOverlayList.Visible = true;
                    sliderstuff.Visible = true;
                    br5.Visible = true;
                    br8.Visible = false;
                }

                else if (OverlayCheckbox.Checked == false)
                {
                    DivOverlayList.Visible = false;
                    sliderstuff.Visible = false;
                    Legend.Visible = false;
                    br5.Visible = false;
                    br8.Visible = true;
                }
                if (DropDownGenusList.SelectedItem.Value == "All")
                {
                    // call PopulateAllMarkers function
                    PopulateAllMarkers();
                }
                else if (DropDownGenusList.SelectedItem.Value == "Empty")
                {

                }
                else
                {
                    PopulateGenusMarkers();

                }
            }
            catch (Exception)
            {

            }
        }


        /// <summary>
        /// Event handler to tell the website what to do when the dropdown menu for selecting genus is changed
        /// Vers     Date           Coder       Comments
        /// 1.0      2021-02-20     Andy        Initial
        /// 1.1      2021-03-02     Taylor      Implemented PopulateMarkers function
        /// 1.2      2021-03-18     Allan       Added in the emissions forms visibility change and the new option of "Clear" for the drop down
        /// 1.3      2021-03-24     Taylor      De-nested if / else
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Selection_Change(object sender, EventArgs e)
        {
            try
            {
                if (HeatMapCheckbox.Checked == true)
                {
                    PopulateHeatMap();
                }

                //Populates the map with all genera when "ALL" is selected
                if (DropDownGenusList.SelectedItem.Value == "All")
                {
                    // call PopulateAllMarkers function
                    PopulateAllMarkers();
                }
                else if (DropDownGenusList.SelectedItem.Value == "Empty")
                {

                }
                else
                {
                    PopulateGenusMarkers();

                }
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Tells the website what to do when the "Submit" button is clicked after a user enters their new site location
        /// Vers     Date           Coder       Comments
        /// 1.0      2021-02-20     Andy        Initial
        /// 1.1      2021-03-01     Andy        Updated to use new sp insert
        /// 1.2      2021-03-03     Taylor      Added try/catch
        /// 1.3      2021-03-11     Allan       Moved the converts into the try catch and added the error string
        ///                                     and added the error string since InsertNewAnimalMarker is now
        ///                                     a public string instead of void.
        /// 1.4     2021-03-24      Taylor      Moved else / if for map settings into finally block
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SubmitSite_Click(object sender, EventArgs e)
        {
            try
            {
                // try / catch block is used for insert to display SQL errors if data is invalid
                string ERROR;
                try
                {
                    double LatitudeInput = Math.Round(Convert.ToDouble(LatitudeText.Text), 8);
                    double LongitudeInput = Math.Round(Convert.ToDouble(LongitudeText.Text), 8);
                    ERROR = DB.InsertNewAnimalMarker(true, LatitudeInput, LongitudeInput, null, null, null, null, null, null, null, null);
                    // if insert was successful, alert pops up to tell the user
                    ShowAlert(ERROR);

                    // add the new site to the master list
                    MasterSiteMarkerList = DB.GetAllSites();
                }
                catch (Exception E)
                {
                    // if there are any validity errors with the data, the SQL error appears in alert, and site is not added
                    ShowAlert(Server.HtmlEncode(E.Message) + " Site was not added");
                }

                // re-set the map to how the user had it 
                finally
                {
                    if (HeatMapCheckbox.Checked == true)
                    {
                        PopulateHeatMap();
                    }

                    if (OverlayCheckbox.Checked == true)
                    {
                        Legend.Visible = true;
                        DivOverlayList.Visible = true;
                        sliderstuff.Visible = true;
                        br5.Visible = true;
                        br8.Visible = false;
                    }
                    else if (OverlayCheckbox.Checked == false)
                    {
                        DivOverlayList.Visible = false;
                        sliderstuff.Visible = false;
                        Legend.Visible = false;
                        br5.Visible = false;
                        br8.Visible = true;
                    }
                    if (DropDownGenusList.SelectedItem.Value == "All")
                    {
                        // call PopulateAllMarkers function
                        PopulateAllMarkers();
                    }
                    else if (DropDownGenusList.SelectedItem.Value == "Empty")
                    {

                    }
                    else
                    {
                        PopulateGenusMarkers();

                    }


                }
                //Resets the text fields after submission
                LatitudeText.Text = "";
                LongitudeText.Text = "";
            }
            catch (Exception)
            {

            }
        }


    /// <summary>
    /// A cancel button so the user can cancel inserting/creating a new site.
    /// 
    /// 1.0     2021-03-17      Allan       Initial
    /// 1.1     2021-03-24      Taylor      De-nested if / else
    /// </summary>
    /// 

    protected void CancelSiteInsert_Click(object sender, EventArgs e)
        {
            try
            {
                // hides the site fields
                LatitudeText.Visible = false;
                LongitudeText.Visible = false;
                SubmitSite.Visible = false;
                CancelSiteInsert.Visible = false;
                br1.Visible = false;
                br2.Visible = false;
                br6.Visible = false;

                if (HeatMapCheckbox.Checked == true)
                {
                    PopulateHeatMap();
                }

                if (OverlayCheckbox.Checked == true)
                {
                    Legend.Visible = true;
                    DivOverlayList.Visible = true;
                    sliderstuff.Visible = true;
                    br5.Visible = true;
                    br8.Visible = false;
                }
                else if (OverlayCheckbox.Checked == false)
                {
                    DivOverlayList.Visible = false;
                    sliderstuff.Visible = false;
                    Legend.Visible = false;
                    br5.Visible = false;
                    br8.Visible = true;
                }
                if (DropDownGenusList.SelectedItem.Value == "All")
                {
                    // call PopulateAllMarkers function
                    PopulateAllMarkers();
                }
                else if (DropDownGenusList.SelectedItem.Value == "Empty")
                {

                }
                else
                {
                    PopulateGenusMarkers();
                }
            }
            catch (Exception)
            {

            }

        }

        /// <summary>
        /// A cancel button so the user can cancel inserting/creating a new emissions site.
        /// 
        /// 1.0     2021-03-17      Allan       Initial
        /// 1.1     2021-03-24      Taylor      De-nested if / else
        ///
        /// </summary>
        /// 

        protected void CancelSiteInsert_ClickE(object sender, EventArgs e)
        {
            try
            {
                // shows the emissions fields
                FacilityTB.Visible = false;
                LatitudeTB.Visible = false;
                LongitudeTB.Visible = false;
                YearTB.Visible = false;
                EmissionsTB.Visible = false;
                SubmitHM.Visible = false;
                CancelTB.Visible = false;
                br3.Visible = false;
                br4.Visible = false;
                br6.Visible = false;
                if (HeatMapCheckbox.Checked == true)
                {
                    PopulateHeatMap();
                }

                if (OverlayCheckbox.Checked == true)
                {
                    Legend.Visible = true;
                    DivOverlayList.Visible = true;
                    sliderstuff.Visible = true;
                    br5.Visible = true;
                    br8.Visible = false;
                }
                else if (OverlayCheckbox.Checked == false)
                {
                    DivOverlayList.Visible = false;
                    sliderstuff.Visible = false;
                    Legend.Visible = false;
                    br5.Visible = false;
                    br8.Visible = true;
                }
                if (DropDownGenusList.SelectedItem.Value == "All")
                {
                    // call PopulateAllMarkers function
                    PopulateAllMarkers();
                }
                else if (DropDownGenusList.SelectedItem.Value == "Empty")
                {

                }
                else
                {
                    PopulateGenusMarkers();
                }
            }
            catch (Exception)
            {

            }
        }



        //Unable to make flower boxes for these functions. These 6 functions are all intiated in markers.js
        //they take variables from markers.js and with some translations send them to the various function in dataAccess

        // this takes the int from a javascript function and sends it to DB.DeleteSiteByID
        //
        //1.0   2021-03-03      Allan       Initial
        //1.1   2021-03-09      Taylor      Added try / catch
        //
        [System.Web.Services.WebMethod()]
        public static string DeleteSite(int ID)
        {
            try
            {
                DB.DeleteSiteByID(Convert.ToInt32(ID));
                return "Site " + ID + " successfully deleted from Database";
            }
            catch (Exception E)
            {
                return E.Message + "\nSite " + ID + " was not deleted from Database";
            }
        }

        // this takes the int of an animal ID from a javascript function and sends it to DB.DeleteAnimalMarkerByID
        //
        //1.0   2021-03-03      Allan       Initial
        //1.1   2021-03-09      Taylor      Added try / catch
        //
        [System.Web.Services.WebMethod()]
        public static string DeleteAnimalSighting(int ID)
        {
            try
            {
                DB.DeleteAnimalMarkerbyID(Convert.ToInt32(ID));
                return "Animal " + ID + " successfully deleted from Database";
            }
            catch (Exception E)
            {
                return E.Message + "\nAnimal " + ID + " was not deleted from Database";
            }
        }

        // Purpose: This function takes a bunch of variables of an animal sighting from a javascript function and sends it to DB.InsertNewAnimalMarker
        // Since DB.InertNewAnimalMarker needs more data than is available to it it uses DB.AnimalInfo with the CommonName to extract the
        // other data like genus, species, and speciesCode
        //
        // Vers  Date            Coder       Comments
        // 1.0   2021-03-03      Allan       Initial
        // 1.1   2021-03-05      Taylor      Added try / catch
        // 1.2   2021-03-11      Allan       Added the error string since InsertNewAnimalMarker returns a string now
        //                                   the error string will return the error message or will indicate the successful
        //                                   addition of an animal sighting
        //
        [System.Web.Services.WebMethod()]
        public static string AddAnimalSighting(double Lat, double Long, int SiteNumber, string Year, string Month, string Day, string CommonName)
        {

            AnimalSightings AddedAnimal = DB.AnimalInfo(CommonName)[0];
            string Genus = AddedAnimal.Genus;
            string SpeciesCode = AddedAnimal.SpeciesCode;
            string Species = AddedAnimal.Species;
            bool InsertChoice = false;
            string ERROR;

            // try / catch block 
            try
            {
                ERROR = DB.InsertNewAnimalMarker(InsertChoice, Lat, Long, SiteNumber, Convert.ToInt32(Year), Convert.ToInt32(Month), Convert.ToInt32(Day), SpeciesCode, Genus, Species, CommonName);

                return ERROR;


            }
            catch (Exception E)
            {

                return E.Message + "\nAnimal sighting was not added to the database";
            }
        }

        // Purpose: This function accepts multiple variables from markers.js and sends them to DB.InsertNewAnimalMaker
        //  Unlike AddAnimalSighting it recieves more data from the user in markers.js and therefor doesn't need
        //  to use DB.AnimalInfo to run DB.InserNewAnimalMaker
        //
        // Vers  Date            Coder       Comments
        //
        // 1.0   2021-03-03      Allan       Initial
        // 1.1   2021-03-05      Taylor      Added try / catch
        // 1.2   2021-03-11      Allan       Added the error string since InsertNewAnimalMarker returns a string now
        //                                  the error string will return the error message or will indicate the successful
        //                                  addition of an animal sighting

        [System.Web.Services.WebMethod()]
        public static string AddAnimalSightingUnknown(double Lat, double Long, int SiteNumber, string Year, string Month, string Day, string SpeciesCode, string Genus, string Species, string CommonName)
        {
            string ERROR;
            bool InsertChoice = false;
            try
            {
                ERROR = DB.InsertNewAnimalMarker(InsertChoice, Lat, Long, SiteNumber, Convert.ToInt32(Year), Convert.ToInt32(Month), Convert.ToInt32(Day), SpeciesCode, Genus, Species, CommonName);
                return ERROR;
            }
            catch (Exception E)
            {

                return E.Message + "\nCommon Name: " + CommonName + " sighting was not added to the database";
            }

        }

        // Purpose: This function accepts multiple variables from a function in markres.js and sends it to
        // DB.UpdateAnimalMarker for updating Site information.
        //
        // 1.0   2021-03-03      Allan       Initial
        // 1.1   2021-03-09      Taylor      Added try / catch
        // 1.2   2021-03-11      Allan       Added the error string since UpdateAnimalMarker returns a string now.
        //                                   The error string will return the error message or will indicate the successful
        //                                   update of a site
        //
        [System.Web.Services.WebMethod()]
        public static string UpdateSite(int ID, double Lat, double Long, int SiteNumber)
        {
            string ERROR;
            try
            {
                ERROR = DB.UpdateAnimalMarker(true, ID, Lat, Long, SiteNumber, null, null, null, null, null, null, null, null);
                return "Site Number: " + SiteNumber +
                       "\nLatitude : " + Lat +
                       "\nLongitude: " + Long +
                       "\n successfully updated in the database";
            }
            catch (Exception E)
            {
                return E.Message + "\nSite Number: " + SiteNumber + " was not updated in the database";

            }
        }

        // Purpose: This function accepts multiple variables from markers.js and sends them to DB.UpdateAnimalMaker
        // It also extracts data from DB.AnimalInfo before sending all the data necessary to run DB.UpdateAnimalMarker
        //
        // 1.0   2021-03-03      Allan       Initial
        // 1.1   2021-03-09      Taylor      Added try / catch and LINQ instead of SQL
        // 1.2   2021-03-11      Allan       Added the error string since UpdateAnimalMarker returns a string now.
        //                                   The error string will return the error message or will indicate the successful
        //                                   update of an animal sighting
        //
        [System.Web.Services.WebMethod()]
        public static string UpdateAnimalSighting(int ID, double Lat, double Long, int SiteNumber, int AnimalID, string CommonName, int Year, int Month, int Day)
        {
            string ERROR;
            AnimalSightings AS = DB.AnimalInfo(CommonName)[0];
            //AnimalSightings AS = (AnimalSightings)AllNames.Select(a => a.CommonName == Common);
            try
            {
                ERROR = DB.UpdateAnimalMarker(false, ID, Lat, Long, SiteNumber, AnimalID, Year, Month, Day, AS.SpeciesCode, AS.Genus, AS.Species, CommonName);
                return ERROR;

            }
            catch (Exception E)
            {

                return E.Message + "\nCommon Name: " + CommonName + " sighting was not updated in the database";
            }
        }

        // Purpose: This function accepts multiple variables from markers.js and sends them to DB.UpdateAnimalMaker
        // It doesn't extract data from DB.AnimalInfo since this is a new animal sighting
        // it accepts all the variables neccessary to run DB.UpdateAnimalMarker from the user in Marker.Js
        //
        // 1.0   2021-03-03      Allan       Initial
        // 1.1   2021-03-09      Taylor      Added try / catch
        // 1.2   2021-03-11      Allan       Added the error string since UpdateAnimalMarker returns a string now.
        //                                   The error string will return the error message or will indicate the successful
        //                                   update of an animal sighting

        [System.Web.Services.WebMethod()]
        public static string UpdateAnimalSightingUnknown(int ID, double Lat, double Long, int SiteNumber, int AnimalID, string Common, string Genus, string Species, string SpeciesCode, int Year, int Month, int Day)
        {
            string ERROR;
            try
            {

                ERROR = DB.UpdateAnimalMarker(false, ID, Lat, Long, SiteNumber, AnimalID, Year, Month, Day, SpeciesCode, Genus, Species, Common);
                return ERROR;

            }
            catch (Exception E)
            {

                return E.Message + "\nCommon Name: " + Common + " sighting was not updated in the database";
            }

        }

        /// <summary>
        /// Purpose: takes a double 'o' from overlay.js and changes the public double opcaity to the value of o.
        /// 
        /// Vers    Date        Coder   Comments
        /// 1.0     2021-03-13  Allan   Initial
        /// 
        /// </summary>
        /// <param name="o"></param>

        [System.Web.Services.WebMethod()]
        public static void OpacUpdate(double o)
        {
            Opacity = o;

        }

        /// <summary>
        /// Purpose: takes an int ID and sends it to the DataAccess layer to delete an air particulate site by the ID
        /// 
        /// Vers    Date        Coder       Comments
        /// 1.0     2021-03-19  Allan       Initial
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>

        [System.Web.Services.WebMethod()]
        public static string DeleteSiteResp(int ID)
        {
            try
            {
                DB.DeleteSiteRespByID(Convert.ToInt32(ID));
                return "Site " + ID + " successfully deleted from Database";
            }
            catch (Exception E)
            {
                return E.Message + "\nSite " + ID + " was not deleted from Database";
            }

        }


    }
}