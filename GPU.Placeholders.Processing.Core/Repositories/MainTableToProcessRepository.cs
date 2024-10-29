using GPU.Placeholders.Processing.Core.Data;

namespace GPU.Placeholders.Processing.Core.Repositories
{
    public class MainTableToProcessRepository : Repository
    {
        public MainTableToProcessRepository(string connectionString) : base(connectionString)
        {

        }

        public MainTableToProcessRepository(Database database) : base(database)
        {

        }

        public int GetMainProcessToProcessTotalCount() =>
           //_database.GetCount("SELECT COUNT(*) FROM MAIN_TABLE_TO_PROCESS WHERE SITEID = 'LG2488'");
           _database.GetCount("SELECT COUNT(*) FROM MAIN_TABLE_TO_PROCESS");

        public IEnumerable<MainTableToProcess> GetMainTableToProcess(int page, int take)
        {
            var values = _database.GetData<MainTableToProcess>(
                @"SELECT * 
                    FROM MAIN_TABLE_TO_PROCESS
                   --WHERE SITEID = 'LG2488'
                   ORDER BY RFI_DATE DESC                    
                  OFFSET @offset ROWS
                   FETCH NEXT @limit ROWS ONLY",

                new
                {
                    offset = (page - 1) * take,
                    limit = take
                }
            );

            return values;
        }
    }
}
