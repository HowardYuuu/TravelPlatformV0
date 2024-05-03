
const hotelType = document.getElementById("divHotelType");
const hotelFacility = document.getElementById("divHotelFacility");
const roomFacility = document.getElementById("divRoomFacility");


const getHotelFacility = async () => {
    try {
        const res = await fetch("https://localhost:7119/api/HotelApi/HotelFacilities");
        if (!res.ok) {
            throw new Error("error");
        }
        const resData = await res.json();
        hotelFacility.innerHTML = resData.map(item =>
            `<div class="col-auto pill" onclick="pillClick(event, ${item.id}, '${item.category}')">
                <div class="border-light rounded-100 py-5 px-20 text-14 lh-14">${item.name}</div>
            </div>`).join('');
    } catch (error) {
        console.log(error.message);
    }

}

const getRoomFacility = async () => {
    try {
        const res = await fetch("https://localhost:7119/api/HotelApi/RoomFacilities");
        if (!res.ok) {
            throw new Error("error");
        }
        const resData = await res.json();
        roomFacility.innerHTML = resData.map(item =>
            `<div class="col-auto pill" onclick="pillClick(event, ${item.id}, '${item.category}')">
                <div class="border-light rounded-100 py-5 px-20 text-14 lh-14 data-category="${item.category}">${item.name}</div>
            </div>`).join('');
    } catch (error) {
        console.log(error.message);
    }
}

const filteList = [];
const pillClick = (event, id, category) => {
    if (hotelFacility.childNodes.length != 0 || RoomFacility.childNodes.length != 0) {

        if (event.target.classList.contains('selected')) {

            event.target.classList.remove('selected');
                              
            const index = filteList.indexOf(`${category}|${id}`);

            filteList.splice(index, 1);
            console.log(filteList);
            return;
        }
        event.target.classList.add('selected');
        filteList.push(`${category}|${id}`);
        console.log(filteList);
    }
}




