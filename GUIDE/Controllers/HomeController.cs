using System.Diagnostics;
using GUIDE.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace GUIDE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;
        private readonly string pythonApiUrl = "http://127.0.0.1:5000/";

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, HttpClient httpClient)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> TakeAttendance()
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"{pythonApiUrl}/take_attendance");
            var result = await response.Content.ReadAsStringAsync();
            ViewBag.Message = result;
            return View("Index");
        }


        public async Task<IActionResult> AttendanceList()
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"{pythonApiUrl}/get_attendance_report");
            var jsonString = await response.Content.ReadAsStringAsync();

            // JSON'u parse et
            var jsonObject = JObject.Parse(jsonString);
            var reportData = jsonObject["report"]?.ToString();

            // CSV formatındaki veriyi tabloya çevirmek
            var attendanceList = new List<AttendanceModel>();

            if (!string.IsNullOrEmpty(reportData))
            {
                var lines = reportData.Split("\n");
                foreach (var line in lines.Skip(1)) // İlk satır başlık olduğu için atla
                {
                    var columns = line.Split(",");
                    if (columns.Length == 4)
                    {
                        var name = columns[0].Split("_");
                        attendanceList.Add(new AttendanceModel
                        {
                            Name = name[0] + " " + name[1],
                            Date = columns[1],
                            Time = columns[2],
                            Emotion = columns[3]
                        });
                    }
                }
            }
            return View(attendanceList);
        }
        public async Task<IActionResult> StudentList()
        {
            var response = await _httpClient.GetAsync($"{pythonApiUrl}/get_students");
            if (!response.IsSuccessStatusCode)
            {
                // Handle error
                return View(new List<StudentModel>());
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var students = JsonConvert.DeserializeObject<List<StudentModel>>(jsonString);

            // Convert image paths to Base64
            foreach (var student in students)
            {
                student.PhotoBase64 = ConvertImageToBase64(student.PhotoUrl);
            }

            return View(students);
        }

        private string ConvertImageToBase64(string imagePath)
        {
            try
            {
                byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);
                return Convert.ToBase64String(imageBytes);
            }
            catch (Exception ex)
            {
                // Log error and return an empty string if conversion fails
                Console.WriteLine($"Error converting image: {ex.Message}");
                return string.Empty;
            }
        }

    }
}
