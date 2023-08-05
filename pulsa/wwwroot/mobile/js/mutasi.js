$(document).ready(function () {
    getMutasi(0, 5);
});


function getMutasi(from, take) {
    var url = `Mutasi/ajaxIndexMutasi?from=${from}&take=${take}`;
    $.get(url, function (data, status) {
        //alert("Data: " + data + "\nStatus: " + status);
        generateHtml(data);
    });
    $("#form").val(parseInt(from) + parseInt(take));

};

function generateHtml(data) {
    $("#more").remove();
    var dataMutasi = data.data;
    for (var i = 0; i < dataMutasi.length; i++) {
        var color = "green"
        if (dataMutasi[i].jumlah_mutasi.substring(0, 1) == "-") {
            color = "red"
        }
        var html = `
        <div class="card" id="${dataMutasi[i].id}" type="${dataMutasi[i].type}">
            <div class="container">
                <h5>${dataMutasi[i].produk}</h5>
                <h10>${dataMutasi[i].note}</h10>
                <div class="card-icon-right"><p style="color:${color}">${dataMutasi[i].jumlah_mutasi}</p></div>
            </div>
        </div>
        `;

        $("#dataMutasi").append(html);
    }
    var html = `
        <div id='more'>
            <button class='btn btn-info btn-sm'>Lebih banyak</button>
            <br><br>
            <br><br>
        <div>
    `;


    $("#dataMutasi").append(html);

}

$(document).on("click", ".card", function () {
    var id = $(this).attr('id'); 
    var type = $(this).attr('type'); 

    if (type == "Pembelian") {
        window.location.href = `${window.location.origin}/mutasi/detail?id=${id}`;
    } else {

    }
});


$(document).on("click", "#more", function () {
    getMutasi($("#form").val(), 5);
});