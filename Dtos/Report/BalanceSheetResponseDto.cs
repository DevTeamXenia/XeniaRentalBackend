namespace XeniaRentalBackend.Dtos.Report
{
    public class BalanceSheetResponseDto
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }

        public List<GroupSummaryDto> IncomeGroups { get; set; } = new();
        public List<GroupSummaryDto> ExpenseGroups { get; set; } = new();
    }

    public class GroupSummaryDto
    {
        public string GroupName { get; set; } = null!;
        public decimal GroupTotal { get; set; }
        public List<LedgerTotalDto> Ledgers { get; set; } = new();
    }


    public class LedgerTotalDto
    {
        public string LedgerName { get; set; } = null!;
        public decimal Amount { get; set; }
    }



}
