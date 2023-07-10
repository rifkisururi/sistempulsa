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
                    if (o.autopay == 1) {
                        label = "<span class='badge bg-warning'>Yes</span><br>"
                        if (o.autopay_day == 0) { 
                            label += "Setiap ada tagihan"
                        } else {
                            label += "Setiap tanggal " + o.autopay_day + " jam " + o.autopay_hour
                        }

                    } else {
                        label = "<span class='badge bg-success'>NO</span>"
                    }

                    return `${label}`;
                }
            },
            {
                "mData": null,
                "bSortable": true,
                "mRender": function (o) {
                    var label = ""
                    if (o.is_active == true) {
                        label = "<span class='badge bg-success'>Active</span>"
                    } else {
                        label = "<span class='badge bg-warning'>Inactive</span>"
                    }

                    return `${label}`;
                }
            },
            {
                "mData": null,
                "bSortable": false,
                "mRender": function (o) {
                    var btnChange = ""
                    btnChange += "<button class='btn btn-warning btnEdit' id='id_" + o.id +"'>Edit</button>"
                    return `${btnChange}`;
                }
            }
        ]
    });
}

$(document).on("click", ".saveData", function () {
    var group = $("#groupTagihan").val();
    var type_tagihan = $("#typeTagihan").val();
    var id_tagihan = $("#idPelanggan").val();
    var group_tagihan = $("#inputGroupTagihan").val();
    var admin = $("#admin").val();
    var admin_notta = $("#adminNotta").val();
    var id = $("#idTagihan").val();
    var nama_pelanggan = $("#nama_pelanggan").val();;
    var is_active = true;

    var dataPost = new Object();
    dataPost.type_tagihan = type_tagihan;
    dataPost.id_tagihan = id_tagihan;
    dataPost.group_tagihan = group_tagihan;
    dataPost.admin = parseInt(admin);
    dataPost.admin_notta = parseInt(admin_notta);
    dataPost.nama_pelanggan = nama_pelanggan;
    dataPost.is_active = is_active;
    if (id == "") {
        dataPost.id = '00000000-0000-0000-0000-000000000000';
        dataPost.action = "add";
    } else {
        dataPost.id = id;
        dataPost.action = "update";
    }
    console.log('dataPost', dataPost);

    debugger

    $.ajax({
        type: "POST",
        url: "../../TagihanAjax/action",
        data: JSON.stringify(dataPost),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            console.log(response);
        },
        failure: function (response) {
            console.log(response);
        }
    });
});

$(document).on("click", ".btnEdit", function () {
    id = $(this).attr("id")
    id = id.replace("id_","")
    $("#tambahTagihan").click()
    $.ajax({
        type: "GET",
        url: "../../TagihanAjax/getDetailMaster?idMaster="+id,
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
