$(document).on("click", ".saveData", function () {
    $(".saveData").attr("disabled", "disabled");

    var jumlah = $("#jumlah").val();
    var metode = $("#metode").val();
    var namePengirim = $("#namePengirim").val();

    var dataPost = new Object();
    dataPost.jumlah = jumlah;
    dataPost.idmetode = metode;
    dataPost.nama_pengirim = namePengirim;
    dataPost.status = 1;
    dataPost.action = "add";

    debugger

    $.ajax({
        type: "POST",
        url: "../../TopUp/action",
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            console.log(response);
            $(".saveData").removeAttr("disabled");
            if (response.status == true) {
                alert('request saldo sukses');
            } else {
                alert('request saldo gagal');
            }
            $("#inlineForm .close").click();
        },
        failure: function (response) {
            console.log(response);
            alert('terjadi kesalahan, hubungi admin');
        }
    });

});


$(document).on("click", ".btnEdit", function () {
    id = $(this).attr("id")
    id = id.replace("id_", "")

    axios.get('/user', {
        params: {
            ID: 12345
        }
    }).then(function (response) {
        console.log(response);
    }).catch(function (error) {
        console.log(error);
    }).then(function () {
        // always executed
        console.log('always executed');
    });

});

$(document).on("click", "#pulsa", function () {
    window.location.href = "Transaksi/Index?produk=pulsa";
});

$(document).on("click", "#paketdata", function () {
    window.location.href = "Transaksi/Index?produk=data";
});

$(document).on("click", "#tokenlistrik", function () {
    window.location.href = "Transaksi/Index?produk=token";
});

$(document).on("click", "#ma", function () {
    window.location.href = "Transaksi/Index?produk=MA";
});

