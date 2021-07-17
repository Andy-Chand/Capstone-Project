/// Purpose: the script of all functions related to the Map and MapPage.
/// Date: 2021/03/03
/// Coder: Andy
/// Version history:
/// 1.0                 Andy    Initial
/// 1.1     2021-03-03  Allan   The event listeners for the markers and the buttons in the infoWindows and Modals
/// 1.2     2021-03-09  Taylor  Added onSuccess / onError to most functions
///1.3      2021-04-09  Taylor  Added map parameter to SitePolylines call


var MarkerList = [];
var SitePolylinesList = [];

// Places site markers on map
function PlaceMarkers(m) {
    //ClearMap();

    MarkerList = document.getElementById("DivMarkerList");
    var childDivs = document.getElementById('DivMarkerList').getElementsByTagName('DIV');
    var infowindow = new google.maps.InfoWindow();
    var marker, i;

    for (i = 0; i < childDivs.length; i++) {
        var ID = childDivs[i].getAttribute("SiteID");
        var SN = childDivs[i].getAttribute("SiteNumber");
        var position = new google.maps.LatLng(childDivs[i].getAttribute("Latitude"),
            childDivs[i].getAttribute("Longitude"));

        const image =
        {
            url: "../../MapImages/Marker.png",
            scaledSize: new google.maps.Size(25, 30),    // scaled size
            origin: new google.maps.Point(0, 0),        // origin
            anchor: new google.maps.Point(10, 25)      // anchor
        };

        var marker = new google.maps.Marker
            ({
                position: position,
                draggable: false,
                ID: ID,
                SN: SN,
                map: m,
                icon: image,
                title: "Site Number: " + SN
            });

        SitePolylinesList.push(marker.position)
    
        //createMarker(ID, SN, position);

        //Creates an event listener for each marker. On click it produces an info window with all the marker info on it.
        //When 'All' is selected it shows all the animal sightings for each site. When a genus is selected it only shows the animal information.
        google.maps.event.addListener(marker, 'click', (function (marker, i) {

            return function () {
                //if statement figuring out if Genus or All has been selected. All has "Sightings" Genus has Animals
                //when a marker is clicked the remainder of this code is activated based off that click

                if (childDivs[i].getAttribute("Sighting0") == null && childDivs[i].getAttribute("Genus") != null) {

                    //childivs is a list of Sites. i = the specifc site chosen by the user.
                    //each site has a list of animal sightings made in SiteSightings. this loop using j pulls out
                    //each animal in the site and turns it into an HTML friendly attribute
                    var string1 = ''
                    for (j = 0; j < childDivs[i].getAttribute("AnimalCount"); j++) {

                        //i made the animals in the site not only diplayable but also clickable.
                        string1 = string1 + '<p onclick="animalWindow(' + j + ',' + i + ')">' + childDivs[i].getAttribute("Animal" + j.toString()) + '</p>'
                    }
                    infowindow.setContent(
                        //the info window contains three site related button: Update Site, Delete Site and
                        //and animal sighting to site. this pulls the attributes made in mapPage.aspx.cs and converts them into 
                        //html p classes.

                        '<button class="button" type="button" onclick="updateSite(' + i + ')">Update Site Information</button>' +
                        '<button class="button" type="button" onclick="deletingSite(' + childDivs[i].getAttribute("SiteID") + ')">Delete Site</button>' +
                        '<button class="button" type="button" onclick="AddAnimalSighting(' + i + ')">Add Animal Sighting to Site</button>' +
                        '<br/>' +
                        '<br/>' +
                        '<p><b>Site ID: </b>' + childDivs[i].getAttribute("SiteID") + '</p>' +
                        '<p><b>Site Number: </b>' + childDivs[i].getAttribute("SiteNumber") + '</p>' +
                        '<p><b>Latitude: </b>' + childDivs[i].getAttribute("Latitude") + '</p>' +
                        '<p><b>Longitude: </b>' + childDivs[i].getAttribute("Longitude") + '</p>' +
                        '<p><b>Genus: </b>' + childDivs[i].getAttribute("Genus") + '</p>' +
                        '<p><b>Animal Sightings: </b>' + childDivs[i].getAttribute("AnimalCount") + '</p>' +
                        string1


                    );
                }
                //here is a very simimlar to the genus script except it uses the sightings attributes
                else {
                    var string1 = ''
                    for (j = 0; j < childDivs[i].getAttribute("SightingCount"); j++) {

                        string1 = string1 + '<p onclick="animalWindow(' + j + ',' + i + ')">' + childDivs[i].getAttribute("Sighting" + j.toString()) + '</p>'
                    }
                    infowindow.setContent(
                        '<button class="button" type="button" onclick="updateSite(' + i + ')">Update Site Information</button>' +
                        '<button class="button" type="button" onclick="deletingSite(' + childDivs[i].getAttribute("SiteID") + ')">Delete Site</button>' +
                        '<button class="button" type="button" onclick="AddAnimalSighting(' + i + ')">Add Animal Sighting to Site</button>' +
                        '<br/>' +
                        '<br/>' +
                        '<p><b>Site ID: </b>' + childDivs[i].getAttribute("SiteID") + '</p>' +
                        '<p><b>Site Number: </b>' + childDivs[i].getAttribute("SiteNumber") + '</p>' +
                        '<p><b>Latitude: </b>' + childDivs[i].getAttribute("Latitude") + '</p>' +
                        '<p><b>Longitude: </b>' + childDivs[i].getAttribute("Longitude") + '</p>' +
                        '<p><b>Animal Sightings: </b>' + childDivs[i].getAttribute("SightingCount") + '</p>' +
                        string1

                    );
                }
                infowindow.open(map, marker);
            }
        })
            (marker, i)); 
    }
    SitePolylines(SitePolylinesList, m)
}


