<%@ Page Title="" Language="C#" MasterPageFile="~/Radagast.Master" AutoEventWireup="true" CodeBehind="MapPage.aspx.cs" Inherits="Radagast_Website.MapPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
    <script src="Styles/js/jquery.min.js"></script>
    <script src="Styles/js/jquery.poptrox.min.js"></script>
    <script src="Styles/js/browser.min.js"></script>
    <script src="Styles/js/breakpoints.min.js"></script>
    <script src="Styles/js/util.js"></script>
    <script src="Styles/js/main.js"></script>

    <!-- script sources for the slider funtion-->
    <script src="https://code.jquery.com/jquery-1.10.2.js"></script>
    <script src="https://code.jquery.com/ui/1.10.4/jquery-ui.js"></script>
    <link href="https://code.jquery.com/ui/1.10.4/themes/ui-lightness/jquery-ui.css" rel="stylesheet">


    <div id="main">
        <div class="row">
            <div class="col-3">
            <h3>Alberta, Canada</h3>
        
                <asp:CheckBox ID="HeatMapCheckbox" runat="server" OnCheckedChanged="HeatMapCheckboxChange" AutoPostBack="true" Text="HeatMap" />
                <asp:CheckBox ID="OverlayCheckbox" runat="server" OnCheckedChanged="OverlayCheckboxChange" AutoPostBack="true" Text="Overlay" />

                <!-- HTML for slider-->
                <div id="sliderstuff" runat="server" visible="false">
                    <div id="slider-1"></div>
                    <p>
                        Use the slider to change the overlay opacity
