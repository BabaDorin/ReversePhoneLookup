﻿using System;
using System.Collections.Generic;

#nullable disable

namespace ReversePhoneLookup.Models.Models.Entities
{
    public partial class Phone
    {
        public Phone()
        {
            Contacts = new HashSet<Contact>();
        }

        public int Id { get; set; }
        public string Value { get; set; }
        public int? OperatorId { get; set; }

        public virtual Operator Operator { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }
    }
}
