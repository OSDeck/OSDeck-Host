using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Text.Json;
using Microsoft.VisualBasic.CompilerServices;
using Testprojekt_1.Helpers;

namespace Testprojekt_1.Components.Layout
{
    public partial class MainLayout
    {
        private bool isLeftSidebarOpen = true;
        private bool isRightSidebarOpen = true;
        private bool isUploadsOpen = true;
        private bool isMultimediaOpen = true;
        private bool isLaunchOpen = true;
        private bool isShapesOpen = true;
        private bool isSlidersOpen = true;
        private string activeTab = "inputs";
        private bool isDragging = false;
        private int isMoving = -1;
        private double posX = 0;
        private double posY = 0;
        private List<DragObject> canvasObjects = new List<DragObject>();


        private ElementReference canvasContainer;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("initializeDragAndDrop", canvasContainer);
            }
        }

        private void ToggleLeftSidebar() => isLeftSidebarOpen = !isLeftSidebarOpen;
        private void ToggleRightSidebar() => isRightSidebarOpen = !isRightSidebarOpen;
        private void SetActiveTab(string tab) => activeTab = tab;
        protected string PosXpx => $"{posX}px";
        protected string PosYpx => $"{posY}px";

        private void ToggleGroup(string group)
        {
            switch (group)
            {
                case "uploads":
                    isUploadsOpen = !isUploadsOpen;
                    break;
                case "multimedia":
                    isMultimediaOpen = !isMultimediaOpen;
                    break;
                case "launch":
                    isLaunchOpen = !isLaunchOpen;
                    break;
                case "shapes":
                    isShapesOpen = !isShapesOpen;
                    break;
                case "sliders":
                    isSlidersOpen = !isSlidersOpen;
                    break;
            }
        }

        // Image Validation
        private string imageValidationMessage;
        private int imageWidth;
        private string imagePreviewUrl;

        private async Task ValidateImage(ChangeEventArgs e)
        {
            var file = e.Value as Microsoft.AspNetCore.Components.Forms.IBrowserFile;

            if (file != null)
            {
                var extension = Path.GetExtension(file.Name).ToLower();
                using var stream = file.OpenReadStream();
                var buffer = new byte[24];

                await stream.ReadAsync(buffer, 0, buffer.Length);

                if (extension == ".bmp")
                {
                    bool isBmp = buffer[0] == 'B' && buffer[1] == 'M';
                    int bitsPerPixel = BitConverter.ToInt16(buffer, 28);

                    if (!isBmp || bitsPerPixel != 24)
                    {
                        imageValidationMessage = "Invalid BMP file. Please upload a 24-bit BMP image.";
                        imageWidth = 0;
                        imagePreviewUrl = null;
                        return;
                    }

                    imageWidth = BitConverter.ToInt32(buffer, 18);
                    imageValidationMessage = "Valid 24-bit BMP image.";
                }
                else if (extension == ".png")
                {
                    bool isPng = buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47;

                    if (!isPng)
                    {
                        imageValidationMessage = "Invalid PNG file.";
                        imageWidth = 0;
                        imagePreviewUrl = null;
                        return;
                    }

                    imageWidth = BitConverter.ToInt32(buffer.Skip(16).Take(4).Reverse().ToArray(), 0);
                    imageValidationMessage = "Valid PNG image.";
                }
                else if (extension == ".jpg" || extension == ".jpeg")
                {
                    bool isJpeg = buffer[0] == 0xFF && buffer[1] == 0xD8;

                    if (!isJpeg)
                    {
                        imageValidationMessage = "Invalid JPEG file.";
                        imageWidth = 0;
                        imagePreviewUrl = null;
                        return;
                    }

                    imageValidationMessage = "Valid JPEG image.";
                    // Image width extraction for JPEG is more complex and often requires parsing the entire file.
                    imageWidth = 0; // Set this to 0 or implement JPEG width extraction.
                }
                else
                {
                    imageValidationMessage = "Unsupported file type.";
                    imageWidth = 0;
                    imagePreviewUrl = null;
                    return;
                }

                // Generate preview URL
                var bufferForPreview = new byte[file.Size];
                await stream.ReadAsync(bufferForPreview, 0, bufferForPreview.Length);
                var base64String = Convert.ToBase64String(bufferForPreview);
                imagePreviewUrl = $"data:{file.ContentType};base64,{base64String}";
            }
            else
            {
                imageValidationMessage = "No file selected.";
                imageWidth = 0;
                imagePreviewUrl = null;
            }
        }

        // Config-File
        private string configValidationMessage;

        private async Task UploadConfigFile(ChangeEventArgs e)
        {
            var file = e.Value as Microsoft.AspNetCore.Components.Forms.IBrowserFile;

            if (file != null)
            {
                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream);
                var configFileContent = await reader.ReadToEndAsync();

                try
                {
                    // Deserialize the config file content
                    var config = JsonSerializer.Deserialize<Config>(configFileContent);

                    // Process config, for example, apply shortcuts
                    if (config != null)
                    {
                        Console.WriteLine($"VolumeShortcut: {config.VolumeShortcut}");
                        Console.WriteLine($"BrightnessShortcut: {config.BrightnessShortcut}");
                        Console.WriteLine($"MuteShortcut: {config.MuteShortcut}");
                    }

                    configValidationMessage = "Config file loaded successfully.";
                }
                catch (JsonException)
                {
                    configValidationMessage = "Invalid config file format.";
                }
            }
            else
            {
                configValidationMessage = "No config file selected.";
            }
        }

        protected void HandlePickup(MouseEventArgs e)
        {
            isDragging = true;
            posX = e.ClientX;
            posY = e.ClientY;

            StateHasChanged();
        }

        protected void HandleDrag(MouseEventArgs e)
        {
            if (isDragging)
            {
                posX = e.ClientX;
                posY = e.ClientY;
            } else if (isMoving != -1)
            {
	            canvasObjects[isMoving].setPosition(e.ClientX, e.ClientY);
            }

            StateHasChanged();
        }

		protected async void HandleDrop(MouseEventArgs e)
        {
	        int[] arr = new int[4];
	        arr = await JS.InvokeAsync<int[]>("getCanvas");

			if (isDragging)
	        {
		        isDragging = false;

		        if (e.ClientX > arr[0] && e.ClientX < arr[0] + arr[2] && e.ClientY > arr[1] &&
		            e.ClientY < arr[1] + arr[3])
		        {
			        canvasObjects.Add(new DragObject(40, e.ClientX, e.ClientY, 0));
		        }
	        } else if (isMoving != -1)
			{
				if (e.ClientX > arr[0] && e.ClientX < arr[0] + arr[2] && e.ClientY > arr[1] &&
				    e.ClientY < arr[1] + arr[3])
				{
					canvasObjects[isMoving].setPosition(e.ClientX, e.ClientY);
				}
				else
				{
                    canvasObjects.RemoveAt(isMoving);
				}

				isMoving = -1;
			}

			StateHasChanged();
		}

        protected void HandleMoveStart(MouseEventArgs e)
        {
	        for (int i = 0; i < canvasObjects.Count; i++)
	        {
                //Todo fix for cases such as rectangle
		        if (e.ClientX > canvasObjects[i].PosX && e.ClientX < canvasObjects[i].PosX + canvasObjects[i].SizeX && e.ClientY > canvasObjects[i].PosY &&
		            e.ClientY < canvasObjects[i].PosY + canvasObjects[i].SizeX)
		        {
			        isMoving = i;
			        break;
		        }
	        }
        }

        private async Task DownloadDefaultConfig()
        {
            var defaultConfig = new Config
            {
                VolumeShortcut = "Ctrl+Up",
                BrightnessShortcut = "Ctrl+Down",
                MuteShortcut = "Ctrl+M",
                // Add more default shortcuts or settings as needed
            };

            var json = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true });
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            var base64 = Convert.ToBase64String(bytes);
            var href = $"data:application/json;base64,{base64}";
            var fileName = "default-config.json";

            await JS.InvokeVoidAsync("downloadFile", href, fileName);
        }

        private class Config
        {
            public string VolumeShortcut { get; set; }
            public string BrightnessShortcut { get; set; }
            public string MuteShortcut { get; set; }
            // Add more configurable shortcuts or settings as needed
        }
    }
}
