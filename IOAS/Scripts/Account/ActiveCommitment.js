var getProjectURL = 'GetCommitDetails'
var db;
$(function () {

    //var loadControlsURL = 'LoadActionDDL',
      //UpdateStatusURL = 'UpdateStatusDetails';
    UpdateStatusURL = 'PopupCommitment';
    var statusList;

    //$.ajax({
    //    type: "GET",
    //    url: loadControlsURL,
    //    data: param = "",
    //    contentType: "application/json; charset=utf-8",
    //    dataType: "json",
    //    async: false,
    //    success: function (result) {
            
    //        statusList = result;
    //    },
    //    error: function (err) {
    //        console.log("Error in loadControls : " + err);
    //    }
    //});

    $("#ActiveCommitList").jsGrid({
        paging: true,
        pageIndex: 1,
        pageSize: 5,
        editing: false,
        filtering: true,
        controller: {
            loadData: function (filter) {
                return $.grep(db, function (ow) {
                    return (!filter.CommitmentType || ow.CommitmentType.toLowerCase().indexOf(filter.CommitmentType.toLowerCase()) > -1)
                    && (!filter.CommitmentNo || ow.CommitmentNo.toLowerCase().indexOf(filter.CommitmentNo.toLowerCase()) > -1)
                    && (!filter.projectNumber || ow.projectNumber.toLowerCase().indexOf(filter.projectNumber.toLowerCase()) > -1)
                    && (!filter.CreatedDate.from || new Date(ow.CreatedDate) >= filter.CreatedDate.from)
                    && (!filter.CreatedDate.to || new Date(ow.CreatedDate) <= filter.CreatedDate.to)
                    && (!filter.Status || ow.Status.toLowerCase().indexOf(filter.Status.toLowerCase()) > -1);
                });
            }

        },
        fields: [
            { name: "SlNo", title: "S.No", editing: false, width: 40 },
            { type: "text", name: "CommitmentType", title: "Type", editing: false, width: 150 },
              { type: "text", name: "projectNumber", title: "Project No", editing: false },
            { type: "text", name: "CommitmentNo", title: "Commitment No", editing: false },
            { type: "text", name: "VendorName", title: "Vendor Name", editing: false, visible: false },
            { name: "CommitmentAmount", title: "Amount", editing: false },
             { name: "AmountSpent", title: "Amount spent", editing: false,visible:false },
            { type: "date", name: "CreatedDate", title: "CreatedDate", editing: false },
            { type: "text", name: "Status", title: "Status", editing: false },
            {
                //type: "", width: 120,title:"Action",
                type: "control", width: 100,deleteButton: false ,editButton:false,
                _createFilterSwitchButton: function () {
                    return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false)
                },
                itemTemplate: function (value, item) {
                    var $result = jsGrid.fields.control.prototype.itemTemplate.apply(this, arguments);
                    if (item.Status == "Active") {
                        statusList = [{ id: 0, name: "Select Action" }, { id: 1, name: "Closed" }]/// { id: 1, name: "Withdraw" }, 
                    } else {
                        statusList = [{ id: 0, name: "Select Action" }, { id: 2, name: "Withdraw" }]
                    }

                    var $customSelect = $("<select>")
                        .attr({"class":"form-control","id":"strAction"}).prop("selectedIndex", 0)
                    $.each(statusList, function (index, itemData) {
                        $customSelect.append($('<option/>', {
                            value: itemData.id,
                            text: itemData.name
                        }));
                    });

                    $customSelect.change(function (e) {
                        var intAction = $(this).val();
                        model = {
                            ProjectId: item.ComitmentId,
                            Status: item.Status,
                            Action: intAction
                        }
                        if ($(this).val() > 0) {
                            $.ajax({
                                type: "POST",
                                url: UpdateStatusURL,
                                data: model,
                                success: function (result) {
                                    
                                    $("#popup").html(result);
                                    $('#PopCommitDetail').modal('toggle');
                                },
                                error: function (err) {
                                    console.log("error1 : " + err);
                                }
                            });
                            //return true;
                            //}
                        }
                        $(this).val(0);
                        return false;
                        e.stopPropagation();
                    });

                    return $result.add($customSelect);
                }
            },           
        ],
    });
    $("#ActiveCommitList").jsGrid("option", "filtering", false);
    loadDetails();

});
var loadDetails = function loadDetails() {
    $.ajax({
        type: "GET",
        url: getProjectURL,
        data: param = "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            db = result;
            $("#ActiveCommitList").jsGrid({ data: db });
        },
        error: function (err) {
            console.log("error : " + err);
        }

    });
};