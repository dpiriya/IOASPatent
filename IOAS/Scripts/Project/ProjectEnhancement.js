$(function () {
    //Declare Proposal List
    var getProjectDetailsURL = 'GetEnhancedProjectList',
        getExtendedProjectDetailsURL = 'GetExtendedProjectList';
    EditEnhancement = 'EditProjectenhancement';
    EditExtension = 'EditProjectextension';
    delEnhancement = 'DeleteEnhancement';
    var dbenhance;
    Getenhancelist();
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
            this._fromPicker = $("<input>").datepicker({ defaultDate: now.setFullYear(now.getFullYear() - 1), changeYear: true });
            this._toPicker = $("<input>").datepicker({ defaultDate: now.setFullYear(now.getFullYear() + 1), changeYear: true });
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
    //Project List for Enhancement

    $("#gridProjectList").jsGrid({
        paging: true,
        pageIndex: 1,
        pageSize: 5,
        editing: false,
        filtering: true,
        controller: {

            loadData: function (filter) {
                return $.grep(dbenhance, function (enhance) {

                    return (!filter.Projecttitle || enhance.Projecttitle.toLowerCase().indexOf(filter.Projecttitle.toLowerCase()) > -1)
                    && (!filter.ProjectNumber || enhance.ProjectNumber.toLowerCase().indexOf(filter.ProjectNumber.toLowerCase()) > -1)
                    && (!filter.PIname || enhance.PIname.toLowerCase().indexOf(filter.PIname.toLowerCase()) > -1)
                    && (!filter.PrsntDueDate.from || new Date(enhance.PrsntDueDate) >= filter.PrsntDueDate.from)
                  && (!filter.PrsntDueDate.to || new Date(enhance.PrsntDueDate) <= filter.PrsntDueDate.to);
                });
            }
        },
        fields: [
            { name: "Sno", title: "S.No", editing: false, align: "left", width: "30px" },
            { type: "number", name: "ProjectID", title: "Project Id", visible: false },
            { type: "number", name: "ProjectEnhancementID", title: "Project EnhancementID", visible: false },
            { type: "text", name: "Projecttitle", title: "Project Title", editing: false },
            { type: "text", name: "ProjectNumber", title: "Project Number", align: "left", editing: false, width: "60px" },
            { type: "text", name: "PIname", title: "Principal Investigator", editing: false, width: "70px" },
            { type: "date", name: "PrsntDueDate", title: "Current Due Date", width: 100, align: "center" },
            { type: "decimal", name: "OldSanctionValue", title: "Old Sanction Value", editing: false, width: "55px" },
            { type: "decimal", name: "EnhancedSanctionValue", title: "Enhanced Sanction Value", editing: false, width: "55px" },


            {
                type: "control", editButton: false, deleteButton: false, width: "25px",
                _createFilterSwitchButton: function () {
                    return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
                },
                itemTemplate: function (value, item) {
                   if (item.isCurrentVersion) {
                        var $result = jsGrid.fields.control.prototype.itemTemplate.apply(this, arguments);
                        var $customButton = $("<button type ='button'>")
                     .attr("class", "ion-ios-trash-outline")
                     .click(function (e) {
                         var choice = confirm("Are you sure! Do you want to delete this enhancement?.");
                         if (choice === true) {
                             $.ajax({
                                 type: "POST",
                                 url: delEnhancement,
                                 data: { EnhanceId: item.ProjectEnhancementID },
                                 success: function (result) {
                                     if (result == true) {
                                         $('#alertSuccess').html("Enhancement has been deleted successfully.");
                                         $('#Success').modal('toggle');
                                         Getenhancelist();
                                     }
                                 },
                                 error: function (err) {
                                     console.log("error1 : " + err);
                                 }
                             });
                         }
                         e.stopPropagation();
                     });
                        // return $result.add($customButton);
                        return $("<div>").append($customButton);
                    }

                }               
            },

        ],

        //onItemDeleting: function (args) {
        //    console.log()
        //},
        //onItemEditing: function (args) {

        //    // cancel editing of the row of item with field 'ID' = 0
        //    if (args.item.ProjectEnhancementID > 0) {
        //        var enhanceid = args.item.ProjectEnhancementID;
        //    }
        //    $('#addnewpage').hide();
        //    $('#gridlist').hide();
        //    $('#gridproject').hide();
        //    $('#saveproject').hide();
        //    $('#ProjectEnhancement').show();
        //    $('#updateproject').show();
        //    $.ajax({
        //        type: "POST",
        //        url: EditEnhancement,
        //        data: { EnhanceId: enhanceid },
        //        //contentType: "application/json; charset=utf-8",
        //        //dataType: "json",

        //        success: function (result) {

        //            $('input[name="ProjectID"]').val(result.ProjectID);
        //            $('input[name="ProjectEnhancementID"]').val(result.ProjectEnhancementID);
        //            $('#projectnum').val(result.ProjectNumber);
        //            $('#projecttitle').val(result.Projecttitle);                
        //            $('#oldsanctndvalue').val(result.OldSanctionValue);


        //            $('#enhancedsanctnvalue').val(result.EnhancedSanctionValue);
        //            $('#docrefnum').val(result.DocumentReferenceNumber);
        //            $('#docname').val(result.AttachmentName);
        //         //   $('#ApprovalDocument').val(result.AttachmentPath);
        //            $('#totalenhanced').val(result.TotalEnhancedAllocationvalue);
        //            $('#totalallocated').val(result.TotalAllocatedvalue);
        //            var dueDate = new Date(parseInt(result.PresentDueDate.replace(/(^.*\()|([+-].*$)/g, '')));

        //            $("#PresentDueDate").datepicker('setDate', dueDate);
        //            $("#ExtendedDueDate").datepicker("option", "minDate", dueDate);                    

        //            //$('#PresentDueDate').val(result.PrsntDueDate);
        //            //$('#ExtendedDueDate').val(result.ExtndDueDate);
        //            /*****************************Added by Benet Shibin 07-09-2018************************************************/
        //            //$('#department').val(result.Department);
        //            //$('#PI').val(result.PI);
        //            //$('#PIdesig').val(result.PIDesignation);
        //            //var DepartmentID = result.CoPIDepartment;
        //            //var CoPiId = result.CoPIid;
        //            //var Designation = result.CoPIDesignation;
        //            //$.each(Designation, function (i, val) {

        //            //    if (i == 0) {
        //            //        document.getElementsByName('CoPIDep')[0].value = DepartmentID[0];
        //            //        document.getElementsByName('CoPIName')[0].value = CoPiId[0];
        //            //        document.getElementsByName('CopiDesig')[0].value = Designation[0];
        //            //    } else {
        //            //        var cln = $("#primaryDiv").clone().find("select").val("").end();
        //            //        //$(cln).find('.dis-none').removeClass('dis-none');
        //            //        $('#divContent').append(cln)
        //            //        document.getElementsByName('CoPIDep')[i].value = DepartmentID[i];
        //            //        document.getElementsByName('CoPIName')[i].value = CoPiId[i];
        //            //        document.getElementsByName('CopiDesig')[i].value = Designation[i];
        //            //    }
        //            //});
        //            /*****************************End*****************************************************************/
        //            if (result.Extension_Qust_1 == "Yes")
        //            {
        //                var extDate = new Date(parseInt(result.ExtendedDueDate.replace(/(^.*\()|([+-].*$)/g, '')));
        //                $("#ExtendedDueDate").datepicker('setDate', extDate);
        //                $('input[name=Extension_Qust_1][value=Yes]').attr('checked', 'checked');
        //                $('#extensiondetail').css("display", "block");
        //            }else {
        //                $('input[name=Extension_Qust_1][value=Yes]').attr('checked', false);
        //                $('#extensiondetail').css("display", "none");
        //            }
        //            if (result.Enhancement_Qust_1 == "Yes") {
        //                $('input[name=Enhancement_Qust_1][value=Yes]').attr('checked', 'checked');
        //                $('#divEnhancement').css("display", "block");
        //            } else {
        //                $('input[name=Enhancement_Qust_1][value=Yes]').attr('checked', false);
        //                $('#divEnhancement').css("display", "none");
        //            }
        //            if (result.AttachmentName != null){
        //                document.getElementsByClassName('link1')[0].text = result.AttachmentName;
        //                document.getElementsByClassName('link1')[0].href = "ShowDocument?file=" + result.AttachmentPath + "&filepath=~%2FContent%2FSupportDocuments%2F";
        //            }
        //            var AllocateId = result.AllocationId;
        //            var AllocateHead = result.Allocationhead;
        //            var OldAllocationvalue = result.OldAllocationvalue;
        //            var EnhancedAllocationvalue = result.EnhancedAllocationvalue;
        //            var HeadwiseTotalvalue = result.HeadwiseTotalAllocationvalue;

        //            $.each(AllocateId, function (i, val) {
        //                if (i == 0) {
        //                    document.getElementsByName('AllocationId')[0].value = AllocateId[0];
        //                    document.getElementsByName('Allocationhead')[0].value = AllocateHead[0];
        //                    document.getElementsByName('OldAllocationvalue')[0].value = OldAllocationvalue[0];
        //                    document.getElementsByName('EnhancedAllocationvalue')[0].value = EnhancedAllocationvalue[0];
        //                    document.getElementsByName('HeadwiseTotalAllocationvalue')[0].value = HeadwiseTotalvalue[0];

        //                } else {
        //                    var cln = $("#primaryAllocateDiv").clone().find("input").val("").end();
        //                    //$(cln).find('.dis-none').removeClass('dis-none');
        //                    $('#divAllocateContent').append(cln)
        //                   document.getElementsByName('AllocationId')[i].value = AllocateId[i];
        //                   document.getElementsByName('Allocationhead')[i].value = AllocateHead[i];
        //                   document.getElementsByName('OldAllocationvalue')[i].value = OldAllocationvalue[i];
        //                   document.getElementsByName('EnhancedAllocationvalue')[i].value = EnhancedAllocationvalue[i];
        //                   document.getElementsByName('HeadwiseTotalAllocationvalue')[i].value = HeadwiseTotalvalue[i];

        //                }
        //            });

        //        },
        //        error: function (err) {
        //            console.log("error1 : " + err);
        //        }
        //    });            
        //},
    });

    var dbextend;
    Getextensionlist();
    $("#gridextendProjectList").jsGrid({
        paging: true,
        pageIndex: 1,
        pageSize: 5,
        editing: true,
        filtering: true,

        controller: {

            loadData: function (filter) {
                return $.grep(dbextend, function (extend) {

                    return (!filter.Projecttitle || extend.Projecttitle.toLowerCase().indexOf(filter.Projecttitle.toLowerCase()) > -1)
                    && (!filter.ProjectNumber || extend.ProjectNumber.toLowerCase().indexOf(filter.ProjectNumber.toLowerCase()) > -1)
                    && (!filter.PIname || extend.PIname.toLowerCase().indexOf(filter.PIname.toLowerCase()) > -1)
                    && (!filter.PrsntDueDate.from || new Date(extend.PrsntDueDate) >= filter.PrsntDueDate.from)
                     && (!filter.PrsntDueDate.to || new Date(extend.PrsntDueDate) <= filter.PrsntDueDate.to);
                });
            }

        },

        fields: [
            { name: "Sno", title: "S.No", editing: false, align: "left", width: "30px" },
            { type: "number", name: "ProjectID", title: "Project Id", visible: false },
            { type: "number", name: "ProjectEnhancementID", title: "Project EnhancementID", visible: false },
            { type: "text", name: "Projecttitle", title: "Project Title", editing: false },
            { type: "text", name: "ProjectNumber", title: "Project Number", align: "left", editing: false, width: "60px" },
            { type: "text", name: "PIname", title: "Principal Investigator", editing: false, width: "70px" },
            { type: "date", name: "PrsntDueDate", title: "Current Due Date", width: 100, align: "center" },
             { name: "EnhancedSanctionValue", title: "Sanction Value", editing: false, width: "55px" },
            {
                type: "control", deleteButton: false, width: "25px",
                _createFilterSwitchButton: function () {
                    return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
                }
            },
        ],


        onItemEditing: function (args) {

            // cancel editing of the row of item with field 'ID' = 0

            if (args.item.ProjectEnhancementID > 0) {
                var enhanceid = args.item.ProjectEnhancementID;
            }
            $('#addnewpage').hide();
            $('#gridlist').hide();
            $('#gridproject').hide();
            $('#saveproject').hide();
            $('#projectextension').show();
            $('#updateproject').show();
            $.ajax({
                type: "POST",
                url: EditExtension,
                data: { ExtensionId: enhanceid },
                //contentType: "application/json; charset=utf-8",
                //dataType: "json",

                success: function (result) {

                    $('input[name="ProjectID"]').val(result.ProjectID);
                    $('input[name="ProjectEnhancementID"]').val(result.ProjectEnhancementID);
                    $('#projectnum').val(result.ProjectNumber);
                    $('#projecttitle').val(result.Projecttitle);
                    $('#PresentDueDate').val(result.PrsntDueDate);
                    $('#ExtendedDueDate').val(result.ExtndDueDate);
                    $('#oldsanctndvalue').val(result.OldSanctionValue);
                    $('#docrefnum').val(result.DocumentReferenceNumber);
                    $('#docname').val(result.AttachmentName);
                    //$('#ApprovalDocument').val(result.AttachmentPath);
                    var docname = result.AttachmentName
                    var path = result.AttachmentPath
                    document.getElementsByClassName('link1')[0].text = docname;
                    document.getElementsByClassName('link1')[0].href = "ShowDocument?file=" + path + "&filepath=~%2FContent%2FSupportDocuments%2F";
                },
                error: function (err) {
                    console.log("error1 : " + err);
                }
            });

        },
    });
    $("#gridProjectList").jsGrid("option", "filtering", false);
    //Get project enhancement flow details
    //$.ajax({
    //    type: "GET",
    //    url: getProjectDetailsURL,
    //    data: param = "",
    //    contentType: "application/json; charset=utf-8",
    //    success: function (result) {
    //       // dataProposal = result;
    //        $("#gridProjectList").jsGrid({ data: result });
    //        $('#ProjectEnhancement').hide();
    //        $('#gridlist').show();
    //    },
    //    error: function (err) {
    //        console.log("error : " + err);
    //    }
    //});

    $("#gridextendProjectList").jsGrid("option", "filtering", false);
    //Get project extension flow details
    //$.ajax({
    //    type: "GET",
    //    url: getExtendedProjectDetailsURL,
    //    data: param = "",
    //    contentType: "application/json; charset=utf-8",
    //    success: function (result) {
    //        // dataProposal = result;
    //        $("#gridextendProjectList").jsGrid({ data: result });
    //        $('#projectextension').hide();
    //        $('#gridlist').show();
    //    },
    //    error: function (err) {
    //        console.log("error : " + err);
    //    }
    //});

    function Getenhancelist() {
        $.ajax({
            type: "GET",
            url: getProjectDetailsURL,
            data: param = "",
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                // dataProposal = result;
                $("#gridProjectList").jsGrid({ data: result });
                //$('#ProjectEnhancement').hide();
                //$('#gridlist').show();
                dbenhance = result;
            },
            error: function (err) {
                console.log("error : " + err);
            }

        });

    }


    function Getextensionlist() {
        
        $.ajax({
            type: "GET",
            url: getExtendedProjectDetailsURL,
            data: param = "",
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                // dataProposal = result;
                $("#gridextendProjectList").jsGrid({ data: result });
                //$('#projectextension').hide();
                //$('#gridlist').show();
                dbextend = result;
            },
            error: function (err) {
                console.log("error : " + err);
            }

        });

    }
});
function fillData(result) {
    $('input[name="ProjectID"]').val(result.ProjectID);
    $("#projectnum").val(result.ProjectNumber);
    $("#projecttitle").val(result.Projecttitle);
    var dueDate = new Date(parseInt(result.PresentDueDate.replace(/(^.*\()|([+-].*$)/g, '')));
    $("#PresentDueDate").datepicker('setDate', dueDate);
    if (result.ExtendedDueDate != null)
    {
        var extenddueDate = new Date(parseInt(result.ExtendedDueDate.replace(/(^.*\()|([+-].*$)/g, '')));
        $("#ExtendedDueDate").datepicker('setDate', extenddueDate);
    }  
    $('#oldsanctndvalue').val(result.OldSanctionValue);
    $('#enhancedsanctnvalue').val(result.EnhancedSanctionValue);
    $('#docrefnum').val(result.DocumentReferenceNumber);
    if (result.Extension_Qust_1 == "Yes" && result.Enhancement_Qust_1 == "No")
    {
        $('input[name=Extension_Qust_1][value=Yes]').prop('checked', true);
        $('input[name=Extension_Qust_1][value=No]').prop('checked', false);
        $('input[name=Enhancement_Qust_1][value=No]').prop('checked', true);
        $('input[name=Enhancement_Qust_1][value=Yes]').prop('checked', false);
        $('#divEnhancement').css("display", "none");
        $('#extensiondetail').css("display", "block");
    }
    if (result.Extension_Qust_1 == "No" && result.Enhancement_Qust_1 == "Yes") {
        $('input[name=Extension_Qust_1][value=No]').prop('checked', true);
        $('input[name=Extension_Qust_1][value=Yes]').prop('checked', false);
        $('input[name=Enhancement_Qust_1][value=Yes]').prop('checked', true);
        $('input[name=Enhancement_Qust_1][value=No]').prop('checked', false);
        $('#divEnhancement').css("display", "block");
        $('#extensiondetail').css("display", "none");
    }
    if (result.Extension_Qust_1 == "Yes" && result.Enhancement_Qust_1 == "Yes") {
        $('input[name=Extension_Qust_1][value=No]').prop('checked', false);
        $('input[name=Extension_Qust_1][value=Yes]').prop('checked', true);
        $('input[name=Enhancement_Qust_1][value=Yes]').prop('checked', true);
        $('input[name=Enhancement_Qust_1][value=No]').prop('checked', false);
        $('#divEnhancement').css("display", "block");
        $('#extensiondetail').css("display", "block");
    }
   
    var AllocateId = result.AllocationId;
    var AllocateHead = result.Allochead;
    var OldAllocationvalue = result.OldAllocationvalue;
    var EnhanceAllocationvalue = result.EnhancedAllocationvalue;
    //    $('#divAllocateContent #primaryAllocateDiv').not(':first').remove();
    document.getElementsByName('Allocationhead')[0].value = "";
    document.getElementsByName('OldAllocationvalue')[0].value = "";
    document.getElementsByName('EnhancedAllocationvalue')[0].value = "";
    document.getElementsByName('AllocationId')[0].value = "";
    document.getElementsByName('HeadwiseTotalAllocationvalue')[0].value = "";
    document.getElementsByName('Allochead')[0].value = "";
    $.each(AllocateHead, function (i, val) {
        if (i == 0) {
            document.getElementsByName('AllocationId')[0].value = AllocateId[0];
            document.getElementsByName('Allocationhead')[0].value = AllocateHead[0];
            document.getElementsByName('OldAllocationvalue')[0].value = OldAllocationvalue[0];
            document.getElementsByName('EnhancedAllocationvalue')[0].value = EnhanceAllocationvalue[0];
            document.getElementsByName('HeadwiseTotalAllocationvalue')[0].value = OldAllocationvalue[0];
            document.getElementsByName('Allochead')[0].value = AllocateHead[0];
        } else {
            var cln = $("#primaryAllocateDiv").clone().find("input").val("").end();
            //$(cln).find('.dis-none').removeClass('dis-none');
            $('#divAllocateContent').append(cln)
            document.getElementsByName('AllocationId')[i].value = AllocateId[i];
            document.getElementsByName('Allocationhead')[i].value = AllocateHead[i];
            document.getElementsByName('OldAllocationvalue')[i].value = OldAllocationvalue[i];
            document.getElementsByName('EnhancedAllocationvalue')[i].value = EnhanceAllocationvalue[i];
            document.getElementsByName('HeadwiseTotalAllocationvalue')[i].value = OldAllocationvalue[i];
            document.getElementsByName('Allochead')[i].value = AllocateHead[i];
        }
    });
    if (AllocateHead == "") {
        var alloc = $("#primaryAllocateDiv").find("input").val("").end();
        $(alloc).find('.allochead').removeAttr("disabled");
        $(alloc).find('.oldalloc').val(0);
    }
    $("#docname").val(result.AttachmentName);
    //var docname = result.AttachmentName
    //var path = result.AttachmentPath
    //if (docname != null) {
    //    document.getElementsByClassName('link1')[0].text = docname;
    //    document.getElementsByClassName('link1')[0].href = "ShowDocument?file=" + path + "&filepath=~%2FContent%2FSupportDocuments%2F";
    //}
    totalSum();
    calculateallocationSum();   
    $("#ProjectEnhancement").show();
    $("#gridlist").hide();
}