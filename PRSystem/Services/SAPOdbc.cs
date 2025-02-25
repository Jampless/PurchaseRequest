using System.Data.Odbc;
using PRSystem.Model;

namespace PRSystem.Services
{
    public class SAPOdbc
    {
        //private readonly string _connectionString;
        private string _connectionString;
        public string SAPCompany { get; set; }

        public void Configure(string server, string user, string password, string databaseNBFI, string databaseEPC, string company)
        {
            SAPCompany = company?.Trim().ToUpperInvariant();

            var driver = "HDBODBC";

            if (SAPCompany == "EPC" || SAPCompany == "AHLC")
            {
                _connectionString = $"DRIVER={{{driver}}};SERVERNODE={server};UID={user};PWD={password};CS={databaseEPC};";
            }
            else if (SAPCompany == "NBFI" || SAPCompany == "ASC" || SAPCompany == "CMC")
            {
                _connectionString = $"DRIVER={{{driver}}};SERVERNODE={server};UID={user};PWD={password};CS={databaseNBFI};";
            }
            else
            {
                throw new InvalidOperationException($"Invalid company: {SAPCompany}");
            }
        }

        //public SAPOdbc(string server, string user, string password, string databaseNBFI, string databaseEPC)
        //{
        //    var driver = Environment.Is64BitProcess ? "HDBODBC" : "HDBODBC32";
        //    if (SAPCompany == "EPC" || SAPCompany == "AHLC")
        //    {
        //        _connectionString = $"DRIVER={{{driver}}};SERVERNODE={server};UID={user};PWD={password};CS={databaseEPC};";
        //    }
        //    else if(SAPCompany == "NBFI" || SAPCompany == "ASC" || SAPCompany == "CMC")
        //    {
        //        _connectionString = $"DRIVER={{{driver}}};SERVERNODE={server};UID={user};PWD={password};CS={databaseNBFI};";
        //    }
        //    else
        //    {
        //        _connectionString = $"DRIVER={{{driver}}};SERVERNODE={server};UID={user};PWD={password};CS={databaseNBFI};";
        //    }

        //    Console.WriteLine($"Connection string initialized: {_connectionString}");
        //}

        public List<OItems> GetItemAsync()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Connection string is not initialized.");
            }

            // string querys; // Declare querys outside the conditionals

            // if (SAPCompany == "EPC" || SAPCompany == "AHLC")
            // {
            //     querys = "SELECT \"ItemCode\", \"ItemName\", \"U_ID007\" AS Size, \"U_ID011\" AS Color, \"InvntryUom\" FROM \"OITM\" WHERE \"ItemCode\" LIKE '299%' ";
            // }
            // else if (SAPCompany == "NBFI" || SAPCompany == "ASC" || SAPCompany == "CMC")
            // {
            //     querys = "SELECT \"ItemCode\", \"ItemName\", \"U_ID007\" AS Size, \"U_ID011\" AS Color, \"InvntryUom\" FROM \"OITM\" WHERE \"ItemCode\" LIKE '199%' ";
            // }
            // else
            // {
            //     // Handle cases where SAPCompany doesn't match any of the conditions (optional)
            //     throw new InvalidOperationException("Unknown SAP Company: " + SAPCompany);
            // }

            var query = SAPCompany switch
            {
                "EPC" or "AHLC" => "SELECT \"ItemCode\", \"ItemName\", \"U_ID007\" AS Size, \"U_ID011\" AS Color, \"InvntryUom\" FROM \"OITM\" WHERE \"ItemCode\" LIKE '299%'",
                "NBFI" or "ASC" or "CMC" => "SELECT \"ItemCode\", \"ItemName\", \"U_ID007\" AS Size, \"U_ID011\" AS Color, \"InvntryUom\" FROM \"OITM\" WHERE \"ItemCode\" LIKE '199%'",
                _ => throw new InvalidOperationException("Unknown SAP Company: " + SAPCompany)
            };

            var results = new List<OItems>();
            try
            {
                Console.WriteLine("Initializing database connection...");
                using var connection = new OdbcConnection(_connectionString);
                //Console.WriteLine($"Connection string: {_connectionString}");
                connection.Open();
                //Console.WriteLine("Database connection opened successfully.");

                using var command = new OdbcCommand(query, connection);
                //Console.WriteLine($"Executing query: {querys}");

                using var reader = command.ExecuteReader();
                //Console.WriteLine("Query executed successfully. Processing results...");

                while (reader.Read())
                {
                    //Console.WriteLine($"Reading row: ItemCode = {reader["ItemCode"]}, ItemName = {reader["ItemName"]}, Size = {reader["Size"]}, Color = {reader["Color"]}, InvntryUom = {reader["InvntryUom"]}");
                    // Map the columns to the Items class properties
                    var item = new OItems
                    {
                        ItemCode = reader["ItemCode"].ToString(),
                        ItemName = reader["ItemName"].ToString(),
                        U_ID007 = reader["Size"].ToString(),
                        U_ID011 = reader["Color"].ToString(),
                        InvntryUom = reader["InvntryUom"].ToString()
                    };
                    results.Add(item);
                }

                Console.WriteLine($"Total items retrieved: {results.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get Item using ODBC: {ex.Message}");
            }
            return results;
        }

        public async Task<List<OSuppliers>> GetSuppliers()
        {
            var results = new List<OSuppliers>();
            string query = "SELECT \"CardCode\", \"CardName\" FROM \"OCRD\" WHERE LEFT(\"CardCode\", 4) = 'LTS-'";
            try
            {
                using var connection = new OdbcConnection(_connectionString);
                connection.Open();
                using var command = new OdbcCommand(query, connection);
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    // Map the columns to the Items class properties
                    var suppliers = new OSuppliers
                    {
                        CardCode = reader["CardCode"].ToString(),
                        CardName = reader["CardName"].ToString()
                    };
                    results.Add(suppliers);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database operation failed: {ex.Message}");
            }
            return results;
        }

        public List<OGLAccounts> GetGLAccounts()
        {
            string querys = "SELECT \"AcctCode\", \"AcctName\" FROM \"OACT\" WHERE \"AcctCode\" LIKE '___-___'";
            var results = new List<OGLAccounts>();
            try
            {
                using var connection = new OdbcConnection(_connectionString);
                connection.Open();
                using var command = new OdbcCommand(querys, connection);
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    // Map the columns to the Items class properties
                    var item = new OGLAccounts
                    {
                        AcctCode = reader["AcctCode"].ToString(),
                        AcctName = reader["AcctName"].ToString()
                    };
                    results.Add(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get Item using ODBC: {ex.Message}");
            }
            return results;
        }

        public List<OTaxCode> GetTaxCode()
        {
            string querys = "SELECT \"Code\", \"Name\" FROM \"OVTG\"";
            var results = new List<OTaxCode>();
            try
            {
                using var connection = new OdbcConnection(_connectionString);
                connection.Open();
                using var command = new OdbcCommand(querys, connection);
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    // Map the columns to the Items class properties
                    var item = new OTaxCode
                    {
                        Code = reader["Code"].ToString(),
                        Name = reader["Name"].ToString()
                    };
                    results.Add(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get TaxCode using ODBC: {ex.Message}");
            }
            return results;
        }
    }
}