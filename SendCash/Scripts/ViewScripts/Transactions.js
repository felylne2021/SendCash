function format(d) {
    // `d` is the original data object for the row
    var bal = d.AccountBalance;

    return '<table cellpadding="5" cellspacing="0" border="0" style="padding-left:50px;">' +
        '<tr>' +
        '<td>Account Balance: <b>IDR ' + bal.toLocaleString() + '<b></td>' +
        '</tr>' +
        '</table>';
}

$(document).ready(function () {
    var transactionDT = $('#transactionTable ').DataTable({

        ajax: {
            url: "https://" + window.location.host + "/api/Transactions",
            dataSrc: ""
        },
        columns: [
            {
                "className": 'details-control',
                "orderable": false,
                "data": null,
                "defaultContent": ''
            },
            {
                data: "TransactionId"
            },
            {
                data: "TransactionDt"
            },
            {
                data: "Sender"
            },
            {
                data: "AccountNumber"
            },
            {
                data: "SenderBank"
            },
            {
                data: "isComplete",
                render: function (data) {
                    if (!data)
                        return "<button isComplete=" + data + " class='btn-danger js-delete'>Process</button>";

                },
                "orderable": false
            }
        ],
        "order": [[1, 'asc']]
    });

    $("#transactionTable").on("click", ".js-delete", function () {
        var button = $(this);
        bootbox.confirm("This account will be permanently deleted. Procees?", function (result) {
            if (result) {
                $.ajax({
                    url: "api/Transactions/" + button.attr("data-account-id"),
                    method: "DELETE",
                    success: function () {
                        transactionDT.row(button.parents("tr")).remove().draw();
                    }
                });
            }
        });

    });

    // Add event listener for opening and closing details
    $('#transactionTable tbody').on('click', 'td.details-control', function () {
        var tr = $(this).closest('tr');
        var row = accountDT.row(tr);

        if (row.child.isShown()) {
            // This row is already open - close it
            row.child.hide();
            tr.removeClass('shown');
        }
        else {
            // Open this row
            row.child(format(row.data())).show();
            tr.addClass('shown');
        }
    });

});
