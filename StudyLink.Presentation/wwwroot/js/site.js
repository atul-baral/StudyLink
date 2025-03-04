// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    var element = $("#Layer_1");
    if (element.length && element.parent().length) {
        element.parent().trigger("click");
    }
});