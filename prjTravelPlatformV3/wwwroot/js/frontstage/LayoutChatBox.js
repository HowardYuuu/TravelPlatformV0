if (claimName) {
    const imgPath = 'C' + claimId;
    $('#ProfileAvatar').attr("src", `/img/uploads/${imgPath}.jpg`);
    $('#ProfileName').html(claimName);
    $('#ProfileCharacter').html("一般會員");
} else {
    $('#ProfileAvatar').attr("src", "/img/avatar.png");
    $('#ProfileName').html("歡迎登入");
    $('#ProfileCharacter').html("登入並上線開啟聊天室");
}


const OutterIcon = document.getElementById('OutterIcon');
const chatBoxStatus = localStorage.getItem('ChatBox');
const onlineStatus = localStorage.getItem('onlineToggle');
if (chatBoxStatus === 'on') {
    const container = document.querySelector('.app-container');
    container.style.display = 'flex';
    OutterIcon.style.display = 'none';
}


function outterclick() {
    const container = document.querySelector('.app-container');
    if (OutterIcon.style.display === 'block') { //開
        container.style.display = 'flex';
        OutterIcon.style.display = 'none';
        localStorage.setItem('ChatBox', 'on');
    }
    else { //關
        container.style.display = 'none';
        OutterIcon.style.display = 'block';
        localStorage.setItem('ChatBox', 'off');
    }
}

function innerclick() {
    OutterIcon.click();
}

if (!claimId) {
    const switchStatus = document.getElementById('switchStatus');
    switchStatus.setAttribute('disabled', 'true');
    const ProfileBox = document.getElementById('ProfileBox');
    ProfileBox.addEventListener('click', function (e) {
        if (e.target.classList.contains('switch-status') || e.target.parentNode.classList.contains('switch-status')) {
            alert('請先登入');
        }
    })
} else {
    const switchStatus = document.getElementById('switchStatus');
    switchStatus.removeAttribute('disabled', 'true');
}
const C = localStorage.getItem('theme');
//document.body.setAttribute('data-theme', 'pink');
document.body.setAttribute('data-theme', C);
let title = document.querySelectorAll(".chat-list-header");
let totalHeight = 0;

for (let i = 0; i < title.length; i++) {
    let totalHeight = 0;
    title[i].addEventListener("click", function () {
        let result = this.nextElementSibling;
        let activeSibling = this.nextElementSibling.classList.contains('active');
        this.classList.toggle('active');
        result.classList.toggle("active");
        if (!activeSibling) {
            for (i = 0; i < result.children.length; i++) {
                totalHeight = totalHeight + result.children[i].scrollHeight + 40;
            }
        } else {
            totalHeight = 0;
        }
        result.style.maxHeight = totalHeight + "px";
    });
}

const themeColors = document.querySelectorAll('.theme-color');

themeColors.forEach(themeColor => {
    themeColor.addEventListener('click', (e) => {
        themeColors.forEach(c => c.classList.remove('active'));
        const theme = themeColor.getAttribute('data-color');
        document.body.setAttribute('data-theme', theme);
        localStorage.setItem('theme', theme);
        themeColor.classList.add('active');
    });
});

themeColors.forEach(element => {
    if (element.classList.contains('active')) {
        element.classList.remove('active');
    }
});


var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();


//上下線切換toggle
function OnOffLineChange(tc) {
    const main = document.querySelector('.app-main');
    const ChatListHeader = document.getElementById('ChatListHeader');
    const ChatList = document.getElementById('ChatList');
    const OnlineServiceHead = document.getElementById('OnlineServiceHead');
    if (!tc) { //離線
        localStorage.setItem('onlineToggle', 'off');
        main.style.display = 'none';
        ChatListHeader.style.display = 'none';
        OnlineServiceHead.style.display = 'none';

        ChatListHeader.classList.add('active');
        ChatList.classList.remove('active');
        ChatList.style.maxHeight = '0px';

        connection.stop();


    } else { //上線
        localStorage.setItem('onlineToggle', 'on');
        ChatListHeader.style.display = 'flex';
        OnlineServiceHead.style.display = 'block';
        //與Server建立連線
        connection.start().then(function () {
            console.log("Hub 連線完成");
            try {
                connection.invoke("ClientConnection", "Customer");
            }
            catch (err) {
                alert(err)
            }
        }).catch(function (err) {
            alert('連線錯誤: ' + err.toString());
        });
    }
}



// 更新用戶個人連線 ID 事件
connection.on("UpdSelfID", function (id) {
    $('#SelfID').html(id);
});

