using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2_Formularz
{
    public class Team
    {
        public string name { get; set; }
        public ObservableCollection<Employee> Staff { get; set; }
        
        public void AddEmployee(Employee employee)
        {
            Staff.Add(employee);
        }
        public void RemoveEmployee(Employee employee)
        {
            Staff.Remove(employee);
        }

        public Team(string name)
        {
            this.name = name;
            Staff = new ObservableCollection<Employee>();
        }

        public Team() { }

        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Nazwa zespołu: {name}");
            sb.AppendLine("Pracownicy:");

            foreach (Employee emp in Staff)
            {
                sb.AppendLine(emp.ToString());
            }
            return sb.ToString();
        }

    }
}

