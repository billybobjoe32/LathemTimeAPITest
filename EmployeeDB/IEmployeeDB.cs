using LathemAPIDAL.Models;

namespace Database
{
    public interface IEmployeeDB
    {
        List<Employee> GetAll();
        void Update(Employee employee);
        void Create(Employee employee);
        void Delete(Employee employee);
        void SaveChanges(List<Employee> employees);
    }
}