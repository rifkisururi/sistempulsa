$(document).on("click", ".pilihProduk", function () {
    var dest = $("#dest").val();
    var suppliyer = $(this).attr("suppliyer");
    var produkId = $(this).attr("produkid");

    console.log('suppliyer', suppliyer);
    console.log('produkId', produkId);
    window.location.href = `generate_order?produkId=${produkId}&suppliyer=${suppliyer}&dest=${dest}`;

});


$(document).on("click", ".history", function () {
    var produk = $("#produk").val();
    var dest = $(this).val();
    window.location.href = `cariproduk?produk=${produk}&dest=${dest}`;
});