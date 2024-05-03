console.log("作用正常");
// 監聽 checkbox 的變更事件
document.getElementById("autoGenerateCheckbox").addEventListener("change", function () {
    var checkbox = this;
    var couponCodeInput = document.getElementById("couponCode");
    console.log("作用正常");
    if (checkbox.checked) {
        // 生成十碼折扣碼
        var discountCode = generateDiscountCode(10);
        // 將折扣碼填充到 input 元素中
        couponCodeInput.value = discountCode;
    } else {
        // 清空 input 元素
        couponCodeInput.value = '';
    }
});

// 生成指定長度的折扣碼
function generateDiscountCode(length) {
    var characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
    var discountCode = '';
    for (var i = 0; i < length; i++) {
        discountCode += characters.charAt(Math.floor(Math.random() * characters.length));
    }
    return discountCode;
};