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
                    if (o.status_bayar == false) {
                        btnBayar = "<button class='btn btn-primary btn-sm'> Bayar </button>";
                    } else {
                        btnBayar = "<button class='btn btn-sucess btn-sm'> Lunas </button>";
                    }

                    if (o.harus_dibayar == null || o.harus_dibayar == true) {
                        btnNonactive = "<button class='btn btn-sm btn-warning tinggalkan' id='" + o.id + "'>Tinggalkan</button>";
                    } else {
                        btnNonactive = "<button class='btn btn-sm btn-warning tambahkan' id='" + o.id + "'>Tambahkan</button>";
                    }

                    return `${btnBayar}  ${btnNonactive}`;
                }
            }

        ]
    });
}