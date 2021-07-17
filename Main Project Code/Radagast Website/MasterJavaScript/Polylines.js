///Purpose: Maps the polylines between markers
///Vers     Coder   Date        Comments
///1.0      Andy    2021-04-07  Initial
///1.1      Taylor  2021-04-09  Added map parameter to SitePolylines function

function SitePolylines(data, m) {
    // change the color of the polyline based on number of vertices
    if (data.length >= 0 && data.length <= 19) {
        var SiteLines = new google.maps.Polyline({
            path: data,
            geodesic: true,
            strokeColor: "#F1EEF6",
            strokeOpacity: 1.0,
            strokeWeight: 2,
        });
        SiteLines.setMap(m);
    }
    else if (data.length >= 20 && data.length <= 39) {
        var SiteLines = new google.maps.Polyline({
            path: data,
            geodesic: true,
            strokeColor: "#D4B9DA",
            strokeOpacity: 1.0,
            strokeWeight: 2,
        });
        SiteLines.setMap(m);
    }

    else if (data.length >= 40 && data.length <= 59) {
        var SiteLines = new google.maps.Polyline({
            path: data,
            geodesic: true,
            strokeColor: "#C994C7",
            strokeOpacity: 1.0,
            strokeWeight: 2,
        });
        SiteLines.setMap(m);
    }

    else if (data.length >= 60 && data.length <= 79) {
        var SiteLines = new google.maps.Polyline({
            path: data,
            geodesic: true,
            strokeColor: "#DF65B0",
            strokeOpacity: 1.0,
            strokeWeight: 2,
            
        });
        SiteLines.setMap(m);
    }

    else if (data.length >= 80 && data.length <= 99) {
        var SiteLines = new google.maps.Polyline({
            path: data,
            geodesic: true,
            strokeColor: "#E7298A",
            strokeOpacity: 1.0,
            strokeWeight: 2,
        });
        SiteLines.setMap(m);
    }

    else if (data.length >= 100 && data.length <= 119) {
        var SiteLines = new google.maps.Polyline({
            path: data,
            geodesic: true,
            strokeColor: "#CE1256",
            strokeOpacity: 1.0,
            strokeWeight: 2,
        });
        SiteLines.setMap(m);
    }

    else {
        var SiteLines = new google.maps.Polyline({
            path: data,
            geodesic: true,
            strokeColor: "#91003F",
            strokeOpacity: 1.0,
            strokeWeight: 2,
        });
        SiteLines.setMap(m);
    }
}