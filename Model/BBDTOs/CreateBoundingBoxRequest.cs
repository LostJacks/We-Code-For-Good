using System.ComponentModel.DataAnnotations;

namespace Model.BBDTOs
{
    public class CreateBoundingBoxRequest
    {
        [Required(ErrorMessage = "MediaId is required")]
        public int MediaId { get; set; }

        [Required(ErrorMessage = "LabelClassId is required")]
        public int LabelClassId { get; set; }

        [Required]
        [Range(0, float.MaxValue, ErrorMessage = "X1 must be non-negative")]
        public float X1 { get; set; }

        [Required]
        [Range(0, float.MaxValue, ErrorMessage = "Y1 must be non-negative")]
        public float Y1 { get; set; }

        [Required]
        [Range(0, float.MaxValue, ErrorMessage = "X2 must be non-negative")]
        public float X2 { get; set; }

        [Required]
        [Range(0, float.MaxValue, ErrorMessage = "Y2 must be non-negative")]
        public float Y2 { get; set; }
    }
}
