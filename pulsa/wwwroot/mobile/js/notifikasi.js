$(document).ready(function () {

    Notification.requestPermission().then((result) => {
        console.log(result);
        if (result == "denied") {
            alert('Mohon izinkan notifikasi untuk pengalaman yang lebih baik');
        }
    });
    var url = `transaksi/listTransaksiPending`;
    $.get(url, function (data, status) {
        console.log(data.data);
        data.data.forEach(cekTransaksiPending);
    });
});

function cekTransaksiPending(id){
    console.log(id);
    var url = `transaksi/cekTransaksiPending?id=${id}`;
    $.get(url, function (data, status) {
        console.log('respond transaksi pending', data);
        if (data.status_transaksi != "1") {
            var status = "";
            if (data.status_transaksi == "2") {
                status = "berhasil";
            } else {
                status = "gagal";
            }

            const notif = new Notification(`Transaksi ${data.tujuan} ${status}`, {
                body: `Transaksi ${data.tujuan} ${status}`,
            });
       }
    });
}