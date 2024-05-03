

const popupWin = document.querySelector('.popup-window');
const ipFilter = document.getElementById("Destination");
const category = document.getElementById("Category");
const divFilter = document.getElementById("divFilter");
const startDate = document.querySelector('.js-first-date');
const endDate = document.querySelector('.js-last-date');
const dateFilter = document.querySelector('#dateFilter');
const person = document.querySelector('.js-count-adult');
const room = document.querySelector('.js-count-room');


const filterObj = {
    MinPrice: 0,
    MaxPrice: 40000,
    Score: [],
    Rank: [],
    Facilities: []
};

const listUrl = "https://localhost:7119/Customer/Hotels/ListEdit"

//list頁面側邊攔篩選功能
const hotelFacility = document.getElementById("divHotelFacility");
const roomFacility = document.getElementById("divRoomFacility");

const getHotelFacility = async () => {

    if (window.location.href === listUrl) {
        try {
            const res = await fetch("https://localhost:7119/api/HotelApi/HotelFacilities");
            if (!res.ok) {
                throw new Error("error");
            }
            const resData = await res.json();
            hotelFacility.innerHTML = resData.map(item =>
                `<div class="col-auto" onclick="pillClick(event, ${item.id}, '${item.category}')">
                <a href="" class="button -blue-1 bg-blue-1-05 text-blue-1 py-5 px-20 rounded-100" >${item.name}</a>
            </div>`).join('');
        } catch (error) {
            console.log(error.message);
        }
    }
}

const getRoomFacility = async () => {

    if (window.location.href === listUrl) {
        try {
            const res = await fetch("https://localhost:7119/api/HotelApi/RoomFacilities");
            if (!res.ok) {
                throw new Error("error");
            }
            const resData = await res.json();
            roomFacility.innerHTML = resData.map(item =>
                `<div class="col-auto" onclick="pillClick(event, ${item.id}, '${item.category}')">
                <a href="" class="button -blue-1 bg-blue-1-05 text-blue-1 py-5 px-20 rounded-100" >${item.name}</a>
            </div>`).join('');
        } catch (error) {
            console.log(error.message);
        }
    }
}

const pillClick = async (event, id, category) => {
    event.preventDefault();
    if (hotelFacility.childNodes.length != 0 || RoomFacility.childNodes.length != 0) {

        if (event.target.classList.contains('selected')) {

            event.target.classList.remove('selected');

            const index = filterObj.Facilities.indexOf(`${category}|${id}`);

            filterObj.Facilities.splice(index, 1);
            //移致畫面上方
            //scrollToTop();
            await sendRequest();
            console.log(filterObj.Facilities);
            return;
        }
        event.target.classList.add('selected');
        filterObj.Facilities.push(`${category}|${id}`);
        console.log(filterObj.Facilities);

        //移致畫面上方
        //scrollToTop();
        await sendRequest();
    }
}


ipFilter.addEventListener('input', async () => {

    const keyword = ipFilter.value;
    divFilter.innerHTML = '';
    popupWin.classList.add('-is-active');
    divFilter.innerHTML =
        `<div id="loadingDData" style="display:block">
         <img src="https://localhost:7119/img/frontstage/tube-spinner.svg" style="width:20px; height: 20px" />
         <span class="ml-10">資料載入中</span>
         </div>`
    await sendDestinationRequest(keyword);

});


const innerToInput = (name, category1) => {
    ipFilter.value = name;
    category.value = category1;
    popupWin.classList.remove('-is-active');
}

