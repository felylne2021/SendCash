
$(document).on("click", "#loginBtn", function () {

    var name = $("#accName").val();
    var number = $("#accNum").val();
    
    $.ajax({
        url: 'https://' + window.location.host + '/Accounts',
        type: 'POST',
        dataType: 'html',
        data: {           
            AccountName: name,
            AccountNumber: number            
        },
        success: function (data) {
            alert("You successfully login, " + name + ".");            
            location.window.href = 'https://' + window.location.host + '/Accounts';
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("Your credentials are incorrect.");
        }
    });
});