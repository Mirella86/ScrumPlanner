/// <reference path="jquery-1.8.2.js" />
$(document).ready(function () {
    $('#AssignedTo').change(onSelectedResourceChanged);
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
        url: "/Task/GetEstimate/",
        type: "POST",
        data: JSON.stringify(param),
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('div#realHours').text(response);
        }
    });
}