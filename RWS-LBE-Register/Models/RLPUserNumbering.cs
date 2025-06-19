using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RWS_LBE_Register.Models
{
    [Table("RLPUserNumbering")] // Optional: if you want to explicitly set table name
    public class RLPUserNumbering
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public uint Id { get; set; }

        [Column("year")]
        public long Year { get; set; }

        [Column("month")]
        public long Month { get; set; }

        [Column("day")]
        public long Day { get; set; }

        [Column("rlp_id")]
        [MaxLength(255)]
        public string RLP_ID { get; set; } = string.Empty;

        [Column("rlp_no")]
        [MaxLength(255)]
        public string RLP_NO { get; set; } = string.Empty;

        [Column("rlp_id_ending_no")]
        public int RLPIDEndingNO { get; set; }
    }
}
