namespace Smart_Server.Controllers
{
    internal class SqlConnection
    {
        private string connStr;

        public SqlConnection(string connStr)
        {
            this.connStr = connStr;
        }
    }
}