<%--         <label for="minval">Opacity:</label>--%>
                        <input type="text" id="minval" style="border: 0; color: #b9cd6d; font-weight: bold;">
                    </p>
                </div>

                <!--Dropdown menu that has the values for the different genera present in the dataset, and can be used to pull specific genera markers onto the map-->
                <div style="line-height: 50%;">
                    <br>
                </div>
                <asp:DropDownList ID="DropDownGenusList" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="Selection_Change" runat="server">
                    <asp:ListItem Text="" Value="DefaultSelection"> - Select Markers - </asp:ListItem>
                    <%--                <asp:ListItem Text="" Value="New"> -- Add New Site Location -- </asp:ListItem>--%>
                    <asp:ListItem Text="" Value="Empty"> - Clear map sites - </asp:ListItem>
                    <asp:ListItem Value="All"> All </asp:ListItem>
                </asp:DropDownList>
                <asp:TextBox ID="LastSelection" runat="server" Visible="false"></asp:TextBox>
                <div style="line-height: 50%;">
                    <br>
                </div>

                <div style="line-height: 50%;">
                    <br id="br7" runat="server" visible="false" />
                </div>
                <asp:Button ID="InsertSite" runat="server" Text="Insert new Site" OnClick="InsertSite_True" />
                <asp:Button ID="InsertHM" runat="server" Text="Insert Emissions" OnClick="InsertHM_True" />

                <!--Textboxes and Submit button that allow the user to enter a new site location-->
                <div style="line-height: 50%;">
                    <br id="br8" runat="server" visible="true" />
                </div>
                <div style="line-height: 50%;">
                    <br id="br6" runat="server" visible="false" />
                </div>
                <asp:TextBox ID="LatitudeText" runat="server" Visible="false" placeholder="Latitude (53° - 60°)"></asp:TextBox>
                <asp:TextBox ID="LongitudeText" runat="server" Visible="false" placeholder="Longitude (-120° - -110°)"></asp:TextBox>
                <div style="line-height: 50%;">
                    <br id="br1" runat="server" visible="false" />
                </div>
                <asp:Button ID="SubmitSite" runat="server" Text="Submit" OnClick="SubmitSite_Click" Visible="false" />
                <asp:Button ID="CancelSiteInsert" runat="server" Text="Cancel" OnClick="CancelSiteInsert_Click" Visible="false" />
                <div style="line-height: 50%;">
                    <br id="br2" runat="server" visible="false" />
                </div>



                <!--Textboxes and Submit button that allow the user to enter  new heat map data-->
                <asp:TextBox ID="FacilityTB" runat="server" Visible="false" placeholder="Facility Name"></asp:TextBox>
                <asp:TextBox ID="LatitudeTB" runat="server" Visible="false" placeholder="Latitude (53° - 60°)"></asp:TextBox>
                <asp:TextBox ID="LongitudeTB" runat="server" Visible="false" placeholder="Longitude (-120° - -110°)"></asp:TextBox>
                <asp:TextBox ID="YearTB" runat="server" Visible="false" placeholder="Year"></asp:TextBox>
                <asp:TextBox ID="EmissionsTB" runat="server" Visible="false" placeholder="Weight (tonnes)"></asp:TextBox>
                <div style="line-height: 50%;">
                    <br id="br3" runat="server" visible="false" />
                </div>
                <asp:Button ID="SubmitHM" runat="server" Text="Submit" OnClick="SubmitHM_Click" Visible="false" />
                <asp:Button ID="CancelTB" runat="server" Text="Cancel" OnClick="CancelSiteInsert_ClickE" Visible="false" />
                <div style="line-height: 50%;">
                    <br id="br4" runat="server" visible="false" />
                </div>

                
            </div>


            <div class="col-9">
                <!-- DIVS for Map data-->
                <div id="DivHeatMapList" runat="server"></div>
                <div id="DivPolylineList" runat="server"></div>
                <div id="DivMarkerList" runat="server"></div>
                <div id="DivOverlayList" runat="server"></div>


                <div id="Map" style="width: 100%; height: 100%;"></div>
                <div style="line-height: 50%;">
                    <br id="br5" runat="server" visible="false" />
                </div>
            

            <%--            HTML for legend for overlay map--%>
            <div id="Legend" class="Legend" runat="server" visible="false">
                <p class="LTitle">2005 Natural Regions and Subregions of Alberta</p>

                <div class="row">

                    <div class="column">
                        <p class="L">Boreal Forest Natural Region</p>
                        <dl>
                            <dt class="CentralMW"></dt>
                            <dd>Central Mixedwood</dd>
                            <dt class="DryMW"></dt>
                            <dd>Dry Mixedwood</dd>
                            <dt class="NorthernMW"></dt>
                            <dd>Northern Mixedwood</dd>
                            <dt class="BorealSub"></dt>
                            <dd>Boreal Subarctic</dd>
                            <dt class="PeaceAthabasc"></dt>
                            <dd>Peace-Athabasca Delta</dd>
                            <dt class="LowerBoreal"></dt>
                            <dd>Lower Boreal Highlands</dd>
                            <dt class="UpperBoreal"></dt>
                            <dd>Upper Boreal Highlands</dd>
                            <dt class="AthabascP"></dt>
                            <dd>Athabasca Plain</dd>
                        </dl>

                    </div>

                    <div class="column">
                        <p class="L">Rocky Mountain Natural Region</p>
                        <dl>
                            <dt class="Alpine"></dt>
                            <dd>Central Alpine</dd>
                            <dt class="Subalpine"></dt>
                            <dd>Subalpine</dd>
                            <dt class="Montane"></dt>
                            <dd>Montane</dd>
                        </dl>

                        <p class="L">Foothills Natural Region</p>
                        <dl>
                            <dt class="UpperFoot"></dt>
                            <dd>Upper Foothills</dd>
                            <dt class="LowerFoot"></dt>
                            <dd>Lower Foothills</dd>
                        </dl>
                    </div>

                    <div class="column">
                        <p class="L">Canadian Shield Natural Region</p>
                        <dl>
                            <dt class="Kazan"></dt>
                            <dd>Kazan Uplands</dd>
                        </dl>

                        <p class="L">Parkland Natural Region</p>
                        <dl>
                            <dt class="Foothills"></dt>
                            <dd>Foothills Parkland</dd>
                            <dt class="PeaceRiver"></dt>
                            <dd>Peace River Parkland</dd>
                            <dt class="CentralPark"></dt>
                            <dd>Central Parkland</dd>
                        </dl>
                    </div>
                    <div class="column">

                        <p class="L">Grassland Natural Region</p>
                        <dl>
                            <dt class="DryMixed"></dt>
                            <dd>Dry Mixedgrass</dd>
                            <dt class="FootFescue"></dt>
                            <dd>Foothills Fescue</dd>
                            <dt class="NorthFescue"></dt>
                            <dd>Northern Fescue</dd>
                            <dt class="MixGrass"></dt>
                            <dd>Mixedgrass</dd>
                        </dl>
                    </div>
                </div>

            </div>
                <div></div>
                </div>
            </div>

 </div>
            <!-- The modal baseline stuff -->
            <div id="myModal" class="modal">
                <div class="modal2-content">
                    <span class="close">&times;</span>
                    <p id="demo">Some text in the Modal..</p>
                </div>
            </div>

            <script src="MasterJavaScript/initializeMap.js"></script>
            <script src="MasterJavaScript/Modal.js"></script>
            <script src="MasterJavaScript/markers.js"></script>
            <script src="MasterJavaScript/Overlay.js"></script>
            <script src="MasterJavaScript/heatMap.js"></script>
            <script src="MasterJavaScript/Polylines.js"></script>

            <script src="https://maps.googleapis.com/maps/api/js?key=AIzaSyAX6haxPnf_GlOOJLMl4XX-_y9id7NBzh8&libraries=visualization&callback=myMap" async defer></script>

       
</asp:Content>
