namespace InSite.Persistence.Plugin.CMDS
{
    public class RunWeeklyJobs
    {
        public void Execute()
        {
            var update = new UpdateComplianceSnapshotCommand();

            update.Execute();
        }
    }
}