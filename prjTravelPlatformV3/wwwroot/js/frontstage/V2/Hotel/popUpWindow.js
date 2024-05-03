var clickHere = document.querySelector('.Click-here');

// 獲取關閉按鈕和背景遮罩元素
var closeBtn = document.querySelector('.close-btn');
var bgOverlay = document.querySelector('.bg-overlay');

// 獲取自定義模態框主體元素
var customModelMain = document.querySelector('.custom-model-main');

// 點擊鏈接時添加模態框打開的類名
clickHere.addEventListener('click', function (hotelId) {

    customModelMain.classList.add('model-open');

});

// 點擊關閉按鈕或背景遮罩時移除模態框打開的類名
closeBtn.addEventListener('click', closeModal);
bgOverlay.addEventListener('click', closeModal);

// 關閉模態框的函數
function closeModal() {
    customModelMain.classList.remove('model-open');
}