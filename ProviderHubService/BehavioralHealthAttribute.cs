using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace ProviderHubService
{

    #region PUBLIC ENUM

    [DataContract]
    public enum BHAttributeType
    {
        [EnumMember]
        Ages = 1,

        [EnumMember]
        Modes = 2,

        [EnumMember]
        Conditions = 3,

        [EnumMember]
        TherapeuticApproaches = 4,

        [EnumMember]
        Other = 5

    }

    #endregion

    [DataContract]
    public class BehavioralHealthAttribute
    {
        #region PUBLIC PROPERTIES

        [DataMember]
        public int MappingID { get; set; }

        [DataMember]
        public int SetID { get; set; }

        [DataMember]
        public BHAttributeType BHSpecialtyType { get; set; }

        [DataMember]
        public int ValueID { get; set; }
        
        [DataMember]
        public string TextValue { get; set; }
        
        [DataMember]
        public bool Status { get; set; }

        #endregion
    }

}