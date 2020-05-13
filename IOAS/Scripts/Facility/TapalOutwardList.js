﻿var getOutwardURL = 'GetOutwardDetails'
var db;
$(function () {
    var tapalDetails = 'PopupTapalDetails';
    var getOutwardForEdit = 'GetOutwardForEdit';

    var DateField = function (config) {
        jsGrid.Field.call(this, config);
    };

    DateField.prototype = new jsGrid.Field({
        sorter: function (date1, date2) {
            return new Date(date1) - new Date(date2);
        },

        itemTemplate: function (value) {
            return new Date(value).toDateString();
        },

        filterTemplate: function () {
            var now = new Date();
            this._fromPicker = $("<input>").datepicker({ defaultDate: now.setFullYear(now.getFullYear() - 1), dateFormat: 'dd-MM-yy', changeYear: true });
            this._toPicker = $("<input>").datepicker({ defaultDate: now.setFullYear(now.getFullYear() + 1), dateFormat: 'dd-MM-yy', changeYear: true });
            return $("<div>").append(this._fromPicker).append(this._toPicker);
        },

        insertTemplate: function (value) {
            return this._insertPicker = $("<input>").datepicker({ defaultDate: new Date() });
        },

        editTemplate: function (value) {
            return this._editPicker = $("<input>").datepicker().datepicker("setDate", new Date(value));
        },

        insertValue: function () {
            return this._insertPicker.datepicker("getDate").toISOString();
        },

        editValue: function () {
            return this._editPicker.datepicker("getDate").toISOString();
        },

        filterValue: function () {
            return {
                from: this._fromPicker.datepicker("getDate"),
                to: this._toPicker.datepicker("getDate")
            };
        }
    });
    jsGrid.fields.date = DateField;
    $("#OutwardList").jsGrid({
        paging: true,
        pageIndex: 1,
        pageSize: 5,
        pageLoading: true,
        autoload: true,
        editing: false,
        filtering: true,
        //selecting: true,
        controller: {
            loadData: function (filter) {
                var deferred = $.Deferred();
                $.ajax({
                    type: "post",
                    url: getOutwardURL,
                    data: filter,
                    dataType: "json",
                    success: function (datas) {
                        var da = {
                            data: datas.Data,
                            itemsCount: datas.TotalRecords
                        }

                        deferred.resolve(da);
                    }
                });
                return deferred.promise();
                //   return $.grep(db, function (ow) {
                //       return (!filter.TapalNo || ow.TapalNo.toLowerCase().indexOf(filter.TapalNo.toLowerCase()) > -1)
                //       && (!filter.TapalType || ow.TapalType.toLowerCase().indexOf(filter.TapalType.toLowerCase()) > -1)
                //       && (!filter.SenderDetails || ow.SenderDetails.toLowerCase().indexOf(filter.SenderDetails.toLowerCase()) > -1)
                //       && (!filter.Department || ow.Department.toLowerCase().indexOf(filter.Department.toLowerCase()) > -1)
                //       && (!filter.User || ow.User.toLowerCase().indexOf(filter.User.toLowerCase()) > -1)
                //       && (!filter.strAction || ow.strAction.toLowerCase().indexOf(filter.strAction.toLowerCase()) > -1)

                //         && (!filter.OutwardDate.from || new Date(ow.OutwardDate) >= filter.OutwardDate.from)
                //&& (!filter.OutwardDate.to || new Date(ow.OutwardDate) <= filter.OutwardDate.to);
                //   });
            }

        },
        fields: [
            { name: "slNo", title: "S.No", editing: false, width: 60 },
            { type: "number", name: "TapalId", title: "TapalId", visible: false },
            { type: "number", name: "CreateUserId", title: "Create UserId", visible: false },
             { type: "text", name: "TapalNo", title: "Tapal No", editing: false, width: 150 },
            { type: "text", name: "SenderDetails", title: "Sender Details", editing: false },
            { name: "OutwardDate", title: "Outward Date", type: "date", width: 100, align: "center" },
            //{ type: "text", name: "InwardDate", title: "Inward Date", editing: false },
            { name: "TapalType", title: "Tapal Type", editing: false },
            { name: "Department", title: "Department", editing: false, width: 130 },
            { name: "User", title: "User", editing: false },
            //{ type: "bool",   name: "Remarks",       title: "Remarks",        editing: false },

            {
                name: "DocDetail",
                title: "Documents", width: 120,
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
              { name: "strAction", title: "Action", editing: false, width: 110 },
            {
                type: "control", width: 100,
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

                    var $customButtonEdit = $("<button>")

                   .attr("class", "ion-edit")
                   .click(function (e) {
                       $.ajax({
                           type: "GET",
                           url: getOutwardForEdit,
                           data: { "TapalId": item.TapalId, "UserId": item.CreateUserId },
                           success: function (result) {
                               if (result == -1) {
                                   $("#FailedAlert").text('Error. Try again!');
                                   $('#Failed').modal('show');
                               } else {
                                   $("#popup").html(result);
                                   $('#EditOutwardModal').modal('toggle');
                               }
                           },
                           error: function (err) {
                               console.log("error1 : " + err);
                           }
                       });
                       e.stopPropagation();
                   });
                    /***** Temporary Hide this $customButtonEdit*******/
                    $customButtonEdit.hide();
                    /*if Tapal Accepted then can't allow to Edit*/
                    if (item.IsClosed == true) {
                        $customButtonEdit.attr("disabled", "disabled");
                        $customButtonEdit.prop("title", "This tapal is accepted, You don't have permission to update?")
                    }
                    if (item.Roles.count > 0) {
                        for (var i = 0; i < item.Roles.count; i++) {
                            if (item.Roles[i].IsUpdate == true) {
                                $customButtonEdit.show();
                            } else {
                                $customButtonEdit.hide();
                            }
                        }
                    }
                    // return $result.add($customButton);
                    return $("<div>").append($customButton).append($customButtonEdit);
                }
            }
        ],
    });
    $("#OutwardList").jsGrid("option", "filtering", false);
    //loadOutwart();

});
var loadOutwart = function loadOutwart() {
    $.ajax({
        type: "GET",
        url: getOutwardURL,
        data: param = "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            db = result;
            $("#OutwardList").jsGrid({ data: db });
        },
        error: function (err) {
            console.log("error : " + err);
        }

    });
    //var srchData = { pageIndex: 1, pageSize: 5 }
    //$.ajax({
    //    type: "post",
    //    url: getOutwardURL,
    //    data: srchData,
    //    dataType: "json",
    //    success: function (datas) {
    //        $("#OutwardList").jsGrid({
    //            data: datas.Data,
    //            itemsCount: datas.TotalRecords
    //        });
    //    }
    //});
};