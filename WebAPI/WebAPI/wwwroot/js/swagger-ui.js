(function () {
    window.addEventListener("load", function () {
        setTimeout(function () {

            var logo = document.getElementsByClassName('link');

            logo[0].children[0].alt = "Logo";
            logo[0].children[0].src = "/img/Logo.jpg";
        });
    });
})();