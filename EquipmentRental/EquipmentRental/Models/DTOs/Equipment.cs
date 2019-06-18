using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace EquipmentRental.Models.DTOs
{
    // Data transfer object for Equipment table in azure database
    public class Equipment
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "item")]
        public string ItemName { get; set; }

        [JsonProperty(PropertyName = "waiting_for_permission")]
        public bool IsWaitingForPermission { get; set; }

        [JsonProperty(PropertyName = "rented")]
        public bool IsRented { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "rented_since")]
        public DateTime? StartDate { get; set; }

        [JsonProperty(PropertyName = "rented_until")]
        public DateTime? EndDate { get; set; }
    }
}
