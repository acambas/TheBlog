//Create Namespace
window.blogNamespace = window.blogNamespace || {};


window.blogNamespace.baseContext = (function ($, Spinner) {
    //Private
    var baseUrl;
    baseContext = {
        init: init,
        rootUrl: rootUrl,
        basicSpinner: basicSpinner
    };
    return baseContext;

    //Public
    function init(Url) {
        this.baseUrl = Url;
    }

    function rootUrl() {
        return this.baseUrl;
    }

    function basicSpinner () {
        $("#spinner-loading").fadeIn();
        var opts = {
            lines: 12, // The number of lines to draw
            length: 7, // The length of each line
            width: 4, // The line thickness
            radius: 10, // The radius of the inner circle
            color: '#000', // #rgb or #rrggbb
            speed: 1, // Rounds per second
            trail: 60, // Afterglow percentage
            shadow: false, // Whether to render a shadow
            hwaccel: false // Whether to use hardware acceleration
        };
        var target = document.getElementById('spinner-loading');
        var spinner = new Spinner(opts).spin(target);
    }

})($,Spinner)