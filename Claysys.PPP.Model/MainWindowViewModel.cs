using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Claysys.PPP.Model
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<NextGenPPP> data;

        public ObservableCollection<NextGenPPP> GridData
        {
            get { return data; }
            set { data = value; NotifyPropertyChanged() }
        }

        ctor

    }
}
