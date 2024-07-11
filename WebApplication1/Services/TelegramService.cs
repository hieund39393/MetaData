using Newtonsoft.Json;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface ITelegramService
    {
        Task<int?> SendMessageAsync(string telegramBot, string channel, string message, int? messageId);
    }
    public class TelegramService : ITelegramService
    {
        public async Task<int?> SendMessageAsync(string telegramBotToken, string channel, string message, int? messageId)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (messageId != null)
                    {
                        string editMessageUrl = $"https://api.telegram.org/bot{telegramBotToken}/editMessageText?chat_id={channel}&message_id={messageId}&text={message}";
                        HttpResponseMessage editResponse = await client.GetAsync(editMessageUrl);
                        string editResponseContent = await editResponse.Content.ReadAsStringAsync();
                        return null;
                    }
                    else
                    {
                        string sendMessageUrl = $"https://api.telegram.org/bot{telegramBotToken}/sendMessage?chat_id={channel}&text={message}";
                        HttpResponseMessage sendResponse = await client.GetAsync(sendMessageUrl);
                        string sendResponseContent = await sendResponse.Content.ReadAsStringAsync();
                        var sendResponseObject = JsonConvert.DeserializeObject<SendMessageResponse>(sendResponseContent);
                        return sendResponseObject.result.message_id;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
