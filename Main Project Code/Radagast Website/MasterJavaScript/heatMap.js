/// Purpose: the script of all functions related to the HeatMap markers
/// Date: 2021/03/14
/// Coder: Jill
/// Version history:
/// 1.0                 Jill    Initial
/// 1.1		2021-03-18	Taylor	Added weight functionality
/// 1.2     2021-03-19  ALlan   added in the invisible location markers, the event listeners, 
///                             the info windows and the delete site function.      
///1.2      2021-03-22  Taylor  Added gradient and radius


var HeatMarkerList = [];
var HeatmapData = [];
const gradient = [
    "rgba(255,247,236, 0)",
    "rgba(255, 0, 0, 0.8)",
    "rgba(221, 0, 0, 0.8)",
    "rgba(187, 0, 0, 0.8)",
    "rgba(154, 0, 0, 0.8)",
    "rgba(121, 0, 1, 0.8)",
    "rgba(90, 0, 4, 0.8)",
    "rgba(58, 0, 2, 0.8)",
    "rgba(18, 0, 0, 0.8)",
    "rgba(0, 0, 0, 0.8)",
];

function PlaceHeatMarkers(m)
{
    //ClearMap();

    HeatMarkerList = document.getElementById("DivHeatMapList");
    var childDivs = document.getElementById('DivHeatMapList').getElementsByTagName('DIV');
    var heatmarker, i;
    var infowindow = new google.maps.InfoWindow();
    var marker, i;

    for (i = 0; i < childDivs.length; i++) {
        var RID = childDivs[i].getAttribute("RespID");
        var FN = childDivs[i].getAttribute("FacilityName");
        var position = new google.maps.LatLng(childDivs[i].getAttribute("Latitude"),
            childDivs[i].getAttribute("Longitude"));
        var emissions = childDivs[i].getAttribute("Emissions");
        var weight = childDivs[i].getAttribute("Weight");


        HeatmapData.push({ location: position, weight: parseFloat(weight) });
        //HeatmapData.push(position);
        const image =
        {
            url: "../../MapImages/RoundMarker.png",
            scaledSize: new google.maps.Size(8, 8),    // scaled size
            origin: new google.maps.Point(0, 0),        // origin
            anchor: new google.maps.Point(4, 4)      // anchor
        };

        var marker = new google.maps.Marker
            ({
                position: position,
                draggable: false,
                ID: RID,
                SN: FN,
                map: m,
                icon: image,
                opacity: 0,
                title: "Emission ID: " + RID
            });

        google.maps.event.addListener(marker, 'click', (function (marker, i) {

            return function () {
                infowindow.setContent(
                    '<p><b>Emissions ID: </b>' + childDivs[i].getAttribute("RespID") + '</p>' +
                    '<p><b>Facility Name: </b>' + childDivs[i].getAttribute("FacilityName") + '</p>'+
                    '<p><b>Latitude: </b>' + childDivs[i].getAttribute("Latitude") + '</p>' +
                    '<p><b>Longitude: </b>' + childDivs[i].getAttribute("Longitude") + '</p>' +
                    '<p><b>Emissions (tonnes): </b>' + childDivs[i].getAttribute("Emissions") + '</p>' +
                    '<button class="button" type="button" onclick="deletingSite(' + childDivs[i].getAttribute("RespID") + ')">Delete Site</button>'
                    );
                infowindow.open(map, marker);
            }
        })(marker, i));

    }
    createHeatMap(position, weight, m);    
}

function createHeatMap(p, w, m) {

    var heatmap = new google.maps.visualization.HeatmapLayer({
        data: HeatmapData,
        gradient: gradient,
        radius: 17,


    });


    heatmap.setMap(m);
}


/// Purpose: Deletes an Air particulate site by ID by using pagemethods to access the delete site functuon in the MapPage.aspx.cs 
/// page.
/// Vers    Date        Coder       Comments
/// 1.0     2021-03-19  Allan       Initial
function deletingSite(i) {
    //it uses a confirm so that the user can be sure they wish to delete the site
    if (confirm('About to delete site ' + i + ' press ok to confirm.')) {

        //using Page.Methods this site can communicate with MapPage.aspx.cs DeleteSite sending i
        //which in this case is the SiteID
        PageMethods.DeleteSiteResp(i, onSuccess, onError);

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


