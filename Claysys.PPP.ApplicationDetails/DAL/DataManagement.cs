using Claysys.PPP.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Claysys.PPP.ApplicationDetails.WebService
{
    public  class DataManagement
    {
        private static SqlConnection _sqlCon = null;
        private static string _connectionString = ConfigurationManager.ConnectionStrings["connectSQL"].ConnectionString;
        private static string _selectApprovedUserInfo = ConfigurationManager.AppSettings["SelectApprovedUserInfo"];
        private static string _getExportData = ConfigurationManager.AppSettings["GetExportData"];
        private static string _updateExportData = ConfigurationManager.AppSettings["UpdateExportData"];


        public static async Task UpdateExportingData(string loanAppNo)
        {

            await UpdateDocusignDoc(loanAppNo);

        }

        async static Task UpdateDocusignDoc(string loanAppNo)
        {
            try
            {
                using (_sqlCon = new SqlConnection(_connectionString))
                {
                    _sqlCon.Open();
                    SqlCommand sql_cmnd = new SqlCommand(_updateExportData, _sqlCon);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;

                    sql_cmnd.Parameters.AddWithValue("@loanAppNo", SqlDbType.BigInt).Value = Convert.ToInt64(loanAppNo);


                    await sql_cmnd.ExecuteNonQueryAsync();
                    _sqlCon.Close();
                }
            }
            catch (Exception ex)
            {
                if (Utility.Utility.IsEventLogged)
                {
                    Utility.Utility.LogAction("Exception : " + ex);
                }
                throw ex;
            }
        }

        public static async Task<List<NextGenPPP>> GetApprovedCollection()
        {

            try
            {
                var newList = await GetApprovedUserData();


                return newList;
            }
            catch (Exception ex)
            {
                if (Utility.Utility.IsEventLogged)
                {
                    Utility.Utility.LogAction("Exception : " + ex);
                }
                throw ex;
            }
        }

        public static async Task<List<ExportData>> GetExportingData()
        {

            try
            {
                var newList = await GetTopExportData();


                return newList;
            }
            catch (Exception ex)
            {
                if (Utility.Utility.IsEventLogged)
                {
                    Utility.Utility.LogAction("Exception : " + ex);
                }
                throw ex;
            }
        }


        async static Task<List<ExportData>> GetTopExportData()
        {

            List<ExportData> pppExport = new List<ExportData>();

            using (_sqlCon = new SqlConnection(_connectionString))
            {
                _sqlCon.Open();
                SqlCommand sql_cmnd = new SqlCommand(_getExportData, _sqlCon);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = await sql_cmnd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    pppExport.Add(new ExportData()
                    {
                        DocuSignDoc = ObjectToByteArray(reader["SignedDocument"]),
                        LoanApplicationNo = Convert.ToString(reader["loanapplicationnumber"]),
                        SBALoanNo = Convert.ToString(reader["sbaloanno"])
                    });
                }
            }

            return pppExport;
        }

        static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        async static Task<List<NextGenPPP>> GetApprovedUserData() {

            List<NextGenPPP> pppInfoList = new List<NextGenPPP>();

            using (_sqlCon = new SqlConnection(_connectionString))
            {
                _sqlCon.Open();
                SqlCommand sql_cmnd = new SqlCommand(_selectApprovedUserInfo, _sqlCon);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = await sql_cmnd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    pppInfoList.Add(new NextGenPPP() { 
                    BusinessName = Convert.ToString(reader["Borrower"]),
                    LoanApplicationNumber = Convert.ToString(reader["loanapplicationnumber"]),
                    SBALoanNo = Convert.ToString(reader["sbaloanno"])
                    });
                }
            }

            return pppInfoList;
        }
        


    }
}