// 更新用戶個人Member ID 事件，順便更新用戶個人頭貼
connection.on("UpdSelfMemberID", function (id) {
    $('#SelfMemberID').html(id);
    //$('#ProfileAvatar').attr("src", `/img/${id}.png`); //改成該用戶頭貼
});

// 更新用戶個人名字
connection.on("UpdSelfName", function (name) {
    $('#ProfileName').html(name);
    //$('#ProfileCharacter').html("客戶");
})


//更新客服員工數量
connection.on("EmployeeCount", function (count) {
    const OnlineServiceHead = document.getElementById('OnlineServiceHead');
    if (count > 0) {
        OnlineServiceHead.style.display = 'block';
    }
    else {
        OnlineServiceHead.style.display = 'none';
    }
})



//更新連線 ID 列表事件
var prevActiveIndex = -1; // 未有active的li時先索引先給-1
connection.on("UpdList", function (jsonList) {
    var list = JSON.parse(jsonList);
    console.log(list)
    $("#ChatList li").remove();
    prevActiveIndex = -1; //重置索引
    for (i = 0; i < list.length; i++) {

        if (list[i].MemberId === $('#SelfMemberID').html()) {
            continue;
        }

        $("#ChatList").append(
            $("<li></li>").addClass("chat-list-item")
                .click(function () {
                    var currentIndex = $(this).index(); // 當前li的索引
                    $("#ChatList li").not(this).removeClass("active");
                    $(this).toggleClass("active");
                    var id = $(this).find('.chat-list-name').attr("id");
                    $("#sendToID").text(id);
                    var mid = $(this).find('.chat-list-name').attr("name");
                    $("#sendToMemberID").text(mid);

                    var MemberID = $(this).find('.chat-list-name').attr("name"); // 當前li中的span的name

                    // 若active的li改變就要刷新app-main(用memberId查詢)
                    if (currentIndex !== prevActiveIndex) {
                        // 发送状态变化的信息
                        console.log("Active changed from " + prevActiveIndex + " to " + currentIndex);
                        prevActiveIndex = currentIndex;
                        //console.log(MemberID);

                        // "要求"查詢並重新載入聊天內容
                        const selfID = document.getElementById('SelfID').innerText;
                        const SelfMemberID = document.getElementById('SelfMemberID').innerText;
                        connection.invoke("SearchHistory", MemberID, SelfMemberID, selfID)
                            .then(function () {
                                var div = document.getElementById('Content');
                                div.scrollTop = div.scrollHeight;
                            })
                            .catch(function (err) {
                                alert('傳送錯誤: ' + err.toString());
                            });
                    }

                    if ($("#ChatList li.active").length === 0) {
                        $(".app-main").css("display", "none");
                    }
                    else {
                        $(".app-main").css("display", "flex");
                    }
                    //關掉線上客服的active
                    const OnlineServiceHead = document.getElementById('OnlineServiceHead');
                    OnlineServiceHead.classList.remove('active');
                })
                .append(
                    $("<img>").attr("src", `/img/uploads/${list[i].MemberId}.jpg`).attr("alt", "chat"),
                    $("<span></span>").addClass("chat-list-name").attr("id", list[i].ConnectString).attr("name", list[i].MemberId).text(list[i].MemberName)
                )
        );
    }
    $("#ChatListCount").text(list.length - 1);
});


//重新載入聊天歷史內容
connection.on("LoadMessageHistoryFromDB", function (MessageHistoryListJson) {
    var MessageHistoryList = JSON.parse(MessageHistoryListJson);
    //console.log(MessageHistoryList);
    $("#Content div").remove(); //先洗掉舊的

    const SelfMID = document.getElementById('SelfMemberID').innerText;
    for (var m of MessageHistoryList) {

        if (m.SelfMemberId == SelfMID) {
            var $messageWrapper = $("<div></div>").addClass("message-wrapper reverse");
            var $profilePic = $("<img>").addClass("message-pp").attr("src", `/img/uploads/${SelfMID}.jpg`).attr("alt", "profile-pic");
            var $messageBoxWrapper = $("<div></div>").addClass("message-box-wrapper");
            var $messageBox = $("<div></div>").addClass("message-box").text(m.Message);
            var $timestamp = $("<span></span>").text(m.TimeStamp.substring(0, 16));

            $messageWrapper.append($profilePic);
            $messageBoxWrapper.append($messageBox, $timestamp);
            $messageWrapper.append($messageBoxWrapper);

            $("#Content").append($messageWrapper);

        } else {
            var $messageWrapper = $("<div></div>").addClass("message-wrapper");
            var $profilePic = $("<img>").addClass("message-pp").attr("src", `/img/uploads/${m.SelfMemberId}.jpg`).attr("alt", "profile-pic");
            var $messageBoxWrapper = $("<div></div>").addClass("message-box-wrapper");
            var $messageBox = $("<div></div>").addClass("message-box").text(m.Message);
            var $timestamp = $("<span></span>").text(m.TimeStamp.substring(0, 16));

            $messageWrapper.append($profilePic);
            $messageBoxWrapper.append($messageBox, $timestamp);
            $messageWrapper.append($messageBoxWrapper);

            $("#Content").append($messageWrapper);
        }

    }
})

