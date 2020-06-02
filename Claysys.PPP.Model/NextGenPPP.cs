namespace Claysys.PPP.Model
{
    public class NextGenPPP : ViewModelBase
    {
        private string loanApplicationNumber;

        public string LoanApplicationNumber
        {
            get { return loanApplicationNumber; }
            set
            {
                loanApplicationNumber = value;
                NotifyPropertyChanged();
            }
        }

        public string SBALoanNo
        {
            get => sBALoanNo; set
            {
                sBALoanNo = value;
                NotifyPropertyChanged();
            }
        }

        public string BusinessName { get => businessName; set { businessName = value; NotifyPropertyChanged(); } }

        private string sBALoanNo;
        private string businessName;
    }
}
