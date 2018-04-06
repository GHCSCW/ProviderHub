using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace ProviderHubService
{
    [DataContract]
    public class Language
    {
        #region PUBLIC PROPERTIES

        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int SequenceNumber { get; set; }

        [DataMember]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        public int MappingID { get; set; }

        [DataMember]
        public bool Status { get; set; }

        #endregion
    }
}