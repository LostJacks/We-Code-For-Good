using LabelingTool.DAL.GeneralRepositories;
using LabelingTool.DAL.Interfaces;
using Model.BBDTOs;
using Model.ExportDTOs;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelingTool.BLL.ExportServices
{
    public class YoloExportService : IYoloExportService
    {
        private readonly IBoundingBoxRepository _boundingBoxRepository;
        private readonly IMediaRepository _mediaRepository;
        private readonly ILabelClassRepository _labelClassRepository;
        private readonly IProjectRepository _projectRepository;

        public YoloExportService(
            IBoundingBoxRepository boundingBoxRepository,
            IMediaRepository mediaRepository,
            ILabelClassRepository labelClassRepository,
            IProjectRepository projectRepository)
        {
            _boundingBoxRepository = boundingBoxRepository;
            _mediaRepository = mediaRepository;
            _labelClassRepository = labelClassRepository;
            _projectRepository = projectRepository;
        }

        /// Generate YOLO export data structure
        public async Task<YoloExportDto> GenerateYoloExportAsync(int projectId)
        {
            // Get project
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null)
                throw new ArgumentException($"Project {projectId} not found");

            // Get all media for project
            var mediaList = await _mediaRepository.GetByProjectIdAsync(projectId);

            // Get all label classes
            var allClasses = await _labelClassRepository.GetAllAsync();

            var yoloImages = new List<YoloImageDto>();
            var usedClasses = new HashSet<int>();

            // Process each image
            foreach (var media in mediaList)
            {
                // Get bounding boxes for this media
                var boxes = await _boundingBoxRepository.GetByMediaIdAsync(media.MediaId);

                if (!boxes.Any())
                    continue; // Skip images with no annotations

                var yoloBoxes = new List<YoloBoxDto>();

                foreach (var box in boxes)
                {
                    var yoloBox = ConvertToYoloBox(box, media.Width, media.Height);
                    yoloBoxes.Add(yoloBox);
                    usedClasses.Add(box.LabelClassId);
                }

                yoloImages.Add(new YoloImageDto
                {
                    MediaId = media.MediaId,
                    ImagePath = media.FilePath,
                    ImageWidth = media.Width,
                    ImageHeight = media.Height,
                    Boxes = yoloBoxes
                });
            }

            // Get class names for used classes
            var classNames = allClasses
                .Where(c => usedClasses.Contains(c.LabelClassId))
                .OrderBy(c => c.LabelClassId)
                .Select(c => c.Name)
                .ToList();

            return new YoloExportDto
            {
                ProjectId = projectId,
                ProjectName = project.Name,
                Images = yoloImages,
                Classes = classNames
            };
        }

        /// Convert to YOLO format (0-1 normalized)
        private YoloBoxDto ConvertToYoloBox(BoundingBoxDto box, int imageWidth, int imageHeight)
        {
            // Calculate center point
            float xCenter = box.X1 + (box.X2 - box.X1) / 2f;
            float yCenter = box.Y1 + (box.Y2 - box.Y1) / 2f;

            // Calculate width and height
            float width = box.X2 - box.X1;
            float height = box.Y2 - box.Y1;

            // Normalize to 0-1 range
            float xCenterNorm = xCenter / imageWidth;
            float yCenterNorm = yCenter / imageHeight;
            float widthNorm = width / imageWidth;
            float heightNorm = height / imageHeight;

            return new YoloBoxDto
            {
                ClassId = box.LabelClassId,
                XCenter = xCenterNorm,
                YCenter = yCenterNorm,
                Width = widthNorm,
                Height = heightNorm
            };
        }

        public string ConvertToYoloFormat(BoundingBoxDto box, int imageWidth, int imageHeight)
        {
            var yoloBox = ConvertToYoloBox(box, imageWidth, imageHeight);
            return $"{yoloBox.ClassId} {yoloBox.XCenter:F6} {yoloBox.YCenter:F6} {yoloBox.Width:F6} {yoloBox.Height:F6}";
        }

        /// Create ZIP file with YOLO format structure
        public async Task<byte[]> CreateYoloZipAsync(YoloExportDto export)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    // 1. Create classes.txt with all class names
                    var classesEntry = zipArchive.CreateEntry("classes.txt");
                    using (var writer = new StreamWriter(classesEntry.Open()))
                    {
                        foreach (var className in export.Classes)
                        {
                            await writer.WriteLineAsync(className);
                        }
                    }

                    // 2. Create labels folder with annotation files
                    foreach (var image in export.Images)
                    {
                        // Use image filename without extension
                        string filename = Path.GetFileNameWithoutExtension(image.ImagePath);
                        string labelFilename = $"labels/{filename}.txt";

                        var labelEntry = zipArchive.CreateEntry(labelFilename);
                        using (var writer = new StreamWriter(labelEntry.Open()))
                        {
                            // Write each bounding box
                            foreach (var box in image.Boxes)
                            {
                                string line = $"{box.ClassId} {box.XCenter:F6} {box.YCenter:F6} {box.Width:F6} {box.Height:F6}";
                                await writer.WriteLineAsync(line);
                            }
                        }
                    }

                    // 3. Create metadata file (optional but helpful)
                    var metadataEntry = zipArchive.CreateEntry("metadata.txt");
                    using (var writer = new StreamWriter(metadataEntry.Open()))
                    {
                        await writer.WriteLineAsync($"Project: {export.ProjectName}");
                        await writer.WriteLineAsync($"Project ID: {export.ProjectId}");
                        await writer.WriteLineAsync($"Export Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                        await writer.WriteLineAsync($"Total Images: {export.Images.Count}");
                        await writer.WriteLineAsync($"Total Classes: {export.Classes.Count}");
                        await writer.WriteLineAsync($"Total Annotations: {export.Images.Sum(i => i.Boxes.Count)}");
                        await writer.WriteLineAsync("");
                        await writer.WriteLineAsync("Classes:");
                        for (int i = 0; i < export.Classes.Count; i++)
                        {
                            await writer.WriteLineAsync($"{i}: {export.Classes[i]}");
                        }
                    }
                }

                return memoryStream.ToArray();
            }
        }
    }
}
