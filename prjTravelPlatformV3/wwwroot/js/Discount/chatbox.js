// // 獲取聊天框頭部和內容
// var chatContainer = document.getElementById("chatContainer");
// var chatHeader = document.getElementById("chatHeader");
// var chatBody = document.getElementById("chatBody");
// var chatExpanded = false; // 變量來跟踪聊天框的狀態

// // 設置鼠標按下事件監聽器
// chatHeader.addEventListener("mousedown", function (event) {
//     // 設置鼠標移動事件監聽器
//     document.addEventListener("mousemove", moveChat);

//     // 設置鼠標釋放事件監聽器
//     document.addEventListener("mouseup", function () {
//         // 刪除鼠標移動事件監聽器
//         document.removeEventListener("mousemove", moveChat);
//     });

//     // 函數用於移動聊天框
//     function moveChat(event) {
//         event.preventDefault();
//         // 設置聊天框的新位置
//         chatContainer.style.right = window.innerWidth - event.clientX + "px";
//         chatContainer.style.bottom = window.innerHeight - event.clientY + "px";
//     }
// });

// // 聊天框頭部點擊事件處理程序
// chatHeader.addEventListener("click", function () {
//     // 切換聊天框內容的顯示狀態
//     if (!chatExpanded) { // 如果聊天框是縮小的，則將其展開
//         chatContainer.style.width = "300px"; // 顯示時正常寬度
//         chatContainer.style.height = "300px"; // 顯示時正常高度
//         chatBody.style.display = "block";
//         chatExpanded = true; // 設置為展開狀態
//     } else { // 如果聊天框是展開的，則將其縮小
//         chatContainer.style.width = "40px"; // 隱藏時較小寬度
//         chatContainer.style.height = "40px"; // 隱藏時較小高度
//         chatBody.style.display = "none";
//         chatExpanded = false; // 設置為縮小狀態
//     }
// });



// 獲取聊天框頭部和內容
var chatContainer01 = document.getElementById("chatContainer01");
var chatHeader01 = document.getElementById("chatHeader01");
var chatHeaderButton01 = document.getElementById("chatHeaderButton01");
var chatBody01 = document.getElementById("chatBody01");
var chatExpanded01 = false; // 變量來跟踪聊天框的狀態

// // 設置鼠標按下事件監聽器
// chatHeader.addEventListener("mousedown", function (event) {
//     // 設置鼠標移動事件監聽器
//     document.addEventListener("mousemove", moveChat);

//     // 設置鼠標釋放事件監聽器
//     document.addEventListener("mouseup", function () {
//         // 刪除鼠標移動事件監聽器
//         document.removeEventListener("mousemove", moveChat);
//     });

//     // 函數用於移動聊天框
//     function moveChat(event) {
//         event.preventDefault();
//         // 設置聊天框的新位置
//         chatContainer.style.right = window.innerWidth - event.clientX + "px";
//         chatContainer.style.bottom = window.innerHeight - event.clientY + "px";
//     }
// });

// 聊天框頭部點擊事件處理程序
chatHeader01.addEventListener("click", function () {
    // 切換聊天框內容的顯示狀態
    if (!chatExpanded01) { // 如果聊天框是縮小的，則將其展開
        chatContainer01.style.width = "400px"; // 顯示時正常寬度
        chatContainer01.style.height = "550px"; // 顯示時正常高度
        chatContainer01.style.borderRadius = "20px";
        chatHeaderButton01.width = "40px";
        chatHeaderButton01.height= "40px";
        /*console.log("AI客服有問題");*/
        chatBody01.style.display = "block";
        chatExpanded01 = true; // 設置為展開狀態
    } else { // 如果聊天框是展開的，則將其縮小
        chatContainer01.style.width = "50px"; // 隱藏時較小寬度
        chatContainer01.style.height = "50px"; // 隱藏時較小高度
        chatContainer01.style.borderRadius = "999em";
        chatHeaderButton01.width = "100px";
        chatHeaderButton01.height = "100px";

        chatBody01.style.display = "none";
        chatExpanded01 = false; // 設置為縮小狀態
    }
});



