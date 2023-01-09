$(document).ready(function () {
    getTagihan("");
});

$(document).on("change", ".getTagihan", function () {
    var group = $("#groupTagihan").val();

    $('#tagihan').dataTable().fnClearTable();
    $('#tagihan').dataTable().fnDestroy();
    getTagihan(group);
});

function getTagihan(group) {
    $('#tagihan').DataTable({
        ajax: `../../TagihanAjax?group=${group}`,
        "columns": [
            { "data": "group_tagihan" },
            { "data": "id_tagihan" },
            { "data": "nama_pelanggan" },
            { "data": "admin" },
            { "data": "admin_notta" },
            {
                "mData": null,
                "bSortable": true,
                "mRender": function (o) {
                    var label = ""
                    if (o.is_active == true) {
                        label = "<span class='badge bg-success'>Active</span>"
                    } else {
                        label = "<span class='badge bg-success'>Inactive</span>"
                    }

                    return `${label}`;
                }
            },
            {
                "mData": null,
                "bSortable": false,
                "mRender": function (o) {
                    
                    var btnChange = ""
                    if (o.is_active == true) {
                        btnChange = "<button class='btn btn-danger'>Hapus</button>"
                    } else {
                        btnChange = "<span class='badge bg-success'>Tambahkan</span>"
                    }
                    return `${btnChange}`;
                }
            }

        ]
    });
}