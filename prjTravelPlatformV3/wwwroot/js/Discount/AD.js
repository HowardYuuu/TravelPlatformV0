
// // 定義圖片數組
// var images = [
//     "~/img/discount/ad/HotelDiscount.png",
//     "~/img/discount/ad/CarRent.png",

// ];

// // 隨機選擇一張圖片索引
// var randomIndex = Math.floor(Math.random() * images.length);

// // 隨機選擇的圖片路徑
// var randomImagePath = images[randomIndex];

// // 設置圖片 src
// document.getElementById('rotatingImage').src = randomImagePath;

// // 透過 query string 傳遞選擇的圖片索引以防止重複
// var imageLink = document.getElementById('imageLink');
// imageLink.href += "?imageIndex=" + randomIndex;

function toggleImage() {
    var adImg = document.getElementById("adImage");
    if (count == 0) {

        adImg.src = "/img/discount/ad/hoteldiscount.png";
    } else if (count == 1) {
        adImg.src = "/img/discount/ad/CarRent.png";
    } else if (count == 2) {
        adImg.src = "/img/discount/ad/Fly.png"
    }
};

document.addEventListener('DOMContentLoaded', function () {
    // 當文檔載入完成後，自動觸發該連結的點擊事件
    var link = document.querySelector('.button01');
    if (link) {
        // var adImg = document.getElementById("adImage");
        // var src01 = "/img/discount/ad/hoteldiscount.png";
        // var src02 = "/img/discount/ad/CarRent.png";


        // toggleImage();
        link.click();
    }
});


//SignalR的程式碼部分
var connection1 = new signalR.HubConnectionBuilder().withUrl("/adHub").build();

connection1.start().then(function (count) {
    console.log("SignalR 連線成功!");
}).catch(function (err) {
    return console.error(err.toString());
});

connection1.on("countMethod", function (count) {
    console.log("count");
    var adImg = document.getElementById("adImage");
    var aURL = document.getElementById("aURL");
    var pop = document.getElementById("popup");
    var url1 = "/Customer/CarRent/index";
    var url2 = "/Customer/Flight/index";
    var url3 = "/Customer/Products/index";
    var url4 = "/Customer/Destionation/index";
    var url5 = "/Customer/Visa/index";
    //var url2 = `@Url.Action("Index","Flight", new { area = "Customer" })`;
    //var url3 = `@Url.Action("Index","Products", new { area = "Customer" })`;
    //var url4 = `@Url.Action("Index","TravelPlan", new { area = "Customer" })`;
    //var url5 = `@Url.Action("Index","Visa", new { area = "Customer" })`;

    if (count == 0) {
        adImg.src = "/img/discount/ad/hotel01.png";
    } else if (count == 1) {
        aURL.href = url1;
        adImg.src = "/img/discount/ad/Car01.png";
    } else if (count == 2) {
        aURL.href = url2;
        adImg.src = "/img/discount/ad/Fly.png"
    } else if (count == 3) {
        aURL.href = url3;
        adImg.src = "/img/discount/ad/Product.png"
    } else if (count == 4) {
        aURL.href = url4;
        adImg.src = "/img/discount/ad/Travel.png"
    } else if (count == 5) {
        aURL.href = url5;
        adImg.src = "/img/discount/ad/Visa.png"
    }
    /* toggleImage()*/
    document.getElementById("button01").click();
    /*connection1.stop();*/
});
connection1.on("buttonClick", function () {
    console.log("服務器發送了按鈕點擊事件！");
    /* console.log(selectValue)*/
    // 在這裡觸發button01按鈕的事件
    document.getElementById("button01").click();

});




// //與Server建立連線
// connection.start().then(function () {
//     console.log("Hub 連線完成");
// }).catch(function (err) {
//     alert('連線錯誤: ' + err.toString());
// });

// // 更新連線 ID 列表事件
// connection.on("UpdList", function (jsonList) {
//     var list = JSON.parse(jsonList);
//     $("#IDList li").remove();
//     for (i = 0; i < list.length; i++) {
//         $("#IDList").append($("<li></li>").attr("class", "list-group-item").text(list[i]));
//     }
// });

// // 更新用戶個人連線 ID 事件
// connection.on("UpdSelfID", function (id) {
//     $('#SelfID').html(id);
// });

// // 更新聊天內容事件
// connection.on("UpdContent", function (msg) {
//     $("#Content").append($("<li></li>").attr("class", "list-group-item").text(msg));
// });

//傳送訊息
// $('#sendButton').on('click', function () {
//     let selfID = $('#SelfID').html();
//     let message = $('#message').val();
//     let sendToID = $('#sendToID').val();
//     connection.invoke("SendMessage", selfID, message, sendToID).catch(function (err) {
//         alert('傳送錯誤: ' + err.toString());
//     });
// });
