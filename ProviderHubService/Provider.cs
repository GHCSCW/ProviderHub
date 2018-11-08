using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Text;
namespace ProviderHubService
{

    #region PUBLIC ENUMS

    [DataContract(Name = "ProviderGender")]
    public enum ProviderGender
    {
        [EnumMember]
        Female = 1,

        [EnumMember]
        Male = 2,

        [EnumMember]
        Unknown = 3
    }

    #endregion

    #region PROVIDER

    [DataContract]
    public class Provider
    {

        #region PRIVATE VARIABLES

        private string fullName;
        private string credentials;
        private string languages;

        #endregion

        #region PUBLIC PROPERTIES

        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string EpicProviderID { get; set; }

        [DataMember]
        public string NPI { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string MiddleName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string ExternalProviderID { get; set; }

        [DataMember]
        public string ExternalProviderName { get; set; }

        [DataMember]
        public DateTime? DateOfBirth { get; set; }

        [DataMember]
        public ProviderGender Gender { get; set; }

        [DataMember]
        public Dictionary<string,bool?> FPRelationship { get; set; }
        
        [DataMember]
        public bool CSP_Indicator { get; set; }
        
        [DataMember]
        public bool MedicaidIndicator { get; set; }

        [DataMember]
        public string MedicaidProviderID { get; set; }

        [DataMember]
        public bool MedicareIndicator { get; set; }

        [DataMember]
        public string MedicarePTAN { get; set; }

        [DataMember]
        public DateTime? MedicareEffectiveDate { get; set; }

        [DataMember]
        public DateTime? MedicareTerminationDate { get; set; }

        [DataMember]
        public DateTime? EffectiveDate { get; set; }

        [DataMember]
        public DateTime? TerminationDate { get; set; }
 
        [DataMember]
        public string InternalNotes { get; set; }

        [DataMember]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public DateTime LastUpdatedDate { get; set; }

        [DataMember]
        public string LastUpdatedBy { get; set; }
        
        [DataMember]
        public List<Language> LanguageList { get; set; }

        [DataMember]
        public List<Credential> CredentialList { get; set; }

        [DataMember]
        public string CredentialListStr { get; set; }

        [DataMember]
        public List<Specialty> ProviderSpecialties { get; set; }

        [DataMember]
        public List<Facility> ProviderFacilities { get; set; }

        [DataMember]
        public string ParentSpecialtyList { get; set; }

        [DataMember]
        public string ChildSpecialtyList { get; set; }

        [DataMember]
        public string SubSpecialtyList { get; set; }

        [DataMember]
        public string PrimarySpecialty { get; set; }

        [DataMember]
        private string FullName
        {
            get { return this.FirstName + " " + this.LastName; }
            set { fullName = value; }
        }

        [DataMember]
        private string Credentials
        {
            get { return String.Join(", ", CredentialList.Select(c => c.Value).ToArray()).ToString(); }
            set { credentials = value; }
        }

        [DataMember]
        public string Languages
        {
            get { return String.Join(", ", LanguageList.Select(l => l.Name).ToArray()).ToString(); }
            set { languages = value; }
        }

        #endregion

        #region CONSTRUCTOR

        public Provider()
        {
            this.CredentialList = new List<Credential>();
            this.ProviderSpecialties = new List<Specialty>();
            this.LanguageList = new List<Language>();
        }

        #endregion
    }

    #endregion

}