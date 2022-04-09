function deleteTodo(i) {
    $.ajax({
        url: 'Home/Delete',
        type: 'POST',
        data: {
            id: i
        }, 
        success: function() {
            window.location.reload();
        }
    });
}

function populateForm(i) {
    $.ajax({
        url: 'Home/PopulateForm',
        type: 'GET',
        data: {
            id: i
        },
        dataType: 'json',
        success: function (response) {
            $("#Todo_Name").val(response.name);
            $("#Todo_Id").val(response.id);
            $("#form-button").val("Update Todo");
            $("#form-action").attr("action", "/Home/Update");
        }
    });
}

function done(i) {
    $.ajax({
        url:'Home/Done',
        type:'POST',
        data: {
            id: i
        },
        success: function(){
        window.location.reload();
        }
    });
}


