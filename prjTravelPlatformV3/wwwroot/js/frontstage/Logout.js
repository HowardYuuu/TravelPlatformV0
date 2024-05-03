const btnLogout = document.getElementById("btnLogout");
const logout = async () => {
    
    const response = await fetch("/api/apilogin", {
        method: "DELETE"
    });
    const responseData = response.text();
    if (responseData) {
        window.location.href = "/"
    } else {
        return;
    }
};