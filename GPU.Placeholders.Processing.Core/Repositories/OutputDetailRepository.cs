using GPU.Placeholders.Processing.Core.Data;

namespace GPU.Placeholders.Processing.Core.Repositories
{
    public class OutputDetailRepository : Repository
    {
        public OutputDetailRepository(string connectionString) : base(connectionString)
        {

        }

        public OutputDetailRepository(Database database) : base(database)
        {
            
        }

        public void SaveOutputDetails(IEnumerable<OutputDetail> outputDetails)
        {
            _database?.BulkInsertWithStoreProcedure("sp_insert_outputs", "@P_DATA", "dbo.TABLE_OUTPUT_DETAIL", outputDetails);
        }
    }
}
