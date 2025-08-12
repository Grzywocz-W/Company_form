using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Lab2_Formularz
{
    public class Employee
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public DateTime birth_date { get; private set; }
        public string job { get; set; }
        public string salary { get; set; }
        public string contract_type {  get; set; }
        

        public Employee(string first_name, string last_name, DateTime birth_date, string job, string salary, string contract_type)
        {
            this.first_name = first_name;
            this.last_name = last_name;
            this.birth_date = birth_date;
            this.job = job;
            this.salary = salary;
            this.contract_type = contract_type;

        }

        public Employee() { }

        public int getAge() 
        {  return (DateTime.Now.Year -  this.birth_date.Year);}
        public override string ToString()
        {
            return $"{first_name} {last_name}, wiek: {getAge()}, stanowiska: {job}, pensja: {salary}, typ umowy: {contract_type}";
        }


    }
}
