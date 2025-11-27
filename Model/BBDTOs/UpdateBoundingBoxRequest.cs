using System.ComponentModel.DataAnnotations;

namespace Model.BBDTOs
{
    public class UpdateBoundingBoxRequest
    {
        [Required]
        public int BoundingBoxId { get; set; }

        [Required]
        public int LabelClassId { get; set; }

        [Required]
        public float X1 { get; set; }

        [Required]
        public float Y1 { get; set; }

        [Required]
        public float X2 { get; set; }

        [Required]
        public float Y2 { get; set; }
    }
}
