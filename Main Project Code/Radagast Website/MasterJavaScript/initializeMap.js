// Only initializes map

var map
const myLatlng = { "lat": 57.07853556994608, "lng": -114.97768851837155 };

function myMap() {

	var mapProp =
	{
		center: myLatlng,
		zoom: 6.8,
	};
	 map = new google.maps.Map(document.getElementById("Map"), mapProp);

	// call the functions to populate map data
	PlaceMarkers(map);
	PlaceHeatMarkers(map);
	populateOverlayDataFromDiv(map);

	// Create the initial InfoWindow.
	let infoWindow = new google.maps.InfoWindow({
		content: "Click to find the latitude and longitude of a location!",
		position: myLatlng,
	});
	infoWindow.open(map);

	// Configure the click listener.
	map.addListener("click", (mapsMouseEvent) =>
	{
		// Close the current InfoWindow.
		infoWindow.close();
		// Create a new InfoWindow.
		infoWindow = new google.maps.InfoWindow
			({
			position: mapsMouseEvent.latLng,
		});
		infoWindow.setContent(
			JSON.stringify(mapsMouseEvent.latLng.toJSON(), null, 2)
		);
		infoWindow.open(map);
	});


}
