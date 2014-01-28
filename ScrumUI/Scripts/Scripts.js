/// <reference path="jquery-1.8.2.js" />
$(document).ready(function () {
    $('#AssignedTo').change(onSelectedResourceChanged);
    $('#EstimatedHours').change(onSelectedResourceChanged);
}
);
function TaskParam(resourceID, estimatedHours) {
    this.ResourceID = resourceID;
    this.EstimatedHours = estimatedHours;
}


function onSelectedResourceChanged() {
    var idResource = $('#AssignedTo option:selected').val();
    var estimadedHours = $('#EstimatedHours').val();
    var param = new TaskParam();
    param.ResourceID = idResource;
    param.EstimatedHours = estimadedHours;

    $.ajax({
        url: "http://localhost/ScrumUI/Task/IsAllowedToAssign/",
        type: "POST",
        data: JSON.stringify(param),
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            if (response == false)
                alert("Resource has exceeded its hours capacity limit, please reassign")
                //   $('div#realHours').text(response);
            else getEstimate(param);
        },
        error: function (err) {
            alert("error");
        }
    });
}
function getEstimate(param) {
    $.ajax({
        url: "http://localhost/ScrumUI/Task/GetEstimate/",
        type: "POST",
        data: JSON.stringify(param),
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('p#realHours').text(response.RealHours);
            $('p#suggestedName').text(response.SuggestedName);
        }
    });
};


function autoassignTasks() {
    var dialog = confirm("This will overwrite all tasks assignments! Changes cannot be reverted. Are you sure you want to proceed? ");
    if (dialog == true) {
        $.ajax({
            url: "http://localhost/ScrumUI/Task/AutoAssignTasks/",
            type: "GET",

            contentType: "application/json; charset=utf-8",
            success: function (response) {
                alert("AutoAssignment completed succesfully");
            },
            error: function (response) {
                alert("Task total hours are bigger than resources total capacity. Unable to assign!");
            }
        });
    }
};