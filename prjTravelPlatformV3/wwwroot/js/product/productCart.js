
const cartItemList = document.getElementById('cartItemList');
const cartItemNum = document.getElementById('cartItemNum');
const dataUserId = document.querySelector('input[data-userId]')

if (cartItemList && cartItemNum && dataUserId) {
    const userId = parseInt(dataUserId.getAttribute('data-userId'));
    async function getCartList (userId) {
        try {
            const res = await fetch(`/api/ProductApi/GetCartList?customerId=${userId}`);
            if (!res.ok) {
                throw new Error("取得購物車資訊時發生錯誤")
            }
            const resData = await res.json();
            cartItemList.innerHTML = resData.map(item => 
                `<div class="text-14 y-gap-15 js-dropdown-list">
            <div style="display: flex;">
                <div class="rounded" style="margin-right: 10px;margin-left: 5px;">
                    <img src="/img/Item/${item.specImg ? item.specImg : item.itemImg}" class="rounded" alt="" style="width: 50px; height: 50px;">
                </div>
                    <div style="display: flex; flex-direction: column;">
                    <h7 class="mb-2">${item.fProductName}</h7>
                    <div style="display: flex; align-items: baseline;">
                        <h7 class="mb-2">${item.fSpecName}</h7>
                        <span style="margin-left: 5px;">x${item.fQty}</span>
                    </div>
                </div>
            </div>
            </div>`
            ).join('');
            cartItemNum.textContent = resData.length;
        } catch (error) {
            console.log(error.message)
        }
    }
    getCartList(userId);
}
