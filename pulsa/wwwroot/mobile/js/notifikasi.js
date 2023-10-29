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
    var url = `transaksi/cekTransaksiPending?id=${id}`;
    // Fungsi untuk melakukan pengecekan status transaksi
    function checkStatus() {
        $.get(url, function (data, status) {
            console.log('respond transaksi pending', data);
            var statusTransaksi = data.status_transaksi;

            if (statusTransaksi !== "1") {
                var status = (statusTransaksi === "2") ? "berhasil" : "gagal";
                notifPopup = `Transaksi ${data.tujuan} ${status}`;
                const notif = new Notification(notifPopup, {
                    body: notifPopup,
                });
                alert(notifPopup);
            } else {
                // Jika status masih "1", lakukan pengecekan ulang setelah jeda waktu (misalnya, 1 detik)
                setTimeout(checkStatus, 1000); // Waktu dalam milidetik
            }
        });
    }

    // Mulai pengecekan pertama kali
    checkStatus();
}