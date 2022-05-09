using LathemAPIDAL.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public class JSONEmployeeDB : IEmployeeDB
    {
        private string fileName = ConfigurationManager.AppSettings["EmployeeDataFileName"] ?? String.Empty;
        public void Create(Employee employee)
        {
            List<Employee> list = GetAll();

            if(list.Any(e => e.EmployeeId == employee.EmployeeId))
            {
                throw new Exception("Cannot create employee in DB");
            }
            else
            {
                list.Add(employee);
                SaveChanges(list);
            }
        }

        public void Delete(Employee employee)
        {
            List<Employee> list = GetAll();

            if (!list.Any(e => e.EmployeeId == employee.EmployeeId))
            {
                throw new Exception("Cannot delete employee in DB");
            }
            else
            {
                list.Remove(employee);
                SaveChanges(list);
            }
        }

        public List<Employee> GetAll()
        {
            List<Employee> list = new List<Employee>();
            if (!String.IsNullOrEmpty(fileName))
            {
                if (File.Exists(fileName))
                {
                    string[] employeeData = File.ReadAllLines(fileName);
                    list = JsonConvert.DeserializeObject<List<Employee>>(employeeData[0]);
                    if (list == null)
                    {
                        list = new List<Employee>();
                    }
                }
                else
                {
                    list = new List<Employee>();
                    string fileStorageString = JsonConvert.SerializeObject(list);
                    File.WriteAllText(fileName, fileStorageString);
                }
            }
            return list;
        }

        public void SaveChanges(List<Employee> employees)
        {
            string fileStorageString = JsonConvert.SerializeObject(employees);
            File.WriteAllText(fileName, fileStorageString);
        }

        public void Update(Employee employee)
        {
            List<Employee> list = GetAll();
            Employee currentEmployee = list.FirstOrDefault(e => e.EmployeeId == employee.EmployeeId);
            if(currentEmployee == null)
            {
                throw new Exception("No employee found, cannot update employee in DB");
            }
            else
            {
                currentEmployee.LastName = employee.LastName;
                currentEmployee.FirstName = employee.FirstName;
                SaveChanges(list);
            }
        }
    }
}