connection.on("LoadMessageHistoryFromLocal", function (MessageHistoryListJson) {
    var MessageHistoryList = JSON.parse(MessageHistoryListJson);
    //console.log(MessageHistoryList);

    const SelfMID = document.getElementById('SelfMemberID').innerText;
    for (var m of MessageHistoryList) {

        if (m.SelfMemberId == SelfMID) {
            var $messageWrapper = $("<div></div>").addClass("message-wrapper reverse");
            var $profilePic = $("<img>").addClass("message-pp").attr("src", `/img/uploads/${SelfMID}.jpg`).attr("alt", "profile-pic");
            var $messageBoxWrapper = $("<div></div>").addClass("message-box-wrapper");
            var $messageBox = $("<div></div>").addClass("message-box").text(m.Message);
            var $timestamp = $("<span></span>").text(m.TimeStamp.substring(0, 16));

            $messageWrapper.append($profilePic);
            $messageBoxWrapper.append($messageBox, $timestamp);
            $messageWrapper.append($messageBoxWrapper);

            $("#Content").append($messageWrapper);

        } else {
            var $messageWrapper = $("<div></div>").addClass("message-wrapper");
            var $profilePic = $("<img>").addClass("message-pp").attr("src", `/img/uploads/${m.SelfMemberId}.jpg`).attr("alt", "profile-pic");
            var $messageBoxWrapper = $("<div></div>").addClass("message-box-wrapper");
            var $messageBox = $("<div></div>").addClass("message-box").text(m.Message);
            var $timestamp = $("<span></span>").text(m.TimeStamp.substring(0, 16));

            $messageWrapper.append($profilePic);
            $messageBoxWrapper.append($messageBox, $timestamp);
            $messageWrapper.append($messageBoxWrapper);

            $("#Content").append($messageWrapper);
        }

    }
})



// 更新聊天內容事件
connection.on("UpdContent", function (messageJson) {
    //$("#Content").append($("<div></div>").attr("class", "message-wrapper").text(msg));
    console.log(1)
    var messageInfo = JSON.parse(messageJson);
    //console.log(messageInfo)
    if (messageInfo.SelfMemberId.includes("E") && prevActiveIndex === -1) { //員工to客戶
        var $messageWrapper = $("<div></div>").addClass("message-wrapper");
        var $profilePic = $("<img>").addClass("message-pp").attr("src", `/img/uploads/${messageInfo.SelfMemberId}.jpg`).attr("alt", "profile-pic");
        var $messageBoxWrapper = $("<div></div>").addClass("message-box-wrapper");
        var $messageBox = $("<div></div>").addClass("message-box").text(messageInfo.Message);
        var $timestamp = $("<span></span>").text(messageInfo.TimeStamp.substring(0, 16));

        $messageWrapper.append($profilePic);
        $messageBoxWrapper.append($messageBox, $timestamp);
        $messageWrapper.append($messageBoxWrapper);

        $("#Content").append($messageWrapper);
        var div = document.getElementById('Content');
        div.scrollTop = div.scrollHeight;
    }
    if (messageInfo.SendToId) {
        const selfID = document.getElementById('SelfID').innerText; // 自己目前的連線ID
        const sendToMemberID = document.getElementById('sendToMemberID').innerText; // 我目前開啟誰的小窗的MemberID
        if (messageInfo.SelfId === selfID) {
            var $messageWrapper = $("<div></div>").addClass("message-wrapper reverse");
            var $profilePic = $("<img>").addClass("message-pp").attr("src", `/img/uploads/${messageInfo.SelfMemberId}.jpg`).attr("alt", "profile-pic");
            var $messageBoxWrapper = $("<div></div>").addClass("message-box-wrapper");
            var $messageBox = $("<div></div>").addClass("message-box").text(messageInfo.Message);
            var $timestamp = $("<span></span>").text(messageInfo.TimeStamp.substring(0, 16));

            $messageWrapper.append($profilePic);
            $messageBoxWrapper.append($messageBox, $timestamp);
            $messageWrapper.append($messageBoxWrapper);

            $("#Content").append($messageWrapper);
        } else {
            if (messageInfo.SelfMemberId === sendToMemberID) { // 如果目前開啟的小窗對象等於寄件者，如果不是要再寫提醒
                var $messageWrapper = $("<div></div>").addClass("message-wrapper");
                var $profilePic = $("<img>").addClass("message-pp").attr("src", `/img/uploads/${messageInfo.SelfMemberId}.jpg`).attr("alt", "profile-pic");
                var $messageBoxWrapper = $("<div></div>").addClass("message-box-wrapper");
                var $messageBox = $("<div></div>").addClass("message-box").text(messageInfo.Message);
                var $timestamp = $("<span></span>").text(messageInfo.TimeStamp.substring(0, 16));

                $messageWrapper.append($profilePic);
                $messageBoxWrapper.append($messageBox, $timestamp);
                $messageWrapper.append($messageBoxWrapper);

                $("#Content").append($messageWrapper);
                var div = document.getElementById('Content');
                div.scrollTop = div.scrollHeight;
            }
        }
    } else {
        console.log(messageInfo.Message);
    }
});

