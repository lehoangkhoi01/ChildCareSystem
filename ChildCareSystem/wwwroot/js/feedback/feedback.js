var count;
function starmark(item) {
    count = item.id[0];
    sessionStorage.starRating = count;
    var subid = item.id.substring(1);
    for (var i = 0; i < 5; i++) {
        if (i < count) {
            document.getElementById(i + 1 + subid).style.color = "#fed330";
        } else {
            document.getElementById(i + 1 + subid).style.color = "black";
        }
    }
    document.getElementById("ratePoint").value = count;
}