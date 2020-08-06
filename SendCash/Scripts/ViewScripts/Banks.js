$(document).ready(function () {
    var bankDT = $('#bankTable ').DataTable({

        ajax: {
            url: "https://" + window.location.host + "/api/Banks",
            dataSrc: ""
        },
        columns: [
            {
                data: "BankName",
                "order": [0, 'asc']
            },
            {
                data: "BankId",
                "orderable": false,
                render: function (data) {
                    return "<button data-bank-id=" + data + " class='btn-primary' id='bankDetailsBtn' data-toggle='modal' data-target='#viewDetailsModal'>View Details</button>";
                }
            }
        ],
        "order": [[1, 'asc']]
    });

});

$(document).on("click", "#bankDetailsBtn", function () {
    $.ajax({
        url: 'https://' + window.location.host + '/api/Banks/' + $(this).attr("data-bank-id"),
        type: 'GET',
        success: function () {
            alert("Succ");
            //return "<b>" + data.BankName + data.BankAddress + data.BankPhone + "</b>"
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("Error.");
        }
    });

});