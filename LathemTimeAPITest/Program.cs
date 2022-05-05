// See https://aka.ms/new-console-template for more information
using LathemTimeAPITest.ApiTasks;
using System.Configuration;

Console.WriteLine("Welcome to the Lathem Api Simulator!");
Console.WriteLine();
int taskTypeInt = 1;
LathemApiTaskExecutor apiTaskExecutor = new LathemApiTaskExecutor();
apiTaskExecutor.InitializeEmployees();
Task task = null;
while (true)
{
    if(task != null)
    {
        await task;
        task = null;
    }
    DateTime currentTime = DateTime.Now;
    int timeIncrement = int.Parse(ConfigurationManager.AppSettings["TimeIncrement"] ?? "60");
    if (currentTime.Second % timeIncrement == 0 || (timeIncrement >= 60 && currentTime.Second == 0))
    {
        TaskType taskType = (TaskType)taskTypeInt;
        switch (taskType)
        {
            case TaskType.Employee:
                task = apiTaskExecutor.DownloadUpdateReportEmployees();
                taskTypeInt++;
                break;
            case TaskType.Punch:
                task = apiTaskExecutor.RandomEmployeePunch(currentTime);
                taskTypeInt++;
                break;
            case TaskType.Nothing:
                taskTypeInt = 1;
                break;
        }
    }
    Thread.Sleep(1000);
}