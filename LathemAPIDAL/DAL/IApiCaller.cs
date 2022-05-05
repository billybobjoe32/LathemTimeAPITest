using LathemAPIDAL.Models;

namespace LathemAPIDAL.DAL
{
    public interface IApiCaller
    {
        public Task<List<Employee>> GetEmployees();
        public Task<PunchResponse> SendPunch(Punch punch);
        public Task<VersionResponse> GetVersion();
    }
}
