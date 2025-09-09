using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using OpenAI;

namespace AdFormCurrencyConversion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TalkController : ControllerBase
    {
        public TalkController() { }

        [HttpGet]
        public async Task<string> Ask([FromQuery] string question)
        {
            return "Uttam";
        }

        //[NonAction]
        //private async Task<string> AskAsync(string question)
        //{
        //    var client = new OpenAIClient("your-api-key");

        //    var response = await client.CHa.GetCompletionAsync(new ChatRequest
        //    {
        //        Model = "gpt-4",
        //        Messages = new List<ChatMessage>
        //    {
        //        ChatMessage.FromSystem("You are a .NET assistant. Answer based on metadata."),
        //        ChatMessage.FromUser(question),
        //        ChatMessage.FromSystem("Metadata: " + JsonSerializer.Serialize(_metadata))
        //    }
        //    });

        //    return response.FirstChoice.Message.Content;
        //}
    
    }
}
