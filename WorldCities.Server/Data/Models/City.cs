using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorldCities.Server.Data.Models
{
    [Table("Cities")]
    [Index(nameof(Name))]
    [Index(nameof(Lat))]
    [Index(nameof(Lon))]
    [Index(nameof(Population))]
    public class City
    {
        #region Properties
        ///<Summary>
        /// The unique id and primary key for this City
        ///</Summary>
        [Key]
        [Required]
        public int Id { get; set; }

        ///<Summary>
        /// City name (in UTF8 format)
        ///</Summary>     
        public required string Name { get; set; }

        ///<Summary>
        /// City Latitude
        ///</Summary>   
        [Column(TypeName = "decimal(7,4)")]
        public decimal Lat { get; set; }

        ///<Summary>
        /// City Longitude
        ///</Summary>   
        [Column(TypeName = "decimal(7,4)")]
        public decimal Lon { get; set; }

        ///<Summary>
        /// Country Id (foreign key)
        ///</Summary>  
        [ForeignKey(nameof(Country))]
        public int CountryId { get; set; }

        ///<Summary>
        /// City population
        ///</Summary>     
        public int Population { get; set; }

        #endregion

        #region Navigation properties

        ///<Summary>
        /// The Country related to this city
        ///</Summary>     
        public Country? Country  { get; set; }

        #endregion
    }
}
