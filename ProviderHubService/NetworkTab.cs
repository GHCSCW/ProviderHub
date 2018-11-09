using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Text;

namespace ProviderHubService
{

    #region VENDOR

    [DataContract]
    public class NetworkTab
    {
        [DataMember]
        public int ID { get; set; }
        //            { data: "Network" }, { data: "NetworkEffectiveDate" }, { data: "Provider" }, { data: "Facility" }, { data: "Specialty" }, { data: "EpicNetworkID" }
        [DataMember]
        public string Network { get; set; }
        [DataMember]
        public DateTime NetworkEffectiveDate { get; set; }
        [DataMember]
        public string Provider { get; set; }
        [DataMember]
        public string Facility { get; set; }
        [DataMember]
        public string Specialty { get; set; }
        [DataMember]
        public int EpicNetworkID { get; set; }
        [DataMember]
        public FacilityProviderRelationship FPRelationship { get; set; }
        #region CONSTRUCTOR

        public NetworkTab()
        {
            //this.AddressesList = new List<Address>();
        }

        #endregion

    }

    #endregion
}