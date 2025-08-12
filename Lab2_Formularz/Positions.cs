using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2_Formularz
{
    public class Positions : ObservableCollection<string>
    {
        public Positions()
        {
            Add("Manager");
            Add("Developer");
            Add("Assistant");
            Add("Accountant");
        }
    }
}
