using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechGearDatabase.Models
{
    public interface Iid
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
