const popupWin = document.querySelector('.popup-window');
const ipFilter = document.getElementById("Destination");
const category = document.getElementById("Category");
const divFilter = document.getElementById("divFilter");
const startDate = document.querySelector('.js-first-date');
const endDate = document.querySelector('.js-last-date');
const dateFilter = document.querySelector('#dateFilter');
const person = document.querySelector('.js-count-adult');
const room = document.querySelector('.js-count-room');
//const btnSearch = document.getElementById("btnSearch");




//const filterDataFormLacol = sessionStorage.getItem('hoteFilterData');
//if (filterDataFormLacol) {
//    const data = JSON.parse(filterDataFormLacol);
//    ipFilter.value = data.destination;
//    startDate.innerText = data.sDate;
//    endDate.innerText = data.eDAte;
//    person.innerText = data.personNum;
//    room.innerText = data.roomNum;
//}


ipFilter.addEventListener('input', () => {

    const keyword = ipFilter.value;
    divFilter.innerHTML = '';
    popupWin.classList.add('-is-active');
    divFilter.innerHTML = `<img id="loadingDData" src="https://localhost:7119/img/frontstage/tube-spinner.svg" style="width:20px; height: 20px; display:block" />`
    //let timerId;
    //clearTimeout(timerId);

    //// 设置一个新的计时器，在用户停止输入后 500 毫秒再调用 API
    //timerId = setTimeout(, 2000);
    sendFetchRequest(keyword)
    
});

btnSearch.addEventListener('click', (event) => {

    let data = {
        'destination' : ipFilter.value,
        'sDate': startDate.innerText,
        'eDAte': endDate.innerText,
        'personNum': person.innerText,
        'roomNum': room.innerText,
    };


    /*sessionStorage.setItem("hoteFilterData", JSON.stringify(data));*/
})


const innerToInput = (name, category) => {
    ipFilter.value = name;
    category.value = category;
    popupWin.classList.remove('-is-active');
}

const sendFetchRequest = async (keyword) => {
    try {
        const response = await fetch(`https://localhost:7119/api/HotelApi/RegionFilter?keyword=${keyword}`);
        const loading = document.getElementById("loadingDData");
        loading.style.display = 'none';
        if (!response.ok) {
            throw new Error('抱歉，查無結果');
        }
        const responseData = await response.json();
        if (responseData.length === 0) {
            divFilter.innerHTML = '抱歉，查無結果';
            return;
        }
        for (const key in responseData) {

            divFilter.innerHTML +=
                `<div class="y-gap-5 js-results">
                    <div>
                        <button type="button" class="-link d-block col-12 text-left rounded-4 px-20 py-15" onclick="innerToInput('${responseData[key].name}', '${responseData[key].category}')">
                            <div class="d-flex">
                            <div class="icon-location-2 text-light-1 text-20 pt-4"></div>
                                <div class="ml-10">
                                    <div class="text-15 fw-500">${responseData[key].name}</div>
                                </div>
                            </div>
                            </button>
                        </div>
                </div>`
        }
    } catch (error) {
        divFilter.innerHTML = '抱歉，查無結果';
    }
}






