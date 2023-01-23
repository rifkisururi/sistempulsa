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
    var is_active = true;

    var dataPost = new Object();
    dataPost.type_tagihan = type_tagihan;
    dataPost.id_tagihan = id_tagihan;
    dataPost.group_tagihan = group_tagihan;
    dataPost.admin = admin;
    dataPost.admin_notta = admin_notta;
    dataPost.is_active = is_active;
    dataPost.id = '00000000-0000-0000-0000-000000000000';
    dataPost.action = "add";
    console.log('dataPost', dataPost);
  

    //var oForm = $('form')[0];
    //var oData = new FormData(oForm);
    //oData.append("submissionData", JSON.stringify(dataPost));

    //var data2 = {
    //    "type_tagihan": id_tagihan,
    //    "group_tagihan": group_tagihan
    //};
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
