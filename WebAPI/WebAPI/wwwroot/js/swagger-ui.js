(function () {
    window.addEventListener("load", function () {
        setTimeout(function () {

            const logo = document.getElementsByClassName('link');
            logo[0].href = "https://blogdaytinhoc.com/";
            logo[0].target = "_blank";
            const linkIcon16 = document.createElement('link');
            linkIcon16.type = 'image/png';
            linkIcon16.rel = 'icon';
            linkIcon16.href = '/img/favicon.png';
            linkIcon16.sizes = '16x16';
            document.getElementsByTagName('head')[0].appendChild(linkIcon16);  
            logo[0].children[0].alt = "Logo";
            logo[0].children[0].src = "/img/Logo.jpg";
        });
    });
})();