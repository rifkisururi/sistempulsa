$(document).ready(function () {
    getTagihan();
});

function getTagihan() {
    // todo add filter date frontend
    $('#tagihanListrik').dataTable().fnClearTable();
    $('#tagihanListrik').dataTable().fnDestroy();
    $('#topupList').DataTable({
        ajax: `../../Topup/getHistoryTopup`,
        "columns": [
            {
                "mData": null,
                "bSortable": true,
                "mRender": function (o) {
                    const dateRequest = new Date(parseInt(o.waktu_action));
                    return `${dateRequest.toLocaleString()}`;
                }
            },
            
            { "data": "penggunaNama" },
            { "data": "idmetode" },
            {
                "mData": null,
                "bSortable": true,
                "mRender": function (o) {
                    var saldoAwal = new Intl.NumberFormat().format(o.saldo_awal)
                    return `${saldoAwal}`;
                }
            },
            {
                "mData": null,
                "bSortable": true,
                "mRender": function (o) {
                    var jumlahRequest = new Intl.NumberFormat().format(o.jumlah)
                    return `${jumlahRequest}`;
                }
            },
            {
                "mData": null,
                "bSortable": true,
                "mRender": function (o) {
                    var saldoAkhir = new Intl.NumberFormat().format(o.saldo_akhir)
                    return `${saldoAkhir}`;
                }
            },
            { "data": "nama_pengirim" },
            { "data": "statusTxt" },
            { "data": "actionNama" }
            

        ]
    });
}


$(document).on("click", ".btnApprove", function () {
    id = $(this).attr("id")
    id = id.replace("id_", "")
    console.log(id);

    var dataPost = new Object();
    dataPost.id = id;
    dataPost.action = "approve";

    action(dataPost);
});

function action(dataPost) {
    debugger;
    $.ajax({
        type: "POST",
        url: "../../TopUp/action",
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            console.log(response);
            if (response.status == true) {
                alert(dataPost.action+' request saldo sukses');
            } else {
                alert(dataPost.action + 'request saldo gagal');
            }
            //getTagihan()
        },
        failure: function (response) {
            console.log(response);
            alert('terjadi kesalahan, hubungi admin');
        }
    });
}
