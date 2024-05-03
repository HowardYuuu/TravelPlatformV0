// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


//sidebar control
$(document).ready(function () {
    //// Retrieve stored values from sessionStorage
    //var mytest = sessionStorage.getItem("test");
    //var mytest2 = sessionStorage.getItem("test2");


    //// If there's a stored ID, show its corresponding sub-item
    //if (mytest) {
    //    $(`#${mytest}`).next('.nav-sub-item').slideToggle();
    //    $(`#${mytest}`).find('.drop').addClass('rotate');
    //}

    //console.log(mytest2);

    // Handle click events on nav-links
    //$('.nav-item .nav-link').click(function () {

    //    var currentSubItem = $(this).next('.nav-sub-item');

    //    $('.nav-sub-item').not(currentSubItem).addClass("side-collapse");
    //    currentSubItem.toggleClass("side-collapse");

    //    //$('.nav-sub-item').not($(this).next('.nav-sub-item')).addClass("side-collapse");
    //    //$('.nav-link').not($(this)).find('.drop').removeClass('rotate');
    //    //$(this).find('.drop').toggleClass('rotate');
    //});



    //存cookie方法
    //var cookieSubItemId = $.cookie('subItemId');
    //if (cookieSubItemId) {
    //    // 根据保存的页面ID执行相应操作
    //    $("#" + cookieSubItemId).removeClass('side-collapse');
    //    // 这里可以根据需要触发其他相关操作
    //}

    //var cookieLinkId = $.cookie('linkId');
    //if (cookieLinkId) {
    //    // 根据保存的状态设置body的class
    //    $("#" + cookieLinkId).addClass('active');
    //    // 这里可以根据需要触发其他相关操作
    //}

    ////子項目收合
    //$("#side-bar .nav-item .nav-link").click(function () {
    //    var currentSubItem = $(this).next('.nav-sub-item');
    //    $('.nav-sub-item').not(currentSubItem).addClass("side-collapse");
    //    currentSubItem.toggleClass("side-collapse");
    //});

    ////將當前選中的子項目id及link id存入cookie
    //$(".nav-sub-item a").click(function () {

    //    const linkId = $(this).attr('id');
    //    const subItemId = $(this).closest('.nav-sub-item').attr('id');

    //    //cookie保存20分鐘
    //    const currentTime = new Date();

    //    const expirationTime = new Date(currentTime.getTime() + 20 * 60 * 1000);
    //    $.cookie('subItemId', subItemId, { path: "/", expires: expirationTime });
    //    $.cookie('linkId', linkId, { path: "/", expires: expirationTime });
    //})



    $(document).ready(function () {

        $('#side-bar .nav-item .nav-link').click(function () {
            $('.nav-sub-item').not($(this).next('.nav-sub-item')).slideUp();
            $(this).next('.nav-sub-item').slideToggle();
            $('#side-bar .nav-item .nav-link').not($(this)).find('.drop').removeClass('rotate');
            $(this).find('.drop').toggleClass('rotate');
        });

       

    })

});