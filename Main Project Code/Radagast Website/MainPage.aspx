<%@ Page Title="" Language="C#" MasterPageFile="~/Radagast.Master" AutoEventWireup="true" CodeBehind="MainPage.aspx.cs" Inherits="Radagast_Website.MainPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
    <div id="main">
        <section id="one">
            <h4>Team Members:</h4>
            <p>Jill Sick, Allan Vickers, Andy Chand, & Taylor Shewchuk</p>
        </section>

        <section id="two">
            <h2>Vision Statement:</h2>
            <p>To showcase rare Alberta animal sightings and their relation to the growing industrial sectors and Alberta's ecological zones.</p>

            <p>
                The purpose of this project is to show the relationship between rare animals of Alberta (ABMI) and respirable particulate emission sources (Government of Canada).
			With the use of SQL data-management, C#/Javascript/HTML coding, and two data sources we will be able to populate and allow user interaction on a Google Map
			to expand the users knowledge and allow for user additions to the database.
            </p>

            <p>
                Our map will showcase animal sightings which have been collected at designated sites as well as air particulate emissions displayed as a heatmap.
			Adding new sites or emissions data is as easy as clicking the "Insert New Site" or "Insert Emissions" buttons respectively, then entering the Latitude, Longitude, and 
			Weight of emissions (if applicable). </p>

                <p>
                    Updating or deleting animal sightings, sites, or emissions data, can be done by clicking on a marker, or the heatmap and following the buttons in the pop-up modal. </p>
            <p>
                The heatmap, and an overlay of Alberta's ecological zones can be toggled on or off with a check box. The overlay can also be updated with an opacity slider.
                Polylines show the connection between visible sites. These are automatically created, updated, or deleted through the site marker's create, update, or delete.
            </p>
        </section>

        <!-- The modal baseline stuff -->
        <div id="myModal" class="modal">
            <div class="modal-content2">
                <span class="close">&times;</span>
                <p id="demo">Some text in the Modal..</p>
            </div>
        </div>

        <script src="MasterJavaScript/Modal.js"></script>
        <script src="MasterJavaScript/connectionString.js"></script>

    </div>
</asp:Content>
