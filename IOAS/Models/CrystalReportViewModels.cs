using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IOAS.Models
{
    public class ProjectNumModel
    {
        public string ProjectNumber { get; set; }
    }
    public class TransactionTypeModel
    {
        public string TransactionType { get; set; }
    }

    public class TrailBalanceModel
    {
        public string Accounts { get; set; }
        public string Groups { get; set; }
        public int AccountGroupId { get; set; }
        public int AccountHeadId { get; set; }
        public string AccountHead { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public bool Creditor_f { get; set; }
        public bool Debtor_f { get; set; }
        public int BOATransactionId { get; set; }
        public decimal ttlAssetDr { get; set; }
        public decimal ttlAssetCr { get; set; }
        public decimal ttlLiabilityDr { get; set; }
        public decimal ttlLiabilityCr { get; set; }
        public decimal ttlIncomeDr { get; set; }
        public decimal ttlIncomeCr { get; set; }
        public decimal ttlExpenseDr { get; set; }
        public decimal ttlExpenseCr { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public int HeadId { get; set; }

    }
    public class PostingsModel
    {
        public DateTime PostedDate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string TransactionTypeCode { get; set; }
        public string TempVoucherNumber { get; set; }
        public string TransType { get; set; }
        public int BOATransactionId { get; set; }
        public int AccountGroupId { get; set; }
        public string Accounts { get; set; }
        public string Groups { get; set; }
        public int AccountHeadId { get; set; }
        public string AccountHead { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public bool Creditor_f { get; set; }
        public bool Debtor_f { get; set; }
    }
    public class AccountTypeModel
    {
        public string FinancialYear { get; set; }
        public int FinancialId { get; set; }
        public int Financial { get; set; }
    }
    public class CommitmentReportModel
    {
        public string ProjectNumber { get; set; }
        public string ProjectId { get; set; }
        public string CommitmentNumber { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int ProjectType { get; set; }
        public string ProjectTypeName { get; set; }
        public string CommitmentType { get; set; }
        public DateTime CommitmentDate { get; set; }
        public decimal CommitmentAmount { get; set; }
        public decimal BookedValue { get; set; }
    }
    public class CashBookModel
    {
        public int AccountHeadId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int BOAPaymentDetailId { get; set; }
        public DateTime ReferenceDate { get; set; }
        public string VoucherNumber { get; set; }
        public decimal Amount { get; set; }
        public string PayeeBank { get; set; }
        public int BOAId { get; set; }
        public string TransactionType { get; set; }
        public int BankHeadID { get; set; }
        public string PayeeName { get; set; }
        public string TransactionTypeCode { get; set; }
        public string VoucherPayee { get; set; }
        public int BankId { get; set; }
    }
    public class ProposalRepotViewModels
    {
        [Required]
        [Display(Name = "From date")]
        public DateTime FromDate { get; set; }
        [Required]
        [Display(Name = "To date")]
        public DateTime ToDate { get; set; }
        [Required]
        [Display(Name = "Project type")]
        public int ProjecttypeId { get; set; }
        public string Proposalnumber { get; set; }
        public string PI { get; set; }
        public string ProposalTitle { get; set; }
        public string Department { get; set; }
        public DateTime InwardDate { get; set; }
        public int Durationofprojectyears { get; set; }
        public int Durationofprojectmonths { get; set; }
        public DateTime Crtd_TS { get; set; }
        public string keysearch { get; set; }

    }
    public class BOATransactionDetailsModels
    {
        public DateTime PostedDate { get; set; }
        public string ProjectNumber { get; set; }
        public string HeadName { get; set; }
        public string CommitmentNumber { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
    }
    public class ProjectTransactionModel
    {
        public string ProjNo { get; set; }
        public int ProjId { get; set; }
        public int ProjectId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateOfTransaction { get; set; }
        public string RefNo { get; set; }
        public string TransType { get; set; }
        public string Code { get; set; }
        public string FunctionName { get; set; }
        public string Category { get; set; }
        public string CommitmentNumber { get; set; }
    }
}