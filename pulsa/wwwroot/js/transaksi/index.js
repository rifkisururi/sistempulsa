
$(document).on("click", "#lanjut", function () {
    var $btn = $(this);
    $btn.prop('disabled', true);
    $btn.find('span.spinner-border').show();
    var produk = $("#produk").val();
    var dest = $("#dest").val();
    window.location = `/transaksi/   choseproduk?produk=${produk}&dest=${dest}`;
});