const sendDestinationRequest = async (keyword) => {
    try {
        const response = await fetch(`https://localhost:7119/api/HotelApi/RegionFilter?keyword=${keyword}`);
        const loading = document.getElementById("loadingDData");
        loading.style.display = 'none'
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


let maxPage = 0;

const payload = {
    Destination: '',
    Category: '',
    StartDate: '',
    EndDate: '',
    PersonNum: '',
    RoomNum: '',
    SortBy: "",
    SortType: "",
    Page: 1,
    PageSize: 5,
    FilterObj: {}
}

const sendRequest = async () => {

    payload.Destination = ipFilter.value;
    payload.Category = category.value;
    payload.StartDate = startDate.textContent.replace(/\s/g, '').replace("-", "");
    payload.EndDate = endDate.textContent.replace(/\s/g, '');
    payload.PersonNum = person.textContent;
    payload.RoomNum = room.textContent;
    payload.FilterObj = filterObj;

    sessionStorage.setItem('filter', JSON.stringify(payload));

    if (window.location.href === "https://localhost:7119/Customer/Hotels"
        || window.location.href.startsWith("https://localhost:7119/Customer/Hotels/HotelDetail")) {
        window.location.href = "https://localhost:7119/Customer/Hotels/ListEdit";
        return;
    }

    if (window.location.href === "https://localhost:7119/Customer/Hotels/ListEdit") {
        const hotelcards = document.getElementById('hotelCards');
        const spanHotelCount = document.getElementById('spanHotelCount');
        const pageNum = document.getElementById('pageNum');

        //一開始先顯示空白Card
        hotelcards.innerHTML = hotelCardsInit();

        const response = await fetch("https://localhost:7119/api/HotelApi/List", {
            headers: {
                "Content-Type": "application/json"
            },
            method: "POST",
            body: JSON.stringify(payload)
        });

        const responseData = await response.json();

        maxPage = responseData.totalPages;

        spanHotelCount.innerHTML = `<span class="fw-700">找到${responseData.totalCount}間住宿</span>`;

        hotelcards.innerHTML = responseData.hotelCards.map(x => {
            let starsHtml = '';
            for (let i = 0; i < x.rank; i++) {
                starsHtml += '<i class="icon-star text-10 text-yellow-2"></i> ';
            }
            let collectedHotel = "";
            if (x.isCollected) {
                collectedHotel = `<button class="button -blue-1 wishlist wishlist-selected size-30 rounded-full shadow-2" data-id="${x.hotelId}">
                                    <i class="icon-heart text-12"></i>
                                  </button>`
            }
            else {
                collectedHotel = `<button class="button -blue-1 wishlist size-30 rounded-full shadow-2" data-id="${x.hotelId}">
                                    <i class="icon-heart text-12"></i>
                                  </button>`
            }
            
            return `<div
    class="col-12">
    <div class="border-light p-4" style="border-radius:46px">
        <div class="row x-gap-20 y-gap-20">
            <div class="col-md-auto">
                <div class="cardImage ratio ratio-1:1 w-250 md:w-1/1 rounded-22">
                    <div class="cardImage__content">
                        <img class="rounded-22 col-12" src="/img/Hotel/${x.hotelsImagies[0]}" alt="image">
                    </div>
                    <!--收藏-->
                    <div class="cardImage__wishlist">
                        ${collectedHotel}
                    </div>
                </div>
            </div>
            <div class="col-md">
                <h3 class="text-18 lh-16 fw-700">
                    ${x.hotelName} <br class="lg:d-none"> <span class="fw-300">${x.hotelEngName}</span>
                        <div class="d-inline-block ml-10">
                            ${starsHtml}
                        </div>
                </h3>
                <div class="row x-gap-10 y-gap-10 items-center pt-10">
                    <div class="col-auto">
                        <p class="text-14">${x.city} ${x.region}</p>
                    </div>
                </div>
                <!--設施-->
                <div class="row x-gap-10 y-gap-10 pt-20">

                    ${x.hotelsFacilitiesName.map(facility =>
                `<div class="col-auto" id="divFacilities">
                        <div class="border-light rounded-100 py-5 px-20 text-14 lh-14">${facility}</div>
                    </div>`
            ).join('')}

                </div>
            </div>
            <div class="col-md-auto text-right md:text-left">
                <div class="row x-gap-10 y-gap-10 justify-end items-center md:justify-start">
                    <div class="col-auto">
                        <div class="text-14 lh-14 fw-500 text-blue-1">${x.scoreStr}</div>
                        <div class="text-14 lh-14 text-blue-1">${x.commentCount}則評論</div>
                    </div>
                    <div class="col-auto">
                        <div class="flex-center text-white fw-600 text-14 size-40 rounded-8 bg-blue-1">${x.score}</div>
                    </div>
                </div>
                <div class="">
                    <div class="text-22 lh-12 fw-600" style="margin-top:112px">NT$ ${x.price}</div>
                    <a href="https://localhost:7119/Customer/Hotels/HotelDetail?id=${x.hotelId}"
                        class="show-detail button -md -dark-1 bg-blue-1 text-white mt-24" style="border-radius:28px">查看詳情</a>
                </div>
            </div>
        </div>
    </div>
    </div>`}).join('');

        let pageEle = '';
        for (let i = 0; i < responseData.totalPages; i++) {
            pageEle += `<div class="col-auto">
                                <div class="size-40 flex-center rounded-full page-btn" onclick="pageNumClick(event, ${i + 1})">${i + 1}</div>
                             </div>`
        }
        pageNum.innerHTML = pageEle;
        setPageNumStyle();
        
        if (window.location.href.startsWith("https://localhost:7119/Customer/Hotels/HotelDetail")) {
            window.location.href = "https://localhost:7119/Customer/Hotels/HotelList";
        }
    }
}

const setPageNumStyle = () => {
    const pageBtn = document.querySelectorAll('.page-btn');
    pageBtn.forEach(btn => {
        if (parseInt(btn.innerText) != payload.Page) {
            btn.classList.remove('selected');
        }
        else {
            btn.classList.add('selected');
        }
    })
}

const hotelCardsInit = () => {

    let str = `<div class="col-12">
      <div class="border-light p-4" style="border-radius:46px">
        <div class="row x-gap-20 y-gap-20">
          <div class="col-md-auto">
            <div class="cardImage ratio ratio-1:1 w-250 md:w-1/1 rounded-22">
              <!--照片外框-->
              <div class="cardImage__content" style="background-color: #D0D0D0">
              </div>
            </div>
          </div>
          <div class="col-md-auto">
            <div class="text-18 lh-16 fw-500 mt-10">
              <div class="h-40 rounded-4" style="background-color: #D0D0D0; width:500px"></div>
              <br class="lg:d-none">
              <div class="h-40 rounded-4" style="background-color: #D0D0D0; width:500px"></div>
              <br class="lg:d-none">
              <div class="h-40 rounded-4" style="background-color: #D0D0D0; width:500px"></div>
            </div>
          </div>         
        </div>
      </div>
    </div>`
    for (let i = 0; i < 5; i++) {
        str += str
    }
    return str
}

const scrollToTop = () => {
    window.scrollTo({
        top: 0,
        behavior: 'smooth' // Smooth scrolling animation
    });
}
