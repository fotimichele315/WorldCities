using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorldCities.Server.Data.Models
{
    [Table("Countries")]
    [Index(nameof(Name))]
    [Index(nameof(ISO2))]
    [Index(nameof(ISO3))]
    public class Country
    {
        #region Properties
        ///<Summary>
        /// The unique id and primary key for this City
        ///</Summary>
        [Key]
        [Required]
        public int Id { get; set; }

        ///<Summary>
        /// Country name (in UTF8 format)
        ///</Summary>  
        public required string Name { get; set; }
    
        ///<Summary>
        /// Country Code (in ISO 3166-1 ALPHA-2 format)
        ///</Summary>     
        public required string ISO2 { get; set; }

        ///<Summary>
        /// Country Code (in ISO 3166-1 ALPHA-3 format)
        ///</Summary>     
        public required string ISO3 { get; set; }

        #endregion

        #region Navigation properties

        ///<Summary>
        /// A collection of all the citie srelated to this country
        ///</Summary>     
        public ICollection<City>? Cities { get; set; }

        #endregion
    }
}
