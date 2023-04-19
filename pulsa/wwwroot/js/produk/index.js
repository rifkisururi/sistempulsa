$(document).ready(function () {
    getData();
});

$(document).on("change", ".getTagihan", function () {
    //var group = $("#groupTagihan").val();

    //$('#tagihan').dataTable().fnClearTable();
    //$('#tagihan').dataTable().fnDestroy();
    //getTagihan(group);
});

function getData() {
    $('#produk').DataTable({
        ajax: `../Produk/listProduk`,
        "columns": [
            { "data": "product_id" },
            {
                "mData": null,
                "bSortable": true,
                "mRender": function (o) {
                    var btnChange = ""
                    str = o.product_name;
                    if (o.margin == "" || o.margin == null) {
                        btnChange += "<div id='product_name_" + o.product_id + "'></div>"

                    } else {
                        btnChange += "<div id='product_name_" + o.product_id + "'>" + o.product_name + "</div>"
                    }
                    return `${btnChange}`;
                }
            },
            { "data": "product_detail" },
            { "data": "product_syarat" },
            { "data": "product_zona" },
            { "data": "category" },
            { "data": "type_produk" },
            { "data": "brand" },
            {
                "mData": null,
                "bSortable": true, 
                "mRender": function (o) {
                    var status_ext = "";
                    if (o.status == true) {
                        status_ext = "<span class='label label-default'>Aktif</span>"
                    } else {
                        status_ext = "<span class='label label-success'>Non aktive</span>"
                    }
                    //var btnChange = ""
                    //btnChange += "<button class='btn btn-warning btnEdit' id='id_" + o.id + "'>Edit</button>"
                    return `${status_ext}`;
                }
            },
            {
                "mData": null,
                "bSortable": true, 
                "mRender": function (o) {
                    var btnChange = ""
                    str = o.margin;
                    if (o.margin == "" || o.margin == null) {
                        btnChange += "<div id='margin_" + o.product_id + "'></div>"

                    } else {
                        btnChange += "<div id='margin_" + o.product_id + "'>" + o.margin + "</div>"
                    }
                    return `${btnChange}`;
                }
            },
            {
                "mData": null,
                "bSortable": true,
                "mRender": function (o) {
                    var btnChange = ""
                    str = o.bagihasil1;
                    if (o.bagihasil1 == "" || o.bagihasil1 == null) {
                        btnChange += "<div id='bagihasil1_" + o.product_id + "'></div>"

                    } else {
                        btnChange += "<div id='bagihasil1_" + o.product_id + "'>" + o.bagihasil1 + "</div>"
                    }
                    return `${btnChange}`;
                }
            },
            {
                "mData": null,
                "bSortable": true,
                "mRender": function (o) {
                    var btnChange = ""
                    str = o.bagihasil2;
                    if (o.bagihasil2 == "" || o.bagihasil2 == null) {
                        btnChange += "<div id='bagihasil2_" + o.product_id + "'></div>"

                    } else {
                        btnChange += "<div id='bagihasil2_" + o.product_id + "'>" + o.bagihasil2 +"</div>"
                    }
                    return `${btnChange}`;
                }
            },
            {
                "mData": null,
                "bSortable": true,
                "mRender": function (o) {
                    var btnChange = ""
                    str = o.price_suggest;
                    if (o.price_suggest == "" || o.price_suggest == null) {
                        btnChange += "<div id='price_suggest_" + o.product_id + "'></div>"

                    } else {
                        btnChange += "<div id='price_suggest_" + o.product_id + "'>" + o.price_suggest + "</div>"
                    }
                    return `${btnChange}`;
                }
            },
            {
                "mData": null,
                "bSortable": false,
                "mRender": function (o) {
                    var btnChange = ""

                    btnChange += "<button type='button' class='btn btn-sm btn-primary btnEdit' data-bs-toggle='modal' style='float:right;' data-bs-target='#inlineFormHarga' id='" + o.product_id +"'>Edit Harga</button>"
                    btnChange += "<button class='btn btn-sm btn-warning btnEdit' id='" + o.product_id +"'>Edit Info</button>"
                    btnChange += "<button class='btn btn-sm btn-danger btnRemove' id='" + o.product_id + "'>Hapus</button>"
                    btnChange += "<button class='btn btn-sm btn-info btnDetail' id='" + o.product_id + "'>Detail</button>"
                    return `${btnChange}`;
                }
            }
        ]
    });
}

$(document).on("click", ".btnEdit", function () {
    var id = $(this).attr("id");
    margin = $(`#margin_${id}`).html();
    bagihasil1 = $(`#bagihasil1_${id}`).html();
    bagihasil2 = $(`#bagihasil2_${id}`).html();
    price_suggest = $(`#price_suggest_${id}`).html();
    product_name = $(`#product_name_${id}`).html();

    $("#TitleProduk").html(product_name);
    $("#margin").val(margin);
    $("#bagihasil1").val(bagihasil1);
    $("#bagihasil2").val(bagihasil2);
    $("#price_suggest").val(price_suggest);
    console.log(margin);
});

$(document).on("click", ".saveData", function () {
    $(".modal-header .close").click();
    product_id = $("#product_id").val();
    margin = $(`#margin`).val();
    bagihasil1 = $(`#bagihasil1`).val();
    bagihasil2 = $(`#bagihasil2`).val();
    price_suggest = $(`#price_suggest`).val();

    var dataPost = new Object();
    dataPost.product_id = product_id
    dataPost.margin = margin
    dataPost.bagihasil1 = bagihasil1
    dataPost.bagihasil2 = bagihasil2
    dataPost.price_suggest = price_suggest
    console.log(dataPost);
    $.ajax({
        type: "POST",
        url: "../../Produk/updateData",
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
