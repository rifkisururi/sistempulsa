$(document).on("click", "#cariProduk", function () {
    var produk = $("#produk").val();
    var dest = $("#dest").val();
    window.location.href = `cariproduk?produk=${produk}&dest=${dest}`;
});

$(document).on("click", "#cekPln", function () {
    var produk = $("#produk").val();
    var dest = $("#dest").val();
    console.log('sini');
    $.get("/Transaksi/cekPln?no=" + dest, function (data) {
        console.log(typeof data); // string
        console.log(data); // HTML content of the jQuery.ajax page
        console.log(data.message); 
        $("#namaPln").html(data.message);

    });
});



$(document).on("click", ".history", function () {
    var produk = $("#produk").val();
    var dest = $(this).val();
    window.location.href = `cariproduk?produk=${produk}&dest=${dest}`;
});
