namespace Model.BBDTOs
{
    public class BoundingBoxDto
    {
        public int BoundingBoxId { get; set; }
        public int MediaId { get; set; }
        public int LabelClassId { get; set; }
        public required string ClassName { get; set; }

        // Two-point coordinate system
        public float X1 { get; set; }  // Top-left X
        public float Y1 { get; set; }  // Top-left Y
        public float X2 { get; set; }  // Bottom-right X
        public float Y2 { get; set; }  // Bottom-right Y
    }
}
