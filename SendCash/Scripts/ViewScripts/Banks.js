$(document).ready(function () {
    var bankDT = $('#bankTable ').DataTable({
        "columnDefs": [
        {
            "className": "dt-center", "targets": 2
        }],
        "order": [[0, 'asc']],
        ajax: {
            url: "https://" + window.location.host + "/api/Banks",
            dataSrc: ""
        },
        columns: [
            {
                data: "BankId",
                defaultContent: ""
            },
            {
                data: "BankName"
            },
            {
                data: "BankId",
                render: function (data) {
                    return "<button data-bank-id=" + data + " class='btn-primary' id='bankDetailsBtn' data-toggle='modal' data-target='#viewDetailsModal'>View Details</button>";
                },
                "searchable": false,
                "orderable": false
            }
        ],
        "order": [[1, 'asc']]
    });

    bankDT.on('order.dt search.dt', function () {
        bankDT.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
});

$(document).on("click", "#bankDetailsBtn", function () {
    $.ajax({
        url: 'https://' + window.location.host + '/api/Banks/' + $(this).attr("data-bank-id"),
        type: 'GET',
        success: function () {
            alert("Succ");
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("Error.");
        }
    });

});