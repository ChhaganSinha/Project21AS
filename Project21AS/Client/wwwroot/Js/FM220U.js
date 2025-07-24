// FM220U-LI fingerprint sensor helper functions based on the sample
// script provided by the manufacturer. The endpoints match the local
// RD service running on the client machine.

function releaseFP() {
    var xhr;
    var url = 'https://localhost:11200/rd/releasefm220?ts=' + Date.now();
    if (window.XMLHttpRequest) {
        try {
            xhr = new XMLHttpRequest();
            xhr.open('RELEASEFM220', url, true);
        } catch (e) {
            xhr = new ActiveXObject('Microsoft.XMLHTTP');
            xhr.open('RELEASEFM220', url, true);
        }
    } else {
        xhr = new ActiveXObject('Microsoft.XMLHTTP');
        xhr.open('RELEASEFM220', url, true);
    }
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            console.log('Released');
        }
    };
    xhr.send();
}

function captureFP() {
    callerfun('https://localhost:4443/FM220/gettmpl', function (result) {
        SuccessFunc(result);
    });
}

function MatchFP() {
    var tmpl = document.getElementById('tmplval').value.toString();
    callerfun('https://localhost:4443/FM220/GetMatchResult?MatchTmpl=' + encodeURIComponent(tmpl), function (result) {
        SuccessMatch(result);
    });
}

function getSerial() {
    callerfun('https://localhost:4443/FM220/getserial', function (result) {
        SuccessFunc(result);
    });
}

function CaptureFinger(quality, timeout) {
    // quality and timeout parameters are currently ignored by the FM220U service
    return new Promise(function (resolve, reject) {
        callerfun('https://localhost:4443/FM220/gettmpl', function (result) {
            if (result && result.errorCode === 0 && result.bmpBase64) {
                resolve(result.bmpBase64);
            } else {
                reject('Capture failed');
            }
        });
    });
}

function callerfun(url, callback) {
    var xhr;
    if (window.XMLHttpRequest) {
        try {
            xhr = new XMLHttpRequest();
            xhr.open('GET', url, true);
        } catch (e) {
            xhr = new ActiveXObject('Microsoft.XMLHTTP');
            xhr.open('GET', url, true);
        }
    } else {
        xhr = new ActiveXObject('Microsoft.XMLHTTP');
        xhr.open('GET', url, true);
    }
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4) {
            if (xhr.status == 200) {
                callback(JSON.parse(xhr.response));
            } else {
                callback({ errorCode: xhr.status });
            }
        }
    };
    xhr.overrideMimeType('application/json');
    xhr.send();
}

function SuccessFunc(result) {
    if (result.errorCode == 0) {
        if (result && result.bmpBase64 && result.bmpBase64.length > 0) {
            document.getElementById('FPImage1').src = 'data:image/bmp;base64,' + result.bmpBase64;
        }
        if (document.getElementById('tmplval')) {
            document.getElementById('tmplval').value = result.templateBase64;
        }
        // Basic output. The original sample builds an HTML table; we keep it simple.
        document.getElementById('result').textContent = 'Serial: ' + result.serialNumber;
    } else {
        document.getElementById('result').textContent = '';
        alert('Fingerprint Capture ErrorCode ' + result.errorCode + ' Desc :-' + result.status);
    }
}

function SuccessMatch(result) {
    if (result.errorCode == 0) {
        var msg = result.matchSuccess ? 'Matching Success. Score: ' + result.matchScore : 'Matching Failed. Score: ' + result.matchScore;
        document.getElementById('result').textContent = msg;
    } else {
        document.getElementById('result').textContent = '';
        alert('Fingerprint Capture ErrorCode ' + result.errorCode + ' Desc :-' + result.status);
    }
}

function ErrorFunc() {
    alert('Check if ACPL FM220 service is running');
}
