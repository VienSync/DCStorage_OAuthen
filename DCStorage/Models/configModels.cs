using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DCStorage.Models
{
    public class BoxAppSettings
    {
        [JsonProperty("clientID")]
        public string ClientID { get; set; }

        [JsonProperty("clientSecret")]
        public string ClientSecret { get; set; }

        [JsonProperty("appAuth")]
        public AppAuth AppAuth { get; set; }
    }

    public class AppAuth
    {
        [JsonProperty("publicKeyID")]
        public string PublicKeyID { get; set; }

        [JsonProperty("privateKey")]
        public string PrivateKey { get; set; }

        [JsonProperty("passphrase")]
        public string Passphrase { get; set; }
    }

    public class ConfigModels
    {
        [JsonProperty("boxAppSettings")]
        public BoxAppSettings BoxAppSettings { get; set; }

        [JsonProperty("enterpriseID")]
        public string EnterpriseID { get; set; }
       
        [JsonProperty("OrderReceived")]
        public List<string> orderReceived { get; set; }
        [JsonProperty("PlaceOrder")]
        public List<string> placeOrder { get; set; }
        [JsonProperty("BoxParentId")]
        public string BoxParentId { get; set; }
        [JsonProperty("UploadFolderName")]
        public List<string> UploadFolderName { get; set; }
        [JsonProperty("RedirectUri")]
        public string RedirectUri { get; set; }
        [JsonProperty("RedirectUriLocal")]
        public string RedirectUriLocal { get; set; }
        [JsonProperty("AuthenticationUrl")]
        public string AuthenticationUrl { get; set; }
        [JsonProperty("ClientId")]
        public string ClientId { get; set; }
        [JsonProperty("ClientSecret")]
        public string ClientSecret { get; set; }
        [JsonProperty("UploadUri")]
        public string UploadUri { get; set; }
    }
}
