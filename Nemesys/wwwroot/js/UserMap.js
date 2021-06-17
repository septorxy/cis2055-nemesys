let map;
var prev = null;
let markers = [];

function initMap() {
    map = new google.maps.Map(document.getElementById("map"), {
        center: { lat: 35.90202266601426, lng: 14.483712302536379 },
        zoom: 17,
    });

    google.maps.event.addListener(map, 'click', function (event) {
        placeMarker(event.latLng);
    });

    placeLater();

}

function placeMarker(location) {
    deleteMarkers()
    var marker = new google.maps.Marker({
        position: location,
        map: map
    });
    markers.push(marker);
    map.setCenter(location);
    var lati = marker.getPosition().lat();
    var longi = marker.getPosition().lng();
    document.getElementById("Longitude").value = longi;
    document.getElementById("Latitude").value = lati;
}


function setMapOnAll(map) {
    for (let i = 0; i < markers.length; i++) {
        markers[i].setMap(map);
    }
}

function clearMarkers() {
    setMapOnAll(null);
}

function showMarkers() {
    setMapOnAll(map);
}

function deleteMarkers() {
    clearMarkers();
    markers = [];
}

function placeLater() {
    var longi = parseFloat(document.getElementById("Longitude").value);
    var lati = parseFloat(document.getElementById("Latitude").value);
    var myLatLng;
    if (longi != 0) {
        myLatLng = { lat: lati, lng: longi };
        placeMarker(myLatLng);
    }
}