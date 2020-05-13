$(function () {
    //Declare Proposal List
    var getInvoiceProcessList = 'LoadInvoiceProcessList',
     EditInvoice = 'EditInvoice',
     DeleteInvoice = 'DeleteInvoice';
    //var dbInvoice;
    //GetInvoicelist();

   // jsGrid.fields.date = DateField;
    
    $("#gridInvoiceList").jsGrid({
        paging: true,
        pageIndex: 1,
        pageSize: 5,
        editing: true,
        filtering: true,

        controller: {

            loadData: function (filter) {
                return $.grep(dbInvoice, function (invoice) {

                    return (!filter.InvoiceNumber || invoice.InvoiceNumber.toLowerCase().indexOf(filter.InvoiceNumber.toLowerCase()) > -1)
                    && (!filter.ProjectNumber || invoice.ProjectNumber.toLowerCase().indexOf(filter.ProjectNumber.toLowerCase()) > -1)
                    && (!filter.NameofPI || invoice.NameofPI.toLowerCase().indexOf(filter.NameofPI.toLowerCase()) > -1)
                    && (!filter.InvoiceValue || invoice.InvoiceValue.toLowerCase().indexOf(filter.InvoiceValue.toLowerCase()) > -1)
                    && (!filter.InvoiceStatus || invoice.InvoiceStatus.toLowerCase().indexOf(filter.InvoiceStatus.toLowerCase()) > -1)
                    && (!filter.InvoiceDate.from || new Date(invoice.InvoiceDate) >= filter.InvoiceDate.from)
                  && (!filter.InvoiceDate.to || new Date(invoice.InvoiceDate) <= filter.InvoiceDate.to);
                });
            }
        },
        fields: [
            { name: "Sno", title: "S.No", editing: false, align: "left", width: "30px" },
            { type: "number", name: "ProjectId", title: "Project Id", visible: false },
            { type: "number", name: "InvoiceId", title: "Invoice Id", visible: false },
            { type: "text", name: "Invoicedatestrng", editing: false, title: "Invoice Date", align: "center", width: "60px" },
            { type: "text", name: "InvoiceNumber", editing: false, title: "Invoice Number", width: "60px" },
            { type: "text", name: "ProjectNumber", title: "Project Number", align: "left", editing: false, width: "80px" },
            { type: "text", name: "NameofPI", title: "Principal Investigator", editing: false, width: "70px" },            
            { type: "decimal", name: "TotalInvoiceValue", title: "Invoice Value", editing: false, width: "55px" },
            { type: "text", name: "InvoiceStatus", title: "Status", editing: false, width: "55px" },
          //  { type: "select", name: "Action", title: "Action", editing: false, width: "55px" },
          //{ name: "Type", type: "select", items: ["Select","Approve"], validate: "required" },
        {

           name: "InvoiceId",
           title: "Action",
           width: "60px",
           itemTemplate: function (value, item) {
               return $("<a>").attr("href", "javascript:void(0)").attr("class", "btn btn-primary").attr("value", "Select").text('Approve').on("click", function () {
                   var invoiceid = item.InvoiceId;
                   var InvoiceDetails = 'LoadInvoiceProcess';
                   $.ajax({
                       type: "POST",
                       url: InvoiceDetails,
                       data: { InvoiceId: invoiceid },
                       //contentType: "application/json; charset=utf-8",
                       //dataType: "json",

                       success: function (result) {

                           $('#ProjectInvoice').show();
                           $('#gridlist').hide();
                           $("#Invdate").html(result.Invoicedatestrng);
                           $('input[name="Invoicedatestrng"]').html(result.Invoicedatestrng);
                           $('input[name="Invoicedatestrng"]').val(result.Invoicedatestrng);
                           $('input[name="InvoiceDate"]').val(result.InvoiceDate);
                           $('input[name="InvoiceId"]').val(result.InvoiceId);
                           $('input[name="InvoiceNumber"]').val(result.InvoiceNumber);
                           $('input[name="ProjectID"]').val(result.ProjectID);
                           $('input[name="ProjectType"]').val(result.ProjectType);
                           $('select[name="ServiceType"]').val(result.ServiceType);
                           $('input[name="TaxStatus"]').val(result.TaxStatus);
                           $('input[name="InvoiceType"]').val(result.InvoiceType);
                           $('input[name="AvailableBalance"]').val(result.AvailableBalance);
                           $('input[name="Sanctionvalue"]').val(result.Sanctionvalue);
                           $('#txtCommunicationAddress').val(result.CommunicationAddress);
                           $('input[name="PONumber"]').val(result.PONumber);
                           $('#txtprojectnumber').html(result.ProjectNumber);

                           $('#txtPIname').html(result.NameofPI);
                           $('#txtPIdepartment').html(result.PIDepartmentName);
                           $('#txtsanctionordernumber').html(result.SanctionOrderNumber);
                           $('#txtsanctionvalue').html(result.Sanctionvalue);
                           $('#txtcurrentfinyear').html(result.CurrentFinancialYear);
                           $('#txtinvoicetype').val(result.InvoiceTypeName);
                         //  $('#txtservicetype').val(result.ServiceTypeName);
                           $('#txtSACNumber').val(result.SACNumber);

                           $('#txtdescriptionofservice').val(result.DescriptionofServices);
                           $('#taxablevalue').val(result.TaxableValue);
                           $('#totalinvoicevalue').val(result.TotalInvoiceValue);
                           $('input[name="TotalInvoiceValue"]').val(result.TotalInvoiceValue);
                           $('#totalinvoicevalue').html(result.TotalInvoiceValue);
                           $('#instalmentnumber').val(result.Instalmentnumber);
                           $('input[name="Instlmntyr"]').val(result.Instlmntyr);
                           $('#SGSTamount').val(result.SGST);
                           $('#SGSTpercent').val(result.SGSTPercentage);
                           $('input[name="SGSTPercentage"]').val(result.SGSTPercentage);
                           $('#CGSTamount').val(result.CGST);
                           $('#CGSTpercent').val(result.CGSTPercentage);
                           $('input[name="CGSTPercentage"]').val(result.CGSTPercentage);
                           $('#IGSTamount').val(result.IGST);
                           $('#IGSTpercent').val(result.IGSTPercentage);
                           $('input[name="IGSTPercentage"]').val(result.IGSTPercentage);
                           $('#Totaltaxamount').val(result.TotalTaxValue);
                           $('#Totaltaxpercent').val(result.TotalTaxpercentage);

                           $('input[name="SponsoringAgency"]').val(result.SponsoringAgency);
                           $('#txtAgencyRegname').html(result.SponsoringAgencyName);
                           $('#txtAgencyAddress').html(result.Agencyregaddress);
                           $('#txtAgencyDistrict').html(result.Agencydistrict);
                           $('#txtPincode').html(result.AgencyPincode);

                           $('#txtState').html(result.Agencystate);
                           $('#txtStatecode').html(result.Agencystatecode);
                           $('#txtGSTIN').html(result.GSTNumber);
                           $('#txtPAN').html(result.PAN);

                           $('#txtTAN').html(result.TAN);
                           $('#txtAgencycontactperson').html(result.Agencycontactperson);
                           $('#txtAgencypersonemail').html(result.AgencycontactpersonEmail);
                           $('#txtAgencypersonmobile').html(result.Agencycontactpersonmobile);

                           $('#txtBankname').html(result.BankName);
                           $('#txtBankAccountNumber').html(result.BankAccountNumber);
                           $('#txtAgencypersonemail').html(result.AgencycontactpersonEmail);
                           $('#txtAgencypersonmobile').html(result.Agencycontactpersonmobile);
                           $('#Availablebalanceamount').html(result.AvailableBalance);  

                           var PrevInvdate = result.PreviousInvoiceDate;
                           var PrevInvId = result.PreviousInvoiceId;
                           var PrevInvNum = result.PreviousInvoiceNumber;
                           var PrevInvVal = result.PreviousInvoicevalue;                          

                           $.each(PrevInvNum, function (i, val) {
                               if (i == 0) {
                                   document.getElementsByName('PreviousInvoiceDate')[0].value = PrevInvdate[0];
                                   document.getElementsByName('PreviousInvoiceId')[0].value = PrevInvId[0];
                                   document.getElementsByName('PreviousInvoiceNumber')[0].value = PrevInvNum[0];
                                   document.getElementsByName('PreviousInvoicevalue')[0].value = PrevInvVal[0];

                               } else {
                                   //var cln = $("#primaryStaffDiv").clone().find("input").val("").end();
                                   ////$(cln).find('.dis-none').removeClass('dis-none');
                                   //$('#divStaffContent').append(cln)
                                   document.getElementsByName('PreviousInvoiceDate')[i].value = PrevInvdate[i];
                                   document.getElementsByName('PreviousInvoiceId')[i].value = PrevInvId[i];
                                   document.getElementsByName('PreviousInvoiceNumber')[i].value = PrevInvNum[i];
                                   document.getElementsByName('PreviousInvoicevalue')[i].value = PrevInvVal[i];

                               }
                           });

                           var Instalmentnumber = result.InstlmntNumber;
                           var Instalmentyear = result.Instalmentyear;
                           var Instalmentvalue = result.InstalValue;
                           var InvoicedStatus = result.Invoiced;

                           $.each(PrevInvNum, function (i, val) {
                               if (i == 0) {
                                   document.getElementsByName('InstlmntNumber')[0].value = Instalmentnumber[0];
                                   document.getElementsByName('Instalmentyear')[0].value = Instalmentyear[0];
                                   document.getElementsByName('InstalValue')[0].value = Instalmentvalue[0];
                                   document.getElementsByName('Invoiced')[0].value = InvoicedStatus[0];

                               } else {
                                   //var cln = $("#primaryStaffDiv").clone().find("input").val("").end();
                                   ////$(cln).find('.dis-none').removeClass('dis-none');
                                   //$('#divStaffContent').append(cln)
                                   document.getElementsByName('InstlmntNumber')[i].value = Instalmentnumber[i];
                                   document.getElementsByName('Instalmentyear')[i].value = Instalmentyear[i];
                                   document.getElementsByName('InstalValue')[i].value = Instalmentvalue[i];
                                   document.getElementsByName('Invoiced')[i].value = InvoicedStatus[i];

                               }
                           });

                       },

                       error: function (err) {
                           console.log("error1 : " + err);
                       }

                   });
                   $("#AddNewEntryModel").modal('hide');

               });
           }
         }
           
        ],


    });

    $("#gridInvoiceList").jsGrid("option", "filtering", false);
    //Get project enhancement flow details
    $.ajax({
        type: "GET",
        url: getInvoiceProcessList,
        data: param = "",
        contentType: "application/json; charset=utf-8",
        success: function (result) {
           // dataProposal = result;
            $("#gridInvoiceList").jsGrid({ data: result });
            $('#ProjectInvoice').hide();
            $('#gridlist').show();
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
                $('#ProjectInvoice').hide();
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