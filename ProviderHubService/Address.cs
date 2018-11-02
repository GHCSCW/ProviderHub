using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace ProviderHubService
{
    #region PUBLIC ENUM

    public enum AddressType
    {
        [EnumMember]
        ClinicalPracticeServiceLocation = 1,

        [EnumMember]
        BusinessAdmin = 2,
        
        [EnumMember]
        Mail = 3

    }

    #endregion

    [DataContract]
    public class Address
    {
        #region PUBLIC PROPERTIES

        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public AddressType AddressType { get; set; }

        [DataMember]
        public string AddressTypeName { get; set; }

        [DataMember]
        public string AddressLine1 { get; set; }

        [DataMember]
        public string AddressLine2 { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string ZipCode { get; set; }

        [DataMember]
        public string County { get; set; }

        [DataMember]
        public string Region { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public string PhoneExtension { get; set; }

        [DataMember]
        public string AlternateExtension { get; set; }

        [DataMember]
        public string AlternatePhoneNumber { get; set; }

        [DataMember]
        public string FaxNumber { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Website { get; set; }

        [DataMember]
        public string ContactFirstName { get; set; }

        [DataMember]
        public string ContactLastName { get; set; }

        [DataMember]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public DateTime LastUpdatedDate { get; set; }

        [DataMember]
        public string LastUpdatedBy { get; set; }

        #endregion

        #region CONSTRUCTOR

        public Address()
        {
        }

        #endregion
    }
}