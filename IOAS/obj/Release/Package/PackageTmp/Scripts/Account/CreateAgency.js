﻿$(function () {
    var Getagencylist = 'Getagencylist',
        Editagencylist = 'GetEditagency', Deleteagencylist = 'deleteagency';
    var db;
    Getagencylists();
    $('#agencylist').jsGrid({
        paging: true,
        editing: true,
        pageIndex: 1,
        pageSize: 10,
        editing: true,
        filtering: true,
        controller: {

            loadData: function (filter) {
                return $.grep(db, function (agency) {

                    return (!filter.AgencyName || agency.AgencyName.toLowerCase().indexOf(filter.AgencyName.toLowerCase()) > -1)
                    && (!filter.AgencyCode || agency.AgencyCode.toLowerCase().indexOf(filter.AgencyCode.toLowerCase()) > -1)
                    && (!filter.CountryName || agency.CountryName.toLowerCase().indexOf(filter.CountryName.toLowerCase()) > -1);

                });
            }

        },
        fields: [
                    { type: "number", name: "sno", title: "S.No", editing: false, filtering: false, align: "left", width: "70px" },
                    { type: "number", name: "AgencyId", title: "Agency Id", editing: false, visible: false },
                    { type: "text", name: "AgencyName", title: "Agency Name", editing: false},
                    { type: "number", name: "AgencyCode", title: "Agency Code"  },
                    { type: "text", name: "CountryName", title: "Country Name", editing: false},
                   
                    {
                        type: "control",
                        _createFilterSwitchButton: function () {
                            return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
                        }
                    }

        ],
        onItemEditing: function (args) {
            if (args.item.AgencyId > 0) {
                var agencyid = args.item.AgencyId;

            }
            $('#createagency,#btnUpdate,#categoryagy').show();
            $('#btnSave,#gridlist,#addnewpage').hide();
            $.ajax({
                type: "POST",
                url: Editagencylist,
                data: { Agencyid: agencyid },
                success: function (result) {
                   
                    $('#ProjAndCountry').val(result.IndianagencyCategoryId);
                    $('#txtagencyid').val(result.AgencyId);
                    $('#txtagencyname').val(result.AgencyName);
                    $('#txtagencycode').val(result.AgencyCode);
                    $('#txtcontactper').val(result.ContactPerson);
                    $('#txtcontactno').val(result.ContactNumber);
                    $('#txtcontactmail').val(result.ContactEmail);
                    $('#txtaddress').val(result.Address);
                    $('#txtstate').val(result.State);
                    $('#ddlcountry').val(result.Country);
                    $('#ddlageny').val(result.AgencyType);
                    $('#ddlscheme').val(result.Scheme);
                    $('#txtpan').val(result.PAN);
                    $('#txtgstin').val(result.GSTIN);
                    $('#txttan').val(result.TAN);
                    $('#ddlstate').val(result.StateId);
                    $('#txtstatecode').val(result.StateCode);
                    $('#txtbankname').val(result.BankName);
                    $('#txtacctno').val(result.AccountNumber);
                    $('#txtbranch').val(result.BranchName);
                    $('#txtswiftcode').val(result.SWIFTCode);
                    $('#txtmicrcode').val(result.MICRCode);
                    $('#txtifsccode').val(result.IFSCCode);
                    $('#txtbankadd').val(result.BankAddress);
                    $('#txtdist').val(result.District);
                    $('#txtpincode').val(result.PinCode);
                    $('#ddlcountrycate').val(result.AgencycountryCategoryId);
                    //$('#ProjAndCountry').val(result.IndianagencyCategoryId);
                    $('#ddlnonsezcategory').val(result.NonSezCategoryId);
                    $('#txtregistername').val(result.AgencyRegisterName);
                    $('#txtregisteraddr').val(result.AgencyRegisterAddress);
                    $('#txtProjectType').val(result.ProjectTypeId);
                    $('#ddlCompanyId').val(result.CompanyId);
                    $('#ddlGovMinistry').val(result.Ministry);
                   
                    if (result.AgencycountryCategoryId ==1)
                    {
                        //$("#ddlcountry").prop('required', false);
                        //$("#lblallcountry").removeClass("required");
                        //$("#ddlcountry").hide();
                        //$("#lblallcountry").hide();
                        //$("#valallcountry").hide();
                        //$('#txtstate').hide();
                        //$('#lblstatetxtvalidation').hide();
                        //$('#ddlstate').show();
                        //$('#txtstatecode').show();
                        //$('#lblstatecode').show();
                        //$('#lblstateddlvalidation').show();

                        $("#ProjAndCountry,#ddlnonsezcategory,#valnonsez").show();
                        $("#lblagycate,#lblnon").show();
                        $("#lblagycate,#lblnon").addClass("required");
                        
                        countrychange();
                       $("#ProjAndCountry,#ddlnonsezcategory").prop('required', true);
                        if (result.AgencycountryCategoryId == 1 && result.IndianagencyCategoryId == 1 && result.NonSezCategoryId == 0) {
                            $('#countryView').show();
                            $('#indcategory').show();
                            
                            $("#lblpan,#lbltan").removeClass("required");
                            $("#lblgstin").removeClass("required");
                            $("#txtpan,#txttan").prop('required', false);
                            $("#lblnon").removeClass("required");
                            $("#ddlnonsezcategory").prop('required', false);
                            $("#lblnon,#valnonsez").hide();
                            $("#ddlnonsezcategory").hide();
                            $('#ddlnonsezcategory').prop('selectedIndex', 0);
                            $("#valtan,#valpan,#valgstin").show();
                        }
                        else if (result.AgencycountryCategoryId == 1 && result.IndianagencyCategoryId == 2 && result.NonSezCategoryId == 0) {
                            $('#countryView').show();
                            $('#indcategory').show();
                           
                            $("#lblnon,#valnonsez").show();
                            $("#ddlnonsezcategory").show();
                            $("#lblnon").addClass("required");
                            $("#ddlnonsezcategory").prop('required', true);
                        }
                        else if (result.AgencycountryCategoryId == 1 && result.IndianagencyCategoryId == 2 && result.NonSezCategoryId == 1) {
                            $('#countryView').show();
                            $('#indcategory').show();
                            $('#nonsezcategory').show();
                            $("#lblgstin").addClass("required");
                            $("#txtpan,#txttan,#txtgstin").prop('required', false);
                            $('#txtgstin"').prop('required', true);
                            $("#valgstin").show();
                        }
                        else if (result.AgencycountryCategoryId == 1 && result.IndianagencyCategoryId == 2 && result.NonSezCategoryId == 2) {
                            $('#countryView').show();
                            $('#indcategory').show();
                            $('#nonsezcategory').show();
                            $('#countryView').show();
                            $("#lblpan,#lbltan").removeClass("required");
                            $("#txtpan,#txttan").prop('required', false);
                            $("#lblgstin").removeClass("required");
                            $("#txtgstin").prop('required', false);
                            $("#valgstin").hide();
                        }
                        else if (result.AgencycountryCategoryId == 1 && result.IndianagencyCategoryId == 1 && result.NonSezCategoryId != 0) {
                            $('#countryView').show();
                            $('#indcategory').show();
                            $('#nonsezcategory').show();
                            $("#lblpan,#lbltan").removeClass("required");
                            $("#txtpan,#txttan").prop('required', false);
                            $("#lblnon").removeClass("required");
                            $("#ddlnonsezcategory").prop('required', false);
                            $("#lblnon,#valnonsez").hide();
                            $("#ddlnonsezcategory").hide();
                            $('#ddlnonsezcategory').prop('selectedIndex', 0);
                        }
                    }
                    if (result.IndianagencyCategoryId == 1&&result.ProjectTypeId==1)
                    {
                        $('#countryView').show();
                        $("#lblpan,#lbltan").addClass("required");
                        $("#txtpan,#txttan").prop('required', true);
                        $("#lblnon").removeClass("required");
                        $("#ddlnonsezcategory").prop('required', false);
                        $("#lblnon,#valnonsez").hide();
                        $("#ddlnonsezcategory").hide();
                        $('#ddlnonsezcategory').prop('selectedIndex', 0);
                    }
                    if (result.AgencycountryCategoryId == 2)
                    {
                        $('#countryView').show();
                        $("#lblpan,#lblgstin,#lbltan,#lblagycate,#lblnon").removeClass("required");
                        $("#ProjAndCountry,#ddlnonsezcategory,#txtpan,#txttan,#txtgstin").prop('required', false);
                        $('#countryView').show();
                        $("#ProjAndCountry,#ddlnonsezcategory,#valnonsez").hide();
                        $("#lblagycate,#lblnon,#valindcat,#valpan,#valgstin,#valtan").hide();
                        $('#ddlnonsezcategory,#ProjAndCountry').prop('selectedIndex', 0);
                    }
                    if (result.AgencyType==2)
                    {
                        $('#countryView').show();
                        $('#lblscheme').show();
                        $('#ddlscheme').show();
                        $('#lblvalmsg').show();
                    }
                    var Docname = result.DocumentName;
                    var Attachname = result.AttachName;
                    var Doctype = result.DocumentType;
                    var Docpath = result.DocPath;
                    var DocID = result.DocumentId;
                    $.each(Docname, function (i, doc) {
                        if (i == 0) {
                            document.getElementsByName('DocumentType')[0].value = Doctype[0];
                            document.getElementsByName('DocumentId')[0].value = DocID[0];
                            document.getElementsByName('AttachName')[0].value = Attachname[0];
                            document.getElementsByClassName('link1')[0].text = Docname[0];
                            document.getElementsByClassName('link1')[0].href = "ShowDocument?file=" + Docpath[0] + "&filepath=~%2FContent%2FProposalDocuments%2F";
                        }
                        else {
                            var cln = $("#DocprimaryDiv").clone().find("input").val("").end();
                            $(cln).find('.dis-none').removeClass('dis-none');
                            $('#DocdivContent').append(cln)
                            document.getElementsByName('DocumentType')[i].value = Doctype[i];
                            document.getElementsByName('DocumentId')[i].value = DocID[i];
                            document.getElementsByName('AttachName')[i].value = Attachname[i];
                            document.getElementsByClassName('link1')[i].text = Docname[i];
                            document.getElementsByClassName('link1')[i].href = "ShowDocument?file=" + Docpath[i] + "&filepath=~%2FContent%2FAgencyDocument%2F";
                        }
                    });
                    
                },
                error: function (err) {
                    console.log("error1 : " + err);
                }
            });

        },
        onItemDeleting: function (args) {
            if (args.item.AgencyId > 0) {
                var agencyid = args.item.AgencyId;
            }

            $.ajax({
                type: "POST",
                url: Deleteagencylist,
                data: { Agencyid: agencyid },
                success: function (result) {
                    
                    if (result == 1) {
                        
                        $('#createagency,#btnUpdate,#btnSave,#categoryagy').hide();
                        $('#gridlist').show();
                        $('#addnewpage').show();
                        Getagencylists();
                        
                        $('#deletemodal').modal('show');
                    }
                    
                    else {

                        $('#createagency,#btnUpdate,#btnSave,#categoryagy').hide();
                        $('#gridlist').show();
                        $('#addnewpage').show();
                        Getagencylists();
                        $('#deletemodalerror').modal('show');
                    }
                },
                error: function (err) {
                    console.log("error1 : " + err);
                }
            });
        },
    });
    $("#agencylist").jsGrid("option", "filtering", false);
    $.ajax({

        type: "GET",
        url: Getagencylist,
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (result) {
            
            $("#agencylist").jsGrid({
                data: result
            });

        },
        error: function (err) {
            console.log("error : " + err);
        }

    });
    function Getagencylists() {
        $.ajax({

            type: "GET",
            url: Getagencylist,
            data: "",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (result) {

                $("#agencylist").jsGrid({
                    data: result
                });
                db = result;
            },
            error: function (err) {
                console.log("error : " + err);
            }

        });
    }
    $('#btnSrchUser').on('click', function () {

        var input = {
            SearchAgencyName: $('#txtsrchname').val(),
            SearchAgencyCode: $('#txtSeaAgencyCode').val(),
            SearchAgencyCountry: $('#ddlSeaCountry').val()
        }

        $.ajax({
            type: "Get",
            url: Getagencylist,
            data: input,
            dataType: "json",
            success: function (result) {
                console.log(result);
                $("#agencylist").jsGrid({ data: result });
                //$('#gridlist').show();

            },
            error: function (err) {
                console.log("error : " + err);
            }

        });
    });
    $('#btnResetSrchUser').on('click', function () {
        $('#txtsrchname,#txtSeaAgencyCode').val('');
        $('#ddlSeaCountry').prop("selectedIndex", 0);
        Getagencylists();
    });
});

