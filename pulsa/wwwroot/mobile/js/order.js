$(document).on("click", "#beli", function () {
    var harga_jual = $("#hargajual").val();
    var pembeli = $("#pembeli").val();
    var pin = $("#pin").val();
    var id = window.location.search;

    window.location.href = `${window.location.origin}/transaksi/submitorder${id}&harga_jual=${harga_jual}&pin=${pin}&pembeli=${pembeli}`;
});