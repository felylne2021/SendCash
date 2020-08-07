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
    var accountDT = $('#accountTable ').DataTable({

        ajax: {
            url: "https://" + window.location.host + "/api/Accounts",
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
                data: "AccountName"
            },
            {
                data: "AccountNumber"
            },
            {
                data: "BankName"
            },
            {
                data: "AccountId",
                render: function (data) {
                    return "<button data-account-id=" + data + " class='btn-danger js-delete'>Delete</button>";
                },
                "orderable": false
            }
        ],
        "order": [[1, 'asc']]
    });

    $("#accountTable").on("click", ".js-delete", function () {
        var button = $(this);
        bootbox.confirm("This account will be permanently deleted. Procees?", function (result) {
            if (result) {
                $.ajax({
                    url: "api/Accounts/" + button.attr("data-account-id"),
                    method: "DELETE",
                    success: function () {
                        accountDT.row(button.parents("tr")).remove().draw();
                    }
                });
            }
        });

    });

    // Add event listener for opening and closing details
    $('#accountTable tbody').on('click', 'td.details-control', function () {
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

    // load banks for dropdown in add account form
    $.ajax({
        type: "GET",
        url: "https://" + window.location.host + "/api/Banks",
        data: "{}",
        success: function (data) {
            var s = '<option value="" disabled selected>Please Select a Bank</option>';
            for (var i = 0; i < data.length; i++) {
                s += '<option value="' + data[i].BankId + '">' + data[i].BankName + '</option>';
            }
            $("#banks").html(s);
        }
    });

});

// save new account
$(document).on("click", "#saveAccount", function () {
    var token = $('input[name="__RequestVerificationToken"]', $('#addAccountForm')).val()
    var name = $("#accName").val();
    var number = $("#accNum").val();
    var bankId = $("#banks").val();
    var bal = $("#accBal").val();

    var flag = 0;   // check validity

    // check number
    if (number == '') {
        $("#validAccNum").text("Please insert account number.");
        flag++;
    }
    else if (isNaN(number)) {
        $("#validAccNum").text("Please insert number only.");
        flag++;
    }
    else $("#validAccNum").text("");

    // check name
    if (name == '') {
        $("#validAccName").text("Please insert account name.");
        flag++;
    }
    else $("#validAccName").text("");

    // check bank

    console.log(bankId);
    if (bankId == null) {
        $("#validBank").text("Please choose a bank.");
        flag++;
    }
    else $("#validBank").text("");

    // check initial balance
    if (bal == '') {
        $("#validAccBal").text("Please insert account's initial balance.");
        flag++;
    }
    else if (isNaN(bal)) {
        $("#validAccBal").text("Please insert number only.");
        flag++;
    }
    else if (bal < 50000) {
        $("#validAccBal").text("Initial balance must be at least IDR 50,000.");
        flag++;
    }
    else $("#validAccBal").text("");

    if (flag != 0) {
        console.log(flag);
        return false;
    }

    $.ajax({
        url: 'https://' + window.location.host + '/Accounts/Create',
        type: 'POST',
        dataType: 'html',
        data: {
            __RequestVerificationToken: token,
            AccountName: name,
            AccountNumber: number,
            BankId: bankId,
            AccountBalance: bal
        },
        success: function (data) {

            $("#addAccountForm")[0].reset();

            $('#accountTable').DataTable().ajax.reload();

        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("Error.");
        }
    });

});

// close form -> reset form
$(document).on("click", "#closeForm", function () {

    $("#addAccountForm")[0].reset();
});