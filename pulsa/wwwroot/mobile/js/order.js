$(document).on("click", "#beli", function () {
    var harga_jual = $("#hargajual").val();
    var pembeli = $("#pembeli").val();
    var pin = $("#pin").val();
    var id = window.location.search;
    $.ajax({
        type: "GET",
        url: `${window.location.origin}/transaksi/submitorder${id}&harga_jual=${harga_jual}&pin=${pin}&pembeli=${pembeli}`,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            console.log(response);
            if (response.status == false) {
                alert(response.message)
            } else {
                alert('transaksi sedang di proses');
                id = id.replace('?idTransaksi=', '')
                window.location.href = `${window.location.origin}/mutasi/detail?id=${id}`;
            }
        },
        failure: function (response) {
            console.log(response);
            alert('terjadi kesalahan, hubungi admin');
        }
    });
});
