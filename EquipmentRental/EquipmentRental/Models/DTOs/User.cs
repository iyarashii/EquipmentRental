using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace EquipmentRental.Models.DTOs
{
    // Data transfer object for User table in azure database
    public class User
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "admin")]
        public bool IsAdmin { get; set; }

        [JsonProperty(PropertyName = "confirmed")]
        public bool IsConfirmed { get; set; }
    }
}
