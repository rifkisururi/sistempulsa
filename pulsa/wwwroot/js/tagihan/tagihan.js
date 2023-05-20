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
                        
                        if (o.request_bayar == true && o.status_bayar == false) {
                            console.log('log record', o);
                            btnBayar = "<button class='btn btn-info btn-sm'> In Proses </button>";
                        } else if (o.request_bayar == true && o.status_bayar == true) {
                            btnBayar = "<button class='btn btn-success btn-sm'> Sukses </button>";
                        } else if (o.request_bayar == false && o.status_bayar == false) {
                            btnBayar = "<button class='btn btn-primary btn-sm btn-bayar' id='" + o.idMaster + "'> Bayar </button>";
                        }
                    }

                    if (o.request_bayar != true) {
                        if (o.harus_dibayar == false) {
                            btnNonactive = "<button class='btn btn-sm btn-warning tambahkan' id='" + o.id + "'>Tambahkan</button>";
                        } else {
                            btnNonactive = "<button class='btn btn-sm btn-warning tinggalkan' id='" + o.id + "'>Tinggalkan</button>";
                        }
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
        url: "../../TagihanAjax/bayarTagihanIni?idMaster=" + id,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            console.log(response);
            if (response.responseCode == 400) {
                alert(response.responseMessage);
            } else {
                alert(response.responseData.message);
            }
            $(".getTagihan").change();
        },
        failure: function (response) {
            console.log(response);
        }
    });

});
