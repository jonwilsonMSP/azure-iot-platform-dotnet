// <copyright file="EmailActionModel.cs" company="3M">
// Copyright (c) 3M. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Mmm.Iot.AsaManager.Services.JsonConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Mmm.Iot.AsaManager.Services.Models.Rules
{
    public class EmailActionModel : IActionModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ActionType Type { get; set; }

        // Parameters dictionary is case-insensitive.
        [JsonConverter(typeof(EmailParametersDictionaryConverter))]
        public IDictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public override bool Equals(object obj)
        {
            if (!(obj is EmailActionModel otherModel))
            {
                return false;
            }

            return this.Type.Equals(otherModel.Type)
                && this.IsEqualDictionary(otherModel.Parameters);
        }

        public override int GetHashCode()
        {
            var hashCode = this.Type.GetHashCode();
            hashCode = (hashCode * 397) ^ (this.Parameters != null ? this.Parameters.GetHashCode() : 0);
            return hashCode;
        }

        // Checks if both the dictionaries have the same keys and values.
        // For a dictionary[key] => list, does a comparison of all the elements of the list, regardless of order.
        private bool IsEqualDictionary(IDictionary<string, object> compareDictionary)
        {
            if (this.Parameters.Count != compareDictionary.Count)
            {
                return false;
            }

            foreach (var key in this.Parameters.Keys)
            {
                if (!compareDictionary.ContainsKey(key) ||
                    this.Parameters[key].GetType() != compareDictionary[key].GetType())
                {
                    return false;
                }

                if (this.Parameters[key] is IList<string> &&
                    !this.AreListsEqual((List<string>)this.Parameters[key], (List<string>)compareDictionary[key]))
                {
                    return false;
                }

                if (!(this.Parameters[key] is IList<string>) &&
                    !compareDictionary[key].Equals(this.Parameters[key]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool AreListsEqual(List<string> list1, List<string> list2)
        {
            return list1.Count == list2.Count && !list1.Except(list2).Any();
        }
    }
}