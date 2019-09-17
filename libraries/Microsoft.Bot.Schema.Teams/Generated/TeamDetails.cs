// <auto-generated>
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Microsoft.Bot.Schema.Teams
{
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Details related to a team
    /// </summary>
    public partial class TeamDetails
    {
        /// <summary>
        /// Initializes a new instance of the TeamDetails class.
        /// </summary>
        public TeamDetails()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the TeamDetails class.
        /// </summary>
        /// <param name="id">Unique identifier representing a team</param>
        /// <param name="name">Name of team.</param>
        /// <param name="aadGroupId">Azure Active Directory (AAD) Group Id for
        /// the team.</param>
        public TeamDetails(string id = default(string), string name = default(string), string aadGroupId = default(string))
        {
            Id = id;
            Name = name;
            AadGroupId = aadGroupId;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets unique identifier representing a team
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets name of team.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets azure Active Directory (AAD) Group Id for the team.
        /// </summary>
        [JsonProperty(PropertyName = "aadGroupId")]
        public string AadGroupId { get; set; }

    }
}