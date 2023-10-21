$(document).on("click", "#cariProduk", function () {
    let produk = $("#produk").val();
    let dest = $("#dest").val();
    let typeProduk = $("#typeProduk").val();
    if (typeProduk == undefined)
        window.location.href = `cariproduk?produk=${produk}&dest=${dest}`;
    else
        window.location.href = `cariproduk?produk=${produk}&dest=${dest}&typeProduk=${typeProduk}`;
});

$(document).on("click", "#cekPln", function () {
    let dest = $("#dest").val();
    console.log('sini');
    $.get("/Transaksi/cekPln?no=" + dest, function (data) {
        console.log(typeof data); // string
        console.log(data); // HTML content of the jQuery.ajax page
        console.log(data.message); 
        $("#namaPln").html(data.message);

    });
});



$(document).on("click", ".history", function () {
    let produk = $("#produk").val();
    let dest = $(this).val();
    window.location.href = `cariproduk?produk=${produk}&dest=${dest}`;
});
