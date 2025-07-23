(function () {
    function append(src) {
        var s = document.createElement('script');
        s.src = src;
        document.body.appendChild(s);
    }

    fetch('/api/config/fingerprint-sensor')
        .then(function (r) { return r.text(); })
        .then(function (sensor) {
            sensor = (sensor || '').trim().toUpperCase();
            if (sensor === 'FM220U') {
                append('/Js/FM220U.js');
            } else {
                append('/Js/MantraMSF100.js');
            }
        })
        .catch(function () {
            append('/Js/MantraMSF100.js');
        });
})();