///Purpose: this is the deleteSite function. It is engaged when the user presses the Delete Site button in the
/// info window of a marker.
///1.0      Allan   2021-03-03 Initial
///1.1      Taylor  2021-03-09  Added onSuccess / onError
///
function deletingSite(i) {
    //it uses a confirm so that the user can be sure they wish to delete the site
    if (confirm('About to delete site ' + i + ' press ok to confirm.')) {

        //using Page.Methods this site can communicate with MapPage.aspx.cs DeleteSite sending i
        //which in this case is the SiteID
        PageMethods.DeleteSite(i, onSuccess, onError);

        //in order for the user to see the effect the page must be reloaded and the markers re-issued
        location.reload();
        PlaceMarkers();

        // onSuccess and onError functions fire depending on outcome of try / catch
        function onSuccess(result) {
            // produces an alert with a successful insert message
            alert(result);
        }
        function onError(result) {
            // produces an alert with an unsuccessful insert message
            alert(result)
        }

    }

    //if the user decides not to delete and hits the cancel button no C# function is initiated.
    else {
        alert('Site ' + i + ' not deleted');
    }


    location.reload();
}


///Purpose: This is the animal window. When an animal marker within the infoWindow is selected two variables are sent to
///     this function: the list index for the site in childDivs (i) and the list index for the sighting with the site (j)
///     (i = q and j = p .. sorry if thats confusing..)
///1.0      Allan   2021-03-03  Initial
///1.1      Allan   2021-03-21  Added more Animal Sighting information to the modal
///
function animalWindow(p, q) {

    //from the two indexes we can pull out the animal name and animal id by spliting j (or in this case p)
    // and using q to represent the site index in a new childDivs thats made from the same DivMarkerList

    var childDivs = document.getElementById('DivMarkerList').getElementsByTagName('DIV');
    if (childDivs[q].getAttribute("Sighting0") == null && childDivs[q].getAttribute("Genus") != null) {
        var r = childDivs[q].getAttribute("Animal" + p.toString());
    }
    else if (childDivs[q].getAttribute("Sighting0") != null) {
        var r = childDivs[q].getAttribute("Sighting" + p.toString());
    }

    var s = r.split(": ");
    var year = childDivs[q].getAttribute("Year" + p.toString());
    var month = childDivs[q].getAttribute("Month" + p.toString());
    var day = childDivs[q].getAttribute("Day" + p.toString());
    var date = year + '/' + month + '/' + day;
    var Latitude = childDivs[q].getAttribute("Latitude");
    var Longitude = childDivs[q].getAttribute("Longitude");
    var Coordinates = Latitude + ', ' + Longitude;
    var SiteID = childDivs[q].getAttribute("SiteID");


    //now that the variables have been determined we go ahead and create a Modal. Inside the modal are only two buttons: Update Animal
    //Sighting Info and Delete Animal sighting. They both engage different functions and take different variables. Update uses the same
    //p and q (and therefor j and i respectively) to be used in the updateAnimalSighting function. Delete takes only the animal ID that 
    //we split away from the animal name above.

    var modal = document.getElementById("myModal");
    document.getElementById("demo").innerHTML =
        '<p><b>Animal ID: </b>' + s[0] + '</p>' +
        '<p><b>Common Name: </b>' + s[1] + '</p>' +
    '<p><b>Date of sighting:</b>(y/m/d) ' + date + '</p>' +
    '<p><b>Site ID = </b>' + SiteID+'</p>'+
    '<p><b>Coordinates: </b>' + Coordinates+'</p>'+
        '<p>Would you like to delete or update this animal sighting?</p>' +
        '<button class="button" type="button" onclick="updateAnimalSighting(' + p + ',' + q + ')"> Update Animal Sighting Information</button > ' +
        '<button class="button" type="button" onclick="deletingAnimalSighting(' + s[0] + ')">Delete Animal Sighting</button>';


    modal.style.display = "block";

}

