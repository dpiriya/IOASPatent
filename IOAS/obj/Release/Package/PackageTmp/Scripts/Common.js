function deductionEligibilityCheck(value) {
    var eligibilityCheck_f = false;
    if (value > 300000)
        eligibilityCheck_f = true;
    return eligibilityCheck_f;
}
function GetSubCodeByTDSSection(tdsSel) {
    var subcode = '1';
    if (tdsSel == "1")
        subcode = '2';
    else if (tdsSel == "2")
        subcode = '3';
    return subcode
}
function IsInterState(stateCode) {
    var isInterState = true;
    if (stateCode == '33')
        isInterState = false;
    return isInterState;
}
function GetIndirectPaymentSubCodeByTDSSection(tdsSel) {
    var subcode = '1';
    if (tdsSel != "")
        subcode = '2';
    return subcode
}
function bootstrapSelect(el, choice, options, select, fillDafaultText, idAsName) {
    $(el).find('select').selectpicker({
        liveSearch: true
    });
    $(el).children().eq(2).siblings().remove();
    if (choice == "add") {
        $(el).find('.selectpicker').append("<option>" + options + "</option>");
    } else if (choice == "all" && select != '') {
        $(el).find('.selectpicker').children().remove();
        if (fillDafaultText !== undefined) $(el).find('.selectpicker').append("<option value=''>Select any</option>");
        if (idAsName !== undefined) {
            for (var i = 0 ; i < options.length ; i++) {
                $(el).find('.selectpicker').append("<option value='" + options[i].name + "'>" + options[i].name + "</option>");
            }
        } else {
            for (var i = 0 ; i < options.length ; i++) {
                $(el).find('.selectpicker').append("<option value=" + options[i].id + ">" + options[i].name + "</option>");
            }
        }
        $(el).find('.selectpicker option[value=' + select + ']').attr('selected', 'selected');
    } else if (choice == "all" && select == '') {
        $(el).find('.selectpicker').children().remove();
        if (fillDafaultText !== undefined) $(el).find('.selectpicker').append("<option value=''>Select any</option>");
        if (idAsName !== undefined) {
            for (var i = 0 ; i < options.length ; i++) {
                $(el).find('.selectpicker').append("<option value='" + options[i].name + "'>" + options[i].name + "</option>");
            }
        } else {
            for (var i = 0 ; i < options.length ; i++) {
                $(el).find('.selectpicker').append("<option value=" + options[i].id + ">" + options[i].name + "</option>");
            }
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
function fillTransactionDetails(typeCode, tSubCode, interstate_f, eligibilityCheck_f, deductionCategoryId, tdsDetailId) {
    if (typeCode === undefined)
        return false;
    EmptyExpenseDeductionDetails();
    var ttlAdvTax = parseFloat($('#lblAdvTtlAmt').html());
    $.ajax({
        type: "GET",
        url: "../CoreAccounts/GetTransactionDetails",
        data: { "interstate_f": interstate_f, "typeCode": typeCode, "tSubCode": tSubCode, "eligibilityCheck_f": eligibilityCheck_f, "TDSDetailId": tdsDetailId, "deductionCategoryId": deductionCategoryId },
        traditional: true,
        dataType: "json",
        success: function (result) {
            $.each(result.ExpenseDetail, function (i, item) {
                if (i == 0) {
                    var trEle = $('#tbodyExpenseList tr:first');
                    $(trEle).find('input[name$=".Amount"]').val('');
                    $(trEle).find('input[name$=".TransactionType"]').val(item.TransactionType);
                    $(trEle).find('#lblTransType').text(item.TransactionType);
                    $(trEle).find("input[name='ExpenseDetail.Index']").val(i);

                    var selectGroup = $(trEle).find('select[name$=".AccountGroupId"]');
                    selectGroup.empty();
                    $.each(item.AccountGroupList, function (index, itemData) {
                        selectGroup.append($('<option/>', {
                            value: itemData.id,
                            text: itemData.name,
                        }));
                    });

                    var selectHead = $(trEle).find('select[name$=".AccountHeadId"]');
                    selectHead.empty();
                    $.each(item.AccountHeadList, function (index, itemData) {
                        selectHead.append($('<option/>', {
                            value: itemData.id,
                            text: itemData.name,
                        }));
                    });
                } else {
                    var trEleNew = $('#tbodyExpenseList tr:first').clone().find('input').val('').end();
                    $(trEleNew).find('input[name$=".Amount"]').val('');
                    $(trEleNew).find("input[name='ExpenseDetail.Index']").val(i);
                    $(trEleNew).find('input[name$=".TransactionType"]').val(item.TransactionType);
                    $(trEleNew).find('#lblTransType').text(item.TransactionType);
                    $(trEleNew).find("input,Select").each(function () {
                        $(this).attr("name", $(this).attr("name").replace(/\d+/, i));
                        $(this).attr("id", $(this).attr("id").replace(/\d+/, i));
                    });
                    $(trEleNew).find("span[data-valmsg-for]").each(function () {
                        $(this).attr("data-valmsg-for", $(this).attr("data-valmsg-for").replace(/\d+/, i));
                    });

                    var selectGroup = $(trEleNew).find('select[name$=".AccountGroupId"]');
                    selectGroup.empty();
                    $.each(item.AccountGroupList, function (index, itemData) {
                        selectGroup.append($('<option/>', {
                            value: itemData.id,
                            text: itemData.name,
                        }));
                    });

                    var selectHead = $(trEleNew).find('select[name$=".AccountHeadId"]');
                    selectHead.empty();
                    $.each(item.AccountHeadList, function (index, itemData) {
                        selectHead.append($('<option/>', {
                            value: itemData.id,
                            text: itemData.name,
                        }));
                    });
                    $('#tbodyExpenseList').append(trEleNew);
                }
            });
            $.each(result.DeductionDetail, function (i, item) {
                if (i == 0) {
                    var trEle = $('#tbodyDeductionList tr:first');
                    var amtEle = $(trEle).find('input[name$=".Amount"]');
                    if (item.TDSPercentage == null) {
                        amtEle.val('');
                    } else {
                        tds = ttlAdvTax * item.TDSPercentage / 100;
                        amtEle.val(tds);
                    }

                    amtEle.addClass('required');
                    $(trEle).find('input[name$=".AccountGroupId"]').val(item.AccountGroupId);
                    $(trEle).find('input[name$=".DeductionHeadId"]').val(item.DeductionHeadId);
                    $(trEle).find('input[name$=".DeductionHead"]').val(item.DeductionHead);
                    $(trEle).find('input[name$=".AccountGroup"]').val(item.AccountGroup);
                    $(trEle).find('td:nth-child(1)').html(item.AccountGroup);
                    $(trEle).find('td:nth-child(2)').html(item.DeductionHead);
                } else {
                    var trEleNew = $('#tbodyDeductionList tr:first').clone().find('input').val('').end();
                    //$(trEleNew).find("input[name='DeductionDetail.Index']").val(i);
                    $(trEleNew).find("input").each(function () {
                        $(this).attr("name", $(this).attr("name").replace(/\d+/, i));
                        $(this).attr("id", $(this).attr("id").replace(/\d+/, i));
                    });
                    $(trEleNew).find("span[data-valmsg-for]").each(function () {
                        $(this).attr("data-valmsg-for", $(this).attr("data-valmsg-for").replace(/\d+/, i));
                    });
                    var amtEle = $(trEleNew).find('input[name$=".Amount"]');
                    amtEle.addClass('required');
                    if (item.TDSPercentage != null) {
                        tds = ttlAdvTax * item.TDSPercentage / 100;
                        amtEle.val(tds);
                    }
                    $(trEleNew).find('input[name$=".AccountGroupId"]').val(item.AccountGroupId);
                    $(trEleNew).find('input[name$=".DeductionHeadId"]').val(item.DeductionHeadId);
                    $(trEleNew).find('input[name$=".DeductionHead"]').val(item.DeductionHead);
                    $(trEleNew).find('input[name$=".AccountGroup"]').val(item.AccountGroup);
                    $(trEleNew).find('td:nth-child(1)').html(item.AccountGroup);
                    $(trEleNew).find('td:nth-child(2)').html(item.DeductionHead);
                    $('#tbodyDeductionList').append(trEleNew);
                }
            });
            $('#NeedUpdateTransDetail').val('false');
            CalculateDeductionTotal();
        },
        error: function (err) {
            console.log("error : " + err);
        }
    });
}
function fillAutoCompleteDropDown(ele, data, fillDafaultText, valueAsName) {
    if (fillDafaultText !== undefined) {
        ele.append($('<option/>', {
            value: '',
            text: 'Select any',
        }));
    }
    if (valueAsName !== undefined) {
        $.each(data, function (index, itemData) {
            ele.append($('<option/>', {
                value: itemData.label,
                text: itemData.label,
            }));
        });
    } else {
        $.each(data, function (index, itemData) {
            ele.append($('<option/>', {
                value: itemData.value,
                text: itemData.label,
            }));
        });
    }
}
function applyPaymentBUAutoComplete(ele, url, setId) {
    $(ele).autocomplete({
        select: function (event, ui) {
            event.preventDefault();
            $(ele).val(ui.item.label);
            //$(ele).closest('tr').find(".lblSelId").text(ui.item.label);
            if (setId == true) {
                $(ele).closest('tr').find("input[name$='.UserId']").val(ui.item.value);
                $(ele).closest('tr').find("input[name$='.Name']").val(ui.item.label);
            } else {
                $(ele).closest('tr').find("input[name$='.UserId']").val('0');
                $(ele).closest('tr').find("input[name$='.Name']").val(ui.item.label);
            }
        },
        focus: function (event, ui) {
            event.preventDefault();
            $(ele).val(ui.item.label);
        },
        source: function (request, response) {
            $.getJSON(url, { term: request.term },
             function (locationdata) {
                 response(locationdata);
             });
        },
        minLength: 3
    });
}
function paymentCategoryChange(el, mode) {
    var selCat = $(el).val();
    if (mode != 'U') {
        $(el).closest('tr').find("input[name$='.autoComplete'],input[name$='.UserId'],input[name$='.Name']").val('');
    }
    if (selCat == 1) {
        $(el).closest('tr').find("td.tdDDLUser").removeClass('dis-none');
        $(el).closest('tr').find("td.tdTxtName").addClass('dis-none');
        $(el).closest('tr').find("input[name$='.UserId']").addClass('required');
        var ele = $(el).closest('tr').find("input[name$='.autoComplete']");
        $(ele).addClass('required');
        $(el).closest('tr').find("input[name$='.Name']").removeClass('required');
        applyPaymentBUAutoComplete(ele, "../CoreAccounts/LoadPIList", true)
    } else if (selCat == 2) {
        $(el).closest('tr').find("td.tdDDLUser").removeClass('dis-none');
        $(el).closest('tr').find("td.tdTxtName").addClass('dis-none');
        $(el).closest('tr').find("input[name$='.UserId']").addClass('required');
        var ele = $(el).closest('tr').find("input[name$='.autoComplete']");
        $(ele).addClass('required');
        $(el).closest('tr').find("input[name$='.Name']").removeClass('required');
        applyPaymentBUAutoComplete(ele, "../CoreAccounts/LoadStudentList", false);
    } else if (selCat == 3) {
        $(el).closest('tr').find("td.tdDDLUser").removeClass('dis-none');
        $(el).closest('tr').find("td.tdTxtName").addClass('dis-none');
        $(el).closest('tr').find("input[name$='.UserId']").addClass('required');
        var ele = $(el).closest('tr').find("input[name$='.autoComplete']");
        $(ele).addClass('required');
        $(el).closest('tr').find("input[name$='.Name']").removeClass('required');
        applyPaymentBUAutoComplete(ele, "../CoreAccounts/LoadClearanceAgentList", true)
        //$(el).closest('tr').find("td.tdDDLUser").addClass('dis-none');
        //$(el).closest('tr').find("td.tdTxtName").removeClass('dis-none');
        //$(el).closest('tr').find("input[name$='.autoComplete']").removeClass('required');
        //var ele = $(el).closest('tr').find("input[name$='.UserId']");
        //$(ele).removeClass('required');
        //$(el).closest('tr').find("input[name$='.Name']").addClass('required');
    } else if(selCat == ''){
        $(el).closest('tr').find("input[name$='.autoComplete'],input[name$='.UserId'],input[name$='.Name']").removeClass('required');
    }
}
function applyAutoCompleteProject(el, mode) {
    var selCat = $(el).val();
    if (mode != 'U') {
        $(el).closest('tr').find("input[name$='.autoComplete'],input[name$='.UserId'],input[name$='.Name']").val('');
    }
    if (selCat == 1) {
        $(el).closest('tr').find("td.tdDDLUser").removeClass('dis-none');
        $(el).closest('tr').find("td.tdTxtName").addClass('dis-none');
        $(el).closest('tr').find("input[name$='.UserId']").addClass('required');
        var ele = $(el).closest('tr').find("input[name$='.autoComplete']");
        $(ele).addClass('required');
        $(el).closest('tr').find("input[name$='.Name']").removeClass('required');
        applyPaymentBUAutoComplete(ele, "../CoreAccounts/LoadPIList", true)
    } else if (selCat == 2) {
        $(el).closest('tr').find("td.tdDDLUser").removeClass('dis-none');
        $(el).closest('tr').find("td.tdTxtName").addClass('dis-none');
        $(el).closest('tr').find("input[name$='.UserId']").addClass('required');
        var ele = $(el).closest('tr').find("input[name$='.autoComplete']");
        $(ele).addClass('required');
        $(el).closest('tr').find("input[name$='.Name']").removeClass('required');
        applyPaymentBUAutoComplete(ele, "../CoreAccounts/LoadStudentList", false);
    } else if (selCat == 3) {
        $(el).closest('tr').find("td.tdDDLUser").removeClass('dis-none');
        $(el).closest('tr').find("td.tdTxtName").addClass('dis-none');
        $(el).closest('tr').find("input[name$='.UserId']").addClass('required');
        var ele = $(el).closest('tr').find("input[name$='.autoComplete']");
        $(ele).addClass('required');
        $(el).closest('tr').find("input[name$='.Name']").removeClass('required');
        applyPaymentBUAutoComplete(ele, "../CoreAccounts/LoadClearanceAgentList", true)
        //$(el).closest('tr').find("td.tdDDLUser").addClass('dis-none');
        //$(el).closest('tr').find("td.tdTxtName").removeClass('dis-none');
        //$(el).closest('tr').find("input[name$='.autoComplete']").removeClass('required');
        //var ele = $(el).closest('tr').find("input[name$='.UserId']");
        //$(ele).removeClass('required');
        //$(el).closest('tr').find("input[name$='.Name']").addClass('required');
    }
}
function travelerCategoryChange(el, mode) {
    var selCat = $(el).val();
    if (mode != 'U') {
        $(el).closest('tr').find("input[name^='autoComplete'],input[name^='.UserId'],input[name^='.Name']").val('');
    }
    if (selCat == 1) {
        $(el).closest('tr').find("td.tdDDLUser").removeClass('dis-none');
        $(el).closest('tr').find("td.tdTxtName").addClass('dis-none');
        $(el).closest('tr').find("input[name$='.UserId']").addClass('required');
        var ele = $(el).closest('tr').find("input[name$='.autoComplete']");
        $(ele).addClass('required');
        $(el).closest('tr').find("input[name$='.Name']").removeClass('required');
        applyPaymentBUAutoComplete(ele, "../CoreAccounts/LoadPIList", true)
    } else if (selCat == 2) {
        $(el).closest('tr').find("td.tdDDLUser").removeClass('dis-none');
        $(el).closest('tr').find("td.tdTxtName").addClass('dis-none');
        $(el).closest('tr').find("input[name$='.UserId']").addClass('required');
        var ele = $(el).closest('tr').find("input[name$='.autoComplete']");
        $(ele).addClass('required');
        $(el).closest('tr').find("input[name$='.Name']").removeClass('required');
        applyPaymentBUAutoComplete(ele, "../CoreAccounts/LoadStudentList", false);
    } else if (selCat == 3) {
        $(el).closest('tr').find("td.tdDDLUser").addClass('dis-none');
        $(el).closest('tr').find("td.tdTxtName").removeClass('dis-none');
        $(el).closest('tr').find("input[name$='.autoComplete']").removeClass('required');
        var ele = $(el).closest('tr').find("input[name$='.UserId']");
        $(ele).removeClass('required');
        $(el).closest('tr').find("input[name$='.Name']").addClass('required');
    }
}
function messageBox(message, type) {
    if (type == "warning") {
        $.alert({
            icon: 'ion-alert-circled',
            title: 'Warning!',
            content: message,
            type: 'orange',
            animation: 'top',
            closeAnimation: 'rotateX',
            animationBounce: 1.5
        });
    } else if (type == "error") {
        $.alert({
            icon: 'ion-close-circled',
            title: 'Error!',
            content: message,
            type: 'red',
            animation: 'top',
            closeAnimation: 'rotateX',
            animationBounce: 1.5
        });
    } else if (type == "success") {
        $.alert({
            icon: 'ion-checkmark-circled',
            title: 'Success!',
            content: message,
            type: 'green',
            animation: 'top',
            closeAnimation: 'rotateX',
            animationBounce: 1.5
        });
    } else {
        $.alert({
            content: message,
        });
    }

}
function fillCommitmentSrchAndSel(result, revised) {
    EmptyCommitmentSelList();
    EmptyCommitmentSrchList();
    var commitmentId = [];
    $.each(result, function (i, item) {
        if (i == 0) {
            commitmentId.push(item.CommitmentId);
            var trEle = $('#tbodyCommitmentSrchList tr:first');
            $(trEle).find('input[name="chkCommitmentId"]').val(item.CommitmentId).prop("checked", false);
            $(trEle).find('td:nth-child(2)').html(item.CommitmentNumber);
            $(trEle).find('td:nth-child(3)').html(item.ProjectNumber);
            $(trEle).find('td:nth-child(4)').html(item.CommitmentBookedAmount);
            $(trEle).find('td:nth-child(5)').html(item.CommitmentBalanceAmount);

            var trSelEle = $('#tbodyCommitmentSelList tr:first');
            $(trSelEle).find('td:nth-child(1)').html(item.CommitmentNumber);
            $(trSelEle).find('td:nth-child(2)').html(item.ProjectNumber);
            $(trSelEle).find('td:nth-child(3)').html(item.AvailableAmount);
            $(trSelEle).find('td:nth-child(4)').html(item.HeadName);
            $(trSelEle).find("input[name='CommitmentDetail.Index']").val(i);
            if (revised) {
                $(trSelEle).find("input[name$='.PaymentAmount']").val(item.PaymentAmount);
                $(trSelEle).find('td:nth-child(6) input[name$=".ReversedAmount"]').remove();
                $(trSelEle).find('td:nth-child(6)').append('<input type="text" id="CommitmentDetail_' + i + '_ReversedAmount" name="CommitmentDetail[' + i + '].ReversedAmount" class = "form-control required" onkeypress = "return ValidateDecimalOnly(event)" onblur = "CalculateReversedAmount()" value="0">');
                $(trSelEle).find('a.removeCommitment').addClass('dis-none');
                $('#tdRecAmt').html('Reversed Amount');
            }
            else {
                $(trSelEle).find("input[name$='.PaymentAmount']").val('');
                $('#tdRecAmt').html('');
                $(trSelEle).find('td:nth-child(6) input').remove();
                $(trSelEle).find('a.removeCommitment').removeClass('dis-none');
            }
            $(trSelEle).find('input[name$=".CommitmentNumber"]').val(item.CommitmentNumber);
            $(trSelEle).find('input[name$=".ProjectNumber"]').val(item.ProjectNumber);
            $(trSelEle).find('input[name$=".AvailableAmount"]').val(item.AvailableAmount);
            $(trSelEle).find('input[name$=".HeadName"]').val(item.HeadName);
            $(trSelEle).find('input[name$=".CommitmentDetailId"]').val(item.CommitmentDetailId);
        } else {
            if (commitmentId.indexOf(item.CommitmentId) == -1) {
                commitmentId.push(item.CommitmentId);
                var trEleNew = $('#tbodyCommitmentSrchList tr:first').clone();
                $(trEleNew).find('input[name="chkCommitmentId"]').val(item.CommitmentId).prop("checked", false);
                $(trEleNew).find('td:nth-child(2)').html(item.CommitmentNumber);
                $(trEleNew).find('td:nth-child(3)').html(item.ProjectNumber);
                $(trEleNew).find('td:nth-child(4)').html(item.CommitmentBookedAmount);
                $(trEleNew).find('td:nth-child(5)').html(item.CommitmentBalanceAmount);
                $('#tbodyCommitmentSrchList').append(trEleNew);
            }
            var trSelEle = $('#tbodyCommitmentSelList tr:first').clone();
            $(trSelEle).find('td:nth-child(1)').html(item.CommitmentNumber);
            $(trSelEle).find('td:nth-child(2)').html(item.ProjectNumber);
            $(trSelEle).find('td:nth-child(3)').html(item.AvailableAmount);
            $(trSelEle).find('td:nth-child(4)').html(item.HeadName);
            if (revised) {
                $(trSelEle).find("input[name$='.PaymentAmount']").val(item.PaymentAmount);
                //$(trSelEle).find('td:nth-child(6)').append('<input type="text" id="CommitmentDetail_' + i + '_ReversedAmount" name="CommitmentDetail[' + i + '].ReversedAmount" class = "form-control required" onkeypress = "return ValidateDecimalOnly(event)" onblur = "CalculateReversedAmount()" value="0">');
                //$(trSelEle).find('a.removeCommitment').addClass('dis-none');
            }
            else
                $(trSelEle).find("input[name$='.PaymentAmount']").val('');
            $(trSelEle).find('input[name$=".CommitmentNumber"]').val(item.CommitmentNumber);
            $(trSelEle).find('input[name$=".ProjectNumber"]').val(item.ProjectNumber);
            $(trSelEle).find('input[name$=".AvailableAmount"]').val(item.AvailableAmount);
            $(trSelEle).find('input[name$=".HeadName"]').val(item.HeadName);
            $(trSelEle).find("input[name='CommitmentDetail.Index']").val(i);
            $(trSelEle).find("input").each(function () {
                $(this).attr("name", $(this).attr("name").replace(/\d+/, i));
                $(this).attr("id", $(this).attr("id").replace(/\d+/, i));
            });
            $(trSelEle).find("span[data-valmsg-for]").each(function () {
                $(this).attr("data-valmsg-for", $(this).attr("data-valmsg-for").replace(/\d+/, i));
            });
            $(trSelEle).find('input[name$=".CommitmentDetailId"]').val(item.CommitmentDetailId);
            $('#tbodyCommitmentSelList').append(trSelEle);
        }
    });
    if (revised)
        CalculateReversedAmount();
    else
        CalculatePaymentValue();
}
function removeCommitmentAndTransValidation() {
    $('#tdRecAmt').html('');
    $('#tbodyCommitmentSelList tr').each(function () {
        $(this).find("input[name$='.PaymentAmount']").removeClass('required');
        $(this).find("input[name$='.ReversedAmount']").remove();
    });
    $('#tbodyExpenseList tr').each(function () {
        $(this).find("select[name$='.AccountGroupId'],select[name$='.AccountHeadId'],input[name$='.Amount']").removeClass('required');
        $(this).find('#lblTransType').html('');
        $(this).find("select").empty();
    });
    EmptyCommitmentSelList();
    EmptyExpenseDeductionDetails();
}
function removeCommitmentValidation() {
    $('#tdRecAmt').html('');
    $('#tbodyCommitmentSelList tr').each(function () {
        $(this).find("input[name$='.PaymentAmount']").removeClass('required');
        $(this).find("input[name$='.ReversedAmount']").remove();
    });
    EmptyCommitmentSelList();
}
function setCommitmentValidation() {
    $('#tbodyCommitmentSelList tr').each(function () {
        $(this).find("input[name$='.PaymentAmount']").addClass('required');
    });
}
function removePaymentBUValidation() {
    $('#tbodyPaymentBU tr').each(function () {
        $(this).find("select[name$='.CategoryId'],select[name$='.ModeOfPayment'],input[name$='.PaymentAmount'],input[name$='.autoComplete'],input[name$='.UserId'],input[name$='.Name']").removeClass('required');
    });
    $("tbodyPaymentBU input[name$='.PaymentAmount'], #PaymentBUTotal").val('0');
    EmptyPaymentBU();
}
function setPaymentBUValidation() {
    $('#tbodyPaymentBU tr').each(function () {
        $(this).find("select[name$='.CategoryId'],select[name$='.ModeOfPayment'],input[name$='.PaymentAmount']").addClass('required');
    });
}
function setCommitmentAndTransValidation() {
    $('#tbodyCommitmentSelList tr').each(function () {
        $(this).find("input[name$='.PaymentAmount']").addClass('required');
    });
    $('#tbodyExpenseList tr').each(function () {
        $(this).find("select[name$='.AccountGroupId'],select[name$='.AccountHeadId'],input[name$='.Amount']").addClass('required');
    });
}
function ValidateDecimalOnly(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode != 46 && charCode > 31
      && (charCode < 48 || charCode > 57))
        return false;

    return true;
}
function ValidateNumberOnly(e) {
    if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
        return false;
    }
}
function fillMasterDropDown(ele, data, fillDafaultText) {
    if (fillDafaultText !== undefined) {
        ele.append($('<option/>', {
            value: '',
            text: 'Select any',
        }));
    }
    $.each(data, function (index, itemData) {
        ele.append($('<option/>', {
            value: itemData.id,
            text: itemData.name,
        }));
    });
}
function applyProjectAutoComplete(ele, hiddenEle) {
    $(ele).autocomplete({
        select: function (event, ui) {
            event.preventDefault();
            $(ele).val(ui.item.label);
            $(hiddenEle).val(ui.item.value);
        },
        focus: function (event, ui) {
            event.preventDefault();
            $(ele).val(ui.item.label);
        },
        source: function (request, response) {
            $.getJSON("../CoreAccounts/LoadProjectList", { term: request.term },
             function (locationdata) {
                 response(locationdata);
             });
        },
        minLength: 3
    });
}
function applyAutoComplete(ele, hiddenEle, url, functionName, withParEle, serviceType,multiParam) {
    $(ele).autocomplete({
        select: function (event, ui) {
            event.preventDefault();
            $(ele).val(ui.item.label);
            $(hiddenEle).val(ui.item.value);
            if (functionName !== undefined && withParEle === undefined) {
                eval(functionName + "()");
            } else if (functionName !== undefined && withParEle !== undefined) {
                dispatch(functionName, $(ele));
            }
        },
        focus: function (event, ui) {
            event.preventDefault();
            $(ele).val(ui.item.label);
        },
        source: function (request, response) {
            if (serviceType !== undefined) {
                $.getJSON(url, { term: request.term, type: serviceType },
                 function (locationdata) {
                     response(locationdata);
                 });
            } else if (multiParam !== undefined) {
                multiParam.term = request.term;
                $.getJSON(url, multiParam,
                 function (locationdata) {
                     response(locationdata);
                 });
            } else {
                $.getJSON(url, { term: request.term },
                 function (locationdata) {
                     response(locationdata);
                 });
            }
        },
        minLength: 3
    }).autocomplete("widget").addClass("auto-com-z-index");
}
function dispatch(fn, args) {
    fn = (typeof fn == "function") ? fn : window[fn];  // Allow fn to be a function object or the name of a global function
    return fn.apply(this, args || []);  // args is optional, use an empty array by default
}
