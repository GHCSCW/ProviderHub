////using System;
////using System.Collections.Generic;
////using System.Data;
////using System.Linq;
////using System.Text;
////using System.Threading.Tasks;
////using Ghc.Utility.DataAccess;

////namespace MentalHealthWeb.Core
////{
////    /// <summary>
////    /// 
////    /// </summary>
////    public class DataLayer
////    {
////        /// <summary>
////        /// The data layer
////        /// </summary>
////        private GHCDataAccessLayer dataLayer;
////        /// <summary>
////        /// The database
////        /// </summary>
////        const string database = "QuotingEngine";

////        /// <summary>
////        /// Initializes a new instance of the <see cref="DataLayer"/> class.
////        /// </summary>
////        public DataLayer()
////        {
////            dataLayer = GHCDataAccessLayerFactory.GetDataAccessLayer(DataProviderType.Sql, database);
////        }
////        public DataTable GetUnProcessedSmallGroupRenewals()
////        {
////            DataTable SmallGroups = null;

////            string sql = "quotingengine.dbo.sp_GetUnprocessedSmallGroupCorps";

////            SmallGroups = dataLayer.ExecuteDataSet(sql, CommandType.StoredProcedure).Tables[0];

////            return SmallGroups;
////        }

////    }

////}

