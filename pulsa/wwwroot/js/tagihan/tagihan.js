$(document).ready(function () {
    getTagihan("", "", "");
});

$(document).on("change", ".getTagihan", function () {
    var group = $("#groupTagihan").val();
    var status = $("#statusTagihan").val();
    var type = $("#typeTagihan").val();

    $('#tagihanListrik').dataTable().fnClearTable();
    $('#tagihanListrik').dataTable().fnDestroy();
    getTagihan(group, status, type);
});

function getTagihan(group, status, type) {
    $('#tagihanListrik').DataTable({
        ajax: `../../TagihanAjax/Listrik?type=${type}&group=${group}&status=${status}`,
        "columns": [
            { "data": "group_tagihan" },
            { "data": "id_tagihan" },
            { "data": "nama_pelanggan" },
            { "data": "jumlah_tagihan" },
            {
                "mData": null,
                "bSortable": false,
                "mRender": function (o) {
                    var btnBayar = "";
                    var btnNonactive = "";
                    if (o.harus_dibayar != false) {
                        if (o.status_bayar == false) {
                            btnBayar = "<button class='btn btn-primary btn-sm btn-bayar' id='" + o.id + "'> Bayar </button>";
                        } else {
                            btnBayar = "<button class='btn btn-sucess btn-sm'> Lunas </button>";
                        }
                    }
                    

                    if (o.harus_dibayar == false) {
                        btnNonactive = "<button class='btn btn-sm btn-warning tambahkan' id='" + o.id + "'>Tambahkan</button>";
                    } else {
                        btnNonactive = "<button class='btn btn-sm btn-warning tinggalkan' id='" + o.id + "'>Tinggalkan</button>";
                    }

                    return `${btnBayar}  ${btnNonactive}`;
                }
            }

        ]
    });
}


$(document).on("click", ".btn-bayar", function () {
    id = $(this).attr("id")
    id = id.replace("id_", "")
    console.log("id",id);
    //$("#tambahTagihan").click()

    $.ajax({
        type: "GET",
        url: "../../TagihanAjax/getDetailMaster?idMaster=" + id,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            console.log(response);
            if (response.status == true) {
                //alert('get berhasil');
                $("#idTagihan").val(response.data.id);
                $("#inputGroupTagihan").val(response.data.group_tagihan);
                $("#typeTagihan").val(response.data.type_tagihan);
                $("#idPelanggan").val(response.data.id_tagihan);
                $("#admin").val(response.data.admin);
                $("#adminNotta").val(response.data.admin_notta);
                $("#nama_pelanggan").val(response.data.nama_pelanggan);

                var is_active = true;
            } else {
                alert('get gagal');
            }
            console.log(response);
        },
        failure: function (response) {
            console.log(response);
        }
    });

});
