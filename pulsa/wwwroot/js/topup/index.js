$(document).ready(function () {
    getTopupPending();
});

function getTopupPending() {
    $('#topupList').dataTable().fnClearTable();
    $('#topupList').dataTable().fnDestroy();
    $('#topupList').DataTable({
        ajax: `../../Topup/getRequestTopup`,
        "columns": [
            {
                "mData": null,
                "bSortable": true,
                "mRender": function (o) {
                    const dateRequest = new Date(parseInt(o.waktu));
                    return `${dateRequest.toLocaleString()}`;
                }
            },
            
            { "data": "penggunaNama" },
            { "data": "idmetode" },
            {
                "mData": null,
                "bSortable": true,
                "mRender": function (o) {
                    var jumlahRequest = new Intl.NumberFormat().format(o.jumlah)
                    return `${jumlahRequest}`;
                }
            },
           
            { "data": "nama_pengirim" },
            {
                "mData": null,
                "bSortable": false,
                "mRender": function (o) {
                    var btnChange = ""
                    btnChange += "<button class='btn btn-primary btnApprove' id='id_" + o.id +"'> <span class='spinner-border spinner-border-sm' role='status' aria-hidden='true' style='display:none;'></span> Terima</button> "
                    btnChange += "<button class='btn btn-danger btnReject' id='id_" + o.id + "'><span class='spinner-border spinner-border-sm' role='status' aria-hidden='true' style='display:none;'></span> Tolak</button>"
                    return `${btnChange}`;
                }
            }

        ]
    });
}


$(document).on("click", ".btnApprove", function () {
    var $btn = $(this);
    $btn.prop('disabled', true);
    $btn.find('span.spinner-border').show();

    id = $(this).attr("id")
    id = id.replace("id_", "")
    console.log(id);

    var dataPost = new Object();
    dataPost.id = id;
    dataPost.action = "approve";

    action(dataPost);
});


$(document).on("click", ".btnReject", function () {
    var $btn = $(this);
    $btn.prop('disabled', true);
    $btn.find('span.spinner-border').show();

    id = $(this).attr("id")
    id = id.replace("id_", "")
    console.log(id);

    var dataPost = new Object();
    dataPost.id = id;
    dataPost.action = "reject";

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
            getTopupPending();
        },
        failure: function (response) {
            console.log(response);
            alert('terjadi kesalahan, hubungi admin');
        }
    });
}
