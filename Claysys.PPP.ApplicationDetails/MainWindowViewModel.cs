using Claysys.PPP.ApplicationDetails;
using Claysys.PPP.ApplicationDetails.WebService;
using Claysys.PPP.ApplicationDetails.Export;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using Claysys.PPP.ApplicationDetails.Utility;

namespace Claysys.PPP.Model
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<NextGenPPP> data;

        public ObservableCollection<NextGenPPP> GridData
        {
            get { return data; }
            set { data = value; NotifyPropertyChanged(); }
        }

        private RelayCommand buttonCommand;

        public RelayCommand ButtonCommand
        {
            get { return buttonCommand; }
            set { buttonCommand = value; NotifyPropertyChanged(); }
        }

        private int progress;

        public int Progress
        {
            get { return progress; }
            set { progress = value; NotifyPropertyChanged(); }
        }

        private Visibility loadingVisible;

        public Visibility LoadingVisible
        {
            get { return loadingVisible; }
            set
            {
                loadingVisible = value; NotifyPropertyChanged();
            }
        }

        private int totalReqCount;

        public int TotalReqCount
        {
            get { return Convert.ToInt32(totalReqCount); }
            set { totalReqCount = value; NotifyPropertyChanged(); }
        }


        private bool enability;

        public bool Enability
        {
            get { return enability; }
            set { enability = value; NotifyPropertyChanged(); }
        }

        private string exportButtonContent;

        public string ExportButtonContent
        {
            get { return exportButtonContent; }
            set { exportButtonContent = value; NotifyPropertyChanged(); }
        }


        public MainWindowViewModel()
        {
            this.LoadingVisible = Visibility.Hidden;
            this.Enability = true;
            this.ExportButtonContent = "Export";
            DataManagement.GetApprovedCollection().ContinueWith(x => { this.GridData = new ObservableCollection<NextGenPPP>(x.Result); this.TotalReqCount = x.Result.Count; }
            );
            this.ButtonCommand = new RelayCommand(this.ButtonAction);
        }

        public async void ButtonAction(object param)
        {
            try
            {
                this.LoadingVisible = Visibility.Visible;
                this.Enability = false;
                this.ExportButtonContent = "Exporting";

                int totReq = this.TotalReqCount;

                for (int i = 0; i < totReq; i++)
                {
                    var result = await DataManagement.GetExportingData();
                    if (result.Count > 0)
                    {
                        await ExportManagement.WriteToDisk(result.First().LoanApplicationNo, result.First().SBALoanNo, result.First().DocuSignDoc);
                        await DataManagement.UpdateExportingData(result.First().LoanApplicationNo);
                        if (Utility.IsEventLogged)
                        {
                            Utility.LogAction("Export successfully : AppID -  " + result.First().LoanApplicationNo);
                        }
                    }
                    else
                    {
                        if (Utility.IsEventLogged)
                        {
                            Utility.LogAction("Warning : Irrivalent application");
                        }
                    }

                    this.Progress += (100 / this.TotalReqCount);
                }
                this.Enability = true;
                this.LoadingVisible = Visibility.Hidden;
                this.ExportButtonContent = "Export";
            }
            catch (Exception ex)
            {
                if (Utility.IsEventLogged)
                {
                    Utility.LogAction("Exception : " + ex) ;
                }
                throw ex;
            }
        }
    }
}
