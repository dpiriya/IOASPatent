var getAcceptedURL = 'GetAcceptedTapalDetails'
var dbAcceptTapal;
$(function () {
    var tapalDetails = 'PopupTapalDetails';
    $("#AcceptedTapalList").jsGrid({
        paging: true,
        pageIndex: 1,
        pageSize: 5,
        editing: false,
        filtering: true,
        // selecting: true,
        controller: {

            loadData: function (filter) {
                return $.grep(dbAcceptTapal, function (ow) {
                    return (!filter.TapalNo || ow.TapalNo.toLowerCase().indexOf(filter.TapalNo.toLowerCase()) > -1)
                         &&(!filter.TapalType || ow.TapalType.toLowerCase().indexOf(filter.TapalType.toLowerCase()) > -1)
                         &&(!filter.SenderDetails || ow.SenderDetails.toLowerCase().indexOf(filter.SenderDetails.toLowerCase()) > -1)
                         &&(!filter.Department || ow.Department.toLowerCase().indexOf(filter.Department.toLowerCase()) > -1)
                         &&(!filter.User || ow.User.toLowerCase().indexOf(filter.User.toLowerCase()) > -1);
                });
            }

        },

        fields: [
           { type: "number", name: "slNo", title: "S.No", editing: false, width: 70 },
            { type: "number", name: "TapalId", title: "TapalId", visible: false },
            { type: "number", name: "CreateUserId", title: "Create UserId", visible: false },
              { type: "text", name: "TapalNo", title: "Tapal No", editing: false, width: 150 },
            { type: "text", name: "TapalType", title: "Tapal Type", editing: false },
            { type: "text", name: "SenderDetails", title: "Sender Details", editing: false },
            //{ type: "text", name: "InwardDate", title: "Inward Date", editing: false },
            { type: "text", name: "Department", title: "Department", editing: false, width: 130 },
            { type: "text", name: "User", title: "User", editing: false },
            //{ type: "text",   name: "Remarks",       title: "Remarks",        editing: false },
            { type: "text", name: "OutwardDate", title: "Outward Date", editing: false },
               { type: "text", name: "strAction", title: "Action", editing: false, width: 110 },
               {
                   name: "DocDetail",
                   title: "Documents",width:120,
                   itemTemplate: function (value, item) {
                       var elementDiv = $("<div>");
                       elementDiv.attr("class", "ls-dts");
                       $.each(item.DocDetail, function (index, itemData) {
                           var $link = $("<a>").attr("class", "ion-document icn").attr("href", itemData.href).attr("target", "_blank").html('');
                           elementDiv.append($link);
                       });
                       return elementDiv;
                   }
               },
               {
                   type: "control", width: 50,deleteButton: false, editButton: false,
                   _createFilterSwitchButton: function () {
                       return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false)
                   },
                   itemTemplate: function (value, item) {
                       var $result = jsGrid.fields.control.prototype.itemTemplate.apply(this, arguments);

                       var $customButton = $("<button>")
                           .attr("class", "ion-eye")
                           .click(function (e) {
                               $.ajax({
                                   type: "POST",
                                   url: tapalDetails,
                                   data: { TapalId: item.TapalId },
                                   success: function (result) {
                                       $("#popup").html(result);
                                       $('#notify_modal').modal('toggle');
                                   },
                                   error: function (err) {
                                       console.log("error1 : " + err);
                                   }
                               });
                               e.stopPropagation();
                           });
                        return $result.add($customButton);
                       //return $("<div>").append($customButton).append($customButtonEdit);
                   }
               },

        ],
    });
    $("#AcceptedTapalList").jsGrid("option", "filtering", false);
    loadDetails();

});
var loadDetails = function loadDetails() {
    $.ajax({
        type: "GET",
        url: getAcceptedURL,
        data: param = "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            dbAcceptTapal = result;
            $("#AcceptedTapalList").jsGrid({ data: dbAcceptTapal });
        },
        error: function (err) {
            console.log("error : " + err);
        }

    });
};