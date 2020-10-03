// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$('.collapse').on('hidden.bs.collapse',
    function() {
        $('#collapseMain').collapse('show');
    });

function copyTextToClipboard() {
    var copyText = $('#linkToCopy').text();
    navigator.clipboard.writeText(copyText);
} 
