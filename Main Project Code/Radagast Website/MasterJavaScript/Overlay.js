
/// Purpose: the script of all functions related to the overlay.
/// Date: 2021/03/08
/// Coder: Jill
/// Version history:
/// 1.0                 Jill    Initial
/// 



function populateOverlayDataFromDiv(m) {

    // The custom ABOverlay object contains the AB image,
    // the bounds of the image, and a reference to the map.
    class ABOverlay extends google.maps.OverlayView {
        constructor(bounds, image) {
            super();
            // Initialize all properties.
            this.bounds_ = bounds;
            this.image_ = image;
            // Define a property to hold the image's div. We'll
            // actually create this div upon receipt of the onAdd()
            // method so we'll leave it null for now.
            this.div_ = null;
        }
        /**
         * onAdd is called when the map's panes are ready and the overlay has been
         * added to the map. Control of the maps' style is here.
         */
        onAdd() {
            this.div_ = document.createElement("div");
            this.div_.style.borderStyle = "none";
            this.div_.style.borderWidth = "0px";
            this.div_.style.position = "absolute";
            // Create the img element and attach it to the div.
            const img = document.createElement("img");
            img.src = this.image_;
            img.style.width = "100%";
            img.style.height = "100%";
            img.style.position = "absolute";
            img.style.opacity = childDivs[0].getAttribute("Opacity");
            this.div_.appendChild(img);
            // Add the element to the "overlayLayer" pane.
            const panes = this.getPanes();
            panes.overlayLayer.appendChild(this.div_);
        }
        draw() {
            // We use the south-west and north-east
            // coordinates of the overlay to peg it to the correct position and size.
            // To do this, we need to retrieve the projection from the overlay.
            const overlayProjection = this.getProjection();
            // Retrieve the south-west and north-east coordinates of this overlay
            // in LatLngs and convert them to pixel coordinates.
            // We'll use these coordinates to resize the div.
            const sw = overlayProjection.fromLatLngToDivPixel(
                this.bounds_.getSouthWest()
            );
            const ne = overlayProjection.fromLatLngToDivPixel(
                this.bounds_.getNorthEast()
            );

            // Resize the image's div to fit the indicated dimensions.
            if (this.div_) {
                this.div_.style.left = sw.x + "px";
                this.div_.style.top = ne.y + "px";
                this.div_.style.width = ne.x - sw.x + "px";
                this.div_.style.height = sw.y - ne.y + "px";
            }
        }
        /**
         * The onRemove() method will be called automatically from the API if
         * we ever set the overlay's map property to 'null'.
         */
        onRemove() {
            if (this.div_) {
                this.div_.parentNode.removeChild(this.div_);
                this.div_ = null;
            }
        }
    }


    /** Relates to MapPage.aspx */
    if (document.getElementById("DivOverlayList") == null) {

    }
    else {

        var OverlayList = document.getElementById("DivOverlayList");

        var childDivs = document.getElementById('DivOverlayList').getElementsByTagName('DIV');


        for (i = 0; i < childDivs.length; ++i) {
            //Bottom Left, Then Top Right.
            var bounds = new google.maps.LatLngBounds(
                new google.maps.LatLng(childDivs[i].getAttribute("BottomLeftLat"), childDivs[i].getAttribute("BottomLeftLng")),
                new google.maps.LatLng(childDivs[i].getAttribute("TopRightLat"), childDivs[i].getAttribute("TopRightLng"))
            );

            // The photograph is courtesy of Alberta Parks.
            var srcImage = childDivs[i].getAttribute("ImagePath");

            var overlay = new ABOverlay(bounds, srcImage);
            overlay.setMap(m);
        }


        // still need to find a way to actually set the opacity! LOL
        // Google maps has a setOpacity() function for their overlays, but I cannot get it to work
        // other ideas = set the div opacity, set the image opacity, etc
        // want to add visibility visible / hidden linked to the original overlay checkbox?
        // want to add similar thing for overlay legend?
        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Vers        Date            Coder       Issue
        /// 1.0         2021-03-12      Taylor      Initial
        ///

        //sets the attributes of the slider
        $(function (overlay) {
            $("#slider-1").slider({
                min: 0,
                max: 1,
                step: 0.1,
                value: childDivs[0].getAttribute("Opacity"),
                animate: "slow",
                orientation: "horizontal",

                // the slide function activates the whole time the slider is moving
                // sets the value of minval to the slider value
                slide: function (event, ui) {

                    $("#minval").val(ui.value * 100 + "%");
                },

                // stop funtion happens only when the slider movement ends
                // sets the opacity of the overlay with the slider value
                stop: function (event, ui, overlay) {
                    var opac = (ui.value);
                    sendOpac(opac);
                    //map.overlay.setOpacity(opac);
                }

            });

            //sets the value for minval at creation of the slider
            $("#minval").val($("#slider-1").slider("value") * 100 + "%");
        });

        function sendOpac(o) {
            PageMethods.OpacUpdate(o);
            location.reload();
        }
    }
    
}
  