///Purpose: this is the deleteAnimalSighting function. It is engaged when the user presses the Delete Animal Sighting button in the
/// modal of the animal sighting
///1.0      Allan   2021-03-03   Initial
///1.1      Taylor  2021-03-09   Added onSuccess / onError
///

function deletingAnimalSighting(i) {
    if (confirm('About to delete animal sighting ' + i + ' press ok to confirm.')) {
        alert('Animal sighting ' + i + ' deleted');

        //similarly to delete site this funciton gives the user an option to stop the delete from happening and if not sends
        //the animal id to mappage.aspx.cs deleteanimalsighting and the refreshes the markers.
        PageMethods.DeleteAnimalSighting(i, onSuccess, onError);
        location.reload();
        PlaceMarkers();

        // onSuccess and onError functions fire depending on outcome of try / catch
        function onSuccess(result) {
            // produces an alert with a successful insert message
            alert(result);
        }
        function onError(result) {
            // produces an alert with an unsuccessful insert message
            alert(result)
        }
    }
    else {
        alert('Animal sighting ' + i + ' not deleted');
    }


    location.reload();
}


///Purpose: this is the AddAnimalSighting function. It is engaged when the user presses the Animal Sighting button in the
/// info window of a marker.
///1.0      Allan   2021-03-03   Initial
///1.1      Taylor  2021-03-05   Added onSuccess, onError functions to catch errors. Removed checks
///
function AddAnimalSighting(b) {
    /// when the user presses the add animal sighting marker in the info window of a site they are sending the index
    /// for the site over to this function. DivMarkerList is pulled again and the index number is used to pull the various
    /// attributes made in the aspx.cs layer

    var childDivs = document.getElementById('DivMarkerList').getElementsByTagName('DIV');
    var SiteID = childDivs[b].getAttribute("SiteID");
    var Latitude = childDivs[b].getAttribute("Latitude");
    var Longitude = childDivs[b].getAttribute("Longitude");
    var SiteNumber = childDivs[b].getAttribute("SiteNumber");
    var GoodToGo = false;

    // this is a very large loop with many loops with it. when goodtogo is true it ends and confirms with the
    // user that the information issued is accurate.
    while (GoodToGo == false) {
        var ValidYear = false;
        while (ValidYear == false) {
            //first we catch to see if the date is valid. If the user doesn't input anything it will revert back to default

            date = prompt("Please enter the date of the sighting. (Numeric values only. Format: yyyymmdd)");
            var d = date;
            var year = date.substring(0, 4);
            var month = date.substring(4, 6);
            var day = date.substring(6);

            if (d == '' || d == 0) {
                //if the user hits cancel, this is what is necessary to cancel
                Cancel = true;
                final = 1;
                ValidYear = true;
                GoodToGo = true;
            }
            else {
                // gives the user the chance to confirm the date early on
                if (confirm(
                    'Confirm the date is correct ' + '\r\n' +
                    + year + '/' + month + '/' + day)) {
                    ValidYear = true;
                }
                else {

                }
            }
        }



        //now that the date is deemed valid we move onto the animal name.
        var ValidName = false;
        var NameMatch = false;
        Common = prompt("Enter common name. ");
        var n = Common.toUpperCase();
        //comparing the name to the list of distinct names created by SP_getAllAnimals.
        //if the name exists the user's job is easy and the genus, species, and species code won't be necessary
        for (j = 0; j < childDivs[b].getAttribute("NameCount"); j++) {
            //since javascript compares everything case sensitively I put both strings into uppercase.
            strName = (childDivs[b].getAttribute("CommonName" + j.toString())).toUpperCase();

            //localeCompare is the way javascript compares strings
            if (n.localeCompare(strName) == 0) {
                ValidName = true;
                NameMatch = true;
            }
        }
        //if the name doesn't match any in the records it will be added along with the new genus, species and species code
        if (ValidName == false) {
            if (confirm("Name does not match records. Press OK to continue on and create a new animal name or press cancel to retry entry.")) {

                //if the user made a typo and wants to try again they can by selecting cancel on this confirm.
                //cancel = validName = false; and loop resets OK = validName = true and NameMatch = false -> this loop ends
                //and the user is sent through the next loop instead of passing over it a matching name
                ValidName = true;
                NameMatch = false;
            }

        }


        //NameMatch starts off true but is set to false if the user wants to use a non-existant animal name
        while (NameMatch == false) {
            //this loop is only engaged if the user decides to give up on creating a new genus, species, and species code and
            //wants to try spelling the name of the animal again. (spelling is hard..)

            while (ValidName == false) {
                Common = prompt("Enter common name. ");
                var n = Common.toUpperCase();
                for (j = 0; j < childDivs[b].getAttribute("NameCount"); j++) {
                    strName = (childDivs[b].getAttribute("CommonName" + j.toString())).toUpperCase();
                    if (n.localeCompare(strName) == 0) {
                        ValidName = true;
                        NameMatch = true;
                    }
                }
                if (ValidName == false) {
                    if (confirm("Name does not match records. Press OK to continue on and create a new animal name or press cancel to retry entry.")) {
                        ValidName = true;
                        NameMatch = false;
                    }
                }
            }
            //after re-entering the name and it matches the first name the user can continue creating the genus and species.
            //the database can accept nulls
            Common = prompt("Re-enter name to confirm spelling.");
            m = Common.toUpperCase();
            if (n.localeCompare(m) == 0) {
                strCommon = Common.toLowerCase();
                Common = strCommon.replace(strCommon.charAt(0), strCommon.charAt(0).toUpperCase());
                badGenus = true;
                while (badGenus == true) {
                    strGenus = (prompt("Enter genus for unknown animal.")).toLowerCase();
                    if (strGenus == '') {
                        alert("Genus can not be left blank.");
                    }
                    else {
                        badGenus = false;
                    }
                }
                newGenus = strGenus.replace(strGenus.charAt(0), strGenus.charAt(0).toUpperCase());
                newSpecies = (prompt("Enter species for unknown animal.")).toLowerCase();
                //The species code is a four letter code. this loop makes sure there aren't any number.
                validSpeciesCode = false;
                while (validSpeciesCode == false) {
                    newSpeciesCode = prompt("Enter 4 letter species code for unknown animal.");
                    var sc = newSpeciesCode;

                    if (sc.length != 4 || sc.includes("1") || sc.includes("2") || sc.includes("3") || sc.includes("4") || sc.includes("5") || sc.includes("6") || sc.includes("7") || sc.includes("8") || sc.includes("9") || sc.includes("0")) {
                        alert("Species code entered was invalid. Must only include alphabetical characters.");

                    }
                    else {
                        validSpeciesCode = true;
                        newSpeciesCode = newSpeciesCode.toUpperCase();
                    }
                }
                if (confirm(
                    'Confirm all information is correct. ' + '\r\n' +
                    'Date of Sighting: ' + year + '/' + month + '/' + day +
                    'Animal: ' + Common + '\r\n' +
                    'Genus: ' + newGenus + '\r\n' +
                    'Species: ' + newSpecies + '\r\n' +
                    'Species Code: ' + newSpeciesCode + '\r\n')) {
                    alert(Common + ' has been added to this site');
                    var final = 1;
                    VaildName = true;
                    NameMatch = true;

                    //all the new animal sighting information is sent to add animal sighting unknown, the page is refreshed and the markers replaced.
                    PageMethods.AddAnimalSightingUnknown(Latitude, Longitude, SiteNumber, year, month, day, newSpeciesCode, newGenus, newSpecies, Common, onSuccess, onError)
                    location.reload();
                    PlaceMarkers();

                }
                else {
                    alert('Cancelled.');
                    NameMatch = true;
                    var final = 1;
                }
            }
            else if (Common == '') {
                alert("Invalid entry. Re-enter Common name of animal");
                NameMatch = false;
                ValidName = false;

            }
            else {
                alert("Incorrect spelling. Re-enter common name of animal.");
                NameMatch = false;
                ValidName = false;
                alert("hi.");
            }

        }
        GoodToGo = true;
    }

    //this is what happens if the user enters a valid animal name. a quick display of the information and the it is sent to AddAnimalSighting
    // in MapPage.aspx.cs . Since the name already has a species code, species and genus attached this information isn't required from the user.
    if (final == null) {
        if (confirm(
            'Confirm all information is correct. ' + '\r\n' +
            'Date of Sighting: ' + year + '/' + month + '/' + day + '\r\n' +
            'Animal: ' + Common)) {
            PageMethods.AddAnimalSighting(Latitude, Longitude, SiteNumber, year, month, day, Common, onSuccess, onError);
            location.reload();
            PlaceMarkers();
        }

        location.reload();
        PlaceMarkers();
    }

    // onSuccess and onError functions fire depending on outcome of try / catch
    function onSuccess(result) {
        // produces an alert with a successful insert message
        alert(result);
    }
    function onError(result) {
        // produces an alert with an unsuccessful insert message
        alert(result)
    }
}

