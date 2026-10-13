window.cityMap = {
    _maps: {},

    init: function (elementId, cities, dotNetRef) {
        if (this._maps[elementId]) {
            this._maps[elementId].remove();
        }

        const map = L.map(elementId).setView([39.5, -98.35], 4);
        this._maps[elementId] = map;

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '© OpenStreetMap contributors'
        }).addTo(map);

        cities.forEach(city => {
            const marker = L.circleMarker([city.lat, city.lon], {
                radius: 6,
                fillColor: '#3388ff',
                color: '#fff',
                weight: 1.5,
                fillOpacity: 0.9
            }).addTo(map);

            marker.bindTooltip(city.name, { permanent: false, direction: 'top' });

            marker.on('click', () => {
                // Reset all markers
                map.eachLayer(layer => {
                    if (layer instanceof L.CircleMarker) {
                        layer.setStyle({ fillColor: '#3388ff' });
                    }
                });
                marker.setStyle({ fillColor: '#e63946' });
                dotNetRef.invokeMethodAsync('CityClicked', city.name, city.lat, city.lon);
            });
        });
    },

    dispose: function (elementId) {
        if (this._maps[elementId]) {
            this._maps[elementId].remove();
            delete this._maps[elementId];
        }
    }
};