//傳送訊息
const sendButton = document.getElementById('sendButton');
sendButton.addEventListener('click', function () {
    const OnlineServiceHead = document.getElementById('OnlineServiceHead');
    if (OnlineServiceHead.style.display === 'none') {
        sendToCustomer();
    }
    else {
        sendToEmployee();
    }
});

function sendToCustomer() {
    const message = document.getElementById('message').value;
    if (!message) { return; }
    const selfID = document.getElementById('SelfID').innerText;
    const SelfMemberID = document.getElementById('SelfMemberID').innerText;
    const sendToID = document.getElementById('sendToID').innerText;
    const sendToMemberID = document.getElementById('sendToMemberID').innerText;

    connection.invoke("SendMessage", selfID, message, sendToID, SelfMemberID, sendToMemberID)
        .then(function () {
            var div = document.getElementById('Content');
            div.scrollTop = div.scrollHeight;
        })
        .catch(function (err) {
            alert('傳送錯誤: ' + err.toString());
        });
    document.getElementById('message').value = '';
    document.getElementById('message').focus();
}


function keyEnter(e) {
    const sendButton = document.getElementById('sendButton');
    if (e.keyCode === 13) {
        sendButton.click();
    }
}


function sendToEmployee() {
    const message = document.getElementById('message').value;
    if (!message) { return; }
    const selfID = document.getElementById('SelfID').innerText;
    const SelfMemberID = document.getElementById('SelfMemberID').innerText;

    connection.invoke("SendMessageToEmployee", selfID, message, SelfMemberID)
        .then(function () {
            var div = document.getElementById('Content');
            div.scrollTop = div.scrollHeight;
        })
        .catch(function (err) {
            alert('傳送錯誤: ' + err.toString());
        });
    document.getElementById('message').value = '';
    document.getElementById('message').focus();
}



function LoadEmployeeMessage(t) {
    const main = document.querySelector('.app-main');
    if (!(t.classList.contains('active'))) { //關=>開
        main.style.display = 'flex';
        t.classList.add('active')
        prevActiveIndex = -1;

        //其他customer的active全部拿掉
        const ChatList = document.getElementById('ChatList');
        const List = ChatList.querySelectorAll('.active');
        List.forEach(element => {
            if (element.classList.contains('active')) {
                element.classList.remove('active');
            }
        });



        //load對話紀錄進來
        const selfID = document.getElementById('SelfID').innerText;
        const SelfMemberID = document.getElementById('SelfMemberID').innerText;
        connection.invoke("SearchEmployeeHistory", SelfMemberID, selfID)
            .then(function () {
                var div = document.getElementById('Content');
                div.scrollTop = div.scrollHeight;
            })
            .catch(function (err) {
                alert('傳送錯誤: ' + err.toString());
            });
    }
    else { //開=>關
        t.classList.remove('active');
        main.style.display = 'none';
    }
}

//顏色主題儲存
const currentColor = document.querySelector('.' + C);
currentColor.classList.add('active');

//上下線儲存
document.addEventListener("DOMContentLoaded", function () {
    const onlineStatus = localStorage.getItem('onlineToggle');

    if (onlineStatus === 'on') {
        switchStatus.click();
    }
});


function demo() {
    document.getElementById('message').value = '你好，請問可以去哪裡購買簽證代辦服務?';
}