///Purpose: this is the UpdateSite function. It is engaged when the user presses the update site button in the
/// info window of a marker. It takes one variable, the index of the site in the childDivs list so that the other attributes
/// can be aquired through this index number and list.
///
///1.0      Allan   2021-03-03  Initial
///1.1      Taylor  2021-03-09  Added onSuccess / onError
///
function updateSite(a) {
    var childDivs = document.getElementById('DivMarkerList').getElementsByTagName('DIV');
    var ID = childDivs[a].getAttribute("SiteID");
    var ValidLat = false;
    var ValidLong = false;
    var ValidSite = false;

    //here's a series of loops ensuring the user has entered valid information for lat and long.

    while (ValidLat == false) {
        //the default value of the lat and long prompts are the current values, taken from the childDivs list
        var Latitude = prompt("Update site latitude.", childDivs[a].getAttribute("Latitude"));
        if (Latitude >= 53 && Latitude <= 60) {
            ValidLat = true;
        }
        //if the user cancels or deletes the default prompt value but leaves it blank the default value will be taken again.
        else if (Latitude == "" || Latitude == null) {
            Latitude = childDivs[a].getAttribute("Latitude");
            ValidLat = true;
        }
        else {
            alert("Invalid latitude. Input a latitude within the area of interest. (53 - 60 degrees).");
        }
    }
    //identical loop as latitude
    while (ValidLong == false) {
        var Longitude = prompt("Update site longitude.", childDivs[a].getAttribute("Longitude"));
        if (Longitude >= -120 && Longitude <= -110) {
            ValidLong = true;
        }
        else if (Longitude == "" || Longitude == null) {
            Longitude = childDivs[a].getAttribute("Longitude");
            ValidLong = true;
        }
        else {
            alert("Invalid longitude. Input a longitude within the area of interest. (-120 - -110 degrees).");
        }
    }

    //this series of loops determines if the user has input a site number that is already in use.
    while (ValidSite == false) {

        //again the default is the current site number
        var SiteNumber = prompt("Update site number.", childDivs[a].getAttribute("SiteNumber"));

        var n = Number(SiteNumber);

        if (Number.isInteger(n) == true && n > 0) {

            var SiteMatch = false;
            for (j = 0; j < childDivs.length; j++) {

                if (n == parseInt(childDivs[j].getAttribute("SiteNumber"))) {

                    //the original site number is still in the list so if the site id matches the one being updated then 
                    //the user isn't really selecting a duplicate, they are likely just not changing the site number
                    if (parseInt(childDivs[j].getAttribute("SiteID")) == parseInt(childDivs[a].getAttribute("SiteID"))) {
                        SiteMatch = false;

                    }
                    else {
                        SiteMatch = true;
                        alert("This site number is already in use.");
                    }


                }

            }
            if (SiteMatch == false) {

                ValidSite = true;
            }
        }
        else if (SiteNumber == '' || n == 0) {
            //null, blank and zero values are reverted back to the original site number
            SiteNumber = childDivs[a].getAttribute("SiteNumber");

            ValidSite = true;
        }
        else {
            alert("You must input a valid number.");
        }


    }

    //this is one last check for the user to see what he's inputing into the database
    if (confirm(
        'Confirm all information is correct for site: ' + ID + '\r\n' +
        'Latitude: ' + Latitude + '\r\n' +
        'Longitude: ' + Longitude + '\r\n' +
        'SiteNumber: ' + SiteNumber + '\r\n')) {

        alert('Site: ' + ID + ' has been updated');

        PageMethods.UpdateSite(ID, Latitude, Longitude, SiteNumber, onSuccess, onError)

        location.reload();
        PlaceMarkers();

        // onSuccess and onError functions fire depending on outcome of try / catch
        function onSuccess(result) {
            // produces an alert with a successful insert message
            alert(result);
        }
        function onError(result) {
            // produces an alert with an unsuccessful insert message
            alert(result)
        }

    }
    else {
        alert('Update cancelled.');
    }
    location.reload();
    PlaceMarkers();

}

///Purpose: this is the UpdateAnimalSighting function. It is engaged when the user presses the Update Animal Sighting button in the
/// modal of an animal sighting within the site info window. It accepts two variables, the site index for childDivs (b) and the animal index
/// from the list of animal sightings within a site. This function bears a stricking resemblance to AddAnimalSightings so refer to it for
/// more information
///
///1.0      Allan   2021-03-03 Initial
///1.1      Taylor  2021-03-09  Added onSuccess / onError
///
function updateAnimalSighting(a, b) {
    // initially we pull all available variables and assign them to reasonable names.
    var childDivs = document.getElementById('DivMarkerList').getElementsByTagName('DIV');
    if (childDivs[b].getAttribute("Sighting0") == null && childDivs[b].getAttribute("Genus") != null) {
        var r = childDivs[b].getAttribute("Animal" + a.toString());
    }
    else if (childDivs[b].getAttribute("Sighting0") != null) {
        var r = childDivs[b].getAttribute("Sighting" + a.toString());
    }
    var s = r.split(": ");
    var AnimalID = s[0];
    var Common = s[1];
    var SiteID = childDivs[b].getAttribute("SiteID");
    var year = childDivs[b].getAttribute("Year" + a.toString());
    var month = childDivs[b].getAttribute("Month" + a.toString());
    var day = childDivs[b].getAttribute("Day" + a.toString());
    var Latitude = childDivs[b].getAttribute("Latitude");
    var Longitude = childDivs[b].getAttribute("Longitude");
    var SiteNumber = childDivs[b].getAttribute("SiteNumber");
    var GoodToGo = false;
    var str1 = "SITE ID";
    var str2 = "ANIMAL NAME";
    var str3 = "YEAR";
    var str4 = "MONTH";
    var str5 = "DAY";
    var str6 = "DONE";

    while (GoodToGo == false) {
        //the user may only want to update one or some variables. this allows them to pick and chose which ones by comparing their
        //prompt input to the available options.
        var attribute = (prompt(
            'Which attribute would you like to update for animal sighting ' + AnimalID + ': ' + Common + '?' + '\r\n' +
            'Site ID: ' + SiteID + '\r\n' +
            'Animal Name: ' + Common + '\r\n' +
            'Year: ' + year + '\r\n' +
            'Month: ' + month + '\r\n' +
            'Day: ' + day + '\r\n' +
            'When finished type \'Done\''
        )).toUpperCase();

        //changing the site id is the only way to change the location of the animal sighting
        if (attribute.localeCompare(str1) == 0) {
            attribute = '';
            var ValidSite = false;
            while (ValidSite == false) {
                //this disables the user from entering invalid numbers (negative integers, decimals, letters and unused site id numbers.)
                SiteID = prompt("Enter updated Site ID: ");
                //Number(var) converts the Site ID into a number. entries into a prompt are always strings and we can't determine
                //whether a string is a double or an int until it is first converted into a number.
                var n = Number(SiteID);
                //after converting the string into a number we test to see if its an int using Number.isInteger(n)
                if (Number.isInteger(n) == true && n > 0) {
                    var SiteMatch = false;
                    //we then pass through the childDivs list to see if the site id matches another site id in the list.
                    for (j = 0; j < childDivs.length; j++) {
                        if (n == parseInt(childDivs[j].getAttribute("SiteID"))) {
                            SiteMatch = true;
                        }
                    }
                    if (SiteMatch == true) {

                        ValidSite = true;
                    }
                    else {
                        alert("You must select a valid site.");
                    }
                }
                //if the user inputs nothing or cancels (inputs a null) the original site id is returned.
                else if (SiteID == '' || n == 0) {
                    SiteID = childDivs[b].getAttribute("SiteID");
                    ValidSite = true;
                }
                else {
                    alert("You must enter a valid number.");
                }
            }



        }

        //the date is also changeable similar to when i dealt with year above. check function AddAnimalSighting
        else if (attribute.localeCompare(str3) == 0) {
            attribute = '';


            var ValidYear = false;
            while (ValidYear == false) {
                year = prompt("Enter updated year: ");
                var n = Number(year);
                if (Number.isInteger(n) == true && n > 0) {
                    ValidYear = true;
                }
                else if (year == '' || n == 0) {
                    ValidYear = true;
                    year = childDivs[b].getAttribute("Year" + a.toString());
                }
                else {
                    alert("You must enter a valid number.");
                }
            }

        }
        else if (attribute.localeCompare(str4) == 0) {
            attribute = '';


            var ValidMonth = false;
            while (ValidMonth == false) {
                month = prompt("Enter updated month: ");
                var n = Number(month);
                if (Number.isInteger(n) == true && n > 0 && n <= 12) {
                    ValidMonth = true;
                }
                else if (month == '' || n == 0) {
                    ValidMonth = true;
                    month = childDivs[b].getAttribute("Month" + a.toString());
                }
                else {
                    alert("You must enter a valid month.");
                }
            }

        }
        else if (attribute.localeCompare(str5) == 0) {
            attribute = '';


            var ValidDay = false;
            while (ValidDay == false) {
                day = prompt("Enter updated day: ");
                var n = Number(day);
                if (month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12) {
                    if (Number.isInteger(n) == true && n > 0 && n <= 31) {
                        ValidDay = true;
                    }
                    else if (day == '' || n == 0) {
                        ValidDay = true;
                        day = childDivs[b].getAttribute("Day" + a.toString());
                    }
                    else {
                        alert("You must enter a valid day.");
                    }
                }
                else if (month == 4 || month == 6 || month == 9 || month == 11) {
                    if (Number.isInteger(n) == true && n > 0 && n <= 30) {
                        ValidDay = true;
                    }
                    else if (day == '' || n == 0) {
                        ValidDay = true;
                        day = childDivs[b].getAttribute("Day" + a.toString());
                    }
                    else {
                        alert("You must enter a valid day.");
                    }
                }
                else {
                    if (Number.isInteger(n) == true && n > 0 && n <= 28) {
                        ValidDay = true;
                    }
                    else if (day == '' || n == 0) {
                        ValidDay = true;
                        day = childDivs[b].getAttribute("Day" + a.toString());
                    }
                    else {
                        alert("You must enter a valid day.");
                    }
                }

            }


        }
        //this it to see if genus, species code and species are required from the user.
        //if the string entered by the user matches the record of distinct common names then the genus, species code and species are filled in
        //automatically in MapPage.aspx.cs otherwise the user is required to input them refer to function AddAnimalSpecies for more in depth explanations
        //of these nested loops
        else if (attribute.localeCompare(str2) == 0) {
            attribute = '';
            var ValidName = false;
            var NameMatch = false;
            while (ValidName == false) {
                Common = prompt("Enter updated common name: ");
                var n = Common.toUpperCase();
                for (j = 0; j < childDivs[b].getAttribute("NameCount"); j++) {
                    strName = (childDivs[b].getAttribute("CommonName" + j.toString())).toUpperCase();
                    if (n.localeCompare(strName) == 0) {
                        ValidName = true;
                        NameMatch = true;
                    }
                    else if (Common == '' || Common == null) {
                        Common = s[1];
                        ValidName = true;
                        NameMatch = true;
                    }
                }
                if (ValidName == false) {
                    if (confirm("Common name - " + Common + " - does not match records. Press OK to continue on and create a new animal name or press cancel to retry entry.")) {

                        //if the user made a typo and wants to try again they can by selecting cancel on this confirm.
                        //cancel = validName = false; and loop resets OK = validName = true and NameMatch = false -> this loop ends
                        //and the user is sent through the next loop instead of passing over it a matching name
                        ValidName = true;
                        NameMatch = false;
                    }
                    else {
                        ValidName = false;
                        NameMatch = true;
                    }
                }

                while (NameMatch == false) {

                    Common = prompt("Re-enter name to confirm spelling.");
                    m = Common.toUpperCase();
                    strCommon = Common.toLowerCase();
                    Common = strCommon.replace(strCommon.charAt(0), strCommon.charAt(0).toUpperCase());


                    if (n.localeCompare(m) == 0) {
                        NameMatch = true;
                        badGenus = true;
                        while (badGenus == true) {
                            strGenus = (prompt("Enter genus for unknown animal.")).toLowerCase();
                            if (strGenus == '') {
                                alert("Genus can not be left blank.");
                            }
                            else {
                                badGenus = false;
                            }
                        }
                        newGenus = strGenus.replace(strGenus.charAt(0), strGenus.charAt(0).toUpperCase());
                        newSpecies = (prompt("Enter species for unknown animal.")).toLowerCase();

                        validSpeciesCode = false;
                        while (validSpeciesCode == false) {
                            newSpeciesCode = prompt("Enter  the Species Code. Must be 4 alphabetic characters.");

                            if (sc.length != 4 || sc.includes("1") || sc.includes("2") || sc.includes("3") || sc.includes("4") || sc.includes("5") || sc.includes("6") || sc.includes("7") || sc.includes("8") || sc.includes("9") || sc.includes("0")) {
                                alert("Invalid species code.");
                            }
                            else {
                                validSpeciesCode = true;

                            }

                         
                        }

                        if (confirm(
                            'Confirm all new animal information is correct. ' + '\r\n' +
                            'Animal: ' + Common + '\r\n' +
                            'Genus: ' + newGenus + '\r\n' +
                            'Species: ' + newSpecies + '\r\n' +
                            'Species Code: ' + newSpeciesCode + '\r\n')) {
                            alert(Common + ' information has been added to the database.');

                        }

                        PageMethods.UpdateAnimalSightingUnknown(SiteID, Latitude, Longitude, SiteNumber, AnimalID, Common, newGenus, newSpecies, newSpeciesCode, year, month, day, onSuccess, onError)

                        // onSuccess and onError functions fire depending on outcome of try / catch
                        function onSuccess(result) {
                            // produces an alert with a successful insert message
                            alert(result);
                        }
                        function onError(result) {
                            // produces an alert with an unsuccessful insert message
                            alert(result)
                        }
                    }
                    else {
                        alert("Incorrect spelling. Re-enter common name of animal.");
                    }
                }
            }
        }

        //this final option for updating for the user allows the user to see their final product before updating the database
        else if (attribute.localeCompare(str6) == 0) {
            attribute = '';
            if (confirm('Have you finished updating?')) {
                GoodToGo = true;
            }
            if (confirm('Do you want to apply your updates?')) {
                PageMethods.UpdateAnimalSighting(SiteID, Latitude, Longitude, SiteNumber, AnimalID, Common, year, month, day, onSuccess, onError)
                // onSuccess and onError functions fire depending on outcome of try / catch
                function onSuccess(result) {
                    // produces an alert with a successful insert message
                    alert(result);
                }
                function onError(result) {
                    // produces an alert with an unsuccessful insert message
                    alert(result)
                }
                location.reload();
                PlaceMarkers();
            }
            else { GoodToGo = false; }
        }

        else {
            attribute = '';
            alert("Invalid input. Please select which attribute you would like to update.");
        }

    }

}
