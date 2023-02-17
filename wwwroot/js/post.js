// Auto Linker for Post
var autolinker = new Autolinker({
    newWindow: true
});
var postTexts = document.getElementsByClassName("post-text");
for (var i = 0; i < postTexts.length; i++) {
    postTexts[i].innerHTML = autolinker.link(postTexts[i].innerHTML);
}

// Video JS Multiple Resolution Support
if (document.getElementById("post-video-player")) {
    videojs('post-video-player').videoJsResolutionSwitcher();
}

// Social Media Share Button
if (document.getElementById("shareButton")) {
    $("#shareButton").jsSocials({
        shareIn: "popup",
        showCount: true,
        showLabel: false,
        shares: ["facebook", "linkedin", "twitter", "pinterest"]
    });
}

// Cookie Timezone
function setTimezoneCookie() {
    var timezone_cookie = "timezoneoffset";
    // if the timezone cookie not exists create one.
    if (!$.cookie(timezone_cookie)) {
        // check if the browser supports cookie
        var test_cookie = 'test';
        $.cookie(test_cookie, true);
        // browser supports cookie
        if ($.cookie(test_cookie)) {
            // delete the test cookie
            $.cookie(test_cookie, null);
            // create a new cookie 
            $.cookie(timezone_cookie, new Date().getTimezoneOffset());
            // re-load the page
            location.reload();
        }
    }
    // if the current timezone and the one stored in cookie are different
    // then store the new timezone in the cookie and refresh the page.
    else {
        var storedOffset = parseInt($.cookie(timezone_cookie));
        var currentOffset = new Date().getTimezoneOffset();
        // user may have changed the timezone
        if (storedOffset !== currentOffset) {
            $.cookie(timezone_cookie, new Date().getTimezoneOffset());
            location.reload();
        }
    }
}

$(function () {
    setTimezoneCookie();
});

// Image Post
$(document).ready(function () {
    if (!document.getElementById("imageGallery")) return;

    var arrSrc = [];
    var imageUrlArray = imagesUrl.split(",");
    for (index = 0; index < imageUrlArray.length; ++index) {
        arrSrc.push({
            src: imageUrlArray[index] + '?w=2048',
            srct: imageUrlArray[index] + '?w=600',
            title: ''
        })
    }
    var mosaicConfig = [];
    switch(imageUrlArray.length) {
        case 1:
            mosaicConfig = [
                { c: 1, r: 1, w: 3, h: 4 }
            ]
            break;
        case 2:
            mosaicConfig = [
                { c: 1, r: 1, w: 2, h: 3 },
                { c: 3, r: 1, w: 2, h: 3 }
            ]
            break;
        case 3:
            mosaicConfig = [
                { c: 1, r: 1, w: 4, h: 3 },
                { c: 1, r: 4, w: 2, h: 2 },
                { c: 3, r: 4, w: 2, h: 2 }
            ]
            break;
        case 4:
            mosaicConfig = [
                { c: 1, r: 1, w: 2, h: 2 },
                { c: 3, r: 1, w: 2, h: 2 },
                { c: 1, r: 3, w: 2, h: 2 },
                { c: 3, r: 3, w: 2, h: 2 }
            ]
            break;
        default: 
            mosaicConfig = [
                { c: 1, r: 1, w: 2, h: 2 },
                { c: 3, r: 1, w: 1, h: 1 },
                { c: 4, r: 1, w: 1, h: 1 },
                { c: 3, r: 2, w: 1, h: 1 },
                { c: 4, r: 2, w: 1, h: 1 }
            ]
    }
    var myCS = { thumbnail: { background: '#FFFFFF', borderColor: '#FFFFFF' } };
    $("#imageGallery").nanogallery2({
        thumbnailHeight: '200',
        thumbnailWidth: '200',
        galleryTheme: myCS,
        thumbnailDisplayInterval: '350',
        thumbnailDisplayTransition: 'slideUp2',
        thumbnailHoverEffect2: 'scale120',
        galleryDisplayMode: 'rows',
        galleryMaxRows: '1',
        thumbnailBorderHorizontal: '4',
        thumbnailBorderVertical: '4',
        galleryMosaic: mosaicConfig,
        items: arrSrc
    });
});