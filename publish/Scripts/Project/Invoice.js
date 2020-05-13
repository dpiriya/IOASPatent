$(function () {
    //Declare Proposal List

    var getProjectDetailsURL = 'LoadProjectList',
     EditInvoice = 'EditInvoice',
     DeleteInvoice = 'DeleteInvoice';

    debugger;
    $("#gridProjectList").jsGrid({
        paging: true,
        pageIndex: 1,
        pageSize: 5,
        editing: true,
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
            { type: "text", name: "Projecttitle", title: "Project Title", editing: false },
            { type: "text", name: "ProjectNumber", title: "Project Number", align: "left", editing: false, width: "60px" },
            { type: "text", name: "PIname", title: "Principal Investigator", editing: false, width: "70px" },
            { type: "date", name: "PrsntDueDate", title: "Current Due Date", width: 100, align: "center" },
            { type: "decimal", name: "OldSanctionValue", title: "Old Sanction Value", editing: false, width: "55px" },
            { type: "decimal", name: "EnhancedSanctionValue", title: "Enhanced Sanction Value", editing: false, width: "55px" },


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
            $('#ProjectEnhancement').show();
            $('#updateproject').show();
            $.ajax({
                type: "POST",
                url: EditEnhancement,
                data: { EnhanceId: enhanceid },
                //contentType: "application/json; charset=utf-8",
                //dataType: "json",

                success: function (result) {
                    console.log(result);
                    debugger;

                    $('input[name="ProjectID"]').val(result.ProjectID);
                    $('input[name="ProjectEnhancementID"]').val(result.ProjectEnhancementID);
                    $('#projectnum').val(result.ProjectNumber);
                    $('#projecttitle').val(result.Projecttitle);
                    $('#oldsanctndvalue').val(result.OldSanctionValue);


                    $('#enhancedsanctnvalue').val(result.EnhancedSanctionValue);
                    $('#docrefnum').val(result.DocumentReferenceNumber);
                    $('#docname').val(result.AttachmentName);
                    //   $('#ApprovalDocument').val(result.AttachmentPath);
                    $('#totalenhanced').val(result.TotalEnhancedAllocationvalue);
                    $('#totalallocated').val(result.TotalAllocatedvalue);
                    $('#PresentDueDate').val(result.PrsntDueDate);
                    $('#ExtendedDueDate').val(result.ExtndDueDate);
                    if (result.ExtndDueDate != null) {
                        $('input[name=Extension_Qust_1][value=Yes]').attr('checked', 'checked');
                        $('#extensiondetail').css("display", "block");
                    }

                    document.getElementsByClassName('link1')[0].text = result.AttachmentName;
                    document.getElementsByClassName('link1')[0].href = "ShowDocument?file=" + result.AttachmentPath + "&filepath=~%2FContent%2FSupportDocuments%2F";

                    var AllocateId = result.AllocationId;
                    var AllocateHead = result.Allocationhead;
                    var OldAllocationvalue = result.OldAllocationvalue;
                    var EnhancedAllocationvalue = result.EnhancedAllocationvalue;
                    var HeadwiseTotalvalue = result.HeadwiseTotalAllocationvalue;

                    $.each(AllocateId, function (i, val) {
                        if (i == 0) {
                            document.getElementsByName('AllocationId')[0].value = AllocateId[0];
                            document.getElementsByName('Allocationhead')[0].value = AllocateHead[0];
                            document.getElementsByName('OldAllocationvalue')[0].value = OldAllocationvalue[0];
                            document.getElementsByName('EnhancedAllocationvalue')[0].value = EnhancedAllocationvalue[0];
                            document.getElementsByName('HeadwiseTotalAllocationvalue')[0].value = HeadwiseTotalvalue[0];

                        } else {
                            var cln = $("#primaryAllocateDiv").clone().find("input").val("").end();
                            //$(cln).find('.dis-none').removeClass('dis-none');
                            $('#divAllocateContent').append(cln)
                            document.getElementsByName('AllocationId')[i].value = AllocateId[i];
                            document.getElementsByName('Allocationhead')[i].value = AllocateHead[i];
                            document.getElementsByName('OldAllocationvalue')[i].value = OldAllocationvalue[i];
                            document.getElementsByName('EnhancedAllocationvalue')[i].value = EnhancedAllocationvalue[i];
                            document.getElementsByName('HeadwiseTotalAllocationvalue')[i].value = HeadwiseTotalvalue[i];

                        }
                    });

                },
                error: function (err) {
                    console.log("error1 : " + err);
                }
            });
        },
    });

    $("#gridProjectList").jsGrid("option", "filtering", false);
    //Get project enhancement flow details
    $.ajax({
        type: "GET",
        url: getProjectDetailsURL,
        data: param = "",
        contentType: "application/json; charset=utf-8",
        success: function (result) {
           // dataProposal = result;
            $("#gridProjectList").jsGrid({ data: result });
            $('#ProjectEnhancement').hide();
            $('#gridlist').show();
        },
        error: function (err) {
            console.log("error : " + err);
        }
    });
    // Get Proposal List for modal Popup
    var dbInvoice;
    GetInvoicelist();
   
    jsGrid.fields.date = DateField;
    //Get Project List grid
    $("#gridInvoiceList").jsGrid({
        paging: true,
        pageIndex: 1,
        pageSize: 5,
        editing: true,
        filtering: true,

        controller: {

            loadData: function (filter) {
                return $.grep(dbProject, function (project) {

                    return (!filter.ProjectNumber || project.ProjectNumber.toLowerCase().indexOf(filter.ProjectNumber.toLowerCase()) > -1)
                   && (!filter.Projecttitle || project.Projecttitle.toLowerCase().indexOf(filter.Projecttitle.toLowerCase()) > -1)
                   && (!filter.SponsoringAgencyName || project.SponsoringAgencyName.toLowerCase().indexOf(filter.SponsoringAgencyName.toLowerCase()) > -1)
                   && (!filter.NameofPI || project.NameofPI.toLowerCase().indexOf(filter.NameofPI.toLowerCase()) > -1)
                   && (!filter.TotalInvoiceValue || project.TotalInvoiceValue.toLowerCase().indexOf(filter.TotalInvoiceValue.toLowerCase()) > -1)
                    && (!filter.InvoiceDate.from || new Date(project.InvoiceDate) >= filter.InvoiceDate.from)
             && (!filter.InvoiceDate.to || new Date(project.InvoiceDate) <= filter.InvoiceDate.to);
                });
            }

        },

        fields: [
            { name: "Sno", title: "S.No", editing: false, align: "left", width: "25px" },
            { type: "number", name: "ProjectID", title: "Project Id", visible: false },
            { type: "number", name: "InvoiceId", title: "Project Id", visible: false },
            { type: "text", name: "Projecttitle", title: "Project Title", editing: false, width: "60px" },
            { type: "text", name: "ProjectNumber", title: "Project Number", align: "left", editing: false, width: "50px" },  
            { type: "text", name: "InvoiceDate", title: "Invoice Date", editing: false, width: "60px" },
            { type: "text", name: "NameofPI", title: "Principal Investigator", editing: false, width: "60px" },
            //{ type: "text", name: "PIDepartmentName", title: "Department of PI", editing: false, width: "75px" },
            { type: "text", name: "TotalInvoiceValue", title: "Invoice Value", editing: false, width: "50px" },
            { type: "text", name: "InvoiceNumber", title: "Invoice Number", editing: false, width: "50px" },
            {
                type: "control", deleteButton: false, width: "25px",
                _createFilterSwitchButton: function () {
                    return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
                },
                
            },
            
        ],
       
        //        success: function (result) {

        //            //window.onload();
        //            //$('#createuserid').hide();                    
        //            $('#createproposal').hide();
        //            $('#saveproposal').hide();
        //            $('#updateproposal').hide();
        //            $('#gridlist').show();
        //            if (result == 4) {
        //                $('#gridlist').show();
        //                $('#myModal2').modal('show');
        //            }
        //        },
            //    error: function (err) {
            //        console.log("error1 : " + err);
            //    }
            //});
        //}
       
    });

    $("#gridInvoiceList").jsGrid("option", "filtering", false);
    $.ajax({
        type: "GET",
        url: getInvoiceDetailsURL,
        data: param = "",
        contentType: "application/json; charset=utf-8",
        success: function (result) {
            // dataProposal = result;
            $("#gridInvoiceList").jsGrid({ data: result });
            $('#gridlist').show();
            $('#addnewpage').show();
        },
        error: function (err) {
            console.log("error : " + err);
        }
    });

    function GetInvoicelist() {

        $.ajax({
            type: "GET",
            url: getInvoiceDetailsURL,
            data: param = "",
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                // dataProposal = result;
                $("#gridInvoiceList").jsGrid({ data: result });                
                $('#gridlist').show();
                $('#addnewpage').show();
               // $('#popupFilter').show();
                dbInvoice = result;
            },
            error: function (err) {
                console.log("error : " + err);
            }

        });

    }
    
    
});

var selectPickerApiElement = function (el, choice, options, select) {
    $(el).find('select').selectpicker({
        liveSearch: true
    });
    $(el).children().eq(2).siblings().remove();
    if (choice == "add") {
        $(el).find('.selectpicker').append("<option>" + options + "</option>");
    } else if (choice == "all" && select != '') {
        $(el).find('.selectpicker').children().remove();
        for (var i = 0 ; i < options.length ; i++) {
            $(el).find('.selectpicker').append("<option value=" + options[i].id + ">" + options[i].name + "</option>");
        }
        $(el).find('.selectpicker option[value=' + select + ']').attr('selected', 'selected');
    } else if (choice == "all" && select == '') {
        $(el).find('.selectpicker').children().remove();
        for (var i = 0 ; i < options.length ; i++) {
            $(el).find('.selectpicker').append("<option value=" + options[i].id + ">" + options[i].name + "</option>");
        }
    } else if (choice == "empty") {
        $(el).find('.selectpicker').children().remove();
        $(el).find('.selectpicker').append("<option value=''>Select any</option>");
    } else {
        var selectOptionsLength = $(el).find('.selectpicker').children().length;
        for (var i = 1 ; i <= selectOptionsLength ; i++) {
            if (options == $(el).find('.selectpicker').children().eq(i).val()) {
                $(el).find('.selectpicker').children().eq(i).remove();
                break;
            } else {
                continue;
            }

        }

    }
    $(el).find('select').selectpicker('refresh');
    return $(el).children().first().unwrap();

}