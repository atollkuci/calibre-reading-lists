﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Opds4Net.Server;
using Reflection4Net;

namespace Opds4Net.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Category : IOpdsDataTypeHost
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Category Parent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [MaxLength(50)]
        [AdaptedName("Title")]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<Book> Books { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<Category> SubCategories { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        [NotMapped]
        public OpdsDataType OpdsDataType { get { return OpdsDataType.Category; } }
    }
}