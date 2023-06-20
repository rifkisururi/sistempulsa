$(document).on("click", "#cariProduk", function () {
    var produk = $("#produk").val();
    var dest = $("#dest").val();
    window.location.href = `cariproduk?produk=${produk}&dest=${dest}`;
});


$(document).on("click", ".history", function () {
    var produk = $("#produk").val();
    var dest = $(this).val();
    window.location.href = `cariproduk?produk=${produk}&dest=${dest}